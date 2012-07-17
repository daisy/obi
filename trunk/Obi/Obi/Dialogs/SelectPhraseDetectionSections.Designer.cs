namespace Obi.Dialogs
{
    partial class SelectPhraseDetectionSections
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
            this.m_cb_StartRangeForNumberOfSections = new System.Windows.Forms.ComboBox();
            this.m_cb_EndRangeForNumberOfSections = new System.Windows.Forms.ComboBox();
            this.startSectionRange = new System.Windows.Forms.Label();
            this.endSectionRange = new System.Windows.Forms.Label();
            this.m_btn_Display = new System.Windows.Forms.Button();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_grpListOfSections = new System.Windows.Forms.GroupBox();
            this.m_lv_ListOfSelectedSectionsForPhraseDetection = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.m_grpSelectRange = new System.Windows.Forms.GroupBox();
            this.m_rb_loadSelectedOnwards = new System.Windows.Forms.RadioButton();
            this.m_rb_loadFromRange = new System.Windows.Forms.RadioButton();
            this.m_rb_loadAllSections = new System.Windows.Forms.RadioButton();
            this.m_lbl_SelectSilencePhrase = new System.Windows.Forms.Label();
            this.m_cb_SilencePhrase = new System.Windows.Forms.ComboBox();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_grpSilencePhrase = new System.Windows.Forms.GroupBox();
            this.m_grpListOfSections.SuspendLayout();
            this.m_grpSelectRange.SuspendLayout();
            this.m_grpSilencePhrase.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_cb_StartRangeForNumberOfSections
            // 
            this.m_cb_StartRangeForNumberOfSections.AccessibleName = "Start Section Range";
            this.m_cb_StartRangeForNumberOfSections.Enabled = false;
            this.m_cb_StartRangeForNumberOfSections.FormattingEnabled = true;
            this.m_cb_StartRangeForNumberOfSections.Location = new System.Drawing.Point(134, 119);
            this.m_cb_StartRangeForNumberOfSections.Name = "m_cb_StartRangeForNumberOfSections";
            this.m_cb_StartRangeForNumberOfSections.Size = new System.Drawing.Size(121, 21);
            this.m_cb_StartRangeForNumberOfSections.TabIndex = 3;
            this.m_cb_StartRangeForNumberOfSections.SelectionChangeCommitted += new System.EventHandler(this.m_cb_StartRangeForNumberOfSections_SelectionChangeCommitted);
            // 
            // m_cb_EndRangeForNumberOfSections
            // 
            this.m_cb_EndRangeForNumberOfSections.AccessibleName = "End Section Range";
            this.m_cb_EndRangeForNumberOfSections.Enabled = false;
            this.m_cb_EndRangeForNumberOfSections.FormattingEnabled = true;
            this.m_cb_EndRangeForNumberOfSections.Location = new System.Drawing.Point(134, 150);
            this.m_cb_EndRangeForNumberOfSections.Name = "m_cb_EndRangeForNumberOfSections";
            this.m_cb_EndRangeForNumberOfSections.Size = new System.Drawing.Size(121, 21);
            this.m_cb_EndRangeForNumberOfSections.TabIndex = 5;
            // 
            // startSectionRange
            // 
            this.startSectionRange.AutoSize = true;
            this.startSectionRange.Location = new System.Drawing.Point(19, 122);
            this.startSectionRange.Name = "startSectionRange";
            this.startSectionRange.Size = new System.Drawing.Size(96, 13);
            this.startSectionRange.TabIndex = 2;
            this.startSectionRange.Text = "Start &section range";
            // 
            // endSectionRange
            // 
            this.endSectionRange.AutoSize = true;
            this.endSectionRange.Location = new System.Drawing.Point(19, 158);
            this.endSectionRange.Name = "endSectionRange";
            this.endSectionRange.Size = new System.Drawing.Size(93, 13);
            this.endSectionRange.TabIndex = 4;
            this.endSectionRange.Text = "End section ran&ge";
            // 
            // m_btn_Display
            // 
            this.m_btn_Display.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Display.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btn_Display.Location = new System.Drawing.Point(155, 191);
            this.m_btn_Display.Name = "m_btn_Display";
            this.m_btn_Display.Size = new System.Drawing.Size(100, 28);
            this.m_btn_Display.TabIndex = 6;
            this.m_btn_Display.Text = "D&isplay\r\n\r\n";
            this.m_btn_Display.UseVisualStyleBackColor = true;
            this.m_btn_Display.Click += new System.EventHandler(this.m_btn_Display_Click);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btn_OK.Location = new System.Drawing.Point(206, 327);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(100, 28);
            this.m_btn_OK.TabIndex = 10;
            this.m_btn_OK.Text = "&OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_grpListOfSections
            // 
            this.m_grpListOfSections.Controls.Add(this.m_lv_ListOfSelectedSectionsForPhraseDetection);
            this.m_grpListOfSections.Location = new System.Drawing.Point(1, 6);
            this.m_grpListOfSections.Name = "m_grpListOfSections";
            this.m_grpListOfSections.Size = new System.Drawing.Size(176, 355);
            this.m_grpListOfSections.TabIndex = 7;
            this.m_grpListOfSections.TabStop = false;
            this.m_grpListOfSections.Text = "Lis&t of Sections";
            // 
            // m_lv_ListOfSelectedSectionsForPhraseDetection
            // 
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.AccessibleDescription = "ListView containing list of sections selected for phrase detection";
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.AutoArrange = false;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.CheckBoxes = true;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.Location = new System.Drawing.Point(6, 19);
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.Name = "m_lv_ListOfSelectedSectionsForPhraseDetection";
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.ShowItemToolTips = true;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.Size = new System.Drawing.Size(164, 330);
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.TabIndex = 7;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.UseCompatibleStateImageBehavior = false;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.View = System.Windows.Forms.View.Details;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.m_lv_ListOfSelectedSectionsForPhraseDetection_ItemCheck);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 200;
            // 
            // m_grpSelectRange
            // 
            this.m_grpSelectRange.Controls.Add(this.m_rb_loadSelectedOnwards);
            this.m_grpSelectRange.Controls.Add(this.m_cb_EndRangeForNumberOfSections);
            this.m_grpSelectRange.Controls.Add(this.m_rb_loadFromRange);
            this.m_grpSelectRange.Controls.Add(this.endSectionRange);
            this.m_grpSelectRange.Controls.Add(this.m_rb_loadAllSections);
            this.m_grpSelectRange.Controls.Add(this.startSectionRange);
            this.m_grpSelectRange.Controls.Add(this.m_cb_StartRangeForNumberOfSections);
            this.m_grpSelectRange.Controls.Add(this.m_btn_Display);
            this.m_grpSelectRange.Location = new System.Drawing.Point(184, 12);
            this.m_grpSelectRange.Name = "m_grpSelectRange";
            this.m_grpSelectRange.Size = new System.Drawing.Size(279, 227);
            this.m_grpSelectRange.TabIndex = 0;
            this.m_grpSelectRange.TabStop = false;
            this.m_grpSelectRange.Text = "Show Selected Sections ";
            // 
            // m_rb_loadSelectedOnwards
            // 
            this.m_rb_loadSelectedOnwards.AccessibleName = "Load Selected onwards radio button";
            this.m_rb_loadSelectedOnwards.AutoSize = true;
            this.m_rb_loadSelectedOnwards.Location = new System.Drawing.Point(22, 81);
            this.m_rb_loadSelectedOnwards.Name = "m_rb_loadSelectedOnwards";
            this.m_rb_loadSelectedOnwards.Size = new System.Drawing.Size(139, 17);
            this.m_rb_loadSelectedOnwards.TabIndex = 1;
            this.m_rb_loadSelectedOnwards.TabStop = true;
            this.m_rb_loadSelectedOnwards.Text = "Load Selec&ted Onwards";
            this.m_rb_loadSelectedOnwards.UseVisualStyleBackColor = true;
            this.m_rb_loadSelectedOnwards.CheckedChanged += new System.EventHandler(this.m_rb_loadSelectedOnwards_CheckedChanged);
            // 
            // m_rb_loadFromRange
            // 
            this.m_rb_loadFromRange.AccessibleDescription = "Load From Range radio button";
            this.m_rb_loadFromRange.AutoSize = true;
            this.m_rb_loadFromRange.Location = new System.Drawing.Point(22, 58);
            this.m_rb_loadFromRange.Name = "m_rb_loadFromRange";
            this.m_rb_loadFromRange.Size = new System.Drawing.Size(110, 17);
            this.m_rb_loadFromRange.TabIndex = 1;
            this.m_rb_loadFromRange.TabStop = true;
            this.m_rb_loadFromRange.Text = "Load &From Range";
            this.m_rb_loadFromRange.UseVisualStyleBackColor = true;
            this.m_rb_loadFromRange.CheckedChanged += new System.EventHandler(this.m_rb_loadFromRange_CheckedChanged);
            // 
            // m_rb_loadAllSections
            // 
            this.m_rb_loadAllSections.AccessibleDescription = "Load All sections radiobutton";
            this.m_rb_loadAllSections.AutoSize = true;
            this.m_rb_loadAllSections.Location = new System.Drawing.Point(22, 35);
            this.m_rb_loadAllSections.Name = "m_rb_loadAllSections";
            this.m_rb_loadAllSections.Size = new System.Drawing.Size(107, 17);
            this.m_rb_loadAllSections.TabIndex = 1;
            this.m_rb_loadAllSections.TabStop = true;
            this.m_rb_loadAllSections.Text = "Load &All Sections";
            this.m_rb_loadAllSections.UseVisualStyleBackColor = true;
            this.m_rb_loadAllSections.CheckedChanged += new System.EventHandler(this.m_rb_loadAllSections_CheckedChanged);
            // 
            // m_lbl_SelectSilencePhrase
            // 
            this.m_lbl_SelectSilencePhrase.AutoSize = true;
            this.m_lbl_SelectSilencePhrase.Location = new System.Drawing.Point(19, 27);
            this.m_lbl_SelectSilencePhrase.Name = "m_lbl_SelectSilencePhrase";
            this.m_lbl_SelectSilencePhrase.Size = new System.Drawing.Size(77, 13);
            this.m_lbl_SelectSilencePhrase.TabIndex = 8;
            this.m_lbl_SelectSilencePhrase.Text = "Silen&ce phrase";
            // 
            // m_cb_SilencePhrase
            // 
            this.m_cb_SilencePhrase.AccessibleName = "Select Silence Phrase";
            this.m_cb_SilencePhrase.FormattingEnabled = true;
            this.m_cb_SilencePhrase.Location = new System.Drawing.Point(112, 19);
            this.m_cb_SilencePhrase.Name = "m_cb_SilencePhrase";
            this.m_cb_SilencePhrase.Size = new System.Drawing.Size(161, 21);
            this.m_cb_SilencePhrase.TabIndex = 9;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btn_Cancel.Location = new System.Drawing.Point(357, 327);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(100, 28);
            this.m_btn_Cancel.TabIndex = 11;
            this.m_btn_Cancel.Text = "Cance&l";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // m_grpSilencePhrase
            // 
            this.m_grpSilencePhrase.Controls.Add(this.m_cb_SilencePhrase);
            this.m_grpSilencePhrase.Controls.Add(this.m_lbl_SelectSilencePhrase);
            this.m_grpSilencePhrase.Location = new System.Drawing.Point(184, 245);
            this.m_grpSilencePhrase.Name = "m_grpSilencePhrase";
            this.m_grpSilencePhrase.Size = new System.Drawing.Size(279, 62);
            this.m_grpSilencePhrase.TabIndex = 9;
            this.m_grpSilencePhrase.TabStop = false;
            // 
            // SelectPhraseDetectionSections
            // 
            this.AcceptButton = this.m_btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.ClientSize = new System.Drawing.Size(475, 365);
            this.Controls.Add(this.m_grpSilencePhrase);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_grpSelectRange);
            this.Controls.Add(this.m_grpListOfSections);
            this.Controls.Add(this.m_btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectPhraseDetectionSections";
            this.Text = "Select phrase detection sections";
            this.m_grpListOfSections.ResumeLayout(false);
            this.m_grpSelectRange.ResumeLayout(false);
            this.m_grpSelectRange.PerformLayout();
            this.m_grpSilencePhrase.ResumeLayout(false);
            this.m_grpSilencePhrase.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox m_cb_StartRangeForNumberOfSections;
        private System.Windows.Forms.ComboBox m_cb_EndRangeForNumberOfSections;
        private System.Windows.Forms.Label startSectionRange;
        private System.Windows.Forms.Label endSectionRange;
        private System.Windows.Forms.Button m_btn_Display;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.GroupBox m_grpListOfSections;
        private System.Windows.Forms.GroupBox m_grpSelectRange;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.Label m_lbl_SelectSilencePhrase;
        private System.Windows.Forms.ComboBox m_cb_SilencePhrase;
        private System.Windows.Forms.GroupBox m_grpSilencePhrase;
        private System.Windows.Forms.ListView m_lv_ListOfSelectedSectionsForPhraseDetection;
        private System.Windows.Forms.RadioButton m_rb_loadSelectedOnwards;
        private System.Windows.Forms.RadioButton m_rb_loadFromRange;
        private System.Windows.Forms.RadioButton m_rb_loadAllSections;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}