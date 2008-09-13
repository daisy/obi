using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// Base progress dialog to open when a long operation is in progress.
    /// </summary>
    public partial class ProgressDialog : Form
    {
        /// <summary>
        /// Create the progress dialog.
        /// </summary>
        public ProgressDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override this method to do the actual work.
        /// </summary>
        protected virtual void Do()
        {
        }

        // Set up a background worker doing the work, closing the form when done.
        // TODO: we need a cancel button and a progress report!
        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = false;
            worker.DoWork += new DoWorkEventHandler(delegate(object sender_, DoWorkEventArgs e_) { Do(); });
            worker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(delegate(object sender_, RunWorkerCompletedEventArgs e_) { Close(); });
            worker.RunWorkerAsync();
        }
    }

    /// <summary>
    /// Export to DAISY 3.
    /// </summary>
    public class ExportProgressDialog : ProgressDialog
    {
        private Session mSession;  // Obi session
        private string mPath;      // export path

        /// <summary>
        /// Create a new dialog showing progress during the export process.
        /// </summary>
        public ExportProgressDialog(Session session, string path): base()
        {
            mSession = session;
            mPath = path;
            Text = Localizer.Message("export_progress_dialog_title");
        }

        // Do the export in the background.
        protected override void Do()
        {
            mSession.Presentation.ExportToZ(mPath, mSession.Path);
        }
    }
}