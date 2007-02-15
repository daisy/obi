using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Visitors
{
    /// <summary>
    /// Visitor to collect all the assets per section. 
    /// First, call SetNewDirectory; then call the visitor functions; then call RemoveOldDirectory
    /// </summary>
    class CleanupAssets : ICoreNodeVisitor
    {
        #region ICoreNodeVisitor Members

        //list of lists of audio assets
        //it needs to be this way because we might not be done with a list before the next one starts
        //right now, it will probably happen this way, because of the way the tree is structured (a section contains
        //its phrases then its subsections), but if we ever change this, this code should remain stable.  (*should*).
        private List<List<Assets.AudioMediaAsset>> mAudioAssLists;
        private string mNewAssetDirectory;
        private string mOldAssetDirectory;

        /// <summary>
        /// the constructor makes a new directory for the assets
        /// </summary>
        public void SetNewDirectory(Project project)
        {
            mOldAssetDirectory = project.AssetManager.AssetsDirectory;

            //these assets go in a new directory (old_dir_name + underscore(s))
            mNewAssetDirectory = project.AssignNewAssetDirectory();

            mAudioAssLists = new List<List<Obi.Assets.AudioMediaAsset>>();
        }

        public void RemoveOldDirectory()
        {
            System.IO.Directory.Delete(mOldAssetDirectory, true);
        }

        public void postVisit(ICoreNode node)
        {
            int idx = mAudioAssLists.Count - 1;
               
            if (node is Obi.SectionNode && mAudioAssLists.Count > 0 && mAudioAssLists[idx].Count > 0)
            { 
                //the name of the audio file will be the ID of the section name
                string sectionAudioPath = mNewAssetDirectory + @"\" + ((SectionNode)node).Id + @".wav";

                //call asset combining function here
                List<Assets.AudioMediaAsset> revisedAssList = Assets.AudioMediaAsset.ExportAssets
                    (mAudioAssLists[idx], sectionAudioPath);

                //make sure we have one asset per phrase
                if (revisedAssList.Count != ((SectionNode)node).PhraseChildCount)
                {
                    throw new Exception("Error during cleanup of audio assets");
                }

                //replace current assets with revised ones
                for (int i = 0; i < ((SectionNode)node).PhraseChildCount; i++)
                {
                    PhraseNode phraseNode = ((SectionNode)node).PhraseChild(i);
                    phraseNode.Asset = revisedAssList[i];
                }
            }

            //this logic is separate in case we have an empty section
            //we still want to remove its audio asset list (even though it might be an empty list)
            if (node is Obi.SectionNode)
            {
                mAudioAssLists.Remove(mAudioAssLists[idx]);
            }
        }

        public bool preVisit(ICoreNode node)
        {
            if (node is Obi.SectionNode)
            {
                List<Assets.AudioMediaAsset> audioAssList = new List<Obi.Assets.AudioMediaAsset>();
                mAudioAssLists.Add(audioAssList);
            }
            else if (node is Obi.PhraseNode)
            {
                if (mAudioAssLists.Count <= 0) throw new Exception("Error while collecting assets for cleanup");
                mAudioAssLists[mAudioAssLists.Count - 1].Add(((Obi.PhraseNode)node).Asset);
            }

            return true;

        }

        #endregion
    }
}
