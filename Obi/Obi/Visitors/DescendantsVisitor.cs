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
        private List<CoreNode> mNodes;            // flat list of all nodes
        private List<SectionNode> mSectionNodes;  // only the section nodes
        private List<PhraseNode> mPhraseNodes;    // only the phrase nodes

        /// <summary>
        /// List of all descendant nodes regardless of their type.
        /// </summary>
        public List<CoreNode> Nodes
        {
            get { return mNodes; }
        }

        /// <summary>
        /// List of all descendant section nodes.
        /// </summary>
        public List<SectionNode> SectionNodes
        {
            get { return mSectionNodes; }
        }

        /// <summary>
        /// List of all descendant phrase nodes.
        /// </summary>
        public List<PhraseNode> PhraseNodes
        {
            get { return mPhraseNodes; }
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
            if (node is SectionNode) mSectionNodes.Add((SectionNode)node);
            if (node is PhraseNode) mPhraseNodes.Add((PhraseNode)node);
            return true;
        }

        #endregion
    }
}
