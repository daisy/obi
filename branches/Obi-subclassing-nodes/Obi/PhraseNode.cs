using System;
using urakawa.core;
using urakawa.media;

namespace Obi
{
    public class PhraseNode : ObiNode
    {
        public static readonly string Name = "phrase";

        private ChannelsProperty mChannel;      // quick reference to the channel property
        private TextMedia mMedia;               // quick reference to the text media object
        private string mAnnotation;             // the annotation for this phrase
        private Assets.AudioMediaAsset mAsset;  // the audio asset for this phrase
        private AssetProperty mAssProperty;     // asset property for the XUK input/output

        /// <summary>
        /// The annotation for this node. At the moment this is the name of the asset.
        /// </summary>
        public string Annotation
        {
            get { return mAnnotation; }
            set
            {
                mAnnotation = value;
                mMedia.setText(value);
                mChannel.setMedia(mProject.AnnotationChannel, mMedia);
            }
        }

        /// <summary>
        /// The asset for this node.
        /// </summary>
        public Assets.AudioMediaAsset Asset
        {
            get { return mAsset; }
            set
            {
                mAssProperty.Asset = value;
                setProperty(mAssProperty);
                UpdateSeq();
            }
        }

        /// <summary>
        /// Index of this node relative to the other phrases.
        /// </summary>
        public int PhraseIndex
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                return parent.indexOf(this);
            }
        }

        /// <summary>
        /// Parent section of this phrase. Null if the phrase has no parent.
        /// </summary>
        public SectionNode ParentSection
        {
            get { return getParent() as SectionNode; }
        }

        /// <summary>
        /// Next phrase for this phrase. Null if this phrase is the last one.
        /// </summary>
        public PhraseNode NextPhrase
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                int index = PhraseIndex;
                return index < parent.PhraseChildCount - 1 ? parent.PhraseChild(index + 1) : null;
            }
        }

        /// <summary>
        /// Page (if set) associated with this phrase.
        /// </summary>
        public PageProperty PageProperty
        {
            get { return getProperty(typeof(PageProperty)) as PageProperty; }  // may be null
            set { setProperty(value); }
        }

        /// <summary>
        /// Create a new phrase node inside the given project with an id.
        /// Don't forget to set the asset afterwards!
        /// </summary>
        internal PhraseNode(Project project, int id)
            : base(project, id)
        {
            mChannel = getPresentation().getPropertyFactory().createChannelsProperty();
            this.setProperty(mChannel);
            mMedia = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            Annotation = "No annotation :(";
            mAssProperty = (AssetProperty)getPresentation().getPropertyFactory().createProperty("AssetProperty",
                ObiPropertyFactory.ObiNS);
            mAsset = null;
        }

        /// <summary>
        /// Update the sequence media object for the asset of this node.
        /// </summary>
        public void UpdateSeq()
        {
            SequenceMedia seq =
                (SequenceMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.EMPTY_SEQUENCE);
            foreach (Assets.AudioClip clip in mAsset.Clips)
            {
                AudioMedia audio = (AudioMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.AUDIO);
                UriBuilder builder = new UriBuilder();
                builder.Scheme = "file";
                builder.Path = clip.Path;
                Uri relUri = mAsset.Manager.BaseURI.MakeRelativeUri(builder.Uri);
                audio.setLocation(new MediaLocation(relUri.ToString()));
                audio.setClipBegin(new Time((long)Math.Round(clip.BeginTime)));
                audio.setClipEnd(new Time((long)Math.Round(clip.EndTime)));
                seq.appendItem(audio);
            }
            mChannel.setMedia(mProject.AudioChannel, seq);
        }

        /// <summary>
        /// Custom element name for XUKOut.
        /// </summary>
        protected override string getLocalName()
        {
            return Name;
        }

        /// <summary>
        /// Make a copy of a phrase node and of its asset (copy it in the asset manager as well.)
        /// </summary>
        public new PhraseNode copy(bool deep)
        {
            PhraseNode copy = getPresentation().getCoreNodeFactory().createNode(Name, ObiPropertyFactory.ObiNS) as PhraseNode;
            if (copy == null)
            {
                throw new urakawa.exception.FactoryCanNotCreateTypeException(
                    String.Format("The CoreNode factory of the Presentation can not create an {0}:{1}",
                    ObiPropertyFactory.ObiNS, Name));
            }
            copy.Asset = mAsset.Manager.CopyAsset(mAsset) as Assets.AudioMediaAsset;
            copy.Annotation = copy.Asset.Name;
            copyProperties(copy);
            if (deep)
            {
                copyChildren(copy);
            }
            return copy;
        }

        /// <summary>
        /// Detach this phrase node from its parent.
        /// </summary>
        internal void DetachFromParent()
        {
            ParentSection.RemoveChildPhrase(this);
        }
    }
}