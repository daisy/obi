using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.NCX
{
    public class NCX
    {
        private Project mParent;       // the project that owns this NCX
        private Navpoint mNavmapRoot;  // root navpoint of the navmap--a dummy node with no content

        public NCX(Project parent)
        {
            mParent = parent;
            mNavmapRoot = new Navpoint(null, null);
        }
    }
}
