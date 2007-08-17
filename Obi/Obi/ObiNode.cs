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

        private static readonly string USED_ATTR_NAME = "used";

        /// <summary>
        /// Create a new node for a project.
        /// </summary>
        protected ObiNode(Project project): base()
        {
                setPresentation(project.getPresentation());
                                    ChannelsProperty prop = getPresentation().getPropertyFactory().createChannelsProperty();
            setProperty(prop);
            mProject = project;
            mUsed = true;
                    }


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
        public abstract int Index { get; }

        /// <summary>
        /// Level of the node in the tree. It is assumed that the root is a TreeNode with a level of 0.
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
        /// Project to which the presentation of this node belongs.
        /// </summary>
        public Project Project { get { return mProject; } }

        /// <summary>
        /// Used flag.
        /// </summary>
        public bool Used
        {
            get { return mUsed; }
            set { mUsed = value; }
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

    public class RootNode : ObiNode
    {
        public static readonly string XUK_ELEMENT_NAME = "root";  // name of the element in the XUK file

        public RootNode(Project project): base(project)
        {
        }

        public override int Index { get { return 0; } }

        public SectionNode SectionChild(int index)
        {
            if (index < 0) index = getChildCount() - index;
            return (SectionNode)getChild(index);
        }

        public override string getXukLocalName()
        {
            return XUK_ELEMENT_NAME;
        }
    }
}