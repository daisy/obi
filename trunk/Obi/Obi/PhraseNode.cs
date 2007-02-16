using Obi.Assets;
using System;
using urakawa.core;
using urakawa.media;

namespace Obi
{
    public class PhraseNode : ObiNode
    {
        public static readonly string Name = "phrase";  // name of the element in the XUK file

        private ChannelsProperty mChannel;   // quick reference to the channel property
        private TextMedia mMedia;            // quick reference to the text media object
        private string mAnnotation;          // the annotation for this phrase
        private AudioMediaAsset mAsset;      // the audio asset for this phrase
    
        /// <summary>
        /// Directions in which a phrase node can be moved.
        /// </summary>
        public enum Direction { Forward, Backward };

        /// <summary>
        /// The annotation for this node; normally the name of the asset.
        /// </summary>
        public string Annotation
        {
            get { return mAnnotation; }
            set
            {
                mAnnotation = value;
                mMedia.setText(value);
                Channel channel = Project.FindChannel(Project.AnnotationChannelName);
                if (channel != null) mChannel.setMedia(channel, mMedia);
            }
        }

        /// <summary>
        /// The asset for this node.
        /// </summary>
        public AudioMediaAsset Asset
        {
            get { return mAsset; }
            set
            {
                mAsset = value;
                if (mAsset.Manager != null) UpdateSeq();
            }
        }

        /// <summary>
        /// Index of this node relative to the other phrases.
        /// </summary>
        public override int Index
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
        /// Previous phrase for this phrase. Null if this phrase is the last one.
        /// </summary>
        public PhraseNode PreviousPhrase
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                int index = Index;
                return index > 0 ? parent.PhraseChild(index - 1) : null;
            }
        }

        /// <summary>
        /// Next phrase for this phrase. Null if this phrase is the last one.
        /// </summary>
        public PhraseNode NextPhrase
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                int index = Index;
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
            Annotation = "";
            mAsset = null;
        }

        /// <summary>
        /// Update the sequence media object for the asset of this node.
        /// </summary>
        public void UpdateSeq()
        {
            SequenceMedia seq =
                (SequenceMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.EMPTY_SEQUENCE);
            foreach (AudioClip clip in mAsset.Clips)
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
          
            Channel channel = Project.FindChannel(Project.AudioChannelName);
            if (channel != null) mChannel.setMedia(channel, seq);
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
        /// <param name="deep">Ignored; the node is shallow.</param>
        public new PhraseNode copy(bool deep)
        {
            PhraseNode copy = (PhraseNode) getPresentation().getCoreNodeFactory().createNode(Name, ObiPropertyFactory.ObiNS);
            //the manager might be null if we are doing a cut/paste
            if (mAsset.Manager == null)
            {
                Project.AssetManager.AddAsset(mAsset);
                copy.Asset = mAsset;
            }
            else
            {
                copy.Asset = (AudioMediaAsset)mAsset.Manager.CopyAsset(mAsset);
            }
            copy.Annotation = copy.Asset.Name;
            copyProperties(copy);
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