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
        
        public Project Project
        {
            get { return mProject; }
        }

        public int Id
        {
            get { return mId; }
        }

        public bool Used
        {
            get { return mUsed; }
            set { mUsed = value; }
        }

        internal ObiNode(Project project, int id)
            : base(project.getPresentation())
        {
            mProject = project;
            mId = id;
            mUsed = true;
        }

        protected override string getNamespaceURI()
        {
            return ObiPropertyFactory.ObiNS;
        }
    }
}