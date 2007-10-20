using System;
using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Node factory that can create plain core nodes, or Obi nodes.
    /// </summary>
    public class ObiNodeFactory : TreeNodeFactory
    {
        private Presentation mPresentation;

        /// <summary>
        /// Create a new node factory. It is not attached to any project yet.
        /// </summary>
        public ObiNodeFactory(): base() { mPresentation = null; }

        public Presentation Presentation
        {
            set
            {
                if (mPresentation != null) throw new Exception("Presentation already set!");
                mPresentation = value;
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
            if (namespaceUri == DataModelFactory.NS)
            {
                if (localName == RootNode.XUK_ELEMENT_NAME)
                {
                    return new RootNode(mPresentation);
                }
                else if (localName == PhraseNode.XUK_ELEMENT_NAME)
                {
                    return new PhraseNode(mPresentation);
                }
                else if (localName == SectionNode.XUK_ELEMENT_NAME)
                {
                    return new SectionNode(mPresentation);
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