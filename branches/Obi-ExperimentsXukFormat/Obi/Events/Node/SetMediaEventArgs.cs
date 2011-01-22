using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Events.Node
{
    public class SetMediaEventArgs: PhraseNodeEventArgs
    {
        private string mChannel;  // the channel on which to set
        //private IMedia mMedia;    // the media object
        private Media mMedia;    // the media object //sdk2
        private bool mCancel;     // can be cancelled

        public string Channel
        {
            get
            {
                return mChannel; 
            }
        }

        //public IMedia Media
        public Media Media//sdk2
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
        public SetMediaEventArgs(object origin, PhraseNode node, string channel, Media media)//sdk2
            : 
            base(origin, node)
        {
            mChannel = channel;
            mMedia = media;
            mCancel = false;
        }
    }
}
