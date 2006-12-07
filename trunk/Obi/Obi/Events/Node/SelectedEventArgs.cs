using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Node
{
    class SelectedEventArgs: EventArgs
    {
        private bool mSelected;

        /// <summary>
        /// Whether a node is selected or not.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
        }

        public SelectedEventArgs(bool selected)
        {
            mSelected = selected;
        }
    }
}
