using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Strips
{
    public class ParStrip: Strip
    {
        private string mLabel;         // the strip label
        private CoreNode mNode;        // the core node corresponding to this strip
        private SequenceMedia mAudio;  // heading audio object

        public delegate void LabelChangedHandler(object sender, LabelChangedEventArgs e);
        public event LabelChangedHandler LabelChanged;

        public string Label
        {
            get
            {
                return mLabel;
            }
            set
            {
                string old_label = mLabel;
                mLabel = value;
                LabelChanged(this, new LabelChangedEventArgs(old_label, mLabel, false));
            }
        }

        /// <summary>
        /// Audio to the heading
        /// </summary>
        public SequenceMedia Audio
        {
            get
            {
                return mAudio;
            }
            set
            {
                Presentation presentation = (Presentation)mNode.getPresentation();
                ChannelsProperty audioprop = (ChannelsProperty)presentation.getPropertyFactory().createChannelsProperty();
                ChannelsManager manager = (ChannelsManager)presentation.getChannelsManager();
                Channel audiochan = (Channel)((manager.getChannelByName(Project.AUDIO_CHANNEL)[0]));
                audioprop.setMedia(audiochan, value);
                mNode.setProperty(audioprop);
            }
        }

        public ParStrip()
        {
            mLabel = null;
            mNode = null;
            mAudio = null;
        }

        public ParStrip(string label, CoreNode node)
        {
            mLabel = label;
            mNode = node;
            mAudio = null;
        }

        public override string ToString()
        {
            return String.Format("<{0}> \"{1}\"", base.ToString(), mLabel);
        }
    }
}
