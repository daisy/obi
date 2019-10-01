namespace Obi.Dialogs
{
    partial class AssignSpecialNodeMark
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssignSpecialNodeMark));
            this.m_cmbBoxSpecialNode = new System.Windows.Forms.ComboBox();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_rdb_btn_SpecialPhrase = new System.Windows.Forms.RadioButton();
            this.m_rdb_btn_RenumberPages = new System.Windows.Forms.RadioButton();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_gp_box_operation = new System.Windows.Forms.GroupBox();
            this.m_rdb_btn_Normalize = new System.Windows.Forms.RadioButton();
            this.m_rdb_btn_ChangeVolume = new System.Windows.Forms.RadioButton();
            this.m_rdb_Delete = new System.Windows.Forms.RadioButton();
            this.m_rdb_Merge = new System.Windows.Forms.RadioButton();
            this.m_rdb_Copy = new System.Windows.Forms.RadioButton();
            this.m_rdb_Cut = new System.Windows.Forms.RadioButton();
            this.m_rtb_btn_TimeElapsed = new System.Windows.Forms.RadioButton();
            this.m_rdb_btn_SpeechRate = new System.Windows.Forms.RadioButton();
            this.m_gp_box_operation.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_cmbBoxSpecialNode
            // 
            this.m_cmbBoxSpecialNode.FormattingEnabled = true;
            this.m_cmbBoxSpecialNode.Items.AddRange(new object[] {
            resources.GetString("m_cmbBoxSpecialNode.Items"),
            resources.GetString("m_cmbBoxSpecialNode.Items1"),
            resources.GetString("m_cmbBoxSpecialNode.Items2"),
            resources.GetString("m_cmbBoxSpecialNode.Items3"),
            resources.GetString("m_cmbBoxSpecialNode.Items4"),
            resources.GetString("m_cmbBoxSpecialNode.Items5")});
            resources.ApplyResources(this.m_cmbBoxSpecialNode, "m_cmbBoxSpecialNode");
            this.m_cmbBoxSpecialNode.Name = "m_cmbBoxSpecialNode";
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btn_OK, "m_btn_OK");
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btn_Cancel, "m_btn_Cancel");
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // m_rdb_btn_SpecialPhrase
            // 
            resources.ApplyResources(this.m_rdb_btn_SpecialPhrase, "m_rdb_btn_SpecialPhrase");
            this.m_rdb_btn_SpecialPhrase.Checked = true;
            this.m_rdb_btn_SpecialPhrase.Name = "m_rdb_btn_SpecialPhrase";
            this.m_rdb_btn_SpecialPhrase.TabStop = true;
            this.m_rdb_btn_SpecialPhrase.UseVisualStyleBackColor = true;
            this.m_rdb_btn_SpecialPhrase.CheckedChanged += new System.EventHandler(this.m_rdb_btn_SpecialPhrase_CheckedChanged);
            // 
            // m_rdb_btn_RenumberPages
            // 
            resources.ApplyResources(this.m_rdb_btn_RenumberPages, "m_rdb_btn_RenumberPages");
            this.m_rdb_btn_RenumberPages.Name = "m_rdb_btn_RenumberPages";
            this.m_rdb_btn_RenumberPages.UseVisualStyleBackColor = true;
            this.m_rdb_btn_RenumberPages.CheckedChanged += new System.EventHandler(this.m_rdb_btn_RenumberPages_CheckedChanged);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // m_gp_box_operation
            // 
            this.m_gp_box_operation.Controls.Add(this.m_rdb_btn_SpeechRate);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_btn_Normalize);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_btn_ChangeVolume);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_Delete);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_Merge);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_Copy);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_Cut);
            this.m_gp_box_operation.Controls.Add(this.m_rtb_btn_TimeElapsed);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_btn_SpecialPhrase);
            this.m_gp_box_operation.Controls.Add(this.m_rdb_btn_RenumberPages);
            resources.ApplyResources(this.m_gp_box_operation, "m_gp_box_operation");
            this.m_gp_box_operation.Name = "m_gp_box_operation";
            this.helpProvider1.SetShowHelp(this.m_gp_box_operation, ((bool)(resources.GetObject("m_gp_box_operation.ShowHelp"))));
            this.m_gp_box_operation.TabStop = false;
            // 
            // m_rdb_btn_Normalize
            // 
            resources.ApplyResources(this.m_rdb_btn_Normalize, "m_rdb_btn_Normalize");
            this.m_rdb_btn_Normalize.Name = "m_rdb_btn_Normalize";
            this.m_rdb_btn_Normalize.UseVisualStyleBackColor = true;
            this.m_rdb_btn_Normalize.CheckedChanged += new System.EventHandler(this.m_rdb_btn_Normalize_CheckedChanged);
            // 
            // m_rdb_btn_ChangeVolume
            // 
            resources.ApplyResources(this.m_rdb_btn_ChangeVolume, "m_rdb_btn_ChangeVolume");
            this.m_rdb_btn_ChangeVolume.Name = "m_rdb_btn_ChangeVolume";
            this.m_rdb_btn_ChangeVolume.UseVisualStyleBackColor = true;
            this.m_rdb_btn_ChangeVolume.CheckedChanged += new System.EventHandler(this.m_rdb_btn_ChangeVolume_CheckedChanged);
            // 
            // m_rdb_Delete
            // 
            resources.ApplyResources(this.m_rdb_Delete, "m_rdb_Delete");
            this.m_rdb_Delete.Name = "m_rdb_Delete";
            this.m_rdb_Delete.UseVisualStyleBackColor = true;
            this.m_rdb_Delete.CheckedChanged += new System.EventHandler(this.m_rdb_Delete_CheckedChanged);
            // 
            // m_rdb_Merge
            // 
            resources.ApplyResources(this.m_rdb_Merge, "m_rdb_Merge");
            this.m_rdb_Merge.Name = "m_rdb_Merge";
            this.m_rdb_Merge.UseVisualStyleBackColor = true;
            this.m_rdb_Merge.CheckedChanged += new System.EventHandler(this.m_rdb_Merge_CheckedChanged);
            // 
            // m_rdb_Copy
            // 
            resources.ApplyResources(this.m_rdb_Copy, "m_rdb_Copy");
            this.m_rdb_Copy.Name = "m_rdb_Copy";
            this.m_rdb_Copy.UseVisualStyleBackColor = true;
            this.m_rdb_Copy.CheckedChanged += new System.EventHandler(this.m_rdb_Copy_CheckedChanged);
            // 
            // m_rdb_Cut
            // 
            resources.ApplyResources(this.m_rdb_Cut, "m_rdb_Cut");
            this.m_rdb_Cut.Name = "m_rdb_Cut";
            this.m_rdb_Cut.UseVisualStyleBackColor = true;
            this.m_rdb_Cut.CheckedChanged += new System.EventHandler(this.m_rdb_Cut_CheckedChanged);
            // 
            // m_rtb_btn_TimeElapsed
            // 
            resources.ApplyResources(this.m_rtb_btn_TimeElapsed, "m_rtb_btn_TimeElapsed");
            this.m_rtb_btn_TimeElapsed.Name = "m_rtb_btn_TimeElapsed";
            this.m_rtb_btn_TimeElapsed.UseVisualStyleBackColor = true;
            this.m_rtb_btn_TimeElapsed.CheckedChanged += new System.EventHandler(this.m_rtb_btn_TimeElapsed_CheckedChanged);
            // 
            // m_rdb_btn_SpeechRate
            // 
            resources.ApplyResources(this.m_rdb_btn_SpeechRate, "m_rdb_btn_SpeechRate");
            this.m_rdb_btn_SpeechRate.Name = "m_rdb_btn_SpeechRate";
            this.m_rdb_btn_SpeechRate.TabStop = true;
            this.m_rdb_btn_SpeechRate.UseVisualStyleBackColor = true;
            // 
            // AssignSpecialNodeMark
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.m_gp_box_operation);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_cmbBoxSpecialNode);
            this.Name = "AssignSpecialNodeMark";
            this.helpProvider1.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.m_gp_box_operation.ResumeLayout(false);
            this.m_gp_box_operation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox m_cmbBoxSpecialNode;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.RadioButton m_rdb_btn_SpecialPhrase;
        private System.Windows.Forms.RadioButton m_rdb_btn_RenumberPages;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.GroupBox m_gp_box_operation;
        private System.Windows.Forms.RadioButton m_rtb_btn_TimeElapsed;
        private System.Windows.Forms.RadioButton m_rdb_Copy;
        private System.Windows.Forms.RadioButton m_rdb_Cut;
        private System.Windows.Forms.RadioButton m_rdb_Merge;
        private System.Windows.Forms.RadioButton m_rdb_Delete;
        private System.Windows.Forms.RadioButton m_rdb_btn_Normalize;
        private System.Windows.Forms.RadioButton m_rdb_btn_ChangeVolume;
        private System.Windows.Forms.RadioButton m_rdb_btn_SpeechRate;
    }
}