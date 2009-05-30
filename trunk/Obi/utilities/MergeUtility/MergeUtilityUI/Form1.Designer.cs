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
                this.m_Txt = new System.Windows.Forms.TextBox();
                this.label1 = new System.Windows.Forms.Label();
                this.label2 = new System.Windows.Forms.Label();
                this.m_grpListOPFfiles.SuspendLayout();
                this.SuspendLayout();
                // 
                // m_lblDaisy3Merger
                // 
                this.m_lblDaisy3Merger.AutoSize = true;
                this.m_lblDaisy3Merger.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_lblDaisy3Merger.Location = new System.Drawing.Point(237, 34);
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
                this.m_grpListOPFfiles.Location = new System.Drawing.Point(180, 90);
                this.m_grpListOPFfiles.Name = "m_grpListOPFfiles";
                this.m_grpListOPFfiles.Size = new System.Drawing.Size(476, 175);
                this.m_grpListOPFfiles.TabIndex = 1;
                this.m_grpListOPFfiles.TabStop = false;
                this.m_grpListOPFfiles.Text = "List of Input";
                // 
                // m_lbOPFfiles
                // 
                this.m_lbOPFfiles.FormattingEnabled = true;
                this.m_lbOPFfiles.ItemHeight = 16;
                this.m_lbOPFfiles.Location = new System.Drawing.Point(15, 19);
                this.m_lbOPFfiles.Name = "m_lbOPFfiles";
                this.m_lbOPFfiles.Size = new System.Drawing.Size(446, 132);
                this.m_lbOPFfiles.TabIndex = 2;
                this.m_lbOPFfiles.SelectedIndexChanged += new System.EventHandler(this.m_lbOPFfiles_SelectedIndexChanged);
                // 
                // m_btnAdd
                // 
                this.m_btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btnAdd.Location = new System.Drawing.Point(12, 103);
                this.m_btnAdd.Name = "m_btnAdd";
                this.m_btnAdd.Size = new System.Drawing.Size(123, 33);
                this.m_btnAdd.TabIndex = 0;
                this.m_btnAdd.Text = "&Add";
                this.m_btnAdd.UseVisualStyleBackColor = true;
                this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
                // 
                // m_BtnDelete
                // 
                this.m_BtnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnDelete.Location = new System.Drawing.Point(662, 220);
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
                this.m_BtnReset.Location = new System.Drawing.Point(391, 408);
                this.m_BtnReset.Name = "m_BtnReset";
                this.m_BtnReset.Size = new System.Drawing.Size(123, 30);
                this.m_BtnReset.TabIndex = 10;
                this.m_BtnReset.Text = "&Reset";
                this.m_BtnReset.UseVisualStyleBackColor = true;
                this.m_BtnReset.Click += new System.EventHandler(this.m_BtnReset_Click);
                // 
                // m_BtnMerge
                // 
                this.m_BtnMerge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnMerge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnMerge.Location = new System.Drawing.Point(195, 408);
                this.m_BtnMerge.Name = "m_BtnMerge";
                this.m_BtnMerge.Size = new System.Drawing.Size(123, 30);
                this.m_BtnMerge.TabIndex = 9;
                this.m_BtnMerge.Text = "&Merge";
                this.m_BtnMerge.UseVisualStyleBackColor = true;
                this.m_BtnMerge.Click += new System.EventHandler(this.m_BtnMerge_Click);
                // 
                // m_BtnOutputDirectory
                // 
                this.m_BtnOutputDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnOutputDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnOutputDirectory.Location = new System.Drawing.Point(646, 347);
                this.m_BtnOutputDirectory.Name = "m_BtnOutputDirectory";
                this.m_BtnOutputDirectory.Size = new System.Drawing.Size(139, 30);
                this.m_BtnOutputDirectory.TabIndex = 8;
                this.m_BtnOutputDirectory.Text = "&Browse";
                this.m_BtnOutputDirectory.UseVisualStyleBackColor = true;
                this.m_BtnOutputDirectory.Click += new System.EventHandler(this.m_BtnOutputDirectory_Click);
                // 
                // m_txtDirectoryPath
                // 
                this.m_txtDirectoryPath.Location = new System.Drawing.Point(195, 356);
                this.m_txtDirectoryPath.Name = "m_txtDirectoryPath";
                this.m_txtDirectoryPath.ReadOnly = true;
                this.m_txtDirectoryPath.Size = new System.Drawing.Size(414, 20);
                this.m_txtDirectoryPath.TabIndex = 7;
                // 
                // m_btnUP
                // 
                this.m_btnUP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btnUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btnUP.Location = new System.Drawing.Point(662, 122);
                this.m_btnUP.Name = "m_btnUP";
                this.m_btnUP.Size = new System.Drawing.Size(123, 30);
                this.m_btnUP.TabIndex = 3;
                this.m_btnUP.Text = "Move &Up";
                this.m_btnUP.UseVisualStyleBackColor = true;
                this.m_btnUP.Click += new System.EventHandler(this.m_btnUP_Click);
                // 
                // m_BtnDown
                // 
                this.m_BtnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_BtnDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_BtnDown.Location = new System.Drawing.Point(662, 170);
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
                this.m_BtnCancel.Location = new System.Drawing.Point(572, 408);
                this.m_BtnCancel.Name = "m_BtnCancel";
                this.m_BtnCancel.Size = new System.Drawing.Size(107, 30);
                this.m_BtnCancel.TabIndex = 11;
                this.m_BtnCancel.Text = "&Cancel";
                this.m_BtnCancel.UseVisualStyleBackColor = true;
                this.m_BtnCancel.Click += new System.EventHandler(this.m_BtnExit_Click);
                // 
                // m_Txt
                // 
                this.m_Txt.Location = new System.Drawing.Point(195, 289);
                this.m_Txt.Multiline = true;
                this.m_Txt.Name = "m_Txt";
                this.m_Txt.Size = new System.Drawing.Size(222, 61);
                this.m_Txt.TabIndex = 4;
                // 
                // label1
                // 
                this.label1.AutoSize = true;
                this.label1.Location = new System.Drawing.Point(22, 308);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(167, 13);
                this.label1.TabIndex = 3;
                this.label1.Text = "Information about the Daisy Book ";
                // 
                // label2
                // 
                this.label2.AutoSize = true;
                this.label2.Location = new System.Drawing.Point(69, 356);
                this.label2.Name = "label2";
                this.label2.Size = new System.Drawing.Size(84, 13);
                this.label2.TabIndex = 6;
                this.label2.Text = "Output Directory";
                // 
                // m_formDaisy3Merger
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(798, 453);
                this.Controls.Add(this.label2);
                this.Controls.Add(this.label1);
                this.Controls.Add(this.m_Txt);
                this.Controls.Add(this.m_BtnCancel);
                this.Controls.Add(this.m_BtnDown);
                this.Controls.Add(this.m_btnUP);
                this.Controls.Add(this.m_txtDirectoryPath);
                this.Controls.Add(this.m_BtnOutputDirectory);
                this.Controls.Add(this.m_BtnMerge);
                this.Controls.Add(this.m_BtnReset);
                this.Controls.Add(this.m_BtnDelete);
                this.Controls.Add(this.m_btnAdd);
                this.Controls.Add(this.m_grpListOPFfiles);
                this.Controls.Add(this.m_lblDaisy3Merger);
                this.Name = "m_formDaisy3Merger";
                this.Text = "DTB Merger Form";
                this.m_grpListOPFfiles.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox m_Txt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        }
    }

