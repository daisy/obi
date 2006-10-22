using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Events.Node
{
    public delegate void SetMediaHandler(object sender, SetMediaEventArgs e);
    public delegate void PhraseAudioSetHandler(object sender, PhraseNode node, SetMediaEventArgs e);

    /// <summary>
    /// This event is fired when a media object has been set on a node.
    /// The node is passed as another argument of the handler.
    /// This event is cancellable (use the Cancel property.)
    /// </summary>
    public class SetMediaEventArgs
    {
        private string mChannel;
        private IMedia mMedia;
        private bool mCancel;

        /// <summary>
        /// The channel on which the media is to be set.
        /// </summary>
        public string Channel
        {
            get { return mChannel; }
        }

        /// <summary>
        /// The media object to set.
        /// </summary>
        public IMedia Media
        {
            get { return mMedia; }
        }

        /// <summary>
        /// If this flag is true, the event is cancelled.
        /// </summary>
        public bool Cancel
        {
            get { return mCancel; }
            set { mCancel = value; }
        }

        /// <summary>
        /// Create a new event.
        /// </summary>
        /// <param name="channel">The channel on which to set a media object.</param>
        /// <param name="media">The media object to set on this channel.</param>
        public SetMediaEventArgs(string channel, IMedia media)
        {
            mChannel = channel;
            mMedia = media;
            mCancel = false;
        }
    }
}
