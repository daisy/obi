using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.core.visitor;

namespace Obi.Visitors
{
    /// <summary>
    /// Get the list of phrases in the book in order.
    /// </summary>
    class PhraseEnumerator: ITreeNodeVisitor 
    {
        private List<TreeNode> mPhrases;

        public List<TreeNode> Phrases
        {
            get
            {
                return mPhrases;
            }
        }

        public PhraseEnumerator()
        {
            mPhrases = new List<TreeNode>();
        }

        #region ITreeNodeVisitor Members

        public void postVisit(TreeNode node)
        {
        }

        public bool preVisit(TreeNode node)
        {
            if (node.GetType() == System.Type.GetType("Obi.PhraseNode"))
            {
                mPhrases.Add((TreeNode)node);
            }
            return true;
        }

        #endregion
    }
}
