namespace Obi.Dialogs
{
    partial class ProgressDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
            this.mProgressBar = new System.Windows.Forms.ProgressBar();
            this.m_BtnCancel = new System.Windows.Forms.Button();
            this.m_lbWaitForCancellation = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // mProgressBar
            // 
            resources.ApplyResources(this.mProgressBar, "mProgressBar");
            this.mProgressBar.Name = "mProgressBar";
            this.mProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.mProgressBar.UseWaitCursor = true;
            // 
            // m_BtnCancel
            // 
            resources.ApplyResources(this.m_BtnCancel, "m_BtnCancel");
            this.m_BtnCancel.Name = "m_BtnCancel";
            this.m_BtnCancel.UseVisualStyleBackColor = true;
            this.m_BtnCancel.UseWaitCursor = true;
            this.m_BtnCancel.Click += new System.EventHandler(this.m_BtnCancel_Click);
            // 
            // m_lbWaitForCancellation
            // 
            resources.ApplyResources(this.m_lbWaitForCancellation, "m_lbWaitForCancellation");
            this.m_lbWaitForCancellation.Name = "m_lbWaitForCancellation";
            this.m_lbWaitForCancellation.UseWaitCursor = true;
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // ProgressDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.m_lbWaitForCancellation);
            this.Controls.Add(this.m_BtnCancel);
            this.Controls.Add(this.mProgressBar);
            this.Name = "ProgressDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar mProgressBar;
        private System.Windows.Forms.Button m_BtnCancel;
        private System.Windows.Forms.Label m_lbWaitForCancellation;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}