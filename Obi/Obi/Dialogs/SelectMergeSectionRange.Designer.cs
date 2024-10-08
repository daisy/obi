namespace Obi.Dialogs
{
    partial class SelectMergeSectionRange
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectMergeSectionRange));
            this.m_statusStripForMergeSection = new System.Windows.Forms.StatusStrip();
            this.m_StatusLabelForMergeSection = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_btn_SelectAll = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.m_btn_ShowContents = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_btn_IncreaseSectionLevel = new System.Windows.Forms.Button();
            this.m_btn_DecreaseSectionLevel = new System.Windows.Forms.Button();
            this.m_grp_SectionLevelOperation = new System.Windows.Forms.GroupBox();
            this.m_btn_Merge = new System.Windows.Forms.Button();
            this.m_btn_Undo = new System.Windows.Forms.Button();
            this.m_btn_Close = new System.Windows.Forms.Button();
            this.m_btnPause = new System.Windows.Forms.Button();
            this.m_grp_SectionAudioOperation = new System.Windows.Forms.GroupBox();
            this.m_btn_Stop = new System.Windows.Forms.Button();
            this.m_btn_Play = new System.Windows.Forms.Button();
            this.m_lb_listofSectionsToMerge = new System.Windows.Forms.ListBox();
            this.m_tb_SectionsSelected = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_btn_NoiseReduction = new System.Windows.Forms.Button();
            this.m_btn_Normalize = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.m_btn_RetainSilenceInLastPhraseOfSection = new System.Windows.Forms.Button();
            this.m_btn_DeleteSilenceFromSectionEnd = new System.Windows.Forms.Button();
            this.m_statusStripForMergeSection.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.m_grp_SectionLevelOperation.SuspendLayout();
            this.m_grp_SectionAudioOperation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_statusStripForMergeSection
            // 
            this.m_statusStripForMergeSection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusLabelForMergeSection});
            this.m_statusStripForMergeSection.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            resources.ApplyResources(this.m_statusStripForMergeSection, "m_statusStripForMergeSection");
            this.m_statusStripForMergeSection.Name = "m_statusStripForMergeSection";
            this.m_statusStripForMergeSection.ShowItemToolTips = true;
            // 
            // m_StatusLabelForMergeSection
            // 
            this.m_StatusLabelForMergeSection.AutoToolTip = true;
            this.m_StatusLabelForMergeSection.Name = "m_StatusLabelForMergeSection";
            resources.ApplyResources(this.m_StatusLabelForMergeSection, "m_StatusLabelForMergeSection");
            // 
            // m_btn_SelectAll
            // 
            resources.ApplyResources(this.m_btn_SelectAll, "m_btn_SelectAll");
            this.m_btn_SelectAll.Name = "m_btn_SelectAll";
            this.m_btn_SelectAll.UseVisualStyleBackColor = true;
            this.m_btn_SelectAll.Click += new System.EventHandler(this.m_btn_SelectAll_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.m_btn_ShowContents);
            this.groupBox2.Controls.Add(this.m_btn_SelectAll);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // m_btn_ShowContents
            // 
            resources.ApplyResources(this.m_btn_ShowContents, "m_btn_ShowContents");
            this.m_btn_ShowContents.Name = "m_btn_ShowContents";
            this.m_btn_ShowContents.UseVisualStyleBackColor = true;
            this.m_btn_ShowContents.Click += new System.EventHandler(this.m_btn_ShowContents_Click);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // m_btn_IncreaseSectionLevel
            // 
            resources.ApplyResources(this.m_btn_IncreaseSectionLevel, "m_btn_IncreaseSectionLevel");
            this.m_btn_IncreaseSectionLevel.Name = "m_btn_IncreaseSectionLevel";
            this.helpProvider1.SetShowHelp(this.m_btn_IncreaseSectionLevel, ((bool)(resources.GetObject("m_btn_IncreaseSectionLevel.ShowHelp"))));
            this.m_btn_IncreaseSectionLevel.UseVisualStyleBackColor = true;
            this.m_btn_IncreaseSectionLevel.Click += new System.EventHandler(this.m_btn_IncreaseSectionLevel_Click);
            // 
            // m_btn_DecreaseSectionLevel
            // 
            resources.ApplyResources(this.m_btn_DecreaseSectionLevel, "m_btn_DecreaseSectionLevel");
            this.m_btn_DecreaseSectionLevel.Name = "m_btn_DecreaseSectionLevel";
            this.helpProvider1.SetShowHelp(this.m_btn_DecreaseSectionLevel, ((bool)(resources.GetObject("m_btn_DecreaseSectionLevel.ShowHelp"))));
            this.m_btn_DecreaseSectionLevel.UseVisualStyleBackColor = true;
            this.m_btn_DecreaseSectionLevel.Click += new System.EventHandler(this.m_btn_DecreaseSectionLevel_Click);
            // 
            // m_grp_SectionLevelOperation
            // 
            this.m_grp_SectionLevelOperation.Controls.Add(this.m_btn_Merge);
            this.m_grp_SectionLevelOperation.Controls.Add(this.m_btn_Undo);
            this.m_grp_SectionLevelOperation.Controls.Add(this.m_btn_DecreaseSectionLevel);
            this.m_grp_SectionLevelOperation.Controls.Add(this.m_btn_IncreaseSectionLevel);
            resources.ApplyResources(this.m_grp_SectionLevelOperation, "m_grp_SectionLevelOperation");
            this.m_grp_SectionLevelOperation.Name = "m_grp_SectionLevelOperation";
            this.helpProvider1.SetShowHelp(this.m_grp_SectionLevelOperation, ((bool)(resources.GetObject("m_grp_SectionLevelOperation.ShowHelp"))));
            this.m_grp_SectionLevelOperation.TabStop = false;
            // 
            // m_btn_Merge
            // 
            resources.ApplyResources(this.m_btn_Merge, "m_btn_Merge");
            this.m_btn_Merge.Name = "m_btn_Merge";
            this.helpProvider1.SetShowHelp(this.m_btn_Merge, ((bool)(resources.GetObject("m_btn_Merge.ShowHelp"))));
            this.m_btn_Merge.UseVisualStyleBackColor = true;
            this.m_btn_Merge.Click += new System.EventHandler(this.m_btn_Merge_Click);
            // 
            // m_btn_Undo
            // 
            resources.ApplyResources(this.m_btn_Undo, "m_btn_Undo");
            this.m_btn_Undo.Name = "m_btn_Undo";
            this.m_btn_Undo.UseVisualStyleBackColor = true;
            this.m_btn_Undo.Click += new System.EventHandler(this.m_btn_Undo_Click);
            // 
            // m_btn_Close
            // 
            resources.ApplyResources(this.m_btn_Close, "m_btn_Close");
            this.m_btn_Close.Name = "m_btn_Close";
            this.helpProvider1.SetShowHelp(this.m_btn_Close, ((bool)(resources.GetObject("m_btn_Close.ShowHelp"))));
            this.m_btn_Close.UseVisualStyleBackColor = true;
            this.m_btn_Close.Click += new System.EventHandler(this.m_btn_Close_Click);
            // 
            // m_btnPause
            // 
            resources.ApplyResources(this.m_btnPause, "m_btnPause");
            this.m_btnPause.Name = "m_btnPause";
            this.helpProvider1.SetShowHelp(this.m_btnPause, ((bool)(resources.GetObject("m_btnPause.ShowHelp"))));
            this.m_btnPause.UseVisualStyleBackColor = true;
            this.m_btnPause.Click += new System.EventHandler(this.m_btnPause_Click);
            // 
            // m_grp_SectionAudioOperation
            // 
            this.m_grp_SectionAudioOperation.Controls.Add(this.m_btnPause);
            this.m_grp_SectionAudioOperation.Controls.Add(this.m_btn_Stop);
            this.m_grp_SectionAudioOperation.Controls.Add(this.m_btn_Play);
            resources.ApplyResources(this.m_grp_SectionAudioOperation, "m_grp_SectionAudioOperation");
            this.m_grp_SectionAudioOperation.Name = "m_grp_SectionAudioOperation";
            this.m_grp_SectionAudioOperation.TabStop = false;
            // 
            // m_btn_Stop
            // 
            resources.ApplyResources(this.m_btn_Stop, "m_btn_Stop");
            this.m_btn_Stop.Name = "m_btn_Stop";
            this.m_btn_Stop.UseVisualStyleBackColor = true;
            this.m_btn_Stop.Click += new System.EventHandler(this.m_btn_Stop_Click);
            // 
            // m_btn_Play
            // 
            resources.ApplyResources(this.m_btn_Play, "m_btn_Play");
            this.m_btn_Play.Name = "m_btn_Play";
            this.m_btn_Play.UseVisualStyleBackColor = true;
            this.m_btn_Play.Click += new System.EventHandler(this.m_btn_Play_Click);
            // 
            // m_lb_listofSectionsToMerge
            // 
            resources.ApplyResources(this.m_lb_listofSectionsToMerge, "m_lb_listofSectionsToMerge");
            this.m_lb_listofSectionsToMerge.FormattingEnabled = true;
            this.m_lb_listofSectionsToMerge.Name = "m_lb_listofSectionsToMerge";
            this.m_lb_listofSectionsToMerge.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.m_lb_listofSectionsToMerge.SelectedIndexChanged += new System.EventHandler(this.m_lb_listofSectionsToMerge_SelectedIndexChanged);
            // 
            // m_tb_SectionsSelected
            // 
            resources.ApplyResources(this.m_tb_SectionsSelected, "m_tb_SectionsSelected");
            this.m_tb_SectionsSelected.Name = "m_tb_SectionsSelected";
            this.m_tb_SectionsSelected.ReadOnly = true;
            // 
            // m_btn_NoiseReduction
            // 
            resources.ApplyResources(this.m_btn_NoiseReduction, "m_btn_NoiseReduction");
            this.m_btn_NoiseReduction.Name = "m_btn_NoiseReduction";
            this.m_btn_NoiseReduction.UseVisualStyleBackColor = true;
            this.m_btn_NoiseReduction.Click += new System.EventHandler(this.m_btn_NoiseReduction_Click);
            // 
            // m_btn_Normalize
            // 
            resources.ApplyResources(this.m_btn_Normalize, "m_btn_Normalize");
            this.m_btn_Normalize.Name = "m_btn_Normalize";
            this.m_btn_Normalize.UseVisualStyleBackColor = true;
            this.m_btn_Normalize.Click += new System.EventHandler(this.m_btn_Normalize_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_btn_NoiseReduction);
            this.groupBox1.Controls.Add(this.m_btn_Normalize);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.m_btn_RetainSilenceInLastPhraseOfSection);
            this.groupBox3.Controls.Add(this.m_btn_DeleteSilenceFromSectionEnd);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // m_btn_RetainSilenceInLastPhraseOfSection
            // 
            resources.ApplyResources(this.m_btn_RetainSilenceInLastPhraseOfSection, "m_btn_RetainSilenceInLastPhraseOfSection");
            this.m_btn_RetainSilenceInLastPhraseOfSection.Name = "m_btn_RetainSilenceInLastPhraseOfSection";
            this.m_btn_RetainSilenceInLastPhraseOfSection.UseVisualStyleBackColor = true;
            this.m_btn_RetainSilenceInLastPhraseOfSection.Click += new System.EventHandler(this.m_btn__RetainSilenceInLastPhraseOfSection_Click);
            // 
            // m_btn_DeleteSilenceFromSectionEnd
            // 
            resources.ApplyResources(this.m_btn_DeleteSilenceFromSectionEnd, "m_btn_DeleteSilenceFromSectionEnd");
            this.m_btn_DeleteSilenceFromSectionEnd.Name = "m_btn_DeleteSilenceFromSectionEnd";
            this.m_btn_DeleteSilenceFromSectionEnd.UseVisualStyleBackColor = true;
            this.m_btn_DeleteSilenceFromSectionEnd.Click += new System.EventHandler(this.m_btn_DeleteSilenceFromSectionEnd_Click);
            // 
            // SelectMergeSectionRange
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_tb_SectionsSelected);
            this.Controls.Add(this.m_lb_listofSectionsToMerge);
            this.Controls.Add(this.m_grp_SectionAudioOperation);
            this.Controls.Add(this.m_grp_SectionLevelOperation);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.m_btn_Close);
            this.Controls.Add(this.m_statusStripForMergeSection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectMergeSectionRange";
            this.helpProvider1.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.m_statusStripForMergeSection.ResumeLayout(false);
            this.m_statusStripForMergeSection.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.m_grp_SectionLevelOperation.ResumeLayout(false);
            this.m_grp_SectionAudioOperation.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip m_statusStripForMergeSection;
        private System.Windows.Forms.ToolStripStatusLabel m_StatusLabelForMergeSection;
        private System.Windows.Forms.Button m_btn_SelectAll;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.GroupBox m_grp_SectionAudioOperation;
        private System.Windows.Forms.Button m_btn_Stop;
        private System.Windows.Forms.Button m_btn_Play;
        private System.Windows.Forms.ListBox m_lb_listofSectionsToMerge;
        private System.Windows.Forms.Button m_btnPause;
        private System.Windows.Forms.Button m_btn_IncreaseSectionLevel;
        private System.Windows.Forms.Button m_btn_DecreaseSectionLevel;
        private System.Windows.Forms.GroupBox m_grp_SectionLevelOperation;
        private System.Windows.Forms.Button m_btn_Undo;
        private System.Windows.Forms.Button m_btn_Merge;
        private System.Windows.Forms.Button m_btn_Close;
        private System.Windows.Forms.TextBox m_tb_SectionsSelected;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button m_btn_ShowContents;
        private System.Windows.Forms.Button m_btn_NoiseReduction;
        private System.Windows.Forms.Button m_btn_Normalize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button m_btn_RetainSilenceInLastPhraseOfSection;
        private System.Windows.Forms.Button m_btn_DeleteSilenceFromSectionEnd;
    }
}