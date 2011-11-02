using System;
using System.Collections.Generic;
using System.Text;


using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio ;



namespace Obi.Events.Node
{
  
    public class SplitPhraseNodeEventArgs: PhraseNodeEventArgs
    {
        private ManagedAudioMedia mNewAudio;  // the new audio from the split

        public SplitPhraseNodeEventArgs(object origin, PhraseNode node, ManagedAudioMedia newAudio)
            : base(origin, node)
        {
            mNewAudio = newAudio;
        }

        public ManagedAudioMedia NewAsset { get { return mNewAudio; } }
    }
}
