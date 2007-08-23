using System;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio ;

namespace Obi
{
    /// <summary>
    /// A phrase node is a node that contains audio data.
    /// </summary>
    public class PhraseNode : ObiNode
    {
        private bool mXukInHeadingFlag;  // got the heading flag from the XUK file

        public static readonly string XUK_ELEMENT_NAME = "phrase";  // name of the element in the XUK file

        /// <summary>
        /// Directions in which a phrase node can be moved.
        /// </summary>
        public enum Direction { Forward, Backward };

        /// <summary>
        /// Create a new phrase node inside the given project with an id.
        /// Don't forget to set the asset afterwards!
        /// </summary>
        public PhraseNode(Project project): base(project)
        {
            Annotation = "";
            mXukInHeadingFlag = false;
        }


        /// <summary>
        /// The annotation for this node.
        /// </summary>
        public string Annotation
        {
            get
            {
                return AnnotationMedia == null ? "" : AnnotationMedia.getText();
            }
            set
            {
                if (value == null || value == "")
                {
                    ChannelsProperty.setMedia(Project.AnnotationChannel, null);
                }
                else if (value != null)
                {
                    if (AnnotationMedia == null)
                    {
                        ITextMedia annotation = getPresentation().getMediaFactory().createTextMedia();
                        ChannelsProperty.setMedia(Project.AnnotationChannel, annotation);
                    }
                    AnnotationMedia.setText(value);
                }
            }
        }

        /// <summary>
        /// The audio media data associated with this node.
        /// </summary>
        public ManagedAudioMedia Audio
        {
            get { return ChannelsProperty.getMedia(Project.AudioChannel) as ManagedAudioMedia; }
            set { ChannelsProperty.setMedia(Project.AudioChannel, value); }
        }

        /// <summary>
        /// Custom element name for XUKOut.
        /// </summary>
        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }

        /// <summary>
        /// True if there is an annotation on the node.
        /// </summary>
        public bool HasAnnotation { get { return Annotation != ""; } }

        public bool HasXukInHeadingFlag { get { return mXukInHeadingFlag; } }

        /// <summary>
        /// Index of this node relative to the other phrases.
        /// </summary>
        public override int Index { get { return getParent().indexOf(this); } }

        public int IndexOutOf { get { return ParentSection.PhraseChildCount; } }

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
        /// Parent section of this phrase. Null if the phrase has no parent.
        /// </summary>
        public SectionNode ParentSection { get { return getParent() as SectionNode; } }

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


        protected override TreeNode copyProtected(bool deep, bool inclProperties)
        {
            PhraseNode copy = (PhraseNode)base.copy(deep, inclProperties);
            copy.Audio = Project.DataManager.CopyAndManage(Audio);
            copy.Used = Used;
            copy.Annotation = Annotation;
            copyProperties(copy);
            return copy;
        }

        protected override void XukInAttributes(System.Xml.XmlReader source)
        {
            string used = source.GetAttribute("heading");
            if (used != null && used == "True") mXukInHeadingFlag = true;
            base.XukInAttributes(source);
        }

        protected override void XukOutAttributes(System.Xml.XmlWriter wr)
        {
            if (IsHeading) wr.WriteAttributeString("heading", "True");
            base.XukOutAttributes(wr);
        }


        /// <summary>
        /// The text media for the annotation.
        /// Maybe null if no annotation was set.
        /// </summary>
        private TextMedia AnnotationMedia { get { return ChannelsProperty.getMedia(Project.AnnotationChannel) as TextMedia; } }
    }
}