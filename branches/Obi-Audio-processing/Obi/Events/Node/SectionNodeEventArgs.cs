using System;
using urakawa.core;

namespace Obi.Events.Node
{
    /// <summary>
    /// Communicate events about section nodes
    /// </summary>
    public class SectionNodeEventArgs
    {
        private object mOrigin;
        private SectionNode mNode;

        /// <summary>
        /// Origin of the event.
        /// </summary>
        public object Origin
        {
            get { return mOrigin; }
        }

        /// <summary>
        /// Section node on which the action was performed.
        /// </summary>
        public SectionNode Node
        {
            get { return mNode; }
        }

        /// <param name="origin">Who is originally responsible for this event.</param>
        /// <param name="node">Which node is concerned.</param>
        public SectionNodeEventArgs(object origin, SectionNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }

    public class SectionNodeHeadingEventArgs : SectionNodeEventArgs
    {
        private PhraseNode mPreviousHeading;

        public PhraseNode PreviousHeading
        {
            get { return mPreviousHeading; }
        }

        public SectionNodeHeadingEventArgs(object origin, SectionNode node, PhraseNode previous)
            : base(origin, node)
        {
            mPreviousHeading = previous;
        }
    }
}
