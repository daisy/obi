using System;

namespace Obi
{
    /// <summary>
    /// The Obi work session. In the future, it may handle several presentations (i.e. several Obi projects.)
    /// </summary>
    public class Session
    {
        private DataModelFactory mDataModelFactory;  // the Obi data model factory (see below)
        private urakawa.Project mProject;            // the current project (as of now 1 presentation = 1 project)
        private string mPath;

        public Session()
        {
            mDataModelFactory = new DataModelFactory();
            mProject = null;
        }


        /// <summary>
        /// Closed the last project.
        /// </summary>
        public void Closed()
        {
        }

        /// <summary>
        /// Create a new presentation in the session, with a path to save its XUK file.
        /// </summary>
        public void NewPresentation(string path)
        {
            mProject = new urakawa.Project();
            mProject.setDataModelFactory(mDataModelFactory);
            mProject.setPresentation(mDataModelFactory.createPresentation(), 0);
            mPath = path;
        }

        /// <summary>
        /// Get the path of the XUK file of the current presentation.
        /// </summary>
        public string Path { get { return mPath; } }

        /// <summary>
        /// Get the current (Obi) presentation.
        /// </summary>
        public Presentation Presentation { get { return (Presentation)mProject.getPresentation(0); } }

        /// <summary>
        /// Save the current presentation to XUK.
        /// </summary>
        public void Save()
        {
        }
        public bool CanSave { get { return true; } }

        public void SaveAs(string path)
        {
        }
    }

    public class DataModelFactory : urakawa.DataModelFactory
    {
        public static readonly string NS = "http://www.daisy.org/urakawa/obi";
        public static readonly string XUK_VERSION = "xuk/obi;pre-1";

        public override urakawa.Presentation createPresentation()
        {
            return createPresentation(typeof(Obi.Presentation).Name, NS);
        }

        // TODO: what about a custom data manager?

        public override urakawa.Presentation createPresentation(string localName, string namespaceUri)
        {
            if (namespaceUri != NS || localName == typeof(Obi.Presentation).Name)
            {
                throw new Exception(String.Format("Cannot create presentation for QName `{0}:{1}'",
                    namespaceUri, localName));
            }
            return new Obi.Presentation();
        }

        public override urakawa.property.PropertyFactory createPropertyFactory(string localName, string namespaceUri)
        {
            if (namespaceUri != NS && localName != typeof(ObiPropertyFactory).Name)
            {
                throw new Exception(String.Format("Cannot create property factory for QName `{0}:{1}'",
                    namespaceUri, localName));
            }
            return new ObiPropertyFactory();
        }

        public override urakawa.core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
        {
            if (namespaceUri != NS && localName != typeof(ObiNodeFactory).Name)
            {
                throw new Exception(String.Format("Cannot create tree node factory for QName `{0}:{1}'",
                    namespaceUri, localName));
            }
            return new ObiNodeFactory();
        }

        public override urakawa.undo.UndoRedoManager createUndoRedoManager(string localName, string namespaceUri)
        {
            if (namespaceUri != NS && localName != typeof(Commands.UndoRedoManager).Name)
            {
                throw new Exception(String.Format("Cannot create undo/redo manager for QName `{0}:{1}'",
                    namespaceUri, localName));
            }
            return new Commands.UndoRedoManager();
        }
    }
}