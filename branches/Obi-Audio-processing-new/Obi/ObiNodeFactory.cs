using System;
using urakawa.core;
using urakawa.xuk;

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
                if (
                    XukAble.GetXukName(typeof(ObiRootNode)).Match(localName)
                    //localName == XukAble.GetXukName(typeof(ObiRootNode)).z(m_Presentation.PrettyFormat)
                    )
                {
                    return m_Presentation.TreeNodeFactory.Create<ObiRootNode>();
                }
                else if (
                    XukAble.GetXukName(typeof(PhraseNode)).Match(localName)
                    //localName == XukAble.GetXukName(typeof(PhraseNode)).z(m_Presentation.PrettyFormat)
                    )
                {
                    return m_Presentation.TreeNodeFactory.Create<PhraseNode>();
                }
                else if (
                    XukAble.GetXukName(typeof(SectionNode)).Match(localName)
                    //localName == XukAble.GetXukName(typeof(SectionNode)).z(m_Presentation.PrettyFormat)
                    )
                {
                    return m_Presentation.TreeNodeFactory.Create<SectionNode>();
                }
                else if (XukAble.GetXukName(typeof(EmptyNode)).Match(localName)
                    //localName == XukAble.GetXukName(typeof(EmptyNode)).z(m_Presentation.PrettyFormat)
                    )
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