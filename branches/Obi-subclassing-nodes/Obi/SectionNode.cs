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

        /// <summary>
        /// Get/set the label of the node as a simple string.
        /// </summary>
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

        /// <summary>
        /// Create a new section node with the default label.
        /// </summary>
        internal SectionNode(Project project, int id)
            : base(project, id)
        {
            mChannel = getPresentation().getPropertyFactory().createChannelsProperty();
            this.setProperty(mChannel);
            mMedia = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            Label = Localizer.Message("default_section_label");
        }

        /// <summary>
        /// Name of the element in the XUK file for this node.
        /// </summary>
        protected override string getLocalName()
        {
            return "section";
        }
    }
}