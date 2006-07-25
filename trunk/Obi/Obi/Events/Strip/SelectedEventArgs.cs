using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Strip
{
    public delegate void SelectedHandler(object sender, SelectedEventArgs e);

    /// <summary>
    /// A strip was selected (Selected = true) or unselected (Selected = false)
    /// </summary>
    public class SelectedEventArgs: EventArgs
    {
        private bool mSelected;

        public bool Selected
        {
            get
            {
                return mSelected;
            }
        }

        public SelectedEventArgs(bool selected)
        {
            mSelected = selected;
        }
    }
}
