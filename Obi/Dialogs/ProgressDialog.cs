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
    public delegate void TimeTakingOperation_Cancelable ( ProgressDialog progress);//@singleSection

    public delegate void OperationCancelledHandler(object sender, EventArgs e);
    /// <summary>
    /// Base progress dialog to open when a long operation is in progress.
    /// </summary>
    public partial class ProgressDialog : Form
    {
        private Exception mException;
        private TimeTakingOperation mOperation;
        private TimeTakingOperation_Cancelable mOperation_Cancelable; //@singleSection
        private bool m_IsCancelled;//@singleSection

        public event OperationCancelledHandler OperationCancelled;

        /// <summary>
        /// Create the progress dialog.
        /// </summary>
        public ProgressDialog()
        {
            mException = null;
            mOperation = null;
            mOperation_Cancelable = null ;
            m_IsCancelled = false; //@singleSection
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");          
        
        }

        /// <summary>
        /// Create a progress dialog with a custom title and operation.
        /// </summary>
        public ProgressDialog(string title, TimeTakingOperation operation, Settings settings)
            : this()
        {
            mOperation = operation;
            Text = title;
            this.Size = new Size(this.Width, 94);
            m_BtnCancel.Visible = false;
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
        }

        //@singleSection
        /// <summary>
        /// Create a cancelable progress dialog with a custom title and operation.
        /// </summary>
        public ProgressDialog(string title, TimeTakingOperation_Cancelable operation)
            : this()
        {
            mOperation_Cancelable = operation ;
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
                    if ( mOperation != null )
                        {
                    mOperation();
                        }
                    else if ( mOperation_Cancelable != null)
                        {
                        mOperation_Cancelable (this) ;
                        }
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

        public bool CancelOperation { get { return m_IsCancelled ; } }

        private void m_BtnCancel_Click(object sender, EventArgs e)
        {
            m_IsCancelled = true;
            if ( OperationCancelled != null ) OperationCancelled ( this, new EventArgs ()) ;
            m_lbWaitForCancellation.Visible = true;
            this.mProgressBar.Location = new System.Drawing.Point(4,34);
        }

        private int m_ProgressbarValue = 0; //member variable for allowing access to progress bar value without using invoke required
        public void UpdateProgressBar(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 0) return;
            if (e.ProgressPercentage < m_ProgressbarValue + 5 && e.ProgressPercentage >= 5) return;
            int progressVal = e.ProgressPercentage;

            if (InvokeRequired)
            {
                Invoke(new System.ComponentModel.ProgressChangedEventHandler(UpdateProgressBar), sender, e);
            }
            else
            {
                if (mProgressBar.Style == ProgressBarStyle.Marquee)
                {
                    mProgressBar.Style = ProgressBarStyle.Continuous;
                    mProgressBar.Step = 5;
                }
                if (progressVal > 100) progressVal = 100;
                mProgressBar.Value = progressVal;
                m_ProgressbarValue = mProgressBar.Value;
            }
        }

    }
}
