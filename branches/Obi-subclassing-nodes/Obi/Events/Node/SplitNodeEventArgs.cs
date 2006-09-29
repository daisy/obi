using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using Obi.Assets;

namespace Obi.Events.Node
{
    public delegate void SplitNodeHandler(object sender, SplitNodeEventArgs e);

    public class SplitNodeEventArgs: NodeEventArgs
    {
        private AudioMediaAsset mNewAsset;  // the new asset created from the split

        public AudioMediaAsset NewAsset
        {
            get { return mNewAsset; }
        }

        public SplitNodeEventArgs(object origin, CoreNode node, AudioMediaAsset newAsset)
            : base(origin, node)
        {
            mNewAsset = newAsset;
        }
    }
}
