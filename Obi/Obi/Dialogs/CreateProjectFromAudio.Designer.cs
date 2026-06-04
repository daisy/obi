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
            m_grpAddFiles = new System.Windows.Forms.GroupBox();
            m_grpArrangeAudioFiles = new System.Windows.Forms.GroupBox();
            m_btnDesendingOrder = new System.Windows.Forms.Button();
            m_btnAscendingOrder = new System.Windows.Forms.Button();
            m_grpStartProcess = new System.Windows.Forms.GroupBox();
            m_grpAddFiles.SuspendLayout();
            m_grpArrangeAudioFiles.SuspendLayout();
            m_grpStartProcess.SuspendLayout();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.AccessibleName = "Progress Bar";
            progressBar.Location = new System.Drawing.Point(21, 295);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(911, 37);
            progressBar.TabIndex = 10;
            // 
            // m_btnStart
            // 
            m_btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnStart.Location = new System.Drawing.Point(151, 21);
            m_btnStart.Name = "m_btnStart";
            m_btnStart.Size = new System.Drawing.Size(158, 35);
            m_btnStart.TabIndex = 12;
            m_btnStart.Text = "&Start";
            m_btnStart.UseVisualStyleBackColor = true;
            m_btnStart.Click += m_btnStart_Click;
            // 
            // m_btnCancel
            // 
            m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnCancel.Location = new System.Drawing.Point(448, 21);
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.Size = new System.Drawing.Size(168, 35);
            m_btnCancel.TabIndex = 13;
            m_btnCancel.Text = "&Cancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new System.Drawing.Point(21, 413);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            txtLog.Size = new System.Drawing.Size(911, 209);
            txtLog.TabIndex = 14;
            // 
            // m_btnClose
            // 
            m_btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnClose.Location = new System.Drawing.Point(382, 629);
            m_btnClose.Name = "m_btnClose";
            m_btnClose.Size = new System.Drawing.Size(110, 35);
            m_btnClose.TabIndex = 15;
            m_btnClose.Text = "C&lose";
            m_btnClose.UseVisualStyleBackColor = true;
            m_btnClose.Click += m_btnClose_Click;
            // 
            // lstAudioFiles
            // 
            lstAudioFiles.AccessibleName = "Audio files list box.";
            lstAudioFiles.FormattingEnabled = true;
            lstAudioFiles.HorizontalScrollbar = true;
            lstAudioFiles.Location = new System.Drawing.Point(18, 27);
            lstAudioFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            lstAudioFiles.Name = "lstAudioFiles";
            lstAudioFiles.Size = new System.Drawing.Size(745, 144);
            lstAudioFiles.TabIndex = 2;
            lstAudioFiles.SelectedIndexChanged += lstAudioFiles_SelectedIndexChanged;
            // 
            // m_btnRemove
            // 
            m_btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnRemove.Location = new System.Drawing.Point(791, 151);
            m_btnRemove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnRemove.Name = "m_btnRemove";
            m_btnRemove.Size = new System.Drawing.Size(99, 35);
            m_btnRemove.TabIndex = 6;
            m_btnRemove.Text = "&Remove";
            m_btnRemove.UseVisualStyleBackColor = true;
            m_btnRemove.Click += m_btnRemove_Click;
            // 
            // m_btnAdd
            // 
            m_btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnAdd.Location = new System.Drawing.Point(791, 107);
            m_btnAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnAdd.Name = "m_btnAdd";
            m_btnAdd.Size = new System.Drawing.Size(99, 35);
            m_btnAdd.TabIndex = 5;
            m_btnAdd.Text = "&Add";
            m_btnAdd.UseVisualStyleBackColor = true;
            m_btnAdd.Click += m_btnAddAudio_Click;
            // 
            // m_btnMoveUp
            // 
            m_btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnMoveUp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnMoveUp.Location = new System.Drawing.Point(791, 20);
            m_btnMoveUp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnMoveUp.Name = "m_btnMoveUp";
            m_btnMoveUp.Size = new System.Drawing.Size(99, 35);
            m_btnMoveUp.TabIndex = 3;
            m_btnMoveUp.Text = "Move &UP";
            m_btnMoveUp.UseVisualStyleBackColor = true;
            m_btnMoveUp.Click += m_btnMoveUp_Click;
            // 
            // m_btnMoveDown
            // 
            m_btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnMoveDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnMoveDown.Location = new System.Drawing.Point(791, 64);
            m_btnMoveDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnMoveDown.Name = "m_btnMoveDown";
            m_btnMoveDown.Size = new System.Drawing.Size(99, 35);
            m_btnMoveDown.TabIndex = 4;
            m_btnMoveDown.Text = "Move &Down";
            m_btnMoveDown.UseVisualStyleBackColor = true;
            m_btnMoveDown.Click += m_btnMoveDown_Click;
            // 
            // m_grpAddFiles
            // 
            m_grpAddFiles.Controls.Add(m_grpArrangeAudioFiles);
            m_grpAddFiles.Controls.Add(lstAudioFiles);
            m_grpAddFiles.Controls.Add(m_btnRemove);
            m_grpAddFiles.Controls.Add(m_btnMoveUp);
            m_grpAddFiles.Controls.Add(m_btnAdd);
            m_grpAddFiles.Controls.Add(m_btnMoveDown);
            m_grpAddFiles.Location = new System.Drawing.Point(19, 12);
            m_grpAddFiles.Name = "m_grpAddFiles";
            m_grpAddFiles.Size = new System.Drawing.Size(911, 272);
            m_grpAddFiles.TabIndex = 1;
            m_grpAddFiles.TabStop = false;
            // 
            // m_grpArrangeAudioFiles
            // 
            m_grpArrangeAudioFiles.AccessibleName = "";
            m_grpArrangeAudioFiles.Controls.Add(m_btnDesendingOrder);
            m_grpArrangeAudioFiles.Controls.Add(m_btnAscendingOrder);
            m_grpArrangeAudioFiles.Location = new System.Drawing.Point(18, 179);
            m_grpArrangeAudioFiles.Name = "m_grpArrangeAudioFiles";
            m_grpArrangeAudioFiles.Size = new System.Drawing.Size(745, 75);
            m_grpArrangeAudioFiles.TabIndex = 7;
            m_grpArrangeAudioFiles.TabStop = false;
            // 
            // m_btnDesendingOrder
            // 
            m_btnDesendingOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnDesendingOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnDesendingOrder.Location = new System.Drawing.Point(403, 27);
            m_btnDesendingOrder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnDesendingOrder.Name = "m_btnDesendingOrder";
            m_btnDesendingOrder.Size = new System.Drawing.Size(168, 35);
            m_btnDesendingOrder.TabIndex = 9;
            m_btnDesendingOrder.Text = "D&escending Order";
            m_btnDesendingOrder.UseVisualStyleBackColor = true;
            m_btnDesendingOrder.Click += m_btnDesendingOrder_Click;
            // 
            // m_btnAscendingOrder
            // 
            m_btnAscendingOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnAscendingOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            m_btnAscendingOrder.Location = new System.Drawing.Point(106, 27);
            m_btnAscendingOrder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            m_btnAscendingOrder.Name = "m_btnAscendingOrder";
            m_btnAscendingOrder.Size = new System.Drawing.Size(158, 35);
            m_btnAscendingOrder.TabIndex = 8;
            m_btnAscendingOrder.Text = "Asce&nding Order";
            m_btnAscendingOrder.UseVisualStyleBackColor = true;
            m_btnAscendingOrder.Click += m_btnAscendingOrder_Click;
            // 
            // m_grpStartProcess
            // 
            m_grpStartProcess.Controls.Add(m_btnStart);
            m_grpStartProcess.Controls.Add(m_btnCancel);
            m_grpStartProcess.Location = new System.Drawing.Point(21, 337);
            m_grpStartProcess.Name = "m_grpStartProcess";
            m_grpStartProcess.Size = new System.Drawing.Size(906, 71);
            m_grpStartProcess.TabIndex = 11;
            m_grpStartProcess.TabStop = false;
            // 
            // CreateProjectFromAudio
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(960, 668);
            Controls.Add(m_grpStartProcess);
            Controls.Add(m_grpAddFiles);
            Controls.Add(m_btnClose);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Name = "CreateProjectFromAudio";
            Text = "Create Project From Audio";
            m_grpAddFiles.ResumeLayout(false);
            m_grpArrangeAudioFiles.ResumeLayout(false);
            m_grpStartProcess.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox m_grpAddFiles;
        private System.Windows.Forms.GroupBox m_grpArrangeAudioFiles;
        private System.Windows.Forms.Button m_btnDesendingOrder;
        private System.Windows.Forms.Button m_btnAscendingOrder;
        private System.Windows.Forms.GroupBox m_grpStartProcess;
    }
}