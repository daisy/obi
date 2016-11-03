namespace MergeUtilityUI
    {
    partial class Daisy3MergerForm
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose ();
                }
            base.Dispose ( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
            {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( Daisy3MergerForm ) );
            this.m_lblDaisy3Merger = new System.Windows.Forms.Label ();
            this.m_BtnReset = new System.Windows.Forms.Button ();
            this.m_BtnMerge = new System.Windows.Forms.Button ();
            this.m_BtnOutputDirectory = new System.Windows.Forms.Button ();
            this.m_txtDirectoryPath = new System.Windows.Forms.TextBox ();
            this.m_BtnCancel = new System.Windows.Forms.Button ();
            this.m_lbOutputDirPath = new System.Windows.Forms.Label ();
            this.m_bgWorker = new System.ComponentModel.BackgroundWorker ();
            this.m_statusStrip = new System.Windows.Forms.StatusStrip ();
            this.m_StatusLabel = new System.Windows.Forms.ToolStripStatusLabel ();
            this.m_grpDirPath = new System.Windows.Forms.GroupBox ();
            this.m_BtnHelp = new System.Windows.Forms.Button ();
            this.m_BtnValidateOutput = new System.Windows.Forms.Button ();
            this.m_grpPageInput = new System.Windows.Forms.GroupBox ();
            this.m_rdbRenumberPages = new System.Windows.Forms.RadioButton ();
            this.m_rdbExistingNumberOfPages = new System.Windows.Forms.RadioButton ();
            this.m_grpListDTBfiles = new System.Windows.Forms.GroupBox ();
            this.m_lbDTBfiles = new System.Windows.Forms.ListBox ();
            this.m_BtnDelete = new System.Windows.Forms.Button ();
            this.m_BtnDown = new System.Windows.Forms.Button ();
            this.m_btnUP = new System.Windows.Forms.Button ();
            this.m_BtnValidateInput = new System.Windows.Forms.Button ();
            this.m_txtDTBookInfo = new System.Windows.Forms.TextBox ();
            this.m_btnAdd = new System.Windows.Forms.Button ();
            this.m_lblDBookInfo = new System.Windows.Forms.Label ();
            this.m_grpManipulateDTB = new System.Windows.Forms.GroupBox ();
            this.m_FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog ();
            this.m_statusStrip.SuspendLayout ();
            this.m_grpDirPath.SuspendLayout ();
            this.m_grpPageInput.SuspendLayout ();
            this.m_grpListDTBfiles.SuspendLayout ();
            this.m_grpManipulateDTB.SuspendLayout ();
            this.SuspendLayout ();
            // 
            // m_lblDaisy3Merger
            // 
            resources.ApplyResources ( this.m_lblDaisy3Merger, "m_lblDaisy3Merger" );
            this.m_lblDaisy3Merger.Name = "m_lblDaisy3Merger";
            // 
            // m_BtnReset
            // 
            resources.ApplyResources ( this.m_BtnReset, "m_BtnReset" );
            this.m_BtnReset.Name = "m_BtnReset";
            this.m_BtnReset.UseVisualStyleBackColor = true;
            this.m_BtnReset.Click += new System.EventHandler ( this.m_BtnReset_Click );
            // 
            // m_BtnMerge
            // 
            resources.ApplyResources ( this.m_BtnMerge, "m_BtnMerge" );
            this.m_BtnMerge.Name = "m_BtnMerge";
            this.m_BtnMerge.UseVisualStyleBackColor = true;
            this.m_BtnMerge.Click += new System.EventHandler ( this.m_BtnMerge_Click );
            // 
            // m_BtnOutputDirectory
            // 
            resources.ApplyResources ( this.m_BtnOutputDirectory, "m_BtnOutputDirectory" );
            this.m_BtnOutputDirectory.Name = "m_BtnOutputDirectory";
            this.m_BtnOutputDirectory.UseVisualStyleBackColor = true;
            this.m_BtnOutputDirectory.Click += new System.EventHandler ( this.m_BtnOutputDirectory_Click );
            // 
            // m_txtDirectoryPath
            // 
            resources.ApplyResources ( this.m_txtDirectoryPath, "m_txtDirectoryPath" );
            this.m_txtDirectoryPath.Name = "m_txtDirectoryPath";
            this.m_txtDirectoryPath.ReadOnly = true;
            // 
            // m_BtnCancel
            // 
            this.m_BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources ( this.m_BtnCancel, "m_BtnCancel" );
            this.m_BtnCancel.Name = "m_BtnCancel";
            this.m_BtnCancel.UseVisualStyleBackColor = true;
            this.m_BtnCancel.Click += new System.EventHandler ( this.m_BtnExit_Click );
            // 
            // m_lbOutputDirPath
            // 
            resources.ApplyResources ( this.m_lbOutputDirPath, "m_lbOutputDirPath" );
            this.m_lbOutputDirPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_lbOutputDirPath.Name = "m_lbOutputDirPath";
            // 
            // m_statusStrip
            // 
            this.m_statusStrip.Items.AddRange ( new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusLabel} );
            resources.ApplyResources ( this.m_statusStrip, "m_statusStrip" );
            this.m_statusStrip.Name = "m_statusStrip";
            // 
            // m_StatusLabel
            // 
            this.m_StatusLabel.Name = "m_StatusLabel";
            resources.ApplyResources ( this.m_StatusLabel, "m_StatusLabel" );
            // 
            // m_grpDirPath
            // 
            this.m_grpDirPath.Controls.Add ( this.m_BtnHelp );
            this.m_grpDirPath.Controls.Add ( this.m_BtnValidateOutput );
            this.m_grpDirPath.Controls.Add ( this.m_BtnMerge );
            this.m_grpDirPath.Controls.Add ( this.m_BtnReset );
            this.m_grpDirPath.Controls.Add ( this.m_BtnCancel );
            this.m_grpDirPath.Controls.Add ( this.m_BtnOutputDirectory );
            this.m_grpDirPath.Controls.Add ( this.m_txtDirectoryPath );
            this.m_grpDirPath.Controls.Add ( this.m_lbOutputDirPath );
            resources.ApplyResources ( this.m_grpDirPath, "m_grpDirPath" );
            this.m_grpDirPath.Name = "m_grpDirPath";
            this.m_grpDirPath.TabStop = false;
            // 
            // m_BtnHelp
            // 
            resources.ApplyResources ( this.m_BtnHelp, "m_BtnHelp" );
            this.m_BtnHelp.Name = "m_BtnHelp";
            this.m_BtnHelp.UseVisualStyleBackColor = true;
            this.m_BtnHelp.Click += new System.EventHandler ( this.m_BtnHelp_Click );
            // 
            // m_BtnValidateOutput
            // 
            resources.ApplyResources ( this.m_BtnValidateOutput, "m_BtnValidateOutput" );
            this.m_BtnValidateOutput.Name = "m_BtnValidateOutput";
            this.m_BtnValidateOutput.UseVisualStyleBackColor = true;
            this.m_BtnValidateOutput.Click += new System.EventHandler ( this.m_BtnValidateOutput_Click );
            // 
            // m_grpPageInput
            // 
            this.m_grpPageInput.Controls.Add ( this.m_rdbRenumberPages );
            this.m_grpPageInput.Controls.Add ( this.m_rdbExistingNumberOfPages );
            resources.ApplyResources ( this.m_grpPageInput, "m_grpPageInput" );
            this.m_grpPageInput.Name = "m_grpPageInput";
            this.m_grpPageInput.TabStop = false;
            // 
            // m_rdbRenumberPages
            // 
            resources.ApplyResources ( this.m_rdbRenumberPages, "m_rdbRenumberPages" );
            this.m_rdbRenumberPages.Checked = true;
            this.m_rdbRenumberPages.Name = "m_rdbRenumberPages";
            this.m_rdbRenumberPages.TabStop = true;
            this.m_rdbRenumberPages.UseVisualStyleBackColor = true;
            // 
            // m_rdbExistingNumberOfPages
            // 
            resources.ApplyResources ( this.m_rdbExistingNumberOfPages, "m_rdbExistingNumberOfPages" );
            this.m_rdbExistingNumberOfPages.Name = "m_rdbExistingNumberOfPages";
            this.m_rdbExistingNumberOfPages.UseVisualStyleBackColor = true;
            // 
            // m_grpListDTBfiles
            // 
            this.m_grpListDTBfiles.Controls.Add ( this.m_lbDTBfiles );
            resources.ApplyResources ( this.m_grpListDTBfiles, "m_grpListDTBfiles" );
            this.m_grpListDTBfiles.Name = "m_grpListDTBfiles";
            this.m_grpListDTBfiles.TabStop = false;
            // 
            // m_lbDTBfiles
            // 
            resources.ApplyResources ( this.m_lbDTBfiles, "m_lbDTBfiles" );
            this.m_lbDTBfiles.FormattingEnabled = true;
            this.m_lbDTBfiles.Name = "m_lbDTBfiles";
            this.m_lbDTBfiles.SelectedIndexChanged += new System.EventHandler ( this.m_lbDTBfiles_SelectedIndexChanged );
            // 
            // m_BtnDelete
            // 
            resources.ApplyResources ( this.m_BtnDelete, "m_BtnDelete" );
            this.m_BtnDelete.Name = "m_BtnDelete";
            this.m_BtnDelete.UseVisualStyleBackColor = true;
            this.m_BtnDelete.Click += new System.EventHandler ( this.m_BtnDelete_Click );
            // 
            // m_BtnDown
            // 
            resources.ApplyResources ( this.m_BtnDown, "m_BtnDown" );
            this.m_BtnDown.Name = "m_BtnDown";
            this.m_BtnDown.UseVisualStyleBackColor = true;
            this.m_BtnDown.Click += new System.EventHandler ( this.m_BtnDown_Click );
            // 
            // m_btnUP
            // 
            resources.ApplyResources ( this.m_btnUP, "m_btnUP" );
            this.m_btnUP.Name = "m_btnUP";
            this.m_btnUP.UseVisualStyleBackColor = true;
            this.m_btnUP.Click += new System.EventHandler ( this.m_btnUP_Click );
            // 
            // m_BtnValidateInput
            // 
            resources.ApplyResources ( this.m_BtnValidateInput, "m_BtnValidateInput" );
            this.m_BtnValidateInput.Name = "m_BtnValidateInput";
            this.m_BtnValidateInput.UseVisualStyleBackColor = true;
            this.m_BtnValidateInput.Click += new System.EventHandler ( this.m_BtnValidateInput_Click );
            // 
            // m_txtDTBookInfo
            // 
            resources.ApplyResources ( this.m_txtDTBookInfo, "m_txtDTBookInfo" );
            this.m_txtDTBookInfo.Name = "m_txtDTBookInfo";
            this.m_txtDTBookInfo.ReadOnly = true;
            // 
            // m_btnAdd
            // 
            this.m_btnAdd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources ( this.m_btnAdd, "m_btnAdd" );
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler ( this.m_btnAdd_Click );
            // 
            // m_lblDBookInfo
            // 
            resources.ApplyResources ( this.m_lblDBookInfo, "m_lblDBookInfo" );
            this.m_lblDBookInfo.Name = "m_lblDBookInfo";
            // 
            // m_grpManipulateDTB
            // 
            this.m_grpManipulateDTB.Controls.Add ( this.m_lblDBookInfo );
            this.m_grpManipulateDTB.Controls.Add ( this.m_btnAdd );
            this.m_grpManipulateDTB.Controls.Add ( this.m_txtDTBookInfo );
            this.m_grpManipulateDTB.Controls.Add ( this.m_BtnValidateInput );
            this.m_grpManipulateDTB.Controls.Add ( this.m_btnUP );
            this.m_grpManipulateDTB.Controls.Add ( this.m_BtnDown );
            this.m_grpManipulateDTB.Controls.Add ( this.m_BtnDelete );
            this.m_grpManipulateDTB.Controls.Add ( this.m_grpListDTBfiles );
            resources.ApplyResources ( this.m_grpManipulateDTB, "m_grpManipulateDTB" );
            this.m_grpManipulateDTB.Name = "m_grpManipulateDTB";
            this.m_grpManipulateDTB.TabStop = false;
            // 
            // Daisy3MergerForm
            // 
            this.AcceptButton = this.m_BtnMerge;
            resources.ApplyResources ( this, "$this" );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add ( this.m_grpPageInput );
            this.Controls.Add ( this.m_grpDirPath );
            this.Controls.Add ( this.m_grpManipulateDTB );
            this.Controls.Add ( this.m_statusStrip );
            this.Controls.Add ( this.m_lblDaisy3Merger );
            this.HelpButton = true;
            this.Name = "Daisy3MergerForm";
            this.Load += new System.EventHandler ( this.Daisy3MergerForm_Load );
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler ( this.Daisy3MergerForm_HelpRequested );
            this.m_statusStrip.ResumeLayout ( false );
            this.m_statusStrip.PerformLayout ();
            this.m_grpDirPath.ResumeLayout ( false );
            this.m_grpDirPath.PerformLayout ();
            this.m_grpPageInput.ResumeLayout ( false );
            this.m_grpPageInput.PerformLayout ();
            this.m_grpListDTBfiles.ResumeLayout ( false );
            this.m_grpManipulateDTB.ResumeLayout ( false );
            this.m_grpManipulateDTB.PerformLayout ();
            this.ResumeLayout ( false );
            this.PerformLayout ();

            }

        #endregion

        private System.Windows.Forms.Label m_lblDaisy3Merger;
        private System.Windows.Forms.Button m_BtnReset;
        private System.Windows.Forms.Button m_BtnMerge;
        private System.Windows.Forms.Button m_BtnOutputDirectory;
        private System.Windows.Forms.TextBox m_txtDirectoryPath;
        private System.Windows.Forms.Button m_BtnCancel;
        private System.Windows.Forms.Label m_lbOutputDirPath;
        private System.ComponentModel.BackgroundWorker m_bgWorker;
        private System.Windows.Forms.StatusStrip m_statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel m_StatusLabel;
        private System.Windows.Forms.GroupBox m_grpDirPath;
        private System.Windows.Forms.Button m_BtnValidateOutput;
        private System.Windows.Forms.GroupBox m_grpPageInput;
        private System.Windows.Forms.RadioButton m_rdbRenumberPages;
        private System.Windows.Forms.RadioButton m_rdbExistingNumberOfPages;
        private System.Windows.Forms.GroupBox m_grpListDTBfiles;
        private System.Windows.Forms.ListBox m_lbDTBfiles;
        private System.Windows.Forms.Button m_BtnDelete;
        private System.Windows.Forms.Button m_BtnDown;
        private System.Windows.Forms.Button m_btnUP;
        private System.Windows.Forms.Button m_BtnValidateInput;
        private System.Windows.Forms.TextBox m_txtDTBookInfo;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Label m_lblDBookInfo;
        private System.Windows.Forms.GroupBox m_grpManipulateDTB;
        private System.Windows.Forms.Button m_BtnHelp;
        private System.Windows.Forms.FolderBrowserDialog m_FolderBrowserDialog;        
        }
    }

