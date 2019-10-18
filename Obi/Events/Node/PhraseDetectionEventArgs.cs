using System;
using urakawa.core;

namespace Obi.Events.Node
{
   
    public class PhraseDetectionEventArgs: PhraseNodeEventArgs
    {
        private long mThreshold;
        private double mGap;
        private double mLeadingSilence;
	
        public long Threshold
        {
            get { return mThreshold; }
        }

        public double Gap
        {
            get { return mGap; }
        }

        public double LeadingSilence
        {
            get { return mLeadingSilence; }
        }

        public PhraseDetectionEventArgs(object origin, PhraseNode node, long threshold, double gap, double leading)
            : base(origin, node)
        {
            mThreshold = threshold;
            mGap = gap;
            mLeadingSilence = leading;
        }
    }
}
