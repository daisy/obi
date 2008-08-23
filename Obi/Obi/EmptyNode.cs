using urakawa.core;
using System;

namespace Obi
{
    /// <summary>
    /// The empty node class is the base class for content nodes.
    /// It doesn't have any actual content yet and will either become a PhraseNode or a ContainerNode.
    /// It can have a kind/custom class.
    /// </summary>
    public class EmptyNode: ObiNode
    {
        private Kind mKind;              // this node's kind
        private string mCustomClass;     // custom class name
        private PageNumber mPageNumber;  // page number
        private bool mTODO;              // marked as TODO

        public static readonly string XUK_ELEMENT_NAME = "empty";             // name of the element in the XUK file
        private static readonly string XUK_ATTR_NAME_KIND = "kind";           // name of the kind attribute
        private static readonly string XUK_ATTR_NAME_CUSTOM = "custom";       // name of the custom attribute
        private static readonly string XUK_ATTR_NAME_PAGE = "page";           // name of the page attribute
        private static readonly string XUK_ATTR_NAME_PAGE_KIND = "pageKind";  // name of the pageKind attribute
        private static readonly string XUK_ATTR_NAME_PAGE_TEXT = "pageText";  // name of the pageText attribute
        private static readonly string XUK_ATTR_NAME_TODO = "TODO";           // name of the TODO attribute

        /// <summary>
        /// Different kinds of content nodes.
        /// Plain is the default.
        /// Page is a page node with a page number.
        /// Heading is the audio heading for a section.
        /// Silence is a silence node for phrase detection.
        /// Custom is a node with a custom class (e.g. sidebar, etc.)
        /// </summary>
        public enum Kind { Plain, Page, Heading, Silence, Custom };

        
        public override string ToString() { return BaseString(); }

        public virtual string BaseString(double durationMs)
        {
            return String.Format(Localizer.Message("phrase_to_string"),
                TODO ? Localizer.Message ("phrase_short_TODO") : "",
                Used ? "" : Localizer.Message("unused"),
                IsRooted ? Index + 1 : 0,
                IsRooted ? ParentAs<ObiNode>().PhraseChildCount : 0,
                durationMs == 0.0 ? Localizer.Message("empty") : Program.FormatDuration_Long(durationMs),
                mKind == Kind.Custom ? String.Format(Localizer.Message("phrase_extra_custom"), mCustomClass) :
                    mKind == Kind.Page ? String.Format(Localizer.Message("phrase_extra_page"), mPageNumber.ToString()) :
                    Localizer.Message("phrase_extra_" + mKind.ToString()));
        }

        public virtual string BaseString() { return BaseString(0.0); }

        public virtual string BaseStringShort(double durationMs)
        {
            return String.Format(Localizer.Message("phrase_short_to_string"),
                TODO ? Localizer.Message("phrase_short_TODO") : "",
                mKind == Kind.Custom ? String.Format(Localizer.Message("phrase_short_custom"), mCustomClass) :
                    mKind == Kind.Page ? String.Format(Localizer.Message("phrase_short_page"), mPageNumber.ToString()) :
                    Localizer.Message("phrase_short_" + mKind.ToString()),
                durationMs == 0.0 ? Localizer.Message("empty") : Program.FormatDuration_Smart(durationMs));
        }

        public virtual string BaseStringShort() { return BaseStringShort(0.0); }

        public override double Duration { get { return 0.0; } }

        /// <summary>
        /// This event is sent when we change the kind or custom class of a node.
        /// </summary>
        public event ChangedKindEventHandler ChangedKind;
        public delegate void ChangedKindEventHandler(object sender, ChangedKindEventArgs e);

        /// <summary>
        /// This event is sent when the page number changes on a node (for a node which previously had a page number.)
        /// </summary>
        public event NodeEventHandler<EmptyNode> ChangedPageNumber;
        public event NodeEventHandler<EmptyNode> ChangedTo_DoStatus;


        /// <summary>
        /// Create a new empty node of a given kind in a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation, Kind kind, string customClass): base(presentation)
        {
            mKind = kind;
            mCustomClass = customClass;
            mPageNumber = null;
        }

        /// <summary>
        /// Create a plain empty node in a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation): this(presentation, Kind.Plain, null) {}

        /// <summary>
        /// Create an empty node of a pre-defined kind a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation, Kind kind): this(presentation, kind, null) {}

        /// <summary>
        /// Create an empty node with a custom class in a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation, string customClass): this(presentation, Kind.Custom, customClass) {}


        /// <summary>
        /// Copy node kind/custom class and page number from another node to this node.
        /// </summary>
        public void CopyKind(EmptyNode node)
        {
            mKind = node.mKind;
            mCustomClass = node.mCustomClass;
            mPageNumber = node.mPageNumber != null ? node.mPageNumber.Clone() : null;
        }

        /// <summary>
        /// Copy the node.
        /// </summary>
        protected override TreeNode copyProtected(bool deep, bool inclProperties)
        {
            EmptyNode copy = (EmptyNode)base.copyProtected(deep, inclProperties);
            copy.CopyKind(this);
            return copy;
        }

        /// <summary>
        /// Custom class (may be null if it is a predefined kind such as plain, page or heading.)
        /// </summary>
        public string CustomClass
        {
            get { return mCustomClass; }
            set { SetKind(Kind.Custom, value); }
        }

        public bool TODO
        {
            get { return mTODO; }
            set { mTODO = value;  }
        }

        /// <summary>
        /// Has no audio so is never a used phrase.
        /// </summary>
        public override PhraseNode FirstUsedPhrase { get { return null; } }

        /// <summary>
        /// Predefined kind of the node.
        /// </summary>
        public Kind NodeKind
        {
            get { return mKind; }
            set { SetKind(value, null); }
        }

        /// <summary>
        /// Get or set the page number for this node.
        /// Will discard previous kind if it was not already a page node.
        /// </summary>
        public PageNumber PageNumber
        {
            get { return mPageNumber; }
            set
            {
                mPageNumber = value;
                if (mKind == Kind.Page)            
                {
                    if (ChangedPageNumber != null) ChangedPageNumber(this, new NodeEventArgs<EmptyNode>(this));
                }
                else
                {
                    NodeKind = Kind.Page;
                }
            }
        }

        /// <summary>
        /// Renumber this node and all following pages of the same kind starting from this number.
        /// </summary>
        public override urakawa.undo.CompositeCommand RenumberCommand(ProjectView.ProjectView view, PageNumber from)
        {
            if (mKind == Kind.Page && mPageNumber.Kind == from.Kind)
            {
                urakawa.undo.CompositeCommand k = base.RenumberCommand(view, from.NextPageNumber());
                if (k == null)
                {
                    k = getPresentation().getCommandFactory().createCompositeCommand();
                    k.setShortDescription(string.Format(Localizer.Message("renumber_pages"),
                        Localizer.Message(string.Format("{0}_pages", from.Kind.ToString()))));
                }
                k.append(new Commands.Node.SetPageNumber(view, this, from));
                return k;
            }
            else
            {
                return base.RenumberCommand(view, from);
            }
        }

        /// <summary>
        /// Set the kind or custom class of the node.
        /// </summary>
        public void SetKind(Kind kind, string customClass)
        {
            if (mKind != kind || mCustomClass != customClass)
            {
                ChangedKindEventArgs args = new ChangedKindEventArgs(this, mKind, mCustomClass);
                mKind = kind;
                mCustomClass = customClass;
                if (kind != Kind.Page) mPageNumber = null;
                if (kind == Kind.Heading) AncestorAs<SectionNode>().Heading = this;
                if (ChangedKind != null) ChangedKind(this, args);
            }
        }

        public void AssignTo_DoMark( bool Val )
        {
            TODO = Val;
            if ( ChangedTo_DoStatus != null )
                ChangedTo_DoStatus(this, new NodeEventArgs<EmptyNode>(this));
        }

        public override void Insert(ObiNode node, int index) { throw new Exception("Empty nodes have no children."); }
        public override SectionNode SectionChild(int index) { throw new Exception("Empty nodes have no children."); }
        public override int SectionChildCount { get { return 0; } }
        public override EmptyNode PhraseChild(int index) { throw new Exception("Emtpy nodes have no children."); }
        public override int PhraseChildCount { get { return 0; } }
        public override EmptyNode LastUsedPhrase { get { throw new Exception("Empty nodes have no children."); } }



        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }

        protected override void xukInAttributes(System.Xml.XmlReader source)
        {
            string kind = source.GetAttribute(XUK_ATTR_NAME_KIND);
            if (kind != null) mKind = kind == Kind.Custom.ToString() ? Kind.Custom :
                                      kind == Kind.Heading.ToString() ? Kind.Heading :
                                      kind == Kind.Page.ToString() ? Kind.Page :
                                      kind == Kind.Silence.ToString() ? Kind.Silence : Kind.Plain;
            if (kind != null && kind != mKind.ToString()) throw new Exception("Unknown kind: " + kind);
            mCustomClass = source.GetAttribute(XUK_ATTR_NAME_CUSTOM);
            if (mKind == Kind.Page)
            {
                string pageKind = source.GetAttribute(XUK_ATTR_NAME_PAGE_KIND);
                if (pageKind != null)
                {
                    string page = source.GetAttribute(XUK_ATTR_NAME_PAGE);
                    int number = SafeParsePageNumber(page);
                    if (pageKind == "Front")
                    {
                        if (number == 0) throw new Exception(string.Format("Invalid page number \"{0}\".", page));
                        mPageNumber = new PageNumber(number, PageKind.Front);
                    }
                    else if (pageKind == "Normal")
                    {
                        if (number == 0) throw new Exception(string.Format("Invalid page number \"{0}\".", page));
                        mPageNumber = new PageNumber(number);
                    }
                    else if (pageKind == "Special")
                    {
                        if (page == null || page == "") throw new Exception("Invalid empty page number.");
                        mPageNumber = new PageNumber(page);
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid page kind \"{0}\".", pageKind));
                    }
                }
            }
            if (mKind != Kind.Custom && mCustomClass != null)
            {
                throw new Exception("Extraneous `custom' attribute.");
            }
            else if (mKind == Kind.Custom && mCustomClass == null)
            {
                throw new Exception("Missing `custom' attribute.");
            }
            // add it to the presentation
            Presentation.AddCustomClass(mCustomClass, this);

            string ToDo = source.GetAttribute(XUK_ATTR_NAME_TODO);
            if (ToDo != null)
            {
                if (ToDo == "True") mTODO = true;
                else mTODO = false;
            }
            base.xukInAttributes(source);
        }

        protected override void xukOutAttributes(System.Xml.XmlWriter wr, Uri baseUri)
        {
            if (mKind != Kind.Plain) wr.WriteAttributeString(XUK_ATTR_NAME_KIND, mKind.ToString());
            if (mKind == Kind.Custom) wr.WriteAttributeString(XUK_ATTR_NAME_CUSTOM, mCustomClass);
            if (mKind == Kind.Page)
            {
                wr.WriteAttributeString(XUK_ATTR_NAME_PAGE, mPageNumber.ArabicNumberOrLabel);
                wr.WriteAttributeString(XUK_ATTR_NAME_PAGE_KIND, mPageNumber.Kind.ToString());
                wr.WriteAttributeString(XUK_ATTR_NAME_PAGE_TEXT, mPageNumber.Unquoted);
            }
            if (mTODO) wr.WriteAttributeString(XUK_ATTR_NAME_TODO, "True");
            base.xukOutAttributes(wr, baseUri);
        }

        /// <summary>
        /// Attempt to parse a page number from a string; return 0 on negative/non-number values.
        /// </summary>
        public static int SafeParsePageNumber(string str)
        {
            int page;
            return Int32.TryParse(str, out page) ? page : 0;
        }
    }

    /// <summary>
    /// Informs that a node's kind has changed and pass along its old kind.
    /// </summary>
    class ChangedKindEventArgs : NodeEventArgs<EmptyNode>
    {
        private EmptyNode.Kind mKind;
        private string mCustomClass;

        public ChangedKindEventArgs(EmptyNode node, EmptyNode.Kind kind, string customClass): base(node)
        {
            mKind = kind;
            mCustomClass = customClass;
        }

        /// <summary>
        /// Previous kind for a content node.
        /// </summary>
        public EmptyNode.Kind PreviousKind { get { return mKind; } }

        /// <summary>
        /// Previous custom class for a content node.
        /// </summary>
        public string PreviousCustomClass { get { return mCustomClass; } }
    }
}