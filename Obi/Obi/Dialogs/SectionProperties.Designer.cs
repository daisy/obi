namespace Obi.Dialogs
{
    partial class SectionProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SectionProperties));
            this.m_lblName = new System.Windows.Forms.Label();
            this.m_txtName = new System.Windows.Forms.TextBox();
            this.m_lblLevel = new System.Windows.Forms.Label();
            this.m_comboLevel = new System.Windows.Forms.ComboBox();
            this.m_lblTimeLength = new System.Windows.Forms.Label();
            this.m_txtTimeLength = new System.Windows.Forms.TextBox();
            this.m_lblPhraseCount = new System.Windows.Forms.Label();
            this.m_txtPhraseCount = new System.Windows.Forms.TextBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_chkUsed = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_lbParentsList = new System.Windows.Forms.ListBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lblName
            // 
            resources.ApplyResources(this.m_lblName, "m_lblName");
            this.m_lblName.Name = "m_lblName";
            // 
            // m_txtName
            // 
            resources.ApplyResources(this.m_txtName, "m_txtName");
            this.m_txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txtName.Name = "m_txtName";
            // 
            // m_lblLevel
            // 
            resources.ApplyResources(this.m_lblLevel, "m_lblLevel");
            this.m_lblLevel.Name = "m_lblLevel";
            // 
            // m_comboLevel
            // 
            resources.ApplyResources(this.m_comboLevel, "m_comboLevel");
            this.m_comboLevel.FormattingEnabled = true;
            this.m_comboLevel.Name = "m_comboLevel";
            // 
            // m_lblTimeLength
            // 
            resources.ApplyResources(this.m_lblTimeLength, "m_lblTimeLength");
            this.m_lblTimeLength.Name = "m_lblTimeLength";
            // 
            // m_txtTimeLength
            // 
            resources.ApplyResources(this.m_txtTimeLength, "m_txtTimeLength");
            this.m_txtTimeLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txtTimeLength.Name = "m_txtTimeLength";
            this.m_txtTimeLength.ReadOnly = true;
            // 
            // m_lblPhraseCount
            // 
            resources.ApplyResources(this.m_lblPhraseCount, "m_lblPhraseCount");
            this.m_lblPhraseCount.Name = "m_lblPhraseCount";
            // 
            // m_txtPhraseCount
            // 
            resources.ApplyResources(this.m_txtPhraseCount, "m_txtPhraseCount");
            this.m_txtPhraseCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txtPhraseCount.Name = "m_txtPhraseCount";
            this.m_txtPhraseCount.ReadOnly = true;
            // 
            // m_btnOk
            // 
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_chkUsed
            // 
            resources.ApplyResources(this.m_chkUsed, "m_chkUsed");
            this.m_chkUsed.Name = "m_chkUsed";
            this.m_chkUsed.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_lbParentsList);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // m_lbParentsList
            // 
            resources.ApplyResources(this.m_lbParentsList, "m_lbParentsList");
            this.m_lbParentsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_lbParentsList.FormattingEnabled = true;
            this.m_lbParentsList.Name = "m_lbParentsList";
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // SectionProperties
            // 
            this.AcceptButton = this.m_btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_chkUsed);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_txtPhraseCount);
            this.Controls.Add(this.m_lblPhraseCount);
            this.Controls.Add(this.m_txtTimeLength);
            this.Controls.Add(this.m_lblTimeLength);
            this.Controls.Add(this.m_comboLevel);
            this.Controls.Add(this.m_lblLevel);
            this.Controls.Add(this.m_txtName);
            this.Controls.Add(this.m_lblName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SectionProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.SectionProperties_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblName;
        private System.Windows.Forms.TextBox m_txtName;
        private System.Windows.Forms.Label m_lblLevel;
        private System.Windows.Forms.ComboBox m_comboLevel;
        private System.Windows.Forms.Label m_lblTimeLength;
        private System.Windows.Forms.TextBox m_txtTimeLength;
        private System.Windows.Forms.Label m_lblPhraseCount;
        private System.Windows.Forms.TextBox m_txtPhraseCount;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.CheckBox m_chkUsed;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox m_lbParentsList;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}