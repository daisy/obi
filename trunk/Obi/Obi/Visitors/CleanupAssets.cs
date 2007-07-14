using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.core.visitor;
using urakawa.media;
using urakawa.media.data;

namespace Obi.Visitors
{
    /// <summary>
    /// Visitor to collect all the assets per section. 
    /// First, call SetNewDirectory; then call the visitor functions; then call RemoveOldDirectory
    /// </summary>
    class CleanupAssets : ITreeNodeVisitor
    {
        #region ITreeNodeVisitor Members

        //list of lists of audio assets
        //it needs to be this way because we might not be done with a list before the next one starts
        //right now, it will probably happen this way, because of the way the tree is structured (a section contains
        //its phrases then its subsections), but if we ever change this, this code should remain stable.  (*should*).
        private List<List<ManagedAudioMedia>> mAudioAssLists;
        string mAssetDirectory;
        bool mbFlagPostProcessOnly;

        /// <summary>
        /// constructor initializes the asset list
        /// specify post-process if necessary, then the visitor will call CreateExportClips instead of ExportAssets
        /// </summary>
        public CleanupAssets(string assetDirectory, bool postProcessOnly)
        {
            mAssetDirectory = assetDirectory;
            mAudioAssLists = new List<List<ManagedAudioMedia>>();
            mbFlagPostProcessOnly = postProcessOnly;
        }
       
        public void postVisit(TreeNode node)
        {
            int idx = mAudioAssLists.Count - 1;
               
            if (node is Obi.SectionNode && mAudioAssLists.Count > 0 && mAudioAssLists[idx].Count > 0)
            {
               //the name of the audio file will be the ID of the section name
                string sectionAudioPath = mAssetDirectory + node.GetHashCode().ToString() + @".wav";

                //if we are doing the first phase of the cleanup (ExportAssets)
                if (mbFlagPostProcessOnly == false)
                {
                    //call asset combining function here
                    // TODO export
                    // Assets.AudioMediaAsset.ExportAssets(mAudioAssLists[idx], sectionAudioPath);

                }
                //otherwise the visitor should call CreateExportClips (the second phrase of the cleanup)
                else
                {
                    // TODO exportclips
                    List<ManagedAudioMedia> revisedAssList = null; // Assets.AudioMediaAsset.CreateExportClips(mAudioAssLists[idx], sectionAudioPath);
                    //make sure we have one asset per phrase
                    if (revisedAssList.Count != ((SectionNode)node).PhraseChildCount)
                    {
                        throw new Exception("Error during cleanup of audio assets");
                    }

                    //replace current assets with revised ones
                    for (int i = 0; i < ((SectionNode)node).PhraseChildCount; i++)
                    {
                        PhraseNode phraseNode = ((SectionNode)node).PhraseChild(i);
                        //causes an error: phraseNode.Asset.Manager.RemoveAsset(phraseNode.Asset);
                        phraseNode.Audio = revisedAssList[i];
                        //causes an error: phraseNode.Asset.Manager.AddAsset(phraseNode.Asset);
                    }
                }
            }

            //this logic is separate in case we have an empty section
            //we still want to remove its audio asset list (even though it might be an empty list)
            if (node is Obi.SectionNode)
            {
                mAudioAssLists.Remove(mAudioAssLists[idx]);
            }
        }

        public bool preVisit(TreeNode node)
        {
            if (node is Obi.SectionNode)
            {
                List<ManagedAudioMedia> audioAssList = new List<ManagedAudioMedia>();
                mAudioAssLists.Add(audioAssList);
            }
            else if (node is Obi.PhraseNode)
            {
                if (mAudioAssLists.Count <= 0) throw new Exception("Error while collecting assets for cleanup");
                mAudioAssLists[mAudioAssLists.Count - 1].Add(((Obi.PhraseNode)node).Audio);
            }

            return true;

        }

        #endregion
    }
}
