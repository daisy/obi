using System;
using System.Collections.Generic;
using System.Text;

using UrakawaApplicationBackend;
using urakawa.core;

namespace Zaboom
{
    /// <summary>
    /// A phrase is a block of audio, which is represented by a node in the tree.
    /// </summary>
    class Phrase
    {
        private CoreNode mNode;          // the node in the core tree for this phrase
        private AudioMediaAsset mAsset;  // the audio asset for this phrase

        public Phrase(CoreNode node, AudioMediaAsset asset)
        {
            mNode = node;
            mAsset = asset;
        }
    }
}
