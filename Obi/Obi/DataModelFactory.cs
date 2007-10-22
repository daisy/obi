using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    /// <summary>
    /// The custom Data Model Factory for Obi.
    /// </summary>
    public class DataModelFactory : urakawa.DataModelFactory
    {
        public static readonly string NS = "http://www.daisy.org/urakawa/obi";
        public static readonly string XUK_VERSION = "xuk/obi;pre-1";

        /// <summary>
        /// Generator string for XUK export
        /// </summary>
        public static string Generator
        {
            get
            {
                return String.Format("{0} v{1} with {2} v{3} (http://urakawa.sf.net/obi)",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                    System.Reflection.Assembly.GetAssembly(typeof(urakawa.Project)).GetName().Name,
                    System.Reflection.Assembly.GetAssembly(typeof(urakawa.Project)).GetName().Version);
            }
        }

        public override urakawa.Presentation createPresentation()
        {
            return createPresentation(typeof(Obi.Presentation).Name, NS);
        }

        // TODO: what about a custom data manager?

        public override urakawa.Presentation createPresentation(string localName, string namespaceUri)
        {
            if (namespaceUri != NS || localName != typeof(Obi.Presentation).Name)
            {
                throw new Exception(String.Format("Cannot create presentation for QName `{0}:{1}'",
                    namespaceUri, localName));
            }
            Obi.Presentation presentation = new Obi.Presentation();
            presentation.setTreeNodeFactory(new ObiNodeFactory());
            presentation.setPropertyFactory(new ObiPropertyFactory());
            presentation.setUndoRedoManager(new Commands.UndoRedoManager());
            return presentation;
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
