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
        private Kind mKind;           // this node's kind
        private string mCustomClass;  // custom class name
        private int mPageNumber;      // page number

        public static readonly string XUK_ELEMENT_NAME = "empty";        // name of the element in the XUK file
        private static readonly string XUK_ATTR_NAME_KIND = "kind";      // name of the kind attribute
        private static readonly string XUK_ATTR_NAME_CUSTOM = "custom";  // name of the custom attribute
        private static readonly string XUK_ATTR_NAME_PAGE = "page";      // name of the page attribute

        /// <summary>
        /// Different kinds of content nodes.
        /// Plain is the default.
        /// Page is a page node with a page number.
        /// Heading is the audio heading for a section.
        /// Silence is a silence node for phrase detection.
        /// Custom is a node with a custom class (e.g. sidebar, etc.)
        /// </summary>
        public enum Kind { Plain, Page, Heading, Silence, Custom };

        /// <summary>
        /// This event is sent when we change the kind or custom class of a node.
        /// </summary>
        public event ChangedKindEventHandler ChangedKind;
        public delegate void ChangedKindEventHandler(object sender, ChangedKindEventArgs e);

        /// <summary>
        /// This event is sent when the page number changes on a node (for a node which previously had a page number.)
        /// </summary>
        public event NodeEventHandler<EmptyNode> ChangedPageNumber;

        /// <summary>
        /// Create a new empty node of a given kind in a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation, Kind kind, string customClass): base(presentation)
        {
            mKind = kind;
            mCustomClass = customClass;
            mPageNumber = 0;
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
        /// Copy the node.
        /// </summary>
        protected override TreeNode copyProtected(bool deep, bool inclProperties)
        {
            EmptyNode copy = (EmptyNode)base.copyProtected(deep, inclProperties);
            copy.mKind = mKind;
            copy.mCustomClass = mCustomClass;
            copy.mPageNumber = mPageNumber;
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
        public int PageNumber
        {
            get { return mPageNumber; }
            set
            {
                if (value <= 0) return;
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
        /// Set the kind or custom class of the node.
        /// </summary>
        public void SetKind(Kind kind, string customClass)
        {
            if (mKind != kind || mCustomClass != customClass)
            {
                ChangedKindEventArgs args = new ChangedKindEventArgs(this, mKind, mCustomClass);
                mKind = kind;
                mCustomClass = customClass;
                if (kind != Kind.Page) mPageNumber = 0;
                if (ChangedKind != null) ChangedKind(this, args);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}] ({2})", GetType(), getParent() == null ? "detached" : Index.ToString(),
                mKind == Kind.Custom ? mCustomClass : mKind.ToString());
        }

        public override void Insert(ObiNode node, int index) { throw new Exception("Empty nodes have no children."); }
        public override SectionNode SectionChild(int index) { throw new Exception("Empty nodes have no children."); }
        public override int SectionChildCount { get { return 0; } }
        public override EmptyNode PhraseChild(int index) { throw new Exception("Emtpy nodes have no children."); }
        public override int PhraseChildCount { get { return 0; } }



        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }

        protected override void xukInAttributes(System.Xml.XmlReader source)
        {
            string kind = source.GetAttribute(XUK_ATTR_NAME_KIND);
            if (kind != null) mKind = kind == Kind.Custom.ToString() ? Kind.Custom :
                                      kind == Kind.Heading.ToString() ? Kind.Heading :
                                      kind == Kind.Page.ToString() ? Kind.Page :
                                      kind == Kind.Silence.ToString () ? Kind.Silence : Kind.Plain;
            if (kind != null && kind != mKind.ToString()) throw new Exception("Unknown kind: " + kind);
            mCustomClass = source.GetAttribute(XUK_ATTR_NAME_CUSTOM);
            if (mKind == Kind.Page) mPageNumber = SafeParsePageNumber(source.GetAttribute(XUK_ATTR_NAME_PAGE));
            if (mKind != Kind.Custom && mCustomClass != null)
            {
                throw new Exception("Extraneous `custom' attribute.");
            }
            else if (mKind == Kind.Custom && mCustomClass == null)
            {
                throw new Exception("Missing `custom' attribute.");
            }
            else if (mKind == Kind.Page && mPageNumber == 0)
            {
                throw new Exception("Missing `page' attribute (page number for page node.)");
            }
            // add it to the presentation
            Presentation.AddCustomClass(mCustomClass, this);
            base.xukInAttributes(source);
        }

        protected override void xukOutAttributes(System.Xml.XmlWriter wr, Uri baseUri)
        {
            if (mKind != Kind.Plain) wr.WriteAttributeString(XUK_ATTR_NAME_KIND, mKind.ToString());
            if (mKind == Kind.Custom) wr.WriteAttributeString(XUK_ATTR_NAME_CUSTOM, mCustomClass);
            if (mKind == Kind.Page) wr.WriteAttributeString(XUK_ATTR_NAME_PAGE, mPageNumber.ToString());
            base.xukOutAttributes(wr, baseUri);
        }

        /// <summary>
        /// Attempt to parse a page number from a string; return 0 on negative/non-number values.
        /// </summary>
        public static int SafeParsePageNumber(string str)
        {
            int page = 0;
            try
            {
                page = Int32.Parse(str);
            }
            catch (Exception) { }
            return page;
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