using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Events.Node
{
    public delegate void SetMediaHandler(object sender, SetMediaEventArgs e);

    public class SetMediaEventArgs: NodeEventArgs
    {
        private string mChannel;  // the channel on which to set
        private IMedia mMedia;    // the media object

        public string Channel
        {
            get
            {
                return mChannel; 
            }
        }

        public IMedia Media
        {
            get
            {
                return mMedia;
            }
        }

        public SetMediaEventArgs(object origin, CoreNode node, string channel, IMedia media): 
            base(origin, node)
        {
            mChannel = channel;
            mMedia = media;
        }
    }
}
