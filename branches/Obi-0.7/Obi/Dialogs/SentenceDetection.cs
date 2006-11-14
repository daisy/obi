using System;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.Dialogs
{
    /// <summary>
    /// Dialog for sentence detection parameter settting.
    /// </summary>
    public partial class SentenceDetection : Form
    {
        private long mThreshold;
        private double mGap;
        private double mLeadingSilence;

        public long Threshold
        {
            get { return mThreshold; }
        }

        public double Gap
        {
            get { return mGap; }
        }

        public double LeadingSilence
        {
            get { return mLeadingSilence; }
        }

        public SentenceDetection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Instantiate the dialog.
        /// </summary>
        /// <param name="silence">The silence phrase.</param>
        /// <param name="phrase">The phrase to split into sentences.</param>
        public SentenceDetection(CoreNode silence, CoreNode phrase)
        {
            InitializeComponent();
            // mThreshold = get threshold from silence node
            // mGap = get default value (e.g. 1000.0ms?)
            // mLeadingSilence = get default value (e.g. 100.0ms?)
        }
    }
}