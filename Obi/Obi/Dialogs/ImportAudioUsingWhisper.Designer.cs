namespace Obi.Dialogs
{
    partial class ImportAudioUsingWhisper
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
            txtLog = new System.Windows.Forms.TextBox();
            progressBar = new System.Windows.Forms.ProgressBar();
            m_btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Location = new System.Drawing.Point(87, 202);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtLog.Size = new System.Drawing.Size(911, 209);
            txtLog.TabIndex = 16;
            // 
            // progressBar
            // 
            progressBar.AccessibleName = "Progress Bar";
            progressBar.Location = new System.Drawing.Point(87, 71);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(911, 37);
            progressBar.TabIndex = 15;
            // 
            // m_btnCancel
            // 
            m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnCancel.Location = new System.Drawing.Point(421, 456);
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.Size = new System.Drawing.Size(168, 35);
            m_btnCancel.TabIndex = 18;
            m_btnCancel.Text = "&Cancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // ImportAudioUsingWhisper
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1119, 526);
            Controls.Add(m_btnCancel);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            MaximizeBox = false;
            Name = "ImportAudioUsingWhisper";
            Text = "Import Audio Using Whisper";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button m_btnCancel;
    }
}