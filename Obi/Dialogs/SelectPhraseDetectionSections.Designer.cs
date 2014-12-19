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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectPhraseDetectionSections));
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
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_grpListOfSections.SuspendLayout();
            this.m_grpSelectRange.SuspendLayout();
            this.m_grpSilencePhrase.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_cb_StartRangeForNumberOfSections
            // 
            resources.ApplyResources(this.m_cb_StartRangeForNumberOfSections, "m_cb_StartRangeForNumberOfSections");
            this.m_cb_StartRangeForNumberOfSections.FormattingEnabled = true;
            this.m_cb_StartRangeForNumberOfSections.Name = "m_cb_StartRangeForNumberOfSections";
            this.m_cb_StartRangeForNumberOfSections.SelectionChangeCommitted += new System.EventHandler(this.m_cb_StartRangeForNumberOfSections_SelectionChangeCommitted);
            // 
            // m_cb_EndRangeForNumberOfSections
            // 
            resources.ApplyResources(this.m_cb_EndRangeForNumberOfSections, "m_cb_EndRangeForNumberOfSections");
            this.m_cb_EndRangeForNumberOfSections.FormattingEnabled = true;
            this.m_cb_EndRangeForNumberOfSections.Name = "m_cb_EndRangeForNumberOfSections";
            // 
            // startSectionRange
            // 
            resources.ApplyResources(this.startSectionRange, "startSectionRange");
            this.startSectionRange.Name = "startSectionRange";
            // 
            // endSectionRange
            // 
            resources.ApplyResources(this.endSectionRange, "endSectionRange");
            this.endSectionRange.Name = "endSectionRange";
            // 
            // m_btn_Display
            // 
            resources.ApplyResources(this.m_btn_Display, "m_btn_Display");
            this.m_btn_Display.Name = "m_btn_Display";
            this.m_btn_Display.UseVisualStyleBackColor = true;
            this.m_btn_Display.Click += new System.EventHandler(this.m_btn_Display_Click);
            // 
            // m_btn_OK
            // 
            resources.ApplyResources(this.m_btn_OK, "m_btn_OK");
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_grpListOfSections
            // 
            this.m_grpListOfSections.Controls.Add(this.m_lv_ListOfSelectedSectionsForPhraseDetection);
            resources.ApplyResources(this.m_grpListOfSections, "m_grpListOfSections");
            this.m_grpListOfSections.Name = "m_grpListOfSections";
            this.m_grpListOfSections.TabStop = false;
            // 
            // m_lv_ListOfSelectedSectionsForPhraseDetection
            // 
            resources.ApplyResources(this.m_lv_ListOfSelectedSectionsForPhraseDetection, "m_lv_ListOfSelectedSectionsForPhraseDetection");
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.AutoArrange = false;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.CheckBoxes = true;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.Name = "m_lv_ListOfSelectedSectionsForPhraseDetection";
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.ShowItemToolTips = true;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.UseCompatibleStateImageBehavior = false;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.View = System.Windows.Forms.View.Details;
            this.m_lv_ListOfSelectedSectionsForPhraseDetection.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.m_lv_ListOfSelectedSectionsForPhraseDetection_ItemCheck);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = global::Obi.messages.phrase_extra_Plain;
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
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
            resources.ApplyResources(this.m_grpSelectRange, "m_grpSelectRange");
            this.m_grpSelectRange.Name = "m_grpSelectRange";
            this.m_grpSelectRange.TabStop = false;
            // 
            // m_rb_loadSelectedOnwards
            // 
            resources.ApplyResources(this.m_rb_loadSelectedOnwards, "m_rb_loadSelectedOnwards");
            this.m_rb_loadSelectedOnwards.Name = "m_rb_loadSelectedOnwards";
            this.m_rb_loadSelectedOnwards.TabStop = true;
            this.m_rb_loadSelectedOnwards.UseVisualStyleBackColor = true;
            this.m_rb_loadSelectedOnwards.CheckedChanged += new System.EventHandler(this.m_rb_loadSelectedOnwards_CheckedChanged);
            // 
            // m_rb_loadFromRange
            // 
            resources.ApplyResources(this.m_rb_loadFromRange, "m_rb_loadFromRange");
            this.m_rb_loadFromRange.Name = "m_rb_loadFromRange";
            this.m_rb_loadFromRange.TabStop = true;
            this.m_rb_loadFromRange.UseVisualStyleBackColor = true;
            this.m_rb_loadFromRange.CheckedChanged += new System.EventHandler(this.m_rb_loadFromRange_CheckedChanged);
            // 
            // m_rb_loadAllSections
            // 
            resources.ApplyResources(this.m_rb_loadAllSections, "m_rb_loadAllSections");
            this.m_rb_loadAllSections.Name = "m_rb_loadAllSections";
            this.m_rb_loadAllSections.TabStop = true;
            this.m_rb_loadAllSections.UseVisualStyleBackColor = true;
            this.m_rb_loadAllSections.CheckedChanged += new System.EventHandler(this.m_rb_loadAllSections_CheckedChanged);
            // 
            // m_lbl_SelectSilencePhrase
            // 
            resources.ApplyResources(this.m_lbl_SelectSilencePhrase, "m_lbl_SelectSilencePhrase");
            this.m_lbl_SelectSilencePhrase.Name = "m_lbl_SelectSilencePhrase";
            // 
            // m_cb_SilencePhrase
            // 
            resources.ApplyResources(this.m_cb_SilencePhrase, "m_cb_SilencePhrase");
            this.m_cb_SilencePhrase.FormattingEnabled = true;
            this.m_cb_SilencePhrase.Name = "m_cb_SilencePhrase";
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btn_Cancel, "m_btn_Cancel");
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // m_grpSilencePhrase
            // 
            this.m_grpSilencePhrase.Controls.Add(this.m_cb_SilencePhrase);
            this.m_grpSilencePhrase.Controls.Add(this.m_lbl_SelectSilencePhrase);
            resources.ApplyResources(this.m_grpSilencePhrase, "m_grpSilencePhrase");
            this.m_grpSilencePhrase.Name = "m_grpSilencePhrase";
            this.m_grpSilencePhrase.TabStop = false;
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // SelectPhraseDetectionSections
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.m_grpSilencePhrase);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_grpSelectRange);
            this.Controls.Add(this.m_grpListOfSections);
            this.Controls.Add(this.m_btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectPhraseDetectionSections";
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
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}