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

        private List<Assets.AudioMediaAsset> mAudioAssList;
        private string mNewAssetDirectory;
        private string mOldAssetDirectory;

        /// <summary>
        /// the constructor makes a new directory for the assets
        /// </summary>
        public void SetNewDirectory()
        {
            mOldAssetDirectory = ((SectionNode)node).Project.AssetManager.AssetsDirectory;

            //these assets go in a new directory (old_dir_name + underscore(s))
            mNewAssetDirectory = ((SectionNode)node).Project.AssignNewAssetDirectory();
        }

        public void RemoveOldDirectory()
        {
            System.IO.Directory.Delete(mOldAssetDirectory, true);
        }

        public void postVisit(ICoreNode node)
        {
            if (node is Obi.SectionNode)
            { 
                //the name of the audio file will be the ID of the section name
                string sectionAudioPath = mNewAssetDirectory + @"\" + ((SectionNode)node).Id + @".wav";

                //call asset combining function here
                List<Assets.AudioMediaAsset> revisedAssList = Assets.AudioMediaAsset.ExportAssets
                    (mAudioAssList, sectionAudioPath);

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
        }

        public bool preVisit(ICoreNode node)
        {
            if (node is Obi.SectionNode)
            {
                mAudioAssList.Clear();
            }
            else if (node is Obi.PhraseNode)
            {
                mAudioAssList.Add(((Obi.PhraseNode)node).Asset);
            }

            return true;

        }

        #endregion
    }
}
