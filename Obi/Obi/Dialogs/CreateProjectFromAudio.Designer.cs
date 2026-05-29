namespace Obi.Dialogs
{
    partial class CreateProjectFromAudio
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
            label1 = new System.Windows.Forms.Label();
            txtAudioPath = new System.Windows.Forms.TextBox();
            m_btnBrowseAudio = new System.Windows.Forms.Button();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblStatus = new System.Windows.Forms.Label();
            m_btnStart = new System.Windows.Forms.Button();
            m_btnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(21, 94);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(76, 20);
            label1.TabIndex = 0;
            label1.Text = "Audio File";
            // 
            // txtAudioPath
            // 
            txtAudioPath.Location = new System.Drawing.Point(128, 91);
            txtAudioPath.Name = "txtAudioPath";
            txtAudioPath.Size = new System.Drawing.Size(795, 27);
            txtAudioPath.TabIndex = 1;
            // 
            // m_btnBrowseAudio
            // 
            m_btnBrowseAudio.Location = new System.Drawing.Point(939, 89);
            m_btnBrowseAudio.Name = "m_btnBrowseAudio";
            m_btnBrowseAudio.Size = new System.Drawing.Size(94, 29);
            m_btnBrowseAudio.TabIndex = 2;
            m_btnBrowseAudio.Text = "&Browse";
            m_btnBrowseAudio.UseVisualStyleBackColor = true;
            m_btnBrowseAudio.Click += m_btnBrowseAudio_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(36, 181);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(997, 29);
            progressBar.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(57, 356);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(50, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Ready";
            // 
            // m_btnStart
            // 
            m_btnStart.Location = new System.Drawing.Point(225, 250);
            m_btnStart.Name = "m_btnStart";
            m_btnStart.Size = new System.Drawing.Size(126, 40);
            m_btnStart.TabIndex = 5;
            m_btnStart.Text = "&Start";
            m_btnStart.UseVisualStyleBackColor = true;
            m_btnStart.Click += m_btnStart_Click;
            // 
            // m_btnCancel
            // 
            m_btnCancel.Location = new System.Drawing.Point(437, 250);
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.Size = new System.Drawing.Size(110, 40);
            m_btnCancel.TabIndex = 6;
            m_btnCancel.Text = "&Cancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // CreateProjectFromAudio
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1082, 450);
            Controls.Add(m_btnCancel);
            Controls.Add(m_btnStart);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Controls.Add(m_btnBrowseAudio);
            Controls.Add(txtAudioPath);
            Controls.Add(label1);
            Name = "CreateProjectFromAudio";
            Text = "CreateProjectFromAudio";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAudioPath;
        private System.Windows.Forms.Button m_btnBrowseAudio;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button m_btnStart;
        private System.Windows.Forms.Button m_btnCancel;
    }
}