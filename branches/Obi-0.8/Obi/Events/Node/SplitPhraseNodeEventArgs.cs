using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using Obi.Assets;

namespace Obi.Events.Node
{
  
    public class SplitPhraseNodeEventArgs: PhraseNodeEventArgs
    {
        private AudioMediaAsset mNewAsset;  // the new asset created from the split

        public AudioMediaAsset NewAsset
        {
            get { return mNewAsset; }
        }

        public SplitPhraseNodeEventArgs(object origin, PhraseNode node, AudioMediaAsset newAsset)
            : base(origin, node)
        {
            mNewAsset = newAsset;
        }
    }
}
