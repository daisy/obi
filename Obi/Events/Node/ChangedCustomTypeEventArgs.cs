using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Node
{
    class ChangedCustomTypeEventArgs : NodeEventArgs
    {
        private string mCustomType;

        /// <summary>
        /// New custom type for a phrase node
        /// </summary>
        public string CustomType
        {
            get { return mCustomType; }
        }

        public ChangedCustomTypeEventArgs(object origin, PhraseNode node, string customType) : base(origin, node)
        {
            mCustomType = customType;
        }
    }
}
