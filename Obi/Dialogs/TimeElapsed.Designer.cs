namespace Obi.Dialogs
{
    partial class TimeElapsed
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeElapsed));
            this.m_lblTimeElapsed = new System.Windows.Forms.Label();
            this.m_txtBoxTotalTimeElapsed = new System.Windows.Forms.TextBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lblTimeElapsed
            // 
            resources.ApplyResources(this.m_lblTimeElapsed, "m_lblTimeElapsed");
            this.m_lblTimeElapsed.Name = "m_lblTimeElapsed";
            // 
            // m_txtBoxTotalTimeElapsed
            // 
            resources.ApplyResources(this.m_txtBoxTotalTimeElapsed, "m_txtBoxTotalTimeElapsed");
            this.m_txtBoxTotalTimeElapsed.Name = "m_txtBoxTotalTimeElapsed";
            this.m_txtBoxTotalTimeElapsed.ReadOnly = true;
            // 
            // m_btnClose
            // 
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnClose, "m_btnClose");
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.UseVisualStyleBackColor = true;
            this.m_btnClose.Click += new System.EventHandler(this.m_btnClose_Click);
            // 
            // TimeElapsed
            // 
            this.AcceptButton = this.m_btnClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnClose;
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_txtBoxTotalTimeElapsed);
            this.Controls.Add(this.m_lblTimeElapsed);
            this.Name = "TimeElapsed";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblTimeElapsed;
        private System.Windows.Forms.TextBox m_txtBoxTotalTimeElapsed;
        private System.Windows.Forms.Button m_btnClose;
    }
}