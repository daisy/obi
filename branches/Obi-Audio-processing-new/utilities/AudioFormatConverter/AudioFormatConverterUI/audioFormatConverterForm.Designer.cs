namespace AudioFormatConverterUI
    {
    partial class m_audioFormatConverterForm
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
                this.m_lblform = new System.Windows.Forms.Label();
                this.m_btn_Add = new System.Windows.Forms.Button();
                this.m_lb_addFiles = new System.Windows.Forms.ListBox();
                this.m_lbl_sampleRate = new System.Windows.Forms.Label();
                this.m_cb_sampleRate = new System.Windows.Forms.ComboBox();
                this.m_lbl_channel = new System.Windows.Forms.Label();
                this.m_cb_channel = new System.Windows.Forms.ComboBox();
                this.m_btn_Browse = new System.Windows.Forms.Button();
                this.m_txt_Browse = new System.Windows.Forms.TextBox();
                this.m_btn_Start = new System.Windows.Forms.Button();
                this.m_btn_Help = new System.Windows.Forms.Button();
                this.m_btn_cancel = new System.Windows.Forms.Button();
                this.m_btnDelete = new System.Windows.Forms.Button();
                this.m_btnReset = new System.Windows.Forms.Button();
                this.m_grp_outputPath = new System.Windows.Forms.GroupBox();
                this.m_lbOutputDirPath = new System.Windows.Forms.Label();
                this.m_grp_InputFiles = new System.Windows.Forms.GroupBox();
                this.m_grpListAudiofiles = new System.Windows.Forms.GroupBox();
                this.m_grp_AudioFormat = new System.Windows.Forms.GroupBox();
                this.helpProvider1 = new System.Windows.Forms.HelpProvider();
                this.m_grp_outputPath.SuspendLayout();
                this.m_grp_InputFiles.SuspendLayout();
                this.m_grpListAudiofiles.SuspendLayout();
                this.m_grp_AudioFormat.SuspendLayout();
                this.SuspendLayout();
                // 
                // m_lblform
                // 
                this.m_lblform.AutoSize = true;
                this.m_lblform.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_lblform.Location = new System.Drawing.Point(214, 30);
                this.m_lblform.Name = "m_lblform";
                this.m_lblform.Size = new System.Drawing.Size(290, 29);
                this.m_lblform.TabIndex = 0;
                this.m_lblform.Text = "Audio Format Converter";
                // 
                // m_btn_Add
                // 
                this.m_btn_Add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btn_Add.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btn_Add.Location = new System.Drawing.Point(20, 30);
                this.m_btn_Add.Name = "m_btn_Add";
                this.m_btn_Add.Size = new System.Drawing.Size(94, 27);
                this.m_btn_Add.TabIndex = 1;
                this.m_btn_Add.Text = "&Add";
                this.m_btn_Add.UseVisualStyleBackColor = true;
                this.m_btn_Add.Click += new System.EventHandler(this.m_btn_Add_Click);
                // 
                // m_lb_addFiles
                // 
                this.m_lb_addFiles.AccessibleName = "";
                this.m_lb_addFiles.FormattingEnabled = true;
                this.m_lb_addFiles.Location = new System.Drawing.Point(10, 17);
                this.m_lb_addFiles.Name = "m_lb_addFiles";
                this.m_lb_addFiles.Size = new System.Drawing.Size(567, 82);
                this.m_lb_addFiles.TabIndex = 3;
                this.m_lb_addFiles.SelectedIndexChanged += new System.EventHandler(this.m_lb_addFiles_SelectedIndexChanged_1);
                // 
                // m_lbl_sampleRate
                // 
                this.m_lbl_sampleRate.AutoSize = true;
                this.m_lbl_sampleRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_lbl_sampleRate.Location = new System.Drawing.Point(17, 37);
                this.m_lbl_sampleRate.Name = "m_lbl_sampleRate";
                this.m_lbl_sampleRate.Size = new System.Drawing.Size(97, 16);
                this.m_lbl_sampleRate.TabIndex = 5;
                this.m_lbl_sampleRate.Text = "Sampling Rate";
                // 
                // m_cb_sampleRate
                // 
                this.m_cb_sampleRate.AccessibleName = "Sampling Rate";
                this.m_cb_sampleRate.FormattingEnabled = true;
                this.m_cb_sampleRate.Items.AddRange(new object[] {
            "44100",
            "22050",
            "11025"});
                this.m_cb_sampleRate.Location = new System.Drawing.Point(130, 29);
                this.m_cb_sampleRate.Name = "m_cb_sampleRate";
                this.m_cb_sampleRate.Size = new System.Drawing.Size(121, 24);
                this.m_cb_sampleRate.TabIndex = 6;
                // 
                // m_lbl_channel
                // 
                this.m_lbl_channel.AutoSize = true;
                this.m_lbl_channel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_lbl_channel.Location = new System.Drawing.Point(17, 70);
                this.m_lbl_channel.Name = "m_lbl_channel";
                this.m_lbl_channel.Size = new System.Drawing.Size(60, 16);
                this.m_lbl_channel.TabIndex = 7;
                this.m_lbl_channel.Text = "Channel ";
                // 
                // m_cb_channel
                // 
                this.m_cb_channel.AccessibleName = "Channels";
                this.m_cb_channel.FormattingEnabled = true;
                this.m_cb_channel.Items.AddRange(new object[] {
            "mono",
            "stereo"});
                this.m_cb_channel.Location = new System.Drawing.Point(130, 65);
                this.m_cb_channel.Name = "m_cb_channel";
                this.m_cb_channel.Size = new System.Drawing.Size(121, 24);
                this.m_cb_channel.TabIndex = 8;
                // 
                // m_btn_Browse
                // 
                this.m_btn_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btn_Browse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btn_Browse.Location = new System.Drawing.Point(603, 29);
                this.m_btn_Browse.Name = "m_btn_Browse";
                this.m_btn_Browse.Size = new System.Drawing.Size(94, 27);
                this.m_btn_Browse.TabIndex = 11;
                this.m_btn_Browse.Text = "&Browse";
                this.m_btn_Browse.UseVisualStyleBackColor = true;
                this.m_btn_Browse.Click += new System.EventHandler(this.m_btn_Browse_Click);
                // 
                // m_txt_Browse
                // 
                this.m_txt_Browse.AccessibleName = "Output directory path:";
                this.m_txt_Browse.Location = new System.Drawing.Point(156, 31);
                this.m_txt_Browse.Name = "m_txt_Browse";
                this.m_txt_Browse.ReadOnly = true;
                this.m_txt_Browse.Size = new System.Drawing.Size(424, 22);
                this.m_txt_Browse.TabIndex = 10;
                // 
                // m_btn_Start
                // 
                this.m_btn_Start.Enabled = false;
                this.m_btn_Start.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btn_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btn_Start.Location = new System.Drawing.Point(20, 93);
                this.m_btn_Start.Name = "m_btn_Start";
                this.m_btn_Start.Size = new System.Drawing.Size(94, 27);
                this.m_btn_Start.TabIndex = 12;
                this.m_btn_Start.Text = "&Start";
                this.m_btn_Start.UseVisualStyleBackColor = true;
                this.m_btn_Start.Click += new System.EventHandler(this.m_btn_Start_Click);
                // 
                // m_btn_Help
                // 
                this.m_btn_Help.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btn_Help.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btn_Help.Location = new System.Drawing.Point(216, 93);
                this.m_btn_Help.Name = "m_btn_Help";
                this.m_btn_Help.Size = new System.Drawing.Size(94, 27);
                this.m_btn_Help.TabIndex = 13;
                this.m_btn_Help.Text = "&Help";
                this.m_btn_Help.UseVisualStyleBackColor = true;
                this.m_btn_Help.Click += new System.EventHandler(this.m_btn_Help_Click);
                // 
                // m_btn_cancel
                // 
                this.m_btn_cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btn_cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btn_cancel.Location = new System.Drawing.Point(609, 93);
                this.m_btn_cancel.Name = "m_btn_cancel";
                this.m_btn_cancel.Size = new System.Drawing.Size(88, 27);
                this.m_btn_cancel.TabIndex = 15;
                this.m_btn_cancel.Text = "&Cancel";
                this.m_btn_cancel.UseVisualStyleBackColor = true;
                this.m_btn_cancel.Click += new System.EventHandler(this.m_btn_cancel_Click);
                // 
                // m_btnDelete
                // 
                this.m_btnDelete.Enabled = false;
                this.m_btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btnDelete.Location = new System.Drawing.Point(20, 72);
                this.m_btnDelete.Name = "m_btnDelete";
                this.m_btnDelete.Size = new System.Drawing.Size(94, 27);
                this.m_btnDelete.TabIndex = 4;
                this.m_btnDelete.Text = "De&lete";
                this.m_btnDelete.UseVisualStyleBackColor = true;
                this.m_btnDelete.Click += new System.EventHandler(this.m_btnDelete_Click);
                // 
                // m_btnReset
                // 
                this.m_btnReset.Enabled = false;
                this.m_btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.m_btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_btnReset.Location = new System.Drawing.Point(416, 93);
                this.m_btnReset.Name = "m_btnReset";
                this.m_btnReset.Size = new System.Drawing.Size(99, 27);
                this.m_btnReset.TabIndex = 14;
                this.m_btnReset.Text = "&Reset";
                this.m_btnReset.UseVisualStyleBackColor = true;
                this.m_btnReset.Click += new System.EventHandler(this.m_btnReset_Click);
                // 
                // m_grp_outputPath
                // 
                this.m_grp_outputPath.Controls.Add(this.m_lbOutputDirPath);
                this.m_grp_outputPath.Controls.Add(this.m_btn_Browse);
                this.m_grp_outputPath.Controls.Add(this.m_txt_Browse);
                this.m_grp_outputPath.Controls.Add(this.m_btn_cancel);
                this.m_grp_outputPath.Controls.Add(this.m_btnReset);
                this.m_grp_outputPath.Controls.Add(this.m_btn_Start);
                this.m_grp_outputPath.Controls.Add(this.m_btn_Help);
                this.m_grp_outputPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_grp_outputPath.Location = new System.Drawing.Point(27, 353);
                this.m_grp_outputPath.Name = "m_grp_outputPath";
                this.m_grp_outputPath.Size = new System.Drawing.Size(718, 136);
                this.m_grp_outputPath.TabIndex = 18;
                this.m_grp_outputPath.TabStop = false;
                // 
                // m_lbOutputDirPath
                // 
                this.m_lbOutputDirPath.AutoSize = true;
                this.m_lbOutputDirPath.Location = new System.Drawing.Point(17, 37);
                this.m_lbOutputDirPath.Name = "m_lbOutputDirPath";
                this.m_lbOutputDirPath.Size = new System.Drawing.Size(133, 16);
                this.m_lbOutputDirPath.TabIndex = 9;
                this.m_lbOutputDirPath.Text = "Output Directory Path";
                // 
                // m_grp_InputFiles
                // 
                this.m_grp_InputFiles.Controls.Add(this.m_grpListAudiofiles);
                this.m_grp_InputFiles.Controls.Add(this.m_btn_Add);
                this.m_grp_InputFiles.Controls.Add(this.m_btnDelete);
                this.m_grp_InputFiles.Location = new System.Drawing.Point(27, 72);
                this.m_grp_InputFiles.Name = "m_grp_InputFiles";
                this.m_grp_InputFiles.Size = new System.Drawing.Size(718, 124);
                this.m_grp_InputFiles.TabIndex = 16;
                this.m_grp_InputFiles.TabStop = false;
                // 
                // m_grpListAudiofiles
                // 
                this.m_grpListAudiofiles.Controls.Add(this.m_lb_addFiles);
                this.m_grpListAudiofiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_grpListAudiofiles.Location = new System.Drawing.Point(120, 13);
                this.m_grpListAudiofiles.Name = "m_grpListAudiofiles";
                this.m_grpListAudiofiles.Size = new System.Drawing.Size(583, 105);
                this.m_grpListAudiofiles.TabIndex = 2;
                this.m_grpListAudiofiles.TabStop = false;
                this.m_grpListAudiofiles.Text = "List of Input Files";
                // 
                // m_grp_AudioFormat
                // 
                this.m_grp_AudioFormat.Controls.Add(this.m_cb_sampleRate);
                this.m_grp_AudioFormat.Controls.Add(this.m_cb_channel);
                this.m_grp_AudioFormat.Controls.Add(this.m_lbl_sampleRate);
                this.m_grp_AudioFormat.Controls.Add(this.m_lbl_channel);
                this.m_grp_AudioFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.m_grp_AudioFormat.Location = new System.Drawing.Point(27, 224);
                this.m_grp_AudioFormat.Name = "m_grp_AudioFormat";
                this.m_grp_AudioFormat.Size = new System.Drawing.Size(718, 109);
                this.m_grp_AudioFormat.TabIndex = 17;
                this.m_grp_AudioFormat.TabStop = false;
                this.m_grp_AudioFormat.Text = "Set Audio Format for output files";
                // 
                // m_audioFormatConverterForm
                // 
                this.AcceptButton = this.m_btn_Start;
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(757, 517);
                this.Controls.Add(this.m_grp_AudioFormat);
                this.Controls.Add(this.m_grp_InputFiles);
                this.Controls.Add(this.m_grp_outputPath);
                this.Controls.Add(this.m_lblform);
                this.HelpButton = true;
                this.Name = "m_audioFormatConverterForm";
                this.Text = "Audio Format Converter";
                this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.m_audioFormatConverterForm_FormClosing);
                this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.m_audioFormatConverterForm_HelpRequested);
                this.m_grp_outputPath.ResumeLayout(false);
                this.m_grp_outputPath.PerformLayout();
                this.m_grp_InputFiles.ResumeLayout(false);
                this.m_grpListAudiofiles.ResumeLayout(false);
                this.m_grp_AudioFormat.ResumeLayout(false);
                this.m_grp_AudioFormat.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Label m_lblform;
        private System.Windows.Forms.Button m_btn_Add;
        private System.Windows.Forms.ListBox m_lb_addFiles;
        private System.Windows.Forms.Label m_lbl_sampleRate;
        private System.Windows.Forms.ComboBox m_cb_sampleRate;
        private System.Windows.Forms.Label m_lbl_channel;
        private System.Windows.Forms.ComboBox m_cb_channel;
        private System.Windows.Forms.Button m_btn_Browse;
        private System.Windows.Forms.TextBox m_txt_Browse;
        private System.Windows.Forms.Button m_btn_Start;
        private System.Windows.Forms.Button m_btn_Help;
        private System.Windows.Forms.Button m_btn_cancel;
        private System.Windows.Forms.Button m_btnDelete;
        private System.Windows.Forms.Button m_btnReset;
        private System.Windows.Forms.GroupBox m_grp_outputPath;
        private System.Windows.Forms.GroupBox m_grp_InputFiles;
        private System.Windows.Forms.GroupBox m_grp_AudioFormat;
        private System.Windows.Forms.GroupBox m_grpListAudiofiles;
        private System.Windows.Forms.Label m_lbOutputDirPath;
        private System.Windows.Forms.HelpProvider helpProvider1;
        }
    }

