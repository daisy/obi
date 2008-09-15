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
        private Exception mException;

        /// <summary>
        /// Create the progress dialog.
        /// </summary>
        public ProgressDialog()
        {
            mException = null;
            InitializeComponent();
        }

        public Exception Exception { get { return mException; } }

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
            worker.DoWork += new DoWorkEventHandler(delegate(object sender_, DoWorkEventArgs e_)
            {
                try
                {
                    Do();
                }
                catch (Exception x)
                {
                    mException = x;
                }
            });
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
        public ExportProgressDialog(Session session, string path)
            : base()
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

    /// <summary>
    /// Import from XHTML.
    /// </summary>
    public class ImportProgressDialog : ProgressDialog
    {
        private string mPath;
        private Presentation mPresentation;

        public ImportProgressDialog(string path, Presentation presentation)
            : base()
        {
            mPath = path;
            mPresentation = presentation;
            Text = Localizer.Message("import_progress_dialog_title");
        }

        protected override void Do()
        {
            (new ImportStructure()).ImportFromXHTML(mPath, mPresentation);
        }
    }

    /// <summary>
    /// Import audio files into phrases.
    /// </summary>
    public class ImportAudioProgressDialog : ProgressDialog
    {
        private Presentation mPresentation;
        private List<PhraseNode> mPhrases;
        private string[] mPaths;
        private double mDuration;

        public ImportAudioProgressDialog(Presentation presentation, string[] paths, double duration)
            : base()
        {
            mPresentation = presentation;
            mPaths = paths;
            mDuration = duration;
            Text = Localizer.Message("import_audio_progress_dialog_title");
        }

        public List<PhraseNode> Phrases { get { return mPhrases; } }

        protected override void Do()
        {
            mPhrases = new List<PhraseNode>(mPaths.Length);
            foreach (string path in mPaths)
            {
                List<PhraseNode> phrases = mPresentation.CreatePhraseNodeList(path, mDuration);
                foreach (PhraseNode p in phrases)
                {
                    try
                    {
                        mPhrases.Add(p);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("import_phrase_error_text"), path),
                            Localizer.Message("import_phrase_error_caption"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

    }
}
