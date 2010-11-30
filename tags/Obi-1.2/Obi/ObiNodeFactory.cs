using System;
using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Node factory that can create plain core nodes, or Obi nodes.
    /// </summary>
    public class ObiNodeFactory : TreeNodeFactory
    {
        /// <summary>
        /// Create a new node given a QName.
        /// </summary>
        /// <param name="localName">the local part of the qname.</param>
        /// <param name="namespaceUri">the namespace URI of the qname.</param>
        /// <returns>A new node or null if the qname corresponds to no known node.</returns>
        public override TreeNode createNode(string localName, string namespaceUri)
        {
            if (namespaceUri == DataModelFactory.NS)
            {
                if (localName == RootNode.XUK_ELEMENT_NAME)
                {
                    return new RootNode(Presentation);
                }
                else if (localName == EmptyNode.XUK_ELEMENT_NAME)
                {
                    return new EmptyNode(Presentation);
                }
                else if (localName == PhraseNode.XUK_ELEMENT_NAME)
                {
                    return new PhraseNode(Presentation);
                }
                else if (localName == SectionNode.XUK_ELEMENT_NAME)
                {
                    return new SectionNode(Presentation);
                }
            }
            return base.createNode(localName, namespaceUri);
        }

        public override string getXukNamespaceUri() { return DataModelFactory.NS; }

        // Override getPresentation() to return an Obi-specific presentation
        private Presentation Presentation { get { return (Presentation)getPresentation(); } }
    }
}