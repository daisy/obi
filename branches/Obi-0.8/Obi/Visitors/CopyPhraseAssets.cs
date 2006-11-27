using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;
using Obi.Assets;

namespace Obi.Visitors
{
    /// <summary>
    /// Make copies of the assets
    /// then assign them to the nodes (which are already copies of the data structure)
    /// </summary>
    //md 20060816
    class CopyPhraseAssets : ICoreNodeVisitor
    {
        private AssetManager mAssManager;
        private Obi.Project mProject;

        public CopyPhraseAssets(AssetManager assetManager, Obi.Project project)
        {
            mAssManager = assetManager;
            mProject = project;
        }
        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
            
        }

        public bool preVisit(ICoreNode node)
        {
            if (Project.GetNodeType((CoreNode)node) == NodeType.Phrase)
            {
                AudioMediaAsset asset = (AudioMediaAsset)mAssManager.CopyAsset
                    (Project.GetAudioMediaAsset((CoreNode)node));


                //mostly copied from Project.SetAudioMediaAsset(...)
                //which for some reason couldn't be used directly;
                //i suspect because it also updates the views
                
                AssetProperty prop = (AssetProperty)node.getProperty(typeof(AssetProperty));
                if (prop != null)
                {
                    prop.Asset = asset;
                    mProject.UpdateSeq((CoreNode)node);
                   //md annotation are not asset names anymore
                   //((TextMedia)Project.GetMediaForChannel((CoreNode)node, Project.AnnotationChannel)).setText(asset.Name);
                }
                else
                {
                    throw new Exception("Cannot set an asset on a node lacking an asset property.");
                }

              
            }

            return true;
        }

        #endregion
    }
}
