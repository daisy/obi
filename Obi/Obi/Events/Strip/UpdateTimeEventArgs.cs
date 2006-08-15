using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Strip
{
    public delegate void UpdateTimeHandler(object sender, UpdateTimeEventArgs e);

    class UpdateTimeEventArgs: EventArgs
    {
        private CoreNode mNode;
        private double mTime;

        public CoreNode Node
        {
            get { return mNode; }
        }

        public double Time
        {
            get { return mTime; }
        }

        public UpdateTimeEventArgs(CoreNode node, double time)
        {
            mNode = node;
            mTime = time;
        }
    }
}
