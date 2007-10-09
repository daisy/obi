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
        private Project mProject;  // project that this node (or rather that its presentation) belongs to
        private bool mUsed;        // mark node as being in use or not

        public event EventHandler UsedStateChanged;  // triggerd when the used state of the node has changed

        private static readonly string USED_ATTR_NAME = "used";

        /// <summary>
        /// Create a new node for a project.
        /// </summary>
        protected ObiNode(Project project)
            : base()
        {
            setPresentation(project.getPresentation());
            ChannelsProperty prop = getPresentation().getPropertyFactory().createChannelsProperty();
            setProperty(prop);
            mProject = project;
            mUsed = true;
        }


        public virtual void Append(ObiNode node) { appendChild(node); }
        public void Detach() { detach(); }
        public abstract void Insert(ObiNode node, int index);
        public void InsertAfter(ObiNode node, ObiNode anchor) { insertAfter(node, anchor); }
        public void InsertBefore(ObiNode node, ObiNode anchor) { insertBefore(node, anchor); }

        /// <summary>
        /// Channels property for the node.
        /// </summary>
        public ChannelsProperty ChannelsProperty
        {
            get { return (ChannelsProperty)getProperty(typeof(ChannelsProperty)); }
        }

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
        /// Level of the node in the tree. It is assumed that the root is an ObiNode with a level of 0.
        /// </summary>
        /// <remarks>Used to be "depth" but "level" makes more sense. We may need "depth" in the future.</remarks>
        public virtual int Level { get { return 1 + ((ObiNode)getParent()).Level; } }

        /// <summary>
        /// Get the parent node as an Obi node.
        /// </summary>
        public ObiNode Parent { get { return getParent() as ObiNode; } }

        /// <summary>
        /// Project to which the presentation of this node belongs.
        /// </summary>
        public Project Project { get { return mProject; } }

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
                    if (UsedStateChanged != null) UsedStateChanged(this, new EventArgs());
                }
            }
        }

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
        protected override void XukOutAttributes(System.Xml.XmlWriter writer)
        {
            if (!mUsed) writer.WriteAttributeString(USED_ATTR_NAME, "False");
            base.XukOutAttributes(writer);
        }

        /// <summary>
        /// Return the correct namespace URI for all Obi nodes.
        /// </summary>
        public override string getXukNamespaceUri() { return Program.OBI_NS; }
    }

    /// <summary>
    /// The root node of a presentation is an ObiNode as well.
    /// The level of the root node is 0. A root node has only section children.
    /// </summary>
    public class RootNode : ObiNode
    {
        public static readonly string XUK_ELEMENT_NAME = "root";  // name of the element in the XUK file

        public RootNode(Project project): base(project) {}


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

        public override int Level { get { return 0; } }

        public SectionNode SectionChild(int index)
        {
            if (index < 0) index = getChildCount() - index;
            return (SectionNode)getChild(index);
        }

        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }
    }
}