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

using System.Text;

namespace DotSpatial.Topology.GeometriesGraph
{
    /// <summary>
    /// A <c>Label</c> indicates the topological relationship of a component
    /// of a topology graph to a given <c>Geometry</c>.
    /// This class supports labels for relationships to two <c>Geometry</c>s,
    /// which is sufficient for algorithms for binary operations.
    /// Topology graphs support the concept of labeling nodes and edges in the graph.
    /// The label of a node or edge specifies its topological relationship to one or
    /// more geometries.  (In fact, since NTS operations have only two arguments labels
    /// are required for only two geometries).  A label for a node or edge has one or
    /// two elements, depending on whether the node or edge occurs in one or both of the
    /// input <c>Geometry</c>s.  Elements contain attributes which categorize the
    /// topological location of the node or edge relative to the parent
    /// <c>Geometry</c>; that is, whether the node or edge is in the interior,
    /// boundary or exterior of the <c>Geometry</c>.  Attributes have a value
    /// from the set <c>{Interior, Boundary, Exterior}</c>.  In a node each
    /// element has a single attribute <c>On</c>. For an edge each element has a
    /// triplet of attributes <c>Left, On, Right</c>.
    /// It is up to the client code to associate the 0 and 1 <c>TopologyLocation</c>s
    /// with specific geometries.
    /// </summary>
    public class Label
    {
        private readonly TopologyLocation[] _elt = new TopologyLocation[2];

        /// <summary>
        /// Construct a Label with a single location for both Geometries.
        /// Initialize the locations to Null.
        /// </summary>
        /// <param name="onLoc"></param>
        public Label(LocationType onLoc)
        {
            _elt[0] = new TopologyLocation(onLoc);
            _elt[1] = new TopologyLocation(onLoc);
        }

        /// <summary>
        /// Construct a Label with a single location for both Geometries.
        /// Initialize the location for the Geometry index.
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="onLoc"></param>
        public Label(int geomIndex, LocationType onLoc)
        {
            _elt[0] = new TopologyLocation(LocationType.Null);
            _elt[1] = new TopologyLocation(LocationType.Null);
            _elt[geomIndex].SetLocation(onLoc);
        }

        /// <summary>
        /// Construct a Label with On, Left and Right locations for both Geometries.
        /// Initialize the locations for both Geometries to the given values.
        /// </summary>
        /// <param name="onLoc"></param>
        /// <param name="leftLoc"></param>
        /// <param name="rightLoc"></param>
        public Label(LocationType onLoc, LocationType leftLoc, LocationType rightLoc)
        {
            _elt[0] = new TopologyLocation(onLoc, leftLoc, rightLoc);
            _elt[1] = new TopologyLocation(onLoc, leftLoc, rightLoc);
        }

        /// <summary>
        /// Construct a Label with On, Left and Right locations for both Geometries.
        /// Initialize the locations for the given Geometry index.
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="onLoc"></param>
        /// <param name="leftLoc"></param>
        /// <param name="rightLoc"></param>
        public Label(int geomIndex, LocationType onLoc, LocationType leftLoc, LocationType rightLoc)
        {
            _elt[0] = new TopologyLocation(LocationType.Null, LocationType.Null, LocationType.Null);
            _elt[1] = new TopologyLocation(LocationType.Null, LocationType.Null, LocationType.Null);
            _elt[geomIndex].SetLocations(onLoc, leftLoc, rightLoc);
        }

        /// <summary>
        /// Construct a Label with the same values as the argument for the
        /// given Geometry index.
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="gl"></param>
        public Label(int geomIndex, TopologyLocation gl)
        {
            _elt[0] = new TopologyLocation(gl.GetLocations());
            _elt[1] = new TopologyLocation(gl.GetLocations());
            _elt[geomIndex].SetLocations(gl);
        }

        /// <summary>
        /// Construct a Label with the same values as the argument Label.
        /// </summary>
        /// <param name="lbl"></param>
        public Label(Label lbl)
        {
            _elt[0] = new TopologyLocation(lbl._elt[0]);
            _elt[1] = new TopologyLocation(lbl._elt[1]);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual int GeometryCount
        {
            get
            {
                int count = 0;
                if (!_elt[0].IsNull)
                    count++;
                if (!_elt[1].IsNull)
                    count++;
                return count;
            }
        }

        /// <summary>
        /// Converts a Label to a Line label (that is, one with no side Locations).
        /// </summary>
        /// <param name="label">Label to convert.</param>
        /// <returns>Label as Line label.</returns>
        public static Label ToLineLabel(Label label)
        {
            Label lineLabel = new Label(LocationType.Null);
            for (int i = 0; i < 2; i++)
                lineLabel.SetLocation(i, label.GetLocation(i));
            return lineLabel;
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void Flip()
        {
            _elt[0].Flip();
            _elt[1].Flip();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="posIndex"></param>
        /// <returns></returns>
        public virtual LocationType GetLocation(int geomIndex, PositionType posIndex)
        {
            return _elt[geomIndex].Get(posIndex);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <returns></returns>
        public virtual LocationType GetLocation(int geomIndex)
        {
            return _elt[geomIndex].Get(PositionType.On);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="posIndex"></param>
        /// <param name="location"></param>
        public virtual void SetLocation(int geomIndex, PositionType posIndex, LocationType location)
        {
            _elt[geomIndex].SetLocation(posIndex, location);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="location"></param>
        public virtual void SetLocation(int geomIndex, LocationType location)
        {
            _elt[geomIndex].SetLocation(PositionType.On, location);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="location"></param>
        public virtual void SetAllLocations(int geomIndex, LocationType location)
        {
            _elt[geomIndex].SetAllLocations(location);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="location"></param>
        public virtual void SetAllLocationsIfNull(int geomIndex, LocationType location)
        {
            _elt[geomIndex].SetAllLocationsIfNull(location);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="location"></param>
        public virtual void SetAllLocationsIfNull(LocationType location)
        {
            SetAllLocationsIfNull(0, location);
            SetAllLocationsIfNull(1, location);
        }

        /// <summary>
        /// Merge this label with another one.
        /// Merging updates any null attributes of this label with the attributes from lbl.
        /// </summary>
        /// <param name="lbl"></param>
        public virtual void Merge(Label lbl)
        {
            for (int i = 0; i < 2; i++)
            {
                if (_elt[i] == null && lbl._elt[i] != null)
                    _elt[i] = new TopologyLocation(lbl._elt[i]);
                else _elt[i].Merge(lbl._elt[i]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <returns></returns>
        public virtual bool IsNull(int geomIndex)
        {
            return _elt[geomIndex].IsNull;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <returns></returns>
        public virtual bool IsAnyNull(int geomIndex)
        {
            return _elt[geomIndex].IsAnyNull;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual bool IsArea()
        {
            return _elt[0].IsArea || _elt[1].IsArea;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <returns></returns>
        public virtual bool IsArea(int geomIndex)
        {
            return _elt[geomIndex].IsArea;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <returns></returns>
        public virtual bool IsLine(int geomIndex)
        {
            return _elt[geomIndex].IsLine;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public virtual bool IsEqualOnSide(Label lbl, int side)
        {
            return _elt[0].IsEqualOnSide(lbl._elt[0], side)
                && _elt[1].IsEqualOnSide(lbl._elt[1], side);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geomIndex"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        public virtual bool AllPositionsEqual(int geomIndex, LocationType loc)
        {
            return _elt[geomIndex].AllPositionsEqual(loc);
        }

        /// <summary>
        /// Converts one GeometryLocation to a Line location.
        /// </summary>
        /// <param name="geomIndex"></param>
        public virtual void ToLine(int geomIndex)
        {
            if (_elt[geomIndex].IsArea)
                _elt[geomIndex] = new TopologyLocation(_elt[geomIndex].GetLocations()[0]);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_elt[0] != null)
            {
                sb.Append("a:");
                sb.Append(_elt[0].ToString());
            }
            if (_elt[1] != null)
            {
                sb.Append(" b:");
                sb.Append(_elt[1].ToString());
            }
            return sb.ToString();
        }
    }
}