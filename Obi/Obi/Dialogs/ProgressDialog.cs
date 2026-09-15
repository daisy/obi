using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        private TextBox m_LogTextBox;
        private bool m_LogEnabled;
        private string? m_LogFilePath;

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

        public void EnableLog()
        {
            EnableLog(null);
        }


        public void EnableLog(string? logFilePath)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string?>(EnableLog), logFilePath);
                return;
            }

            if (m_LogEnabled)
                return;

            m_LogEnabled = true;

            m_LogFilePath = logFilePath;

            if (!string.IsNullOrEmpty(m_LogFilePath))
            {
                try
                {
                    string? directory =
                        Path.GetDirectoryName(m_LogFilePath);

                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllText(
                        m_LogFilePath,
                        string.Empty);
                }
                catch
                {
                    // Logging must never break the import.
                }
            }

            // ----------------------------------------------------------
            // Use the same overall size as ImportAudioUsingWhisper.
            // ----------------------------------------------------------

            this.AutoSize = false;
            this.MinimumSize = Size.Empty;
            this.MaximumSize = Size.Empty;

            this.Size = new Size(965, 572);

            this.CenterToScreen();

            int marginLeft = 32;
            int marginRight = 22;

            int logTop = 24;
            int logHeight = 350;

            int progressTop = 390;
            int progressHeight = 37;

            int buttonTop = 470;

            // ----------------------------------------------------------
            // Create the log textbox.
            // ----------------------------------------------------------

            if (m_LogTextBox == null)
            {
                m_LogTextBox = new TextBox();

                m_LogTextBox.Multiline = true;
                m_LogTextBox.ReadOnly = true;
                m_LogTextBox.ScrollBars = ScrollBars.Vertical;
                m_LogTextBox.WordWrap = false;
                m_LogTextBox.BackColor = SystemColors.Window;
                m_LogTextBox.Font = this.Font;

                Controls.Add(m_LogTextBox);
            }

            m_LogTextBox.SetBounds(
                marginLeft,
                logTop,
                ClientSize.Width - marginLeft - marginRight,
                logHeight);

            // ----------------------------------------------------------
            // Progress bar
            // ----------------------------------------------------------

            mProgressBar.SetBounds(
                marginLeft,
                progressTop,
                ClientSize.Width - marginLeft - marginRight,
                progressHeight);

            // ----------------------------------------------------------
            // Cancel button
            // ----------------------------------------------------------

            m_BtnCancel.SetBounds(
                (ClientSize.Width - m_BtnCancel.Width) / 2,
                buttonTop,
                m_BtnCancel.Width,
                m_BtnCancel.Height);

            // ----------------------------------------------------------
            // Cancellation message
            // ----------------------------------------------------------

            m_lbWaitForCancellation.SetBounds(
                marginLeft,
                m_BtnCancel.Bottom + 8,
                ClientSize.Width - marginLeft - marginRight,
                m_lbWaitForCancellation.Height);

            m_LogTextBox.BringToFront();

            m_LogTextBox.Clear();

            this.PerformLayout();
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (!m_LogEnabled || m_LogTextBox == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<string>(Log), message);
                return;
            }

            // ----------------------------------------------------------
            // Display in the progress dialog.
            // ----------------------------------------------------------

            m_LogTextBox.AppendText(
                message + Environment.NewLine);

            m_LogTextBox.SelectionStart =
                m_LogTextBox.Text.Length;

            m_LogTextBox.ScrollToCaret();

            // ----------------------------------------------------------
            // Also write to the log file.
            // ----------------------------------------------------------

            if (!string.IsNullOrEmpty(m_LogFilePath))
            {
                try
                {
                    File.AppendAllText(
                        m_LogFilePath,
                        message + Environment.NewLine);
                }
                catch
                {
                    // Never let logging break the import.
                }
            }
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

            if (OperationCancelled != null)
                OperationCancelled(this, new EventArgs());

            m_lbWaitForCancellation.Visible = true;

            if (!m_LogEnabled)
            {
                this.mProgressBar.Location =
                    new System.Drawing.Point(4, 34);
            }
        }

        private int m_ProgressbarValue = 0; //member variable for allowing access to progress bar value without using invoke required

        public void UpdateProgressBar(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 0)
                return;

            if (InvokeRequired)
            {
                Invoke(new System.ComponentModel.ProgressChangedEventHandler(UpdateProgressBar), sender, e);

                return;
            }

            // ----------------------------------------------------------
            // 0 means: start a new operation.
            // ----------------------------------------------------------

            if (e.ProgressPercentage == 0)
            {
                m_ProgressbarValue = 0;
                mProgressBar.Value = 0;
                return;
            }

            // ----------------------------------------------------------
            // Ignore very small progress changes.
            // ----------------------------------------------------------

            if (e.ProgressPercentage <
                m_ProgressbarValue + 5)
            {
                return;
            }

            int progressVal =
                Math.Min(
                    100,
                    e.ProgressPercentage);

            if (mProgressBar.Style ==
                ProgressBarStyle.Marquee)
            {
                mProgressBar.Style =
                    ProgressBarStyle.Continuous;

                mProgressBar.Step = 5;
            }

            mProgressBar.Value = progressVal;

            m_ProgressbarValue =
                mProgressBar.Value;
        }

    }
}
