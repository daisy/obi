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
            label1 = new System.Windows.Forms.Label();
            cmbModel = new System.Windows.Forms.ComboBox();
            m_btnStart = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Location = new System.Drawing.Point(32, 259);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtLog.Size = new System.Drawing.Size(911, 209);
            txtLog.TabIndex = 4;
            // 
            // progressBar
            // 
            progressBar.AccessibleName = "Progress Bar";
            progressBar.Location = new System.Drawing.Point(32, 128);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(911, 37);
            progressBar.TabIndex = 3;
            // 
            // m_btnCancel
            // 
            m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnCancel.Location = new System.Drawing.Point(491, 506);
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.Size = new System.Drawing.Size(168, 35);
            m_btnCancel.TabIndex = 6;
            m_btnCancel.Text = "&Cancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(32, 27);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(157, 20);
            label1.TabIndex = 1;
            label1.Text = "Select Whisper Model:";
            // 
            // cmbModel
            // 
            cmbModel.FormattingEnabled = true;
            cmbModel.Location = new System.Drawing.Point(250, 24);
            cmbModel.Name = "cmbModel";
            cmbModel.Size = new System.Drawing.Size(226, 28);
            cmbModel.TabIndex = 2;
            // 
            // m_btnStart
            // 
            m_btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnStart.Location = new System.Drawing.Point(168, 506);
            m_btnStart.Name = "m_btnStart";
            m_btnStart.Size = new System.Drawing.Size(168, 35);
            m_btnStart.TabIndex = 5;
            m_btnStart.Text = "&Start";
            m_btnStart.UseVisualStyleBackColor = true;
            m_btnStart.Click += m_btnStart_Click;
            // 
            // ImportAudioUsingWhisper
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = m_btnCancel;
            ClientSize = new System.Drawing.Size(965, 572);
            Controls.Add(m_btnStart);
            Controls.Add(cmbModel);
            Controls.Add(label1);
            Controls.Add(m_btnCancel);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ImportAudioUsingWhisper";
            Text = "Import Audio Using Whisper";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.Button m_btnStart;
    }
}