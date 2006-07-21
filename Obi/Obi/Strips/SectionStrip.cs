using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Strips
{
    class SectionStrip: Strip
    {
        private CoreNode mNode;  // corresponding node in the tree
        private string mLabel;   // label of the strip is the text of the section
    }
}
