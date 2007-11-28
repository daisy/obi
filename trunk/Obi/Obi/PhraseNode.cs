using System;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio ;

namespace Obi
{
    public delegate void ChangedCustomTypeEventHandler(object sender, Events.Node.ChangedCustomTypeEventArgs e);

    /// <summary>
    /// A phrase node is a node that contains audio data.
    /// </summary>
    public class PhraseNode : ObiNode
    {
        private Kind mKind;          // this block's kind
        private string mCustomKind;  // custom kind name
        public event ChangedCustomTypeEventHandler CustomTypeChanged;
        public static readonly string XUK_ELEMENT_NAME = "phrase";  // name of the element in the XUK file

        /// <summary>
        /// Directions in which a phrase node can be moved.
        /// </summary>
        public enum Direction { Forward, Backward };

        /// <summary>
        /// Different kinds of phrases
        /// </summary>
        public enum Kind { Plain, Page, Heading, Custom }; 

        /// <summary>
        /// Create a new phrase node inside the given project with an id.
        /// Don't forget to set the asset afterwards!
        /// </summary>
        public PhraseNode(Presentation presentation): base(presentation)
        {
            Annotation = "";
            mKind = Kind.Plain;
            mCustomKind = null;
        }

        public PhraseNode(Presentation presentation, Kind kind) : this(presentation) { mKind = kind; }
        public PhraseNode(Presentation presentation, string custom) : this(presentation, Kind.Custom) { mCustomKind = custom; }


        /// <summary>
        /// The annotation for this node.
        /// </summary>
        public string Annotation
        {
            get { return AnnotationMedia == null ? "" : AnnotationMedia.getText(); }
            set
            {
                if (value == null || value == "")
                {
                    //ChannelsProperty.setMedia(Project.AnnotationChannel, null);
                }
                else if (value != null)
                {
                    if (AnnotationMedia == null)
                    {
                        ITextMedia annotation = getPresentation().getMediaFactory().createTextMedia();
                        //ChannelsProperty.setMedia(Project.AnnotationChannel, annotation);
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
            get { return (ManagedAudioMedia)ChannelsProperty.getMedia(Presentation.AudioChannel); }
            set { ChannelsProperty.setMedia(Presentation.AudioChannel, value); }
        }

        /// <summary>
        /// Custom kind (may be null if it is Plain, Page or Heading.)
        /// </summary>
        public string CustomKind 
        { 
            get { return mCustomKind; }
            set 
            { 
                mCustomKind = value;
                if (CustomTypeChanged != null) CustomTypeChanged(this, new Events.Node.ChangedCustomTypeEventArgs(this, this, mCustomKind));
            }
        }

        public override PhraseNode FirstUsedPhrase { get { return Used ? this : null; } }

        /// <summary>
        /// Custom element name for XUKOut.
        /// </summary>
        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }

        /// <summary>
        /// True if there is an annotation on the node.
        /// </summary>
        public bool HasAnnotation { get { return Annotation != ""; } }

        /// <summary>
        /// Index of this node relative to the other phrases.
        /// </summary>
        public override int Index { get { return getParent().indexOf(this); } }

        /// <summary>
        /// Allow only phrase nodes to be inserted.
        /// If the index is negative, count backward from the end (-1 is last.)
        /// </summary>
        public override void Insert(ObiNode node, int index)
        {
            if (!(node is PhraseNode)) throw new Exception("Only phrase nodes can be added as children of a phrase node.");
            if (index < 0) index += getChildCount();
            insert(node, index);
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
                    //setProperty(value);
                }
                else
                {
                    //removeProperty(typeof(PageProperty));
                }
            }
        }

        /// <summary>
        /// The kind of node.
        /// </summary>
        public Kind PhraseKind 
        { 
            get { return mKind; }
            set { mKind = value; }                    
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
                    for (prevSection = ParentAs<SectionNode>().PreviousSection;
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
            PhraseNode copy = Presentation.CreatePhraseNode();
            copy.Audio = Presentation.DataManager.CopyAndManage(Audio);
            copy.Used = Used;
            copy.Annotation = Annotation;
            copy.mKind = mKind;
            copy.mCustomKind = mCustomKind;
            //copyProperties(copy);
            return copy;
        }

        protected override void xukInAttributes(System.Xml.XmlReader source)
        {
            string kind = source.GetAttribute("kind");
            if (kind != null) mKind = kind == "Custom" ?  Kind.Custom :
                                      kind == "Heading" ? Kind.Heading :
                                      kind == "Page" ?    Kind.Page :
                                                          Kind.Plain;
            if (kind != null && kind != mKind.ToString()) throw new Exception("Unknown kind: " + kind);
            mCustomKind = source.GetAttribute("custom");
            if (mKind != Kind.Custom && mCustomKind != null)
            {
                throw new Exception("Extraneous `custom' attribute.");
            }
            else if (mKind == Kind.Custom && mCustomKind == null)
            {
                throw new Exception("Missing `custom' attribute.");
            }
            //add it to the presentation
            this.Presentation.AddCustomType(mCustomKind);
            base.xukInAttributes(source);
        }

        protected override void xukOutAttributes(System.Xml.XmlWriter wr, Uri baseUri)
        {
            if (mKind != Kind.Plain) wr.WriteAttributeString("kind", mKind.ToString());
            if (mKind == Kind.Custom) wr.WriteAttributeString("custom", mCustomKind);
            base.xukOutAttributes(wr, baseUri);
        }


        /// <summary>
        /// The text media for the annotation.
        /// Maybe null if no annotation was set.
        /// </summary>
        private TextMedia AnnotationMedia { get { return null; } } // ChannelsProperty.getMedia(Project.AnnotationChannel) as TextMedia; } }

        public override SectionNode SectionChild(int index) { throw new Exception("A phrase node has no section child!"); }
        public override int SectionChildCount { get { return 0; } }
        public override PhraseNode PhraseChild(int index) { throw new Exception("No child yet."); }
        public override int PhraseChildCount { get { return 0; } }
    }
}