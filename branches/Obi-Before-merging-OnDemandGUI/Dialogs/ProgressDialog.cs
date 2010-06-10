using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public delegate void TimeTakingOperation();

    /// <summary>
    /// Base progress dialog to open when a long operation is in progress.
    /// </summary>
    public partial class ProgressDialog : Form
    {
        private Exception mException;
        private TimeTakingOperation mOperation;

        /// <summary>
        /// Create the progress dialog.
        /// </summary>
        public ProgressDialog()
        {
            mException = null;
            mOperation = null;
            InitializeComponent();
        }

        /// <summary>
        /// Create a progress dialog with a custom title and operation.
        /// </summary>
        public ProgressDialog(string title, TimeTakingOperation operation)
            : this()
        {
            mOperation = operation;
            Text = title;
        }

        public Exception Exception { get { return mException; } }

        // Set up a background worker doing the work, closing the form when done.
        // TODO: we need a cancel button and a progress report!
        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = false;
            worker.DoWork += new DoWorkEventHandler(delegate(object sender_, DoWorkEventArgs e_)
            {
                try
                {
                    mOperation();
                }
                catch (Exception x)
                {
                    mException = x;
                }
                //Close();
            });
            worker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(delegate(object sender_, RunWorkerCompletedEventArgs e_) { Close(); });
            worker.RunWorkerAsync();
        }
    }
}
