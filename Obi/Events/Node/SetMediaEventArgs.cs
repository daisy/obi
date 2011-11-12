

namespace Obi.Events.Node
{
    public class SetMediaEventArgs: PhraseNodeEventArgs
    {
        private string mChannel;  // the channel on which to set
        //private IMedia mMedia;    // the media object
        private urakawa.media.Media mMedia;    // the media object //sdk2
        private bool mCancel;     // can be cancelled

        public string Channel
        {
            get
            {
                return mChannel; 
            }
        }

        //public IMedia Media
        public urakawa.media.Media Media//sdk2
        {
            get
            {
                return mMedia;
            }
        }

        public bool Cancel
        {
            get { return mCancel; }
            set { mCancel = value; }
        }

        //public SetMediaEventArgs(object origin, PhraseNode node, string channel, IMedia media): 
        public SetMediaEventArgs(object origin, PhraseNode node, string channel, urakawa.media.Media media)//sdk2
            : 
            base(origin, node)
        {
            mChannel = channel;
            mMedia = media;
            mCancel = false;
        }
    }
}
