// ********************************************************************************************************
// Product Name: DotSpatial.Controls.dll
// Description:  The Windows Forms user interface controls like the map, legend, toolbox, ribbon and others.
// ********************************************************************************************************
// The contents of this file are subject to the MIT License (MIT)
// you may not use this file except in compliance with the License. You may obtain a copy of the License at
// http://dotspatial.codeplex.com/license
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and
// limitations under the License.
//
// The Original Code is from MapWindow.dll version 6.0
//
// The Initial Developer of this Original Code is Ted Dunsford. Created 11/17/2008 1:37:48 PM
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
//
// ********************************************************************************************************

using DotSpatial.Symbology;

namespace DotSpatial.Controls
{
    /// <summary>
    /// IGeoLabelLayer
    /// </summary>
    public interface IMapLabelLayer : ILabelLayer, IMapLayer
    {
        /// <summary>
        /// Gets or sets the feature layer that this label layer is attached to.
        /// </summary>
        new IMapFeatureLayer FeatureLayer
        {
            get;
            set;
        }

        /// <summary>
        /// Resolves ambiguity
        /// </summary>
        new void Invalidate();
    }
}