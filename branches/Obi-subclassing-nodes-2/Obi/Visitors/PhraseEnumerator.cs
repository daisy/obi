using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Visitors
{
    /// <summary>
    /// Get the list of phrases in the book in order.
    /// </summary>
    class PhraseEnumerator: ICoreNodeVisitor 
    {
        private List<CoreNode> mPhrases;

        public List<CoreNode> Phrases
        {
            get
            {
                return mPhrases;
            }
        }

        public PhraseEnumerator()
        {
            mPhrases = new List<CoreNode>();
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
        }

        public bool preVisit(ICoreNode node)
        {
            if (node.GetType() == System.Type.GetType("Obi.PhraseNode"))
            {
                mPhrases.Add((CoreNode)node);
            }
            return true;
        }

        #endregion
    }
}
