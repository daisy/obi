using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Node factory that can create plain core nodes, or Obi nodes.
    /// </summary>
    public class ObiNodeFactory : CoreNodeFactory
    {
        private Project mProject;  // project that the presenation belongs to
        private int mNextId;       // counter to give node ids

        public Project Project
        {
            set { if (mProject == null) mProject = value; }
        }

        public ObiNodeFactory()
            : base()
        {
            mProject = null;
            mNextId = 0;
        }

        /// <summary>
        /// Create a new node given a QName.
        /// </summary>
        /// <param name="localName">the local part of the qname.</param>
        /// <param name="namespaceUri">the namespace URI of the qname.</param>
        /// <returns>A new node or null if the qname corresponds to no known node.</returns>
        public override CoreNode createNode(string localName, string namespaceUri)
        {
            if (namespaceUri == ObiPropertyFactory.ObiNS)
            {
                // we handle the Obi NS
                if (localName == SectionNode.Name)
                {
                    return new SectionNode(mProject, mNextId++);
                }
                else if (localName == PhraseNode.Name)
                {
                    return new PhraseNode(mProject, mNextId++);
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