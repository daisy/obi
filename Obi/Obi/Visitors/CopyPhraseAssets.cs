using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.core.visitor;
using urakawa.media;
using Obi.Assets;

namespace Obi.Visitors
{
    /// <summary>
    /// Make copies of the assets
    /// then assign them to the nodes (which are already copies of the data structure)
    /// </summary>
    //md 20060816
    class CopyPhraseAssets : ITreeNodeVisitor
    {
        private AssetManager mAssManager;
        private Obi.Project mProject;

        public CopyPhraseAssets(AssetManager assetManager, Obi.Project project)
        {
            mAssManager = assetManager;
            mProject = project;
        }
        #region ITreeNodeVisitor Members

        public void postVisit(TreeNode node)
        {
            
        }

        public bool preVisit(TreeNode node)
        {
            if (node.GetType() == System.Type.GetType("Obi.PhraseNode"))
            {
                AudioMediaAsset asset = (AudioMediaAsset)mAssManager.CopyAsset
                    (((PhraseNode)node).Asset);


                //mostly copied from Project.SetAudioMediaAsset(...)
                //which for some reason couldn't be used directly;
                //i suspect because it also updates the views

                 ((PhraseNode)node).Asset = asset;
                 mProject.UpdateSeq((PhraseNode)node);
                 
            }

            return true;
        }

        #endregion
    }
}
