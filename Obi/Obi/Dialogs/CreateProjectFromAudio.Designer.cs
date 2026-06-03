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
            progressBar = new System.Windows.Forms.ProgressBar();
            m_btnStart = new System.Windows.Forms.Button();
            m_btnCancel = new System.Windows.Forms.Button();
            txtLog = new System.Windows.Forms.TextBox();
            m_btnClose = new System.Windows.Forms.Button();
            lstAudioFiles = new System.Windows.Forms.ListBox();
            m_btnRemove = new System.Windows.Forms.Button();
            m_btnAdd = new System.Windows.Forms.Button();
            m_btnMoveUp = new System.Windows.Forms.Button();
            m_btnMoveDown = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.AccessibleName = "Progress Bar";
            progressBar.Location = new System.Drawing.Point(37, 281);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(997, 29);
            progressBar.TabIndex = 6;
            // 
            // m_btnStart
            // 
            m_btnStart.Location = new System.Drawing.Point(261, 341);
            m_btnStart.Name = "m_btnStart";
            m_btnStart.Size = new System.Drawing.Size(126, 40);
            m_btnStart.TabIndex = 7;
            m_btnStart.Text = "&Start";
            m_btnStart.UseVisualStyleBackColor = true;
            m_btnStart.Click += m_btnStart_Click;
            // 
            // m_btnCancel
            // 
            m_btnCancel.Location = new System.Drawing.Point(472, 341);
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.Size = new System.Drawing.Size(110, 40);
            m_btnCancel.TabIndex = 8;
            m_btnCancel.Text = "&Cancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new System.Drawing.Point(80, 405);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtLog.Size = new System.Drawing.Size(900, 400);
            txtLog.TabIndex = 10;
            // 
            // m_btnClose
            // 
            m_btnClose.Location = new System.Drawing.Point(696, 341);
            m_btnClose.Name = "m_btnClose";
            m_btnClose.Size = new System.Drawing.Size(110, 40);
            m_btnClose.TabIndex = 9;
            m_btnClose.Text = "C&lose";
            m_btnClose.UseVisualStyleBackColor = true;
            m_btnClose.Click += m_btnClose_Click;
            // 
            // lstAudioFiles
            // 
            lstAudioFiles.AccessibleName = "Audio files list box.";
            lstAudioFiles.FormattingEnabled = true;
            lstAudioFiles.HorizontalScrollbar = true;
            lstAudioFiles.Location = new System.Drawing.Point(147, 30);
            lstAudioFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            lstAudioFiles.Name = "lstAudioFiles";
            lstAudioFiles.Size = new System.Drawing.Size(718, 224);
            lstAudioFiles.TabIndex = 1;
            lstAudioFiles.SelectedIndexChanged += lstAudioFiles_SelectedIndexChanged;
            // 
            // m_btnRemove
            // 
            m_btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnRemove.Location = new System.Drawing.Point(901, 199);
            m_btnRemove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnRemove.Name = "m_btnRemove";
            m_btnRemove.Size = new System.Drawing.Size(99, 29);
            m_btnRemove.TabIndex = 5;
            m_btnRemove.Text = "&Remove";
            m_btnRemove.UseVisualStyleBackColor = true;
            m_btnRemove.Click += m_btnRemove_Click;
            // 
            // m_btnAdd
            // 
            m_btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnAdd.Location = new System.Drawing.Point(899, 143);
            m_btnAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnAdd.Name = "m_btnAdd";
            m_btnAdd.Size = new System.Drawing.Size(102, 29);
            m_btnAdd.TabIndex = 4;
            m_btnAdd.Text = "&Add";
            m_btnAdd.UseVisualStyleBackColor = true;
            m_btnAdd.Click += m_btnAddAudio_Click;
            // 
            // m_btnMoveUp
            // 
            m_btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnMoveUp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnMoveUp.Location = new System.Drawing.Point(899, 33);
            m_btnMoveUp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnMoveUp.Name = "m_btnMoveUp";
            m_btnMoveUp.Size = new System.Drawing.Size(102, 32);
            m_btnMoveUp.TabIndex = 2;
            m_btnMoveUp.Text = "Move &UP";
            m_btnMoveUp.UseVisualStyleBackColor = true;
            m_btnMoveUp.Click += m_btnMoveUp_Click;
            // 
            // m_btnMoveDown
            // 
            m_btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnMoveDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnMoveDown.Location = new System.Drawing.Point(899, 87);
            m_btnMoveDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnMoveDown.Name = "m_btnMoveDown";
            m_btnMoveDown.Size = new System.Drawing.Size(102, 29);
            m_btnMoveDown.TabIndex = 3;
            m_btnMoveDown.Text = "Move &Down";
            m_btnMoveDown.UseVisualStyleBackColor = true;
            m_btnMoveDown.Click += m_btnMoveDown_Click;
            // 
            // CreateProjectFromAudio
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1082, 825);
            Controls.Add(m_btnRemove);
            Controls.Add(m_btnAdd);
            Controls.Add(m_btnMoveUp);
            Controls.Add(m_btnMoveDown);
            Controls.Add(lstAudioFiles);
            Controls.Add(m_btnClose);
            Controls.Add(txtLog);
            Controls.Add(m_btnCancel);
            Controls.Add(m_btnStart);
            Controls.Add(progressBar);
            Name = "CreateProjectFromAudio";
            Text = "Create Project From Audio";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button m_btnStart;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.ListBox lstAudioFiles;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_btnMoveUp;
        private System.Windows.Forms.Button m_btnMoveDown;
    }
}