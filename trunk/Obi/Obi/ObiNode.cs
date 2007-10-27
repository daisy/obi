using System;
using urakawa.core;
using urakawa.exception;
using urakawa.property.channel;

namespace Obi
{
    /// <summary>
    /// Base class for nodes in the Obi tree; doesn't do much.
    /// All nodes in the tree will be of a derived class.
    /// </summary>
    public abstract class ObiNode : TreeNode
    {
        private bool mUsed;  // mark node as being in use or not

        /// <summary>
        /// Create a new node for a presentation.
        /// </summary>
        protected ObiNode(Presentation presentation) : base()
        {
            setPresentation(presentation);
            mUsed = true;
        }


        // Our own overrides

        public virtual void AppendChild(ObiNode node) { appendChild(node); }
        public void Detach() { detach(); }
        public abstract void Insert(ObiNode node, int index);
        public void InsertAfter(ObiNode node, ObiNode anchor) { insertAfter(node, anchor); }
        public void InsertBefore(ObiNode node, ObiNode anchor) { insertBefore(node, anchor); }
        public void RemoveChild(ObiNode child) { removeChild(child); }

        /// <summary>
        /// Channels property for the node.
        /// </summary>
        public ChannelsProperty ChannelsProperty { get { return getProperty<ChannelsProperty>(); } }

        /// <summary>
        /// Index of this node in its parent's list of children.
        /// </summary>
        public virtual int Index { get { return getParent().indexOf(this); } }

        /// <summary>
        /// Check whether the node is rooted in the tree or not.
        /// </summary>
        public bool IsRooted
        {
            get { return this is RootNode || (getParent() is ObiNode && ((ObiNode)getParent()).IsRooted); }
        }

        /// <summary>
        /// Last descendant of the same kind as this node.
        /// </summary>
        public virtual ObiNode LastDescendant { get { return this; } }

        /// <summary>
        /// Level of the node in the tree. It is assumed that the root is an ObiNode with a level of 0.
        /// </summary>
        public virtual int Level { get { return 1 + ((ObiNode)getParent()).Level; } }

        /// <summary>
        /// Get the parent node as an ObiNode or any subclass thereof. I am not sure what "thereof" means, though.
        /// </summary>
        public T ParentAs<T>() where T : ObiNode { return getParent() as T; }

        /// <summary>
        /// Presentation to which this node belongs.
        /// </summary>
        public Presentation Presentation { get { return (Presentation)getPresentation(); } }

        public abstract SectionNode SectionChild(int index);
        public abstract int SectionChildCount { get; }
        public abstract PhraseNode PhraseChild(int index);
        public abstract int PhraseChildCount { get; }

        /// <summary>
        /// Used flag.
        /// </summary>
        public bool Used
        {
            get { return mUsed; }
            set
            {
                if (mUsed != value)
                {
                    mUsed = value;
                    Presentation.SignalUsedStatusChanged(this);
                }
            }
        }

        // XUK stuff
        
        private static readonly string USED_ATTR_NAME = "used";  // name of the used attribute

        /// <summary>
        /// Read back the used attribute.
        /// </summary>
        protected override void XukInAttributes(System.Xml.XmlReader reader)
        {
            string used = reader.GetAttribute(USED_ATTR_NAME);
            if (used != null && used == "False") mUsed = false;
            base.XukInAttributes(reader);
        }

        /// <summary>
        /// Write the used attribute if its value is false (true being the default.)
        /// </summary>
        protected override void XukOutAttributes(System.Xml.XmlWriter destination, Uri baseUri)
        {
            if (!mUsed) destination.WriteAttributeString(USED_ATTR_NAME, "False");
            base.XukOutAttributes(destination, baseUri);
        }

        /// <summary>
        /// Return the correct namespace URI for all Obi nodes.
        /// </summary>
        public override string getXukNamespaceUri() { return DataModelFactory.NS; }

        /// <summary>
        /// Copy the used flag as well as properties.
        /// </summary>
        protected override TreeNode copyProtected(bool deep, bool inclProperties)
        {
            ObiNode copy = (ObiNode)base.copyProtected(deep, inclProperties);
            copy.mUsed = mUsed;
            return copy;
        }
    }

    /// <summary>
    /// The root node of a presentation is an ObiNode as well.
    /// The level of the root node is 0. A root node has only section children.
    /// </summary>
    public class RootNode : ObiNode
    {
        public static readonly string XUK_ELEMENT_NAME = "root";  // name of the element in the XUK file

        public RootNode(Presentation presentation) : base(presentation) { }

        /// <summary>
        /// Allow only section nodes to be inserted.
        /// If the index is negative, count backward from the end (-1 is last.)
        /// </summary>
        public override void Insert(ObiNode node, int index)
        {
            if (!(node is SectionNode)) throw new Exception("Only section nodes can be added as children of a phrase node.");
            if (index < 0) index += getChildCount();
            insert(node, index);
        }

        /// <summary>
        /// The level of the root node is always 0; top-level sections have a level of 1.
        /// </summary>
        public override int Level { get { return 0; } }

        public override SectionNode SectionChild(int index)
        {
            if (index < 0) index = getChildCount() - index;
            return (SectionNode)getChild(index);
        }

        public override int SectionChildCount { get { return getChildCount(); } }
        public override PhraseNode PhraseChild(int index) { throw new Exception("A root node has no phrase children."); }
        public override int PhraseChildCount { get { return 0; } }
        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }
    }
}