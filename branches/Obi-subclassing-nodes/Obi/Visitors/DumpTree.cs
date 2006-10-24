using System;
using urakawa.core;

namespace Obi.Visitors
{
    /// <summary>
    /// Visitor to dump the core tree (or subtree) to the debug console.
    /// </summary>
    public class DumpTree: ICoreNodeVisitor
    {
        private string indent = "+ ";

        // Remove one indentation level
        public void postVisit(ICoreNode node)
        {
            indent = indent.Substring(2);
        }

        // Dump the content of this node and increase the indentation level for its children.
        public bool preVisit(ICoreNode node)
        {
            string info = String.Format("{0}{1}", indent, node.GetType());
            if (node is ObiNode) info += ((ObiNode)node).InfoString;
            System.Diagnostics.Debug.Print(info);
            indent = "  " + indent;
            return true;
        }
    }
}
