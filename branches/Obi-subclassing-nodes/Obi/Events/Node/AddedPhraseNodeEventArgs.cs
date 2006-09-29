using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void AddedPhraseNodeHandler(object sender, AddedPhraseNodeEventArgs e);

    class AddedPhraseNodeEventArgs: NodeEventArgs
    {
        private int mIndex;

        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        public AddedPhraseNodeEventArgs(object origin, CoreNode node, int index)
            : base(origin, node)
        {
            mIndex = index;
        }
    }
}
