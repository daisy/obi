using System;
using System.Collections.Generic;
using System.Text;

namespace Protobi
{
    public class ContainerStrip: Strip
    {
        public ContainerStrip(string label)
            : base(label)
        {
        }
    }

    public class ContainerStripController : StripController
    {
        List<StripController> mStrips;

        public ContainerStripController(StripManagerController manager, ContainerStrip strip)
            : base(manager, strip)
        {
            mStrips = new List<StripController>();
        }
    }
}
