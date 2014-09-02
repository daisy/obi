using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Node
{
    public class SelectedEventArgs: EventArgs
    {
        private bool mSelected;
        private object mWidget; //the widget that was selected or deselected

        /// <summary>
        /// Whether a node is selected or not.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
        }

        public object Widget
        {
            get { return mWidget; }
            set { mWidget = value; }
        }

        public SelectedEventArgs(bool selected, object widget)
        {
            mSelected = selected;
            mWidget = widget;
        }
    }
}
