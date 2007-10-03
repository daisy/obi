using System;
using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Node factory that can create plain core nodes, or Obi nodes.
    /// </summary>
    public class ObiNodeFactory : TreeNodeFactory
    {
        private Project mProject;  // project that the presenation belongs to

        /// <summary>
        /// Create a new node factory. It is not attached to any project yet.
        /// </summary>
        public ObiNodeFactory(): base() { mProject = null; }

        /// <summary>
        /// Set the project for the node factory. The project cannot be set again.
        /// </summary>
        public Project Project
        {
            set
            {
                if (mProject != null && mProject != value) throw new Exception("Project already set.");
                mProject = value; 
            }
        }

        /// <summary>
        /// Create a new node given a QName.
        /// </summary>
        /// <param name="localName">the local part of the qname.</param>
        /// <param name="namespaceUri">the namespace URI of the qname.</param>
        /// <returns>A new node or null if the qname corresponds to no known node.</returns>
        public override TreeNode createNode(string localName, string namespaceUri)
        {
            if (namespaceUri == Program.OBI_NS)
            {
                if (localName == RootNode.XUK_ELEMENT_NAME)
                {
                    return new RootNode(mProject);
                }
                else if (localName == PhraseNode.XUK_ELEMENT_NAME)
                {
                    return new PhraseNode(mProject);
                }
                else if (localName == SectionNode.XUK_ELEMENT_NAME)
                {
                    return new SectionNode(mProject);
                }
                else
                {
                    throw new Exception(String.Format("Unknown node type `{0}' in Obi namespace.", localName));
                }
            }
            else
            {
                // other namespaces are handled by the default factory.
                return base.createNode(localName, namespaceUri);
            }
        }
    }
}