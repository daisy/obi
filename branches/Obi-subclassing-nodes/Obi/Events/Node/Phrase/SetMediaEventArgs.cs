using urakawa.media;

namespace Obi.Events.Node.Phrase
{
    public delegate void SetMediaHandler(object sender, SetMediaEventArgs e);
    public delegate void MediaSetHandler(object sender, SetMediaEventArgs e);

    public class SetMediaEventArgs : EventArgs
    {
        private string mChannel;  // the channel on which to set
        private IMedia mMedia;    // the media object
        private bool mCancel;     // can be cancelled

        public string Channel
        {
            get { return mChannel; }
        }

        public IMedia Media
        {
            get { return mMedia; }
        }

        public bool Cancel
        {
            get { return mCancel; }
            set { mCancel = value; }
        }

        public SetMediaEventArgs(object origin, PhraseNode node, string channel, IMedia media)
            : base(origin, node)
        {
            mChannel = channel;
            mMedia = media;
            mCancel = false;
        }
    }
}