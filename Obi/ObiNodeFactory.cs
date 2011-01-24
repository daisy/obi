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

        private string cachePhraseNode_XUK_ELEMENT_NAME = null;
        private string cacheRootNode_XUK_ELEMENT_NAME = null;
        private string cacheSectionNode_XUK_ELEMENT_NAME = null;
        private string cacheEmptyNode_XUK_ELEMENT_NAME = null;

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
                if (string.IsNullOrEmpty(cachePhraseNode_XUK_ELEMENT_NAME))
                {
                    cachePhraseNode_XUK_ELEMENT_NAME = new PhraseNode().XUK_ELEMENT_NAME;
                }
                if (string.IsNullOrEmpty(cacheRootNode_XUK_ELEMENT_NAME))
                {
                    cacheRootNode_XUK_ELEMENT_NAME = new ObiRootNode().XUK_ELEMENT_NAME;
                }
                if (string.IsNullOrEmpty(cacheEmptyNode_XUK_ELEMENT_NAME))
                {
                    cacheEmptyNode_XUK_ELEMENT_NAME = new EmptyNode().XUK_ELEMENT_NAME;
                }
                if (string.IsNullOrEmpty(cacheSectionNode_XUK_ELEMENT_NAME))
                {
                    cacheSectionNode_XUK_ELEMENT_NAME = new SectionNode().XUK_ELEMENT_NAME;
                }

                if (localName == cacheRootNode_XUK_ELEMENT_NAME)
                    {
                        return m_Presentation.TreeNodeFactory.Create<ObiRootNode>();
                    }
                else if (localName == cachePhraseNode_XUK_ELEMENT_NAME)
                    {
                        return m_Presentation.TreeNodeFactory.Create<PhraseNode>();
                }
                else if (localName == cacheSectionNode_XUK_ELEMENT_NAME)
                {
                    return m_Presentation.TreeNodeFactory.Create<SectionNode>();
                }
                else if (localName == cacheEmptyNode_XUK_ELEMENT_NAME)
                {
                    return m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                }
            }
            //base.CreateNode(localName, namespaceUri);
            return m_Presentation.TreeNodeFactory.Create(localName, namespaceUri);//sdk2
        }

        //sdk2
        //public override string getXukNamespaceUri() { return DataModelFactory.NS; }
    }
}