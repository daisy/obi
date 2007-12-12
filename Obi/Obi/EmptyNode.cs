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

        public static readonly string XUK_ELEMENT_NAME = "empty";        // name of the element in the XUK file
        private static readonly string XUK_ATTR_NAME_KIND = "kind";      // name of the kind attribute
        private static readonly string XUK_ATTR_NAME_CUSTOM = "custom";  // name of the custom attribute

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
        /// Create a new empty node of a given kind in a presentation.
        /// </summary>
        public EmptyNode(Presentation presentation, Kind kind, string customClass): base(presentation)
        {
            mKind = kind;
            mCustomClass = customClass;
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
            get { return mKind == Kind.Page ? getProperty<PageProperty>().PageNumber : -1; }
            set
            {
                if (mKind == Kind.Page)
                {
                    getProperty<PageProperty>().PageNumber = value;
                }
                else
                {
                    PageProperty prop = (PageProperty)
                        getPresentation().getPropertyFactory().createProperty(PageProperty.XUK_ELEMENT_NAME, DataModelFactory.NS);
                    prop.setPresentation(Presentation);
                    prop.PageNumber = value;
                    addProperty(prop);
                    NodeKind = Kind.Page;
                }
            }
        }



        /// <summary>
        /// Set the kind or custom class of the node.
        /// </summary>
        public void SetKind(Kind kind, string customClass)
        {
            ChangedKindEventArgs args = new ChangedKindEventArgs(this, mKind, mCustomClass);
            mKind = kind;
            mCustomClass = customClass;
            if (ChangedKind != null) ChangedKind(this, args);
        }

        public override void Insert(ObiNode node, int index) { throw new Exception("Empty nodes have no children."); }
        public override SectionNode SectionChild(int index) { throw new Exception("Empty nodes have no children."); }
        public override int SectionChildCount { get { return 0; } }
        public override PhraseNode PhraseChild(int index) { throw new Exception("Emtpy nodes have no children."); }
        public override int PhraseChildCount { get { return 0; } }


        protected override void xukInAttributes(System.Xml.XmlReader source)
        {
            string kind = source.GetAttribute(XUK_ATTR_NAME_KIND);
            if (kind != null) mKind = kind == Kind.Custom.ToString() ? Kind.Custom :
                                      kind == Kind.Heading.ToString() ? Kind.Heading :
                                      kind == Kind.Page.ToString() ? Kind.Page :
                                      kind == Kind.Silence.ToString () ? Kind.Silence : Kind.Plain;
            if (kind != null && kind != mKind.ToString()) throw new Exception("Unknown kind: " + kind);
            mCustomClass = source.GetAttribute(XUK_ATTR_NAME_CUSTOM);
            if (mKind != Kind.Custom && mCustomClass != null)
            {
                throw new Exception("Extraneous `custom' attribute.");
            }
            else if (mKind == Kind.Custom && mCustomClass == null)
            {
                throw new Exception("Missing `custom' attribute.");
            }
            // add it to the presentation
            Presentation.AddCustomClass(mCustomClass);
            base.xukInAttributes(source);
        }

        protected override void xukOutAttributes(System.Xml.XmlWriter wr, Uri baseUri)
        {
            if (mKind != Kind.Plain) wr.WriteAttributeString(XUK_ATTR_NAME_KIND, mKind.ToString());
            if (mKind == Kind.Custom) wr.WriteAttributeString(XUK_ATTR_NAME_CUSTOM, mCustomClass);
            base.xukOutAttributes(wr, baseUri);
        }

        /*
        /// <summary>
        /// Page (if set) associated with this phrase.
        /// </summary>
        public PageProperty PageProperty
        {
            get { return getProperty<PageProperty>(); }
            set
            {
                if (mKind == Kind.Page) removeProperty(getProperty<PageProperty>());
                if (value != null)
                {
                    addProperty(value);
                    if (mKind != Kind.Page) NodeKind = Kind.Page;
                }
            }
        }
        */
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