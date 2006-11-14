using System;
using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void RequestToApplyPhraseDetectionHandler(object sender, PhraseDetectionEventArgs e);
 
    public class PhraseDetectionEventArgs: NodeEventArgs
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

        public PhraseDetectionEventArgs(object origin, CoreNode node, long threshold, double gap, double leading)
            : base(origin, node)
        {
            mThreshold = threshold;
            mGap = gap;
            mLeadingSilence = leading;
        }
    }
}
