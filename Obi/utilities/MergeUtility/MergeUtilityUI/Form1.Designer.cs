namespace MergeUtilityUI
    {
    partial class m_formDaisy3Merger
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
                this.m_lblDaisy3Merger = new System.Windows.Forms.Label();
                this.m_grpListOPFfiles = new System.Windows.Forms.GroupBox();
                this.m_lbOPFfiles = new System.Windows.Forms.ListBox();
                this.m_btnAdd = new System.Windows.Forms.Button();
                this.m_BtnDelete = new System.Windows.Forms.Button();
                this.m_BtnReset = new System.Windows.Forms.Button();
                this.m_BtnMerge = new System.Windows.Forms.Button();
                this.m_BtnOutputDirectory = new System.Windows.Forms.Button();
                this.m_txtDirectoryPath = new System.Windows.Forms.TextBox();
                this.m_btnUP = new System.Windows.Forms.Button();
                this.m_BtnDown = new System.Windows.Forms.Button();
                this.m_BtnCancel = new System.Windows.Forms.Button();
                this.m_txtDTBookInfo = new System.Windows.Forms.TextBox();
                this.m_lblDBookInfo = new System.Windows.Forms.Label();
                this.m_lbOutputDirPath = new System.Windows.Forms.Label();
                this.m_bgWorker = new System.ComponentModel.BackgroundWorker();
                this.m_statusStrip = new System.Windows.Forms.StatusStrip();
                this.m_StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
                this.m_grpManipulateOPF = new System.Windows.Forms.GroupBox();
                this.m_BtnValidateInput = new System.Windows.Forms.Button();
                this.m_grpDaisyBookInfo = new System.Windows.Forms.GroupBox();
                this.m_grpDirPath = new System.Windows.Forms.GroupBox();
                this.m_BtnValidateOutput = new System.Windows.Forms.Button();
                this.m_grpListOPFfiles.SuspendLayout();
                this.m_statusStrip.SuspendLayout();
                this.m_grpManipulateOPF.SuspendLayout();
                this.m_grpDaisyBookInfo.SuspendLayout();
                this.m_grpDirPath.SuspendLayout();
                this.SuspendLayout();
                // 
                // m_lblDaisy3Merger
                // 
                this.m_lblDaisy3Merger.AutoSize = true;
                this.m_lblDaisy3Merger.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_lblDaisy3Merger.Location = new System.Drawing.Point(371, 32);
                this.m_lblDaisy3Merger.Name = "m_lblDaisy3Merger";
                this.m_lblDaisy3Merger.Size = new System.Drawing.Size(198, 25);
                this.m_lblDaisy3Merger.TabIndex = 0;
                this.m_lblDaisy3Merger.Text = "DAISY3 MERGER";
                this.m_lblDaisy3Merger.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
                // 
                // m_grpListOPFfiles
                // 
                this.m_grpListOPFfiles.Controls.Add(this.m_lbOPFfiles);
                this.m_grpListOPFfiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_grpListOPFfiles.Location = new System.Drawing.Point(139, 25);
                this.m_grpListOPFfiles.Name = "m_grpListOPFfiles";
                this.m_grpListOPFfiles.Size = new System.Drawing.Size(529, 167);
                this.m_grpListOPFfiles.TabIndex = 1;
                this.m_grpListOPFfiles.TabStop = false;
                this.m_grpListOPFfiles.Text = "List of Input";
                // 
                // m_lbOPFfiles
                // 
                this.m_lbOPFfiles.FormattingEnabled = true;
                this.m_lbOPFfiles.ItemHeight = 16;
                this.m_lbOPFfiles.Location = new System.Drawing.Point(18, 22);
                this.m_lbOPFfiles.Name = "m_lbOPFfiles";
                this.m_lbOPFfiles.Size = new System.Drawing.Size(495, 132);
                this.m_lbOPFfiles.TabIndex = 2;
                this.m_lbOPFfiles.SelectedIndexChanged += new System.EventHandler(this.m_lbOPFfiles_SelectedIndexChanged);
                // 
                // m_btnAdd
                // 
                this.m_btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btnAdd.Location = new System.Drawing.Point(6, 47);
                this.m_btnAdd.Name = "m_btnAdd";
                this.m_btnAdd.Size = new System.Drawing.Size(123, 33);
                this.m_btnAdd.TabIndex = 0;
                this.m_btnAdd.Text = "&Add";
                this.m_btnAdd.UseVisualStyleBackColor = true;
                this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
                // 
                // m_BtnDelete
                // 
                this.m_BtnDelete.Enabled = false;
                this.m_BtnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnDelete.Location = new System.Drawing.Point(6, 100);
                this.m_BtnDelete.Name = "m_BtnDelete";
                this.m_BtnDelete.Size = new System.Drawing.Size(123, 30);
                this.m_BtnDelete.TabIndex = 5;
                this.m_BtnDelete.Text = "&Delete";
                this.m_BtnDelete.UseVisualStyleBackColor = true;
                this.m_BtnDelete.Click += new System.EventHandler(this.m_BtnDelete_Click);
                // 
                // m_BtnReset
                // 
                this.m_BtnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnReset.Location = new System.Drawing.Point(469, 86);
                this.m_BtnReset.Name = "m_BtnReset";
                this.m_BtnReset.Size = new System.Drawing.Size(146, 30);
                this.m_BtnReset.TabIndex = 10;
                this.m_BtnReset.Text = "&Reset";
                this.m_BtnReset.UseVisualStyleBackColor = true;
                this.m_BtnReset.Click += new System.EventHandler(this.m_BtnReset_Click);
                // 
                // m_BtnMerge
                // 
                this.m_BtnMerge.Enabled = false;
                this.m_BtnMerge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnMerge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnMerge.Location = new System.Drawing.Point(238, 86);
                this.m_BtnMerge.Name = "m_BtnMerge";
                this.m_BtnMerge.Size = new System.Drawing.Size(148, 30);
                this.m_BtnMerge.TabIndex = 9;
                this.m_BtnMerge.Text = "&Merge";
                this.m_BtnMerge.UseVisualStyleBackColor = true;
                this.m_BtnMerge.Click += new System.EventHandler(this.m_BtnMerge_Click);
                // 
                // m_BtnOutputDirectory
                // 
                this.m_BtnOutputDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnOutputDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnOutputDirectory.Location = new System.Drawing.Point(692, 33);
                this.m_BtnOutputDirectory.Name = "m_BtnOutputDirectory";
                this.m_BtnOutputDirectory.Size = new System.Drawing.Size(139, 30);
                this.m_BtnOutputDirectory.TabIndex = 8;
                this.m_BtnOutputDirectory.Text = "&Browse";
                this.m_BtnOutputDirectory.UseVisualStyleBackColor = true;
                this.m_BtnOutputDirectory.Click += new System.EventHandler(this.m_BtnOutputDirectory_Click);
                // 
                // m_txtDirectoryPath
                // 
                this.m_txtDirectoryPath.Location = new System.Drawing.Point(184, 37);
                this.m_txtDirectoryPath.Name = "m_txtDirectoryPath";
                this.m_txtDirectoryPath.ReadOnly = true;
                this.m_txtDirectoryPath.Size = new System.Drawing.Size(484, 22);
                this.m_txtDirectoryPath.TabIndex = 7;
                // 
                // m_btnUP
                // 
                this.m_btnUP.Enabled = false;
                this.m_btnUP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btnUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btnUP.Location = new System.Drawing.Point(692, 47);
                this.m_btnUP.Name = "m_btnUP";
                this.m_btnUP.Size = new System.Drawing.Size(123, 30);
                this.m_btnUP.TabIndex = 3;
                this.m_btnUP.Text = "Move &Up";
                this.m_btnUP.UseVisualStyleBackColor = true;
                this.m_btnUP.Click += new System.EventHandler(this.m_btnUP_Click);
                // 
                // m_BtnDown
                // 
                this.m_BtnDown.Enabled = false;
                this.m_BtnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnDown.Location = new System.Drawing.Point(692, 106);
                this.m_BtnDown.Name = "m_BtnDown";
                this.m_BtnDown.Size = new System.Drawing.Size(123, 30);
                this.m_BtnDown.TabIndex = 4;
                this.m_BtnDown.Text = "Move Dow&n";
                this.m_BtnDown.UseVisualStyleBackColor = true;
                this.m_BtnDown.Click += new System.EventHandler(this.m_BtnDown_Click);
                // 
                // m_BtnCancel
                // 
                this.m_BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnCancel.Location = new System.Drawing.Point(692, 86);
                this.m_BtnCancel.Name = "m_BtnCancel";
                this.m_BtnCancel.Size = new System.Drawing.Size(139, 30);
                this.m_BtnCancel.TabIndex = 11;
                this.m_BtnCancel.Text = "&Cancel";
                this.m_BtnCancel.UseVisualStyleBackColor = true;
                this.m_BtnCancel.Click += new System.EventHandler(this.m_BtnExit_Click);
                // 
                // m_txtDTBookInfo
                // 
                this.m_txtDTBookInfo.Location = new System.Drawing.Point(262, 21);
                this.m_txtDTBookInfo.Multiline = true;
                this.m_txtDTBookInfo.Name = "m_txtDTBookInfo";
                this.m_txtDTBookInfo.Size = new System.Drawing.Size(569, 101);
                this.m_txtDTBookInfo.TabIndex = 4;
                // 
                // m_lblDBookInfo
                // 
                this.m_lblDBookInfo.AutoSize = true;
                this.m_lblDBookInfo.Location = new System.Drawing.Point(11, 53);
                this.m_lblDBookInfo.Name = "m_lblDBookInfo";
                this.m_lblDBookInfo.Size = new System.Drawing.Size(236, 16);
                this.m_lblDBookInfo.TabIndex = 3;
                this.m_lblDBookInfo.Text = "Information about the Daisy Book";
                // 
                // m_lbOutputDirPath
                // 
                this.m_lbOutputDirPath.AutoSize = true;
                this.m_lbOutputDirPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_lbOutputDirPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_lbOutputDirPath.Location = new System.Drawing.Point(11, 39);
                this.m_lbOutputDirPath.Name = "m_lbOutputDirPath";
                this.m_lbOutputDirPath.Size = new System.Drawing.Size(158, 18);
                this.m_lbOutputDirPath.TabIndex = 6;
                this.m_lbOutputDirPath.Text = "Output Directory Path :";
                // 
                // m_statusStrip
                // 
                this.m_statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusLabel});
                this.m_statusStrip.Location = new System.Drawing.Point(0, 608);
                this.m_statusStrip.Name = "m_statusStrip";
                this.m_statusStrip.Size = new System.Drawing.Size(883, 22);
                this.m_statusStrip.TabIndex = 12;
                // 
                // m_StatusLabel
                // 
                this.m_StatusLabel.Name = "m_StatusLabel";
                this.m_StatusLabel.Size = new System.Drawing.Size(307, 17);
                this.m_StatusLabel.Text = "Please Clik Add Button to add the OPF files in Listbox...    ";
                // 
                // m_grpManipulateOPF
                // 
                this.m_grpManipulateOPF.Controls.Add(this.m_btnAdd);
                this.m_grpManipulateOPF.Controls.Add(this.m_BtnValidateInput);
                this.m_grpManipulateOPF.Controls.Add(this.m_btnUP);
                this.m_grpManipulateOPF.Controls.Add(this.m_BtnDown);
                this.m_grpManipulateOPF.Controls.Add(this.m_BtnDelete);
                this.m_grpManipulateOPF.Controls.Add(this.m_grpListOPFfiles);
                this.m_grpManipulateOPF.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_grpManipulateOPF.Location = new System.Drawing.Point(25, 75);
                this.m_grpManipulateOPF.Name = "m_grpManipulateOPF";
                this.m_grpManipulateOPF.Size = new System.Drawing.Size(846, 208);
                this.m_grpManipulateOPF.TabIndex = 13;
                this.m_grpManipulateOPF.TabStop = false;
                this.m_grpManipulateOPF.Text = "Input of OPF Files...";
                // 
                // m_BtnValidateInput
                // 
                this.m_BtnValidateInput.Enabled = false;
                this.m_BtnValidateInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnValidateInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnValidateInput.Location = new System.Drawing.Point(6, 150);
                this.m_BtnValidateInput.Name = "m_BtnValidateInput";
                this.m_BtnValidateInput.Size = new System.Drawing.Size(123, 29);
                this.m_BtnValidateInput.TabIndex = 12;
                this.m_BtnValidateInput.Text = "Validate &Input";
                this.m_BtnValidateInput.UseVisualStyleBackColor = true;
                this.m_BtnValidateInput.Click += new System.EventHandler(this.m_BtnValidateInput_Click);
                // 
                // m_grpDaisyBookInfo
                // 
                this.m_grpDaisyBookInfo.Controls.Add(this.m_txtDTBookInfo);
                this.m_grpDaisyBookInfo.Controls.Add(this.m_lblDBookInfo);
                this.m_grpDaisyBookInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_grpDaisyBookInfo.Location = new System.Drawing.Point(25, 300);
                this.m_grpDaisyBookInfo.Name = "m_grpDaisyBookInfo";
                this.m_grpDaisyBookInfo.Size = new System.Drawing.Size(846, 135);
                this.m_grpDaisyBookInfo.TabIndex = 14;
                this.m_grpDaisyBookInfo.TabStop = false;
                this.m_grpDaisyBookInfo.Text = "Daisy Book Information";
                // 
                // m_grpDirPath
                // 
                this.m_grpDirPath.Controls.Add(this.m_BtnValidateOutput);
                this.m_grpDirPath.Controls.Add(this.m_BtnMerge);
                this.m_grpDirPath.Controls.Add(this.m_BtnReset);
                this.m_grpDirPath.Controls.Add(this.m_BtnCancel);
                this.m_grpDirPath.Controls.Add(this.m_BtnOutputDirectory);
                this.m_grpDirPath.Controls.Add(this.m_txtDirectoryPath);
                this.m_grpDirPath.Controls.Add(this.m_lbOutputDirPath);
                this.m_grpDirPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_grpDirPath.Location = new System.Drawing.Point(25, 459);
                this.m_grpDirPath.Name = "m_grpDirPath";
                this.m_grpDirPath.Size = new System.Drawing.Size(846, 131);
                this.m_grpDirPath.TabIndex = 16;
                this.m_grpDirPath.TabStop = false;
                this.m_grpDirPath.Text = "Merged OPF file Path...";
                // 
                // m_BtnValidateOutput
                // 
                this.m_BtnValidateOutput.Enabled = false;
                this.m_BtnValidateOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnValidateOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnValidateOutput.Location = new System.Drawing.Point(14, 86);
                this.m_BtnValidateOutput.Name = "m_BtnValidateOutput";
                this.m_BtnValidateOutput.Size = new System.Drawing.Size(155, 30);
                this.m_BtnValidateOutput.TabIndex = 13;
                this.m_BtnValidateOutput.Text = "Validate &Output";
                this.m_BtnValidateOutput.UseVisualStyleBackColor = true;
                this.m_BtnValidateOutput.Click += new System.EventHandler(this.m_BtnValidateOutput_Click);
                // 
                // m_formDaisy3Merger
                // 
                this.AcceptButton = this.m_BtnMerge;
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(883, 630);
                this.Controls.Add(this.m_grpDirPath);
                this.Controls.Add(this.m_grpDaisyBookInfo);
                this.Controls.Add(this.m_grpManipulateOPF);
                this.Controls.Add(this.m_statusStrip);
                this.Controls.Add(this.m_lblDaisy3Merger);
                this.Name = "m_formDaisy3Merger";
                this.Text = "DTB Merger Form";
                this.m_grpListOPFfiles.ResumeLayout(false);
                this.m_statusStrip.ResumeLayout(false);
                this.m_statusStrip.PerformLayout();
                this.m_grpManipulateOPF.ResumeLayout(false);
                this.m_grpDaisyBookInfo.ResumeLayout(false);
                this.m_grpDaisyBookInfo.PerformLayout();
                this.m_grpDirPath.ResumeLayout(false);
                this.m_grpDirPath.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Label m_lblDaisy3Merger;
        private System.Windows.Forms.GroupBox m_grpListOPFfiles;
        private System.Windows.Forms.ListBox m_lbOPFfiles;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_BtnDelete;
        private System.Windows.Forms.Button m_BtnReset;
        private System.Windows.Forms.Button m_BtnMerge;
        private System.Windows.Forms.Button m_BtnOutputDirectory;
        private System.Windows.Forms.TextBox m_txtDirectoryPath;
        private System.Windows.Forms.Button m_btnUP;
        private System.Windows.Forms.Button m_BtnDown;
        private System.Windows.Forms.Button m_BtnCancel;
        private System.Windows.Forms.TextBox m_txtDTBookInfo;
        private System.Windows.Forms.Label m_lblDBookInfo;
        private System.Windows.Forms.Label m_lbOutputDirPath;
        private System.ComponentModel.BackgroundWorker m_bgWorker;
        private System.Windows.Forms.StatusStrip m_statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel m_StatusLabel;
        private System.Windows.Forms.GroupBox m_grpManipulateOPF;
        private System.Windows.Forms.GroupBox m_grpDaisyBookInfo;
        private System.Windows.Forms.GroupBox m_grpDirPath;
        private System.Windows.Forms.Button m_BtnValidateOutput;
        private System.Windows.Forms.Button m_BtnValidateInput;
        }
    }

