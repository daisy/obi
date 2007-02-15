using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Visitors
{
    /// <summary>
    /// Visitor to collect all the assets per section
    /// </summary>
    class CollectAssetsForExport : ICoreNodeVisitor
    {
        #region ICoreNodeVisitor Members

        private List<Assets.AudioMediaAsset> mAudioAssList;

        public void postVisit(ICoreNode node)
        {
            if (node is Obi.SectionNode)
            {
                string sectionAudioPath = "";

                //figure out the path to the new audio file

                //call asset combining function here
                Assets.AudioMediaAsset bigAss = Assets.AudioMediaAsset.ExportAssets
                    (mAudioAssList, sectionAudioPath);

                //recalculate all assets; or at least the clip begin/end and filepath data
                //in the pre-existing assets

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
