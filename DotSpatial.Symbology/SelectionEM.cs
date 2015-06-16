// ********************************************************************************************************
// Product Name: DotSpatial.Symbology.dll
// Description:  Contains the business logic for symbology layers and symbol categories.
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
// The Initial Developer of this Original Code is Ted Dunsford. Created 3/24/2009 9:39:52 AM
//
// Contributor(s): (Open source contributors should list themselves and their modifications here).
//
// ********************************************************************************************************

using DotSpatial.Topology;

namespace DotSpatial.Symbology
{
    /// <summary>
    /// SelectionEM
    /// </summary>
    public static class SelectionEM
    {
        #region Methods

        /// <summary>
        /// Inverts the selection based on the current SelectionMode
        /// </summary>
        /// <param name="self">The ISelection to invert</param>
        /// <param name="region">The geographic region to reverse the selected state</param>
        public static bool InvertSelection(this ISelection self, IEnvelope region)
        {
            IEnvelope ignoreMe;
            return self.InvertSelection(region, out ignoreMe);
        }

        #endregion
    }
}