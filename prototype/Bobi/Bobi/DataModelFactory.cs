using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi
{
    public class DataModelFactory : urakawa.DataModelFactory
    {
        public static readonly string NS = "http://www.daisy.org/urakawa/bobi";  // Bobi-specific namespace

        // Custom factories for tree nodes.
        // For consistency, override both methods without parameters and with
        // localname/nsuri parameters.

        public override urakawa.core.TreeNodeFactory createTreeNodeFactory()
        {
            return createTreeNodeFactory(typeof(NodeFactory).Name, NS);
        }

        public override urakawa.core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(NodeFactory).Name ?
                new NodeFactory() : base.createTreeNodeFactory(localName, namespaceUri);
        }
    }
}
