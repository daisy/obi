using System;
using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Node factory that can create plain core nodes, or Obi nodes.
    /// </summary>
    public class ObiNodeFactory 
    {
        private Presentation m_Presentation;

        public ObiNodeFactory(Presentation presentation)
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
            //base.CreateNode(localName, namespaceUri);
            return m_Presentation.TreeNodeFactory.Create(localName, namespaceUri);//sdk2
        }

        //public override string getXukNamespaceUri() { return DataModelFactory.NS; }
        public string getXukNamespaceUri() { return DataModelFactory.NS; }//sdk2

        // Override getPresentation() to return an Obi-specific presentation
        private Presentation Presentation { get { return m_Presentation; } }
    }
}