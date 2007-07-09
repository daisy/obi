using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Node factory that can create plain core nodes, or Obi nodes.
    /// </summary>
    public class ObiNodeFactory : TreeNodeFactory
    {
        private Project mProject;  // project that the presenation belongs to

        public Project Project
        {
            set { if (mProject == null) mProject = value; }
        }

        public ObiNodeFactory()
            : base()
        {
            mProject = null;
        }

        /// <summary>
        /// Create a new node given a QName.
        /// </summary>
        /// <param name="localName">the local part of the qname.</param>
        /// <param name="namespaceUri">the namespace URI of the qname.</param>
        /// <returns>A new node or null if the qname corresponds to no known node.</returns>
        public override TreeNode createNode(string localName, string namespaceUri)
        {
            if (namespaceUri == ObiPropertyFactory.ObiNS)
            {
                // we handle the Obi NS
                if (localName == SectionNode.XUK_ELEMENT_NAME)
                {
                    return new SectionNode(mProject);
                }
                else if (localName == PhraseNode.Name)
                {
                    return new PhraseNode(mProject);
                }
                else
                {
                    return null;
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