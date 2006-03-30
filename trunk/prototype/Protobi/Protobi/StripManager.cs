using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    /// <summary>
    /// The strip manager keeps track of all strips.
    /// </summary>
    public class StripManager
    {
        List<StripController> mStrips;
        StripController mSelected;

        public List<StripController> Strips { get { return mStrips; } }
        public string NextLabel { get { return String.Format(Localizer.GetString("strip_name"), mStrips.Count + 1); } }

        public StripController Selected
        {
            get
            {
                return mSelected;
            }
            set
            {
                if (mSelected != null) mSelected.Selected = false;
                mSelected = value;
            }
        }

        public StripManager()
        {
            mStrips = new List<StripController>();
            mSelected = null;
        }
    }

    public class StripManagerController
    {
        private StripManager mManager;
        private FlowLayoutPanel mLayout;

        public StripController Selected { get { return mManager.Selected; } }

        public StripManagerController(FlowLayoutPanel layout)
        {
            mLayout = layout;
            mManager = new StripManager();
        }

        /// <summary>
        /// Append a newly created strip.
        /// </summary>
        /// <returns>The strip that was just added.</returns>
        public StripController AppendStrip()
        {
            Strip strip = new Strip(mManager.NextLabel);
            StripController stripController = new StripController(this, strip);
            StripUserControl userControl = new StripUserControl(stripController, true);
            stripController.UserControl = userControl;
            mManager.Strips.Add(stripController);
            mLayout.Controls.Add(userControl);
            return stripController;
        }

        public StripController AppendContainerStrip()
        {
            ContainerStrip strip = new ContainerStrip(mManager.NextLabel);
            ContainerStripController stripController = new ContainerStripController(this, strip);
            ContainerStripUserControl userControl = new ContainerStripUserControl(stripController);
            stripController.UserControl = userControl;
            mManager.Strips.Add(stripController);
            mLayout.Controls.Add(userControl);
            return stripController;
        }

        /// <summary>
        /// Append an existing strip.
        /// </summary>
        /// <param name="stripController"></param>
        public void AppendStrip(StripController stripController)
        {
            mLayout.Controls.Add(stripController.UserControl);
            mManager.Strips.Add(stripController);
        }

        public void RemoveLastStrip()
        {
            mManager.Strips[mManager.Strips.Count - 1].Selected = false;
            mManager.Strips[mManager.Strips.Count - 1].UserControl.Selected = false;
            mManager.Strips.RemoveAt(mManager.Strips.Count - 1);
            mLayout.Controls.RemoveAt(mLayout.Controls.Count - 1);
        }

        public void Select(StripController strip)
        {
            if (mManager.Selected != strip)
            {
                mManager.Selected = strip;
                mLayout.Refresh();
                ((WorkAreaForm)mLayout.Parent).EnableDeselect();
            }
        }
    }
}
