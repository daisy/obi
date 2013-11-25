namespace Obi.Dialogs
{
    partial class MergeProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeProject));
            this.mbtnDesendingOrder = new System.Windows.Forms.Button();
            this.lstManualArrange = new System.Windows.Forms.ListBox();
            this.m_grpArrangeAudioFiles = new System.Windows.Forms.GroupBox();
            this.mbtnAscendingOrder = new System.Windows.Forms.Button();
            this.m_grpAddFiles = new System.Windows.Forms.GroupBox();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.m_btnMoveUp = new System.Windows.Forms.Button();
            this.m_btnMoveDown = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.m_grpArrangeAudioFiles.SuspendLayout();
            this.m_grpAddFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbtnDesendingOrder
            // 
            resources.ApplyResources(this.mbtnDesendingOrder, "mbtnDesendingOrder");
            this.mbtnDesendingOrder.Name = "mbtnDesendingOrder";
            this.mbtnDesendingOrder.UseVisualStyleBackColor = true;
            this.mbtnDesendingOrder.Click += new System.EventHandler(this.mbtnDesendingOrder_Click);
            // 
            // lstManualArrange
            // 
            resources.ApplyResources(this.lstManualArrange, "lstManualArrange");
            this.lstManualArrange.FormattingEnabled = true;
            this.lstManualArrange.Name = "lstManualArrange";
            this.lstManualArrange.SelectedIndexChanged += new System.EventHandler(this.lstManualArrange_SelectedIndexChanged);
            // 
            // m_grpArrangeAudioFiles
            // 
            resources.ApplyResources(this.m_grpArrangeAudioFiles, "m_grpArrangeAudioFiles");
            this.m_grpArrangeAudioFiles.Controls.Add(this.mbtnDesendingOrder);
            this.m_grpArrangeAudioFiles.Controls.Add(this.mbtnAscendingOrder);
            this.m_grpArrangeAudioFiles.Name = "m_grpArrangeAudioFiles";
            this.m_grpArrangeAudioFiles.TabStop = false;
            // 
            // mbtnAscendingOrder
            // 
            resources.ApplyResources(this.mbtnAscendingOrder, "mbtnAscendingOrder");
            this.mbtnAscendingOrder.Name = "mbtnAscendingOrder";
            this.mbtnAscendingOrder.UseVisualStyleBackColor = true;
            this.mbtnAscendingOrder.Click += new System.EventHandler(this.mbtnAscendingOrder_Click);
            // 
            // m_grpAddFiles
            // 
            resources.ApplyResources(this.m_grpAddFiles, "m_grpAddFiles");
            this.m_grpAddFiles.Controls.Add(this.m_btnRemove);
            this.m_grpAddFiles.Controls.Add(this.m_grpArrangeAudioFiles);
            this.m_grpAddFiles.Controls.Add(this.lstManualArrange);
            this.m_grpAddFiles.Controls.Add(this.m_btnAdd);
            this.m_grpAddFiles.Controls.Add(this.m_btnMoveUp);
            this.m_grpAddFiles.Controls.Add(this.m_btnMoveDown);
            this.m_grpAddFiles.Name = "m_grpAddFiles";
            this.m_grpAddFiles.TabStop = false;
            // 
            // m_btnRemove
            // 
            resources.ApplyResources(this.m_btnRemove, "m_btnRemove");
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            this.m_btnRemove.Click += new System.EventHandler(this.m_btnRemove_Click);
            // 
            // m_btnAdd
            // 
            resources.ApplyResources(this.m_btnAdd, "m_btnAdd");
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
            // 
            // m_btnMoveUp
            // 
            resources.ApplyResources(this.m_btnMoveUp, "m_btnMoveUp");
            this.m_btnMoveUp.Name = "m_btnMoveUp";
            this.m_btnMoveUp.UseVisualStyleBackColor = true;
            this.m_btnMoveUp.Click += new System.EventHandler(this.m_btnMoveUp_Click);
            // 
            // m_btnMoveDown
            // 
            resources.ApplyResources(this.m_btnMoveDown, "m_btnMoveDown");
            this.m_btnMoveDown.Name = "m_btnMoveDown";
            this.m_btnMoveDown.UseVisualStyleBackColor = true;
            this.m_btnMoveDown.Click += new System.EventHandler(this.m_btnMoveDown_Click);
            // 
            // mCancelButton
            // 
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mOKButton
            // 
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // MergeProject
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.m_grpAddFiles);
            this.Name = "MergeProject";
            this.m_grpArrangeAudioFiles.ResumeLayout(false);
            this.m_grpAddFiles.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbtnDesendingOrder;
        private System.Windows.Forms.ListBox lstManualArrange;
        private System.Windows.Forms.GroupBox m_grpArrangeAudioFiles;
        private System.Windows.Forms.Button mbtnAscendingOrder;
        private System.Windows.Forms.GroupBox m_grpAddFiles;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_btnMoveUp;
        private System.Windows.Forms.Button m_btnMoveDown;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Button mOKButton;


    }
}