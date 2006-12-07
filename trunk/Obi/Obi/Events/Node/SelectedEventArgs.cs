using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Node
{
    class SelectedEventArgs: EventArgs
    {
        private bool mSelected;
        private bool mCanMoveIn;
        private bool mCanMoveOut;

        /// <summary>
        /// Whether a node is selected or not.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
        }

        /// <summary>
        /// Whether the selected node can move in (= increase level).
        /// </summary>
        public bool CanMoveIn
        {
            get { return mCanMoveIn; }
            set { mCanMoveIn = value; }
        }

        /// <summary>
        /// Whether the selected node can move out (= decrease level).
        /// </summary>
        public bool CanMoveOut
        {
            get { return mCanMoveOut; }
            set { mCanMoveOut = value; }
        }

        public SelectedEventArgs(bool selected)
        {
            mCanMoveIn = mCanMoveOut = mSelected = selected;
        }
    }
}
