using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Protobi
{
    /// <summary>
    /// Base class for a strip controller.
    /// </summary>
    public class Strip
    {
        private StripManager mManager;            // the parent manager
        protected StripUserControl mUserControl;  // the corresponding user control
        protected StripModel mModel;              // the corresponding model
        private bool mSelected;                   // a strip can be selected

        public StripManager Manager { get { return mManager; } }
        public StripUserControl UserControl { get { return mUserControl; } }
        public StripModel Model { get { return mModel; } }

        public virtual bool CanAddAudioStrip { get { return false; } }
        public virtual bool CanAddAudioFile { get { return false; } }

        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                mUserControl.Selected = value;
            }
        }

        public string Label
        {
            get { return mModel.Label; }
            set
            {
                mModel.Label = value;
                mUserControl.Label = value;
            }
        }

        /// <summary>
        /// The size of the corresponding user control (this is used when renaming, adding, etc.)
        /// </summary>
        public Size Size { get { return mUserControl.Size; } }

        /// <summary>
        /// Empty constructor to make the compiler happy.
        /// </summary>
        protected Strip()
        {
        }

        /// <summary>
        /// Create a new strip, as well as a user control and a model.
        /// </summary>
        /// <param name="manager">The strip manager to which this strip belongs.</param>
        /// <param name="label">The label of the strip.</param>
        public Strip(StripManager manager, string label)
        {
            InitBaseMembers(manager);
            InitModelUserControl(label);
        }

        /// <summary>
        /// The actual base constructor.
        /// Do not overload the constructor as it will not initialize the most specific model and user control.
        /// </summary>
        /// <param name="manager">The parent manager of the controller.</param>
        protected void InitBaseMembers(StripManager manager)
        {
            mManager = manager;
            mSelected = false;
        }

        /// <summary>
        /// This function initializes model and user controls for this controller.
        /// </summary>
        /// <param name="label">The label of the strip.</param>
        protected virtual void InitModelUserControl(string label)
        {
            mModel = new StripModel(label);
            mUserControl = new StripUserControl(this);
        }

        /// <summary>
        /// Select this strip and make the manager aware of it.
        /// </summary>
        public void Select()
        {
            mSelected = true;
            mManager.Select(this);
        }
    }

    /// <summary>
    /// Base class for a strip. There is really nothing much yet.
    /// </summary>
    public class StripModel
    {
        private string mLabel;

        public string Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        public StripModel(string label)
        {
            mLabel = label;
        }
    }
}
