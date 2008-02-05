using System;

namespace Obi.Commands
{
    public class UndoRedoManager: urakawa.undo.UndoRedoManager
    {
        public override string getXukNamespaceUri() { return DataModelFactory.NS; }

        // Do not save anything
        protected override void xukOutChildren(System.Xml.XmlWriter destination, Uri baseUri) {}
    }
}