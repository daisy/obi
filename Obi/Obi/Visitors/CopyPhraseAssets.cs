using System;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.media;
using urakawa.media.data;

namespace Obi.Visitors
{
    /// <summary>
    /// Make copies of the assets
    /// then assign them to the nodes (which are already copies of the data structure)
    /// </summary>
    class CopyPhraseAssets : ITreeNodeVisitor
    {
        private Obi.Project mProject;

        public CopyPhraseAssets()
        {
        }

        #region ITreeNodeVisitor Members

        public void postVisit(TreeNode node)
        {    
        }

        public bool preVisit(TreeNode node)
        {
            PhraseNode phrase = node as PhraseNode;
            if (phrase != null) phrase.Audio = phrase.Project.CopyAudioMedia(phrase.Audio);
            return true;
        }

        #endregion
    }
}
