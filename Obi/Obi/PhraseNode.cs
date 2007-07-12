using Obi.Assets;
using System;
using urakawa.core;
using urakawa.media;

namespace Obi
{
    /// <summary>
    /// A phrase node is a node that contains audio data.
    /// </summary>
    public class PhraseNode : ObiNode
    {
        private bool mXukInHeadingFlag;  // got the heading flag from the XUK file

        private ITextMedia mAnnotation;  // annotation (to be removed)
        private AudioMediaAsset mAsset;  // asset (to be removed)

        public static readonly string XUK_ELEMENT_NAME = "phrase";  // name of the element in the XUK file
    
        /// <summary>
        /// Directions in which a phrase node can be moved.
        /// </summary>
        public enum Direction { Forward, Backward };


        /// <summary>
        /// True if there is an annotation on the node.
        /// </summary>
        /// <remarks>This has to be reviewed (along with actually setting the annotation!)</remarks>
        public bool HasAnnotation
        {
            get { return mAnnotation.getText() != ""; }
        }

        /// <summary>
        /// The annotation for this node.
        /// </summary>
        public string Annotation
        {
            get { return mAnnotation.getText(); }
            set
            {
                if (Project.AnnotationChannel != null)
                {
                    mAnnotation.setText(value);
                    ChannelsProperty.setMedia(Project.AnnotationChannel, mAnnotation);
                    Project.SetMedia(this, Project.AnnotationChannel, mAnnotation);
                }
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

        public int IndexOutOf
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                return parent.PhraseChildCount;
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
        /// Previous phrase node in linear order in the whole project.
        /// Null if it is the first phrase in the project.
        /// </summary>
        public PhraseNode PreviousPhraseInProject
        {
            get
            {
                PhraseNode prev = PreviousPhraseInSection;
                if (prev == null)
                {
                    SectionNode prevSection;
                    for (prevSection = ParentSection.PreviousSection;
                        prevSection != null && prevSection.PhraseChildCount == 0;
                        prevSection = prevSection.PreviousSection) { }
                    if (prevSection != null && prevSection.PhraseChildCount != 0) prev = prevSection.PhraseChild(-1);
                }
                return prev;
            }
        }

        /// <summary>
        /// Previous phrase for this phrase in its section. Null if this phrase is the first one.
        /// </summary>
        public PhraseNode PreviousPhraseInSection
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                int index = Index;
                return index > 0 ? parent.PhraseChild(index - 1) : null;
            }
        }

        /// <summary>
        /// Next phrase for this phrase in its section. Null if this phrase is the last one.
        /// </summary>
        public PhraseNode NextPhraseInSection
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
            set
            {
                if (value != null)
                {
                    setProperty(value);
                }
                else
                {
                    removeProperty(typeof(PageProperty));
                }
            }
        }

        /// <summary>
        /// True if the phrase node is a heading of its parent section.
        /// False otherwise (not a heading or no parent.)
        /// </summary>
        /// <remarks>The node doesn't really know that it is a heading,
        /// the information is only kept by the parent section in order
        /// to minimize the risk of inconsistencies.</remarks>
        public bool IsHeading
        {
            get { return ParentSection != null && ParentSection.Heading == this; }
        }

        /// <summary>
        /// Create a new phrase node inside the given project with an id.
        /// Don't forget to set the asset afterwards!
        /// </summary>
        internal PhraseNode(Project project)
            : base(project)
        {
            mAnnotation = getPresentation().getMediaFactory().createTextMedia();
            Annotation = "";
            mAsset = null;
            mXukInHeadingFlag = false;
        }

        /// <summary>
        /// Update the sequence media object for the asset of this node.
        /// </summary>
        public void UpdateSeq()
        {
            /*SequenceMedia seq =
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
            Channel channel = Project.FindChannel(Project.AUDIO_CHANNEL_NAME);
            // if (channel != null) ChannelsProperty.setMedia(channel, seq);
            if (channel != null) mProject.SetMedia(this, channel, seq);
             */
        }

        /// <summary>
        /// Custom element name for XUKOut.
        /// </summary>
        public override string getXukLocalName()
        {
            return XUK_ELEMENT_NAME;
        }

        /// <summary>
        /// Make a copy of a phrase node and of its asset (copy it in the asset manager as well.)
        /// </summary>
        /// <param name="deep">Ignored; the node is shallow.</param>
        public new PhraseNode copy(bool deep)
        {
            PhraseNode copy = (PhraseNode) getPresentation().getTreeNodeFactory().createNode(XUK_ELEMENT_NAME, Program.OBI_NS);
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
            copy.Used = Used;
            copy.Annotation = Annotation;
            copyProperties(copy);
            return copy;
        }

        /// <summary>
        /// Detach this phrase node from its parent.
        /// </summary>
        public void DetachFromParent()
        {
            ParentSection.RemoveChildPhrase(this);
        }

        protected override void XukOutAttributes(System.Xml.XmlWriter wr)
        {
            if (IsHeading) wr.WriteAttributeString("heading", "True");
            base.XukOutAttributes(wr);
        }

        protected override void XukInAttributes(System.Xml.XmlReader source)
        {
            string used = source.GetAttribute("heading");
            if (used != null && used == "True") mXukInHeadingFlag = true;
            base.XukInAttributes(source);
        }

        public bool HasXukInHeadingFlag
        {
            get { return mXukInHeadingFlag; }
        }
    }
}