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

        // TODO: what about a custom data manager?

        public override urakawa.Presentation createPresentation()
        {
            return createPresentation(typeof(Obi.Presentation).Name, NS);
        }

        public override urakawa.Presentation createPresentation(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(Obi.Presentation).Name ?
                new Obi.Presentation() : base.createPresentation(localName, namespaceUri);
        }

        public override urakawa.property.PropertyFactory createPropertyFactory()
        {
            return createPropertyFactory(typeof(ObiPropertyFactory).Name, NS);
        }

        public override urakawa.property.PropertyFactory createPropertyFactory(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(ObiPropertyFactory).Name ?
                new ObiPropertyFactory() : base.createPropertyFactory(localName, namespaceUri);
        }

        public override urakawa.core.TreeNodeFactory createTreeNodeFactory()
        {
            return createTreeNodeFactory(typeof(ObiNodeFactory).Name, NS);
        }

        public override urakawa.core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(ObiNodeFactory).Name ?
                new ObiNodeFactory() : base.createTreeNodeFactory(localName, namespaceUri);
        }

        public override urakawa.undo.UndoRedoManager createUndoRedoManager()
        {
            return createUndoRedoManager(typeof(Commands.UndoRedoManager).Name, NS);
        }

        public override urakawa.undo.UndoRedoManager createUndoRedoManager(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(Commands.UndoRedoManager).Name ?
                new Commands.UndoRedoManager() : base.createUndoRedoManager(localName, namespaceUri);
        }
    }
}
