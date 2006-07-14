using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// Views of the core tree (table of contents tree view and strip view)
    /// implement this interface which keeps them in sync with the core tree.
    /// </summary>
    public interface ICoreTreeView
    {
        /// <summary>
        /// A new section node has been added in the core tree.
        /// </summary>
        /// <param name="newNode">The newly created node.</param>
        /// <param name="relNode">Its preceding sibling node.</param>
        void AddNewSiblingSection(CoreNode newNode, CoreNode relNode);

        /// <summary>
        /// Add a new heading as a child of the relative node.
        /// </summary>
        /// <param name="newNode">The new heading to add to the tree</param>
        /// <param name="relNode">The parent node for the new heading</param>
        void AddNewChildSection(CoreNode newNode, CoreNode relNode);

        /// <summary>
        /// Edit the label of the currently selected heading.
        /// </summary>
        void BeginEditingNodeLabel(CoreNode node);
    }
}
