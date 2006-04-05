using System;
using System.Collections.Generic;
using System.Text;

namespace Protobi
{
    /// <summary>
    /// Base class for content strip controllers
    /// </summary>
    public class SeqStrip : Strip
    {
        protected ParStrip mParent;

        public ParStrip Parent { get { return mParent; } }

        protected SeqStrip()
        {
        }

        public SeqStrip(StripManager manager, string label, ParStrip parent)
        {
            InitBaseMembers(manager, parent);
            InitModelUserControl(label);
        }

        public void InitBaseMembers(StripManager manager, ParStrip parent)
        {
            base.InitBaseMembers(manager);
            mParent = parent;
        }

        /// <summary>
        /// This function initializes model and user controls for this controller.
        /// </summary>
        /// <param name="label">The label of the strip.</param>
        protected override void InitModelUserControl(string label)
        {
            mModel = new SeqStripModel(label, (ParStripModel)mParent.Model);
            mUserControl = new SeqStripUserControl(this);
        }
    }

    /// <summary>
    /// Base class for content strips (i.e. seqs)
    /// </summary>
    public class SeqStripModel : StripModel
    {
        private ParStripModel mParent;  // the container (i.e. par)

        public SeqStripModel(string label, ParStripModel parent)
            : base(label)
        {
            mParent = parent;
        }
    }
}
