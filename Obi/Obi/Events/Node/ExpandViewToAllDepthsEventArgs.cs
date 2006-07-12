using System;
using System.Collections.Generic;
using System.Text;


namespace Obi.Events.Node
{
    public delegate void ExpandViewToAllDepthsHandler(object sender,
    ExpandViewToAllDepthsEventArgs e);

    /// <summary>
    /// This event is fired when the view should be limited to the depth of the given node
    /// </summary>
    class ExpandViewToAllDepthsEventArgs : EventArgs
    {
        public ExpandViewToAllDepthsEventArgs()
        {
            
        }
    }
}
