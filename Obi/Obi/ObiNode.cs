using System;
using urakawa.core;
using urakawa.exception;

namespace Obi
{
    /// <summary>
    /// Base class for nodes in the Obi tree; doesn't do much.
    /// Note that all nodes in the tree will be obi nodes, except for the root node which is a regular core node.
    /// </summary>
    public abstract class ObiNode : CoreNode
    {
        protected Project mProject;  // project that this node (or rather that its presentation) belongs to
        private int mId;             // unique id for this node
        protected bool mUsed;        // mark node as being in use or not

        /// <summary>
        /// Channels property for the node.
        /// </summary>
        public ChannelsProperty ChannelsProperty
        {
            get { return (ChannelsProperty)getProperty(typeof(ChannelsProperty)); }
        }

        /// <summary>
        /// Project to which the node belongs (and, from the project, the presentation to which it belongs.)
        /// </summary>
        public Project Project
        {
            get { return mProject; }
        }

        /// <summary>
        /// Number ID for identifiying nodes.
        /// </summary>
        public int Id
        {
            get { return mId; }
        }

        /// <summary>
        /// Used flag.
        /// </summary>
        public bool Used
        {
            get { return mUsed; }
            set { mUsed = value; }
        }

        /// <summary>
        /// Level of the node in the tree. It is assumed that the root is a CoreNode with a level of 0.
        /// </summary>
        /// <remarks>Used to be "depth" but "level" makes more sense. We may need "depth" in the future.</remarks>
        public int Level
        {
            get
            {
                ObiNode parent = getParent() as ObiNode;
                return 1 + (parent == null ? 0 : parent.Level);
            }
        }

        /// <summary>
        /// Index of this node in its parent's list of children.
        /// </summary>
        public abstract int Index
        {
            get;
        }

        /// <summary>
        /// Information string for dumping the core tree to a console.
        /// </summary>
        public virtual string InfoString
        {
            get { return String.Format(" <{0}>", mId); }
        }

        /// <summary>
        /// Create a new node for a project with a new id. The factory gives ids to nodes.
        /// </summary>
        internal ObiNode(Project project, int id)
            : base(project.getPresentation())
        {
            ChannelsProperty prop = getPresentation().getPropertyFactory().createChannelsProperty();
            setProperty(prop);
            mProject = project;
            mId = id;
            mUsed = true;
        }

        /// <summary>
        /// Write the id of the node as an id attribute.
        /// </summary>
        protected override bool XUKOutAttributes(System.Xml.XmlWriter wr)
        {
            if (!mUsed) wr.WriteAttributeString("used", "False");
            return base.XUKOutAttributes(wr);
        }

        /// <summary>
        /// Read back the id attribute.
        /// </summary>
        protected override bool XUKInAttributes(System.Xml.XmlReader source)
        {
            string used = source.GetAttribute("used");
            if (used != null && used == "False") mUsed = false;
            return base.XUKInAttributes(source);
        }

        /// <summary>
        /// Return the correct namespace URI for all Obi nodes.
        /// </summary>
        protected override string getNamespaceURI()
        {
            return ObiPropertyFactory.ObiNS;  // have to move this where it makes sense
        }
    }
}