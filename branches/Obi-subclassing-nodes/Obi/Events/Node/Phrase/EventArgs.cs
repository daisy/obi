namespace Obi.Events.Node.Phrase
{
    /// <summary>
    /// Base class for PhraseNode event arguments.
    /// </summary>
    public class EventArgs : System.EventArgs
    {
        private object mOrigin;    // the origin of the event (initial requester)
        private PhraseNode mNode;  // the node on which the operation is performed

        public object Origin
        {
            get { return mOrigin; }
        }

        public PhraseNode Node
        {
            get { return mNode; }
        }

        public EventArgs(object origin, PhraseNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}