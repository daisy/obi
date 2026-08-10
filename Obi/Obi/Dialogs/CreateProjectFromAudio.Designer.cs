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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateProjectFromAudio));
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
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.Name = "progressBar";
            // 
            // m_btnStart
            // 
            resources.ApplyResources(m_btnStart, "m_btnStart");
            m_btnStart.Name = "m_btnStart";
            m_btnStart.UseVisualStyleBackColor = true;
            m_btnStart.Click += m_btnStart_Click;
            // 
            // m_btnCancel
            // 
            resources.ApplyResources(m_btnCancel, "m_btnCancel");
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // txtLog
            // 
            resources.ApplyResources(txtLog, "txtLog");
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            // 
            // m_btnClose
            // 
            resources.ApplyResources(m_btnClose, "m_btnClose");
            m_btnClose.Name = "m_btnClose";
            m_btnClose.UseVisualStyleBackColor = true;
            m_btnClose.Click += m_btnClose_Click;
            // 
            // lstAudioFiles
            // 
            resources.ApplyResources(lstAudioFiles, "lstAudioFiles");
            lstAudioFiles.FormattingEnabled = true;
            lstAudioFiles.Name = "lstAudioFiles";
            lstAudioFiles.SelectedIndexChanged += lstAudioFiles_SelectedIndexChanged;
            // 
            // m_btnRemove
            // 
            resources.ApplyResources(m_btnRemove, "m_btnRemove");
            m_btnRemove.Name = "m_btnRemove";
            m_btnRemove.UseVisualStyleBackColor = true;
            m_btnRemove.Click += m_btnRemove_Click;
            // 
            // m_btnAdd
            // 
            resources.ApplyResources(m_btnAdd, "m_btnAdd");
            m_btnAdd.Name = "m_btnAdd";
            m_btnAdd.UseVisualStyleBackColor = true;
            m_btnAdd.Click += m_btnAddAudio_Click;
            // 
            // m_btnMoveUp
            // 
            resources.ApplyResources(m_btnMoveUp, "m_btnMoveUp");
            m_btnMoveUp.Name = "m_btnMoveUp";
            m_btnMoveUp.UseVisualStyleBackColor = true;
            m_btnMoveUp.Click += m_btnMoveUp_Click;
            // 
            // m_btnMoveDown
            // 
            resources.ApplyResources(m_btnMoveDown, "m_btnMoveDown");
            m_btnMoveDown.Name = "m_btnMoveDown";
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
            resources.ApplyResources(m_grpAddFiles, "m_grpAddFiles");
            m_grpAddFiles.Name = "m_grpAddFiles";
            m_grpAddFiles.TabStop = false;
            // 
            // m_grpArrangeAudioFiles
            // 
            resources.ApplyResources(m_grpArrangeAudioFiles, "m_grpArrangeAudioFiles");
            m_grpArrangeAudioFiles.Controls.Add(m_btnDesendingOrder);
            m_grpArrangeAudioFiles.Controls.Add(m_btnAscendingOrder);
            m_grpArrangeAudioFiles.Name = "m_grpArrangeAudioFiles";
            m_grpArrangeAudioFiles.TabStop = false;
            // 
            // m_btnDesendingOrder
            // 
            resources.ApplyResources(m_btnDesendingOrder, "m_btnDesendingOrder");
            m_btnDesendingOrder.Name = "m_btnDesendingOrder";
            m_btnDesendingOrder.UseVisualStyleBackColor = true;
            m_btnDesendingOrder.Click += m_btnDesendingOrder_Click;
            // 
            // m_btnAscendingOrder
            // 
            resources.ApplyResources(m_btnAscendingOrder, "m_btnAscendingOrder");
            m_btnAscendingOrder.Name = "m_btnAscendingOrder";
            m_btnAscendingOrder.UseVisualStyleBackColor = true;
            m_btnAscendingOrder.Click += m_btnAscendingOrder_Click;
            // 
            // m_grpStartProcess
            // 
            m_grpStartProcess.Controls.Add(m_btnStart);
            m_grpStartProcess.Controls.Add(m_btnCancel);
            resources.ApplyResources(m_grpStartProcess, "m_grpStartProcess");
            m_grpStartProcess.Name = "m_grpStartProcess";
            m_grpStartProcess.TabStop = false;
            // 
            // CreateProjectFromAudio
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(m_grpStartProcess);
            Controls.Add(m_grpAddFiles);
            Controls.Add(m_btnClose);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Name = "CreateProjectFromAudio";
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