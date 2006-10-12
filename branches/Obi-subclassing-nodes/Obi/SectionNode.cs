using urakawa.core;
using urakawa.exception;
using urakawa.media;

namespace Obi
{
    public class SectionNode : ObiNode
    {
        public static readonly string Name = "section";

        private ChannelsProperty mChannel;  // quick reference to the channel property
        private TextMedia mMedia;           // quick reference to the text media object
        private string mLabel;              // string version of the text

        public string Label
        {
            get { return mLabel; }
            set
            {
                mLabel = value;
                mMedia.setText(value);
                mChannel.setMedia(mProject._TextChannel, mMedia);
            }
        }

        internal SectionNode(Project project, int id)
            : base(project, id)
        {
            mChannel = getPresentation().getPropertyFactory().createChannelsProperty();
            this.setProperty(mChannel);
            mMedia = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            Label = Localizer.Message("default_section_label");
        }

        protected override string getLocalName()
        {
            return "section";
        }
    }
}