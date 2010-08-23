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
            this.m_lb_listOfSelectedSectionsForPhraseDetection = new System.Windows.Forms.ListBox();
            this.startSectionRange = new System.Windows.Forms.Label();
            this.endSectionRange = new System.Windows.Forms.Label();
            this.m_btn_Display = new System.Windows.Forms.Button();
            this.m_btn_RemoveFromList = new System.Windows.Forms.Button();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_grpListOfSections = new System.Windows.Forms.GroupBox();
            this.m_grpSelectRange = new System.Windows.Forms.GroupBox();
            this.m_lbl_SelectSilencePhrase = new System.Windows.Forms.Label();
            this.m_cb_SilencePhrase = new System.Windows.Forms.ComboBox();
            this.m_grpRemove = new System.Windows.Forms.GroupBox();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_grpSilencePhrase = new System.Windows.Forms.GroupBox();
            this.m_grpListOfSections.SuspendLayout();
            this.m_grpSelectRange.SuspendLayout();
            this.m_grpRemove.SuspendLayout();
            this.m_grpSilencePhrase.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_cb_StartRangeForNumberOfSections
            // 
            this.m_cb_StartRangeForNumberOfSections.AccessibleName = "Start Section Range";
            this.m_cb_StartRangeForNumberOfSections.FormattingEnabled = true;
            this.m_cb_StartRangeForNumberOfSections.Location = new System.Drawing.Point(160, 34);
            this.m_cb_StartRangeForNumberOfSections.Name = "m_cb_StartRangeForNumberOfSections";
            this.m_cb_StartRangeForNumberOfSections.Size = new System.Drawing.Size(121, 21);
            this.m_cb_StartRangeForNumberOfSections.TabIndex = 1;
            this.m_cb_StartRangeForNumberOfSections.SelectionChangeCommitted += new System.EventHandler(this.m_cb_StartRangeForNumberOfSections_SelectionChangeCommitted);
            // 
            // m_cb_EndRangeForNumberOfSections
            // 
            this.m_cb_EndRangeForNumberOfSections.AccessibleName = "End Section Range";
            this.m_cb_EndRangeForNumberOfSections.FormattingEnabled = true;
            this.m_cb_EndRangeForNumberOfSections.Location = new System.Drawing.Point(160, 70);
            this.m_cb_EndRangeForNumberOfSections.Name = "m_cb_EndRangeForNumberOfSections";
            this.m_cb_EndRangeForNumberOfSections.Size = new System.Drawing.Size(121, 21);
            this.m_cb_EndRangeForNumberOfSections.TabIndex = 3;
            // 
            // m_lb_listOfSelectedSectionsForPhraseDetection
            // 
            this.m_lb_listOfSelectedSectionsForPhraseDetection.FormattingEnabled = true;
            this.m_lb_listOfSelectedSectionsForPhraseDetection.Location = new System.Drawing.Point(5, 19);
            this.m_lb_listOfSelectedSectionsForPhraseDetection.Name = "m_lb_listOfSelectedSectionsForPhraseDetection";
            this.m_lb_listOfSelectedSectionsForPhraseDetection.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.m_lb_listOfSelectedSectionsForPhraseDetection.Size = new System.Drawing.Size(120, 316);
            this.m_lb_listOfSelectedSectionsForPhraseDetection.TabIndex = 6;
            // 
            // startSectionRange
            // 
            this.startSectionRange.AutoSize = true;
            this.startSectionRange.Location = new System.Drawing.Point(37, 37);
            this.startSectionRange.Name = "startSectionRange";
            this.startSectionRange.Size = new System.Drawing.Size(96, 13);
            this.startSectionRange.TabIndex = 0;
            this.startSectionRange.Text = "Start &section range";
            // 
            // endSectionRange
            // 
            this.endSectionRange.AutoSize = true;
            this.endSectionRange.Location = new System.Drawing.Point(40, 73);
            this.endSectionRange.Name = "endSectionRange";
            this.endSectionRange.Size = new System.Drawing.Size(93, 13);
            this.endSectionRange.TabIndex = 2;
            this.endSectionRange.Text = "End section ran&ge";
            // 
            // m_btn_Display
            // 
            this.m_btn_Display.Location = new System.Drawing.Point(206, 102);
            this.m_btn_Display.Name = "m_btn_Display";
            this.m_btn_Display.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Display.TabIndex = 4;
            this.m_btn_Display.Text = "D&isplay Range";
            this.m_btn_Display.UseVisualStyleBackColor = true;
            this.m_btn_Display.Click += new System.EventHandler(this.m_btn_Display_Click);
            // 
            // m_btn_RemoveFromList
            // 
            this.m_btn_RemoveFromList.Location = new System.Drawing.Point(206, 25);
            this.m_btn_RemoveFromList.Name = "m_btn_RemoveFromList";
            this.m_btn_RemoveFromList.Size = new System.Drawing.Size(75, 23);
            this.m_btn_RemoveFromList.TabIndex = 8;
            this.m_btn_RemoveFromList.Text = "Re&move";
            this.m_btn_RemoveFromList.UseVisualStyleBackColor = true;
            this.m_btn_RemoveFromList.Click += new System.EventHandler(this.m_btn_RemoveFromList_Click);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.Location = new System.Drawing.Point(209, 333);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 12;
            this.m_btn_OK.Text = "&OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_grpListOfSections
            // 
            this.m_grpListOfSections.Controls.Add(this.m_lb_listOfSelectedSectionsForPhraseDetection);
            this.m_grpListOfSections.Location = new System.Drawing.Point(24, 21);
            this.m_grpListOfSections.Name = "m_grpListOfSections";
            this.m_grpListOfSections.Size = new System.Drawing.Size(131, 347);
            this.m_grpListOfSections.TabIndex = 5;
            this.m_grpListOfSections.TabStop = false;
            this.m_grpListOfSections.Text = "Lis&t of Sections";
            // 
            // m_grpSelectRange
            // 
            this.m_grpSelectRange.Controls.Add(this.m_cb_EndRangeForNumberOfSections);
            this.m_grpSelectRange.Controls.Add(this.endSectionRange);
            this.m_grpSelectRange.Controls.Add(this.startSectionRange);
            this.m_grpSelectRange.Controls.Add(this.m_cb_StartRangeForNumberOfSections);
            this.m_grpSelectRange.Controls.Add(this.m_btn_Display);
            this.m_grpSelectRange.Location = new System.Drawing.Point(161, 21);
            this.m_grpSelectRange.Name = "m_grpSelectRange";
            this.m_grpSelectRange.Size = new System.Drawing.Size(296, 140);
            this.m_grpSelectRange.TabIndex = 0;
            this.m_grpSelectRange.TabStop = false;
            // 
            // m_lbl_SelectSilencePhrase
            // 
            this.m_lbl_SelectSilencePhrase.AutoSize = true;
            this.m_lbl_SelectSilencePhrase.Location = new System.Drawing.Point(4, 27);
            this.m_lbl_SelectSilencePhrase.Name = "m_lbl_SelectSilencePhrase";
            this.m_lbl_SelectSilencePhrase.Size = new System.Drawing.Size(77, 13);
            this.m_lbl_SelectSilencePhrase.TabIndex = 10;
            this.m_lbl_SelectSilencePhrase.Text = "Silen&ce phrase";
            // 
            // m_cb_SilencePhrase
            // 
            this.m_cb_SilencePhrase.AccessibleName = "Select Silence Phrase";
            this.m_cb_SilencePhrase.FormattingEnabled = true;
            this.m_cb_SilencePhrase.Location = new System.Drawing.Point(87, 24);
            this.m_cb_SilencePhrase.Name = "m_cb_SilencePhrase";
            this.m_cb_SilencePhrase.Size = new System.Drawing.Size(194, 21);
            this.m_cb_SilencePhrase.TabIndex = 11;
            // 
            // m_grpRemove
            // 
            this.m_grpRemove.Controls.Add(this.m_btn_RemoveFromList);
            this.m_grpRemove.Location = new System.Drawing.Point(161, 242);
            this.m_grpRemove.Name = "m_grpRemove";
            this.m_grpRemove.Size = new System.Drawing.Size(296, 68);
            this.m_grpRemove.TabIndex = 7;
            this.m_grpRemove.TabStop = false;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.Location = new System.Drawing.Point(367, 333);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 13;
            this.m_btn_Cancel.Text = "Cance&l";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // m_grpSilencePhrase
            // 
            this.m_grpSilencePhrase.Controls.Add(this.m_cb_SilencePhrase);
            this.m_grpSilencePhrase.Controls.Add(this.m_lbl_SelectSilencePhrase);
            this.m_grpSilencePhrase.Location = new System.Drawing.Point(161, 174);
            this.m_grpSilencePhrase.Name = "m_grpSilencePhrase";
            this.m_grpSilencePhrase.Size = new System.Drawing.Size(296, 62);
            this.m_grpSilencePhrase.TabIndex = 9;
            this.m_grpSilencePhrase.TabStop = false;
            // 
            // SelectPhraseDetectionSections
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 380);
            this.Controls.Add(this.m_grpSilencePhrase);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_grpRemove);
            this.Controls.Add(this.m_grpSelectRange);
            this.Controls.Add(this.m_grpListOfSections);
            this.Controls.Add(this.m_btn_OK);
            this.Name = "SelectPhraseDetectionSections";
            this.Text = "SelectPhraseDetectionSections";
            this.m_grpListOfSections.ResumeLayout(false);
            this.m_grpSelectRange.ResumeLayout(false);
            this.m_grpSelectRange.PerformLayout();
            this.m_grpRemove.ResumeLayout(false);
            this.m_grpSilencePhrase.ResumeLayout(false);
            this.m_grpSilencePhrase.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox m_cb_StartRangeForNumberOfSections;
        private System.Windows.Forms.ComboBox m_cb_EndRangeForNumberOfSections;
        private System.Windows.Forms.ListBox m_lb_listOfSelectedSectionsForPhraseDetection;
        private System.Windows.Forms.Label startSectionRange;
        private System.Windows.Forms.Label endSectionRange;
        private System.Windows.Forms.Button m_btn_Display;
        private System.Windows.Forms.Button m_btn_RemoveFromList;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.GroupBox m_grpListOfSections;
        private System.Windows.Forms.GroupBox m_grpSelectRange;
        private System.Windows.Forms.GroupBox m_grpRemove;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.Label m_lbl_SelectSilencePhrase;
        private System.Windows.Forms.ComboBox m_cb_SilencePhrase;
        private System.Windows.Forms.GroupBox m_grpSilencePhrase;
    }
}