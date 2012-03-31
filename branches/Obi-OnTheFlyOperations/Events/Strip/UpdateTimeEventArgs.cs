using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using Obi.Events.Node;

namespace Obi.Events.Strip
{
    public class UpdateTimeEventArgs: PhraseNodeEventArgs
    {
        private double mTime;

        public double Time
        {
            get { return mTime; }
        }

        public UpdateTimeEventArgs(Object origin, PhraseNode node, double time) : 
            base(origin, node)
        {
            mTime = time;
        }
    }
}
