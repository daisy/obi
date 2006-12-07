using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Visitors
{
    /// <summary>
    /// Visitor building a flat list of all descendant nodes (including the root.)
    /// </summary>
    public class DescendantsVisitor: ICoreNodeVisitor
    {
        private List<CoreNode> mNodes;
        //md 20061130 extended with specific node types
        private List<SectionNode> mSectionNodes;
        private List<PhraseNode> mPhraseNodes;

        public List<CoreNode> Nodes
        {
            get
            {
                return mNodes;
            }
        }

        public List<SectionNode> SectionNodes
        {
            get
            {
                return mSectionNodes;
            }
        }

        public List<PhraseNode> PhraseNodes
        {
            get
            {
                return mPhraseNodes;
            }
        }

        public DescendantsVisitor()
        {
            mNodes = new List<CoreNode>();
            mSectionNodes = new List<SectionNode>();
            mPhraseNodes = new List<PhraseNode>();
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
        }

        public bool preVisit(ICoreNode node)
        {
            mNodes.Add((CoreNode)node);
            if (node.GetType() == Type.GetType("Obi.SectionNode")) mSectionNodes.Add((SectionNode)node);
            if (node.GetType() == Type.GetType("Obi.PhraseNode")) mPhraseNodes.Add((PhraseNode)node);

            return true;
        }

        #endregion
    }
}
