using System;
using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Node factory that can create plain core nodes, or Obi nodes.
    /// </summary>
    public class ObiNodeFactory 
    {
        private ObiPresentation m_Presentation;

        public ObiNodeFactory(ObiPresentation presentation)
        {
            m_Presentation = presentation;
        }

        /// <summary>
        /// Create a new node given a QName.
        /// </summary>
        /// <param name="localName">the local part of the qname.</param>
        /// <param name="namespaceUri">the namespace URI of the qname.</param>
        /// <returns>A new node or null if the qname corresponds to no known node.</returns>
        public TreeNode createNode(string localName, string namespaceUri)
        {
            if (namespaceUri == DataModelFactory.NS)
            {
                //if (localName == RootNode.XUK_ELEMENT_NAME)//sdk2
                //{
                    //return new RootNode(Presentation);
                //}
                if (localName == EmptyNode.XUK_ELEMENT_NAME)
                {
                    return new EmptyNode(m_Presentation);
                }
                else if (localName == PhraseNode.XUK_ELEMENT_NAME)
                {
                    return new PhraseNode(m_Presentation);
                }
                else if (localName == SectionNode.XUK_ELEMENT_NAME)
                {
                    return new SectionNode(m_Presentation);
                }
            }
            //base.CreateNode(localName, namespaceUri);
            return m_Presentation.TreeNodeFactory.Create(localName, namespaceUri);//sdk2
        }

        //sdk2
        //public override string getXukNamespaceUri() { return DataModelFactory.NS; }
    }
}