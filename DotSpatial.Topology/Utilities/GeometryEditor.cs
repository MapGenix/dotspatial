// ********************************************************************************************************
// Product Name: DotSpatial.Topology.dll
// Description:  The basic topology module for the new dotSpatial libraries
// ********************************************************************************************************
// The contents of this file are subject to the Lesser GNU Public License (LGPL)
// you may not use this file except in compliance with the License. You may obtain a copy of the License at
// http://dotspatial.codeplex.com/license  Alternately, you can access an earlier version of this content from
// the Net Topology Suite, which is also protected by the GNU Lesser Public License and the sourcecode
// for the Net Topology Suite can be obtained here: http://sourceforge.net/projects/nts.
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and
// limitations under the License.
//
// The Original Code is from the Net Topology Suite, which is a C# port of the Java Topology Suite.
//
// The Initial Developer to integrate this code into MapWindow 6.0 is Ted Dunsford.
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
// |         Name         |    Date    |                              Comment
// |----------------------|------------|------------------------------------------------------------
// |                      |            |
// ********************************************************************************************************

using System.Collections;
using System.Collections.Generic;

namespace DotSpatial.Topology.Utilities
{
    /// <summary>
    /// Supports creating a new <c>Geometry</c> which is a modification of an existing one.
    /// Geometry objects are intended to be treated as immutable.
    /// This class allows you to "modify" a Geometry
    /// by traversing it and creating a new Geometry with the same overall structure but
    /// possibly modified components.
    /// The following kinds of modifications can be made:
    /// <para>
    /// The values of the coordinates may be changed.
    /// Changing coordinate values may make the result Geometry invalid;
    /// this is not checked by the GeometryEditor.
    /// </para>
    /// <para>The coordinate lists may be changed
    /// (e.g. by adding or deleting coordinates).
    /// The modifed coordinate lists must be consistent with their original parent component
    /// (e.g. a LinearRing must always have at least 4 coordinates, and the first and last
    /// coordinate must be equal).
    /// </para>
    /// <para>Components of the original point may be deleted
    /// (e.g. holes may be removed from a Polygon, or LineStrings removed from a MultiLineString).
    /// Deletions will be propagated up the component tree appropriately.
    /// </para>
    /// Notice that all changes must be consistent with the original Geometry's structure
    /// (e.g. a Polygon cannot be collapsed into a LineString).
    /// The resulting Geometry is not checked for validity.
    /// If validity needs to be enforced, the new Geometry's IsValid should be checked.
    /// </summary>
    public class GeometryEditor
    {
        /// <summary>
        /// The factory used to create the modified Geometry.
        /// </summary>
        private IGeometryFactory _factory;

        /// <summary>
        /// Creates a new GeometryEditor object which will create
        /// an edited <c>Geometry</c> with the same {GeometryFactory} as the input Geometry.
        /// </summary>
        public GeometryEditor() { }

        /// <summary>
        /// Creates a new GeometryEditor object which will create
        /// the edited Geometry with the given GeometryFactory.
        /// </summary>
        /// <param name="factory">The GeometryFactory to create the edited Geometry with.</param>
        public GeometryEditor(IGeometryFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Edit the input <c>Geometry</c> with the given edit operation.
        /// Clients will create subclasses of GeometryEditorOperation or
        /// CoordinateOperation to perform required modifications.
        /// </summary>
        /// <param name="geometry">The Geometry to edit.</param>
        /// <param name="operation">The edit operation to carry out.</param>
        /// <returns>A new <c>Geometry</c> which is the result of the editing.</returns>
        public virtual IGeometry Edit(IGeometry geometry, IGeometryEditorOperation operation)
        {
            // if client did not supply a GeometryFactory, use the one from the input Geometry
            if (_factory == null)
                _factory = geometry.Factory;
            if (geometry is GeometryCollection)
                return EditGeometryCollection(geometry, operation);
            if (geometry is Polygon)
                return EditPolygon(geometry, operation);
            if (geometry is Point)
                return operation.Edit(geometry, _factory);
            if (geometry is LineString)
                return operation.Edit(geometry, _factory);
            throw new UnsupportedGeometryException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private IPolygon EditPolygon(IGeometry polygon, IGeometryEditorOperation operation)
        {
            Polygon newPolygon = (Polygon)operation.Edit(polygon, _factory);
            if (newPolygon.IsEmpty)
                //RemoveSelectedPlugIn relies on this behaviour. [Jon Aquino]
                return newPolygon;

            LinearRing shell = (LinearRing)Edit(newPolygon.Shell, operation);
            if (shell.IsEmpty)
                //RemoveSelectedPlugIn relies on this behaviour. [Jon Aquino]
                return _factory.CreatePolygon(null, null);

            ArrayList holes = new ArrayList();
            for (int i = 0; i < newPolygon.NumHoles; i++)
            {
                LinearRing hole = (LinearRing)Edit(newPolygon.GetInteriorRingN(i), operation);
                if (hole.IsEmpty) continue;
                holes.Add(hole);
            }

            return _factory.CreatePolygon(shell, (LinearRing[])holes.ToArray(typeof(LinearRing)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private IGeometryCollection EditGeometryCollection(IGeometry collection,
                                                          IGeometryEditorOperation operation)
        {
            GeometryCollection newCollection = (GeometryCollection)operation.Edit(collection, _factory);
            ArrayList geometries = new ArrayList();
            for (int i = 0; i < newCollection.NumGeometries; i++)
            {
                IGeometry geometry = Edit(newCollection.GetGeometryN(i), operation);
                if (geometry.IsEmpty) continue;
                geometries.Add(geometry);
            }

            if (newCollection is MultiPoint)
                return _factory.CreateMultiPoint((Point[])geometries.ToArray(typeof(Point)));

            if (newCollection is MultiLineString)
                return _factory.CreateMultiLineString((LineString[])geometries.ToArray(typeof(LineString)));

            if (newCollection is MultiPolygon)
                return _factory.CreateMultiPolygon((Polygon[])geometries.ToArray(typeof(Polygon)));

            return _factory.CreateGeometryCollection((Geometry[])geometries.ToArray(typeof(Geometry)));
        }

        #region Nested type: CoordinateOperation

        /// <summary>
        /// A GeometryEditorOperation which modifies the coordinate list of a <c>Geometry</c>.
        /// Operates on Geometry subclasses which contains a single coordinate list.
        /// </summary>
        public abstract class CoordinateOperation : IGeometryEditorOperation
        {
            #region IGeometryEditorOperation Members

            /// <summary>
            ///
            /// </summary>
            /// <param name="geometry"></param>
            /// <param name="factory"></param>
            /// <returns></returns>
            public virtual IGeometry Edit(IGeometry geometry, IGeometryFactory factory)
            {
                if (geometry is LinearRing)
                    return factory.CreateLinearRing(Edit(geometry.Coordinates, geometry));

                if (geometry is LineString)
                    return factory.CreateLineString(Edit(geometry.Coordinates, geometry));

                if (geometry is Point)
                {
                    IList<Coordinate> newCoordinates = Edit(geometry.Coordinates, geometry);
                    return factory.CreatePoint((newCoordinates.Count > 0) ? newCoordinates[0] : null);
                }

                return geometry;
            }

            #endregion

            /// <summary>
            /// Edits the array of <c>Coordinate</c>s from a <c>Geometry</c>.
            /// </summary>
            /// <param name="coordinates">The coordinate array to operate on.</param>
            /// <param name="geometry">The point containing the coordinate list.</param>
            /// <returns>An edited coordinate array (which may be the same as the input).</returns>
            public abstract IList<Coordinate> Edit(IList<Coordinate> coordinates, IGeometry geometry);
        }

        #endregion

        #region Nested type: IGeometryEditorOperation

        /// <summary>
        /// A interface which specifies an edit operation for Geometries.
        /// </summary>
        public interface IGeometryEditorOperation
        {
            /// <summary>
            /// Edits a Geometry by returning a new Geometry with a modification.
            /// The returned Geometry might be the same as the Geometry passed in.
            /// </summary>
            /// <param name="geometry">The Geometry to modify.</param>
            /// <param name="factory">
            /// The factory with which to construct the modified Geometry
            /// (may be different to the factory of the input point).
            /// </param>
            /// <returns>A new Geometry which is a modification of the input Geometry.</returns>
            IGeometry Edit(IGeometry geometry, IGeometryFactory factory);
        }

        #endregion
    }
}