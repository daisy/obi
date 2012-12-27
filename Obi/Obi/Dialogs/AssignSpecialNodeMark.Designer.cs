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
            this.m_rdb_btn_RenumberPages.TabStop = true;
            this.m_rdb_btn_RenumberPages.UseVisualStyleBackColor = true;
            this.m_rdb_btn_RenumberPages.CheckedChanged += new System.EventHandler(this.m_rdb_btn_RenumberPages_CheckedChanged);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // AssignSpecialNodeMark
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.m_rdb_btn_RenumberPages);
            this.Controls.Add(this.m_rdb_btn_SpecialPhrase);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_cmbBoxSpecialNode);
            this.Name = "AssignSpecialNodeMark";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox m_cmbBoxSpecialNode;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.RadioButton m_rdb_btn_SpecialPhrase;
        private System.Windows.Forms.RadioButton m_rdb_btn_RenumberPages;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}