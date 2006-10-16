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
        /// Create a new node for a project with a new id. The factory gives ids to nodes.
        /// </summary>
        internal ObiNode(Project project, int id)
            : base(project.getPresentation())
        {
            mProject = project;
            mId = id;
            mUsed = true;
        }

        /// <summary>
        /// Write the id of the node as an id attribute.
        /// </summary>
        protected override bool XUKOutAttributes(System.Xml.XmlWriter wr)
        {
            wr.WriteAttributeString("id", "n_" + mId.ToString());
            return base.XUKOutAttributes(wr);
        }

        /// <summary>
        /// Read back the id attribute.
        /// </summary>
        protected override bool XUKInAttributes(System.Xml.XmlReader source)
        {
            string id = source.GetAttribute("id");
            mId = System.Int32.Parse(id.Substring(2));  // the id is of the form "n_xxx"
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