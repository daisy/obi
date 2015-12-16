namespace Obi.Dialogs
{
    partial class PhraseProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhraseProperties));
            this.m_lblParentSection = new System.Windows.Forms.Label();
            this.m_txtParentSection = new System.Windows.Forms.TextBox();
            this.m_lblLocationInsideSection = new System.Windows.Forms.Label();
            this.m_txtLocationInsideSection = new System.Windows.Forms.TextBox();
            this.m_lblPhraseRole = new System.Windows.Forms.Label();
            this.m_comboPhraseRole = new System.Windows.Forms.ComboBox();
            this.m_lblCustomClassName = new System.Windows.Forms.Label();
            this.m_chkUsed = new System.Windows.Forms.CheckBox();
            this.m_chkToDo = new System.Windows.Forms.CheckBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblTimeLength = new System.Windows.Forms.Label();
            this.m_txtTimeLength = new System.Windows.Forms.TextBox();
            this.m_comboCustomClassName = new System.Windows.Forms.ComboBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_lbParentsList = new System.Windows.Forms.ListBox();
            this.m_lblPageNumberDetails = new System.Windows.Forms.Label();
            this.m_txtPageNumberDetails = new System.Windows.Forms.TextBox();
            this.m_chkChangePageNumber = new System.Windows.Forms.CheckBox();
            this.m_lbl_ReferredNote = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_lblCurrentCursorPosition = new System.Windows.Forms.Label();
            this.m_txtCurrentCursorPosition = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lblParentSection
            // 
            resources.ApplyResources(this.m_lblParentSection, "m_lblParentSection");
            this.m_lblParentSection.Name = "m_lblParentSection";
            // 
            // m_txtParentSection
            // 
            resources.ApplyResources(this.m_txtParentSection, "m_txtParentSection");
            this.m_txtParentSection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txtParentSection.Name = "m_txtParentSection";
            this.m_txtParentSection.ReadOnly = true;
            // 
            // m_lblLocationInsideSection
            // 
            resources.ApplyResources(this.m_lblLocationInsideSection, "m_lblLocationInsideSection");
            this.m_lblLocationInsideSection.Name = "m_lblLocationInsideSection";
            // 
            // m_txtLocationInsideSection
            // 
            resources.ApplyResources(this.m_txtLocationInsideSection, "m_txtLocationInsideSection");
            this.m_txtLocationInsideSection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txtLocationInsideSection.Name = "m_txtLocationInsideSection";
            this.m_txtLocationInsideSection.ReadOnly = true;
            // 
            // m_lblPhraseRole
            // 
            resources.ApplyResources(this.m_lblPhraseRole, "m_lblPhraseRole");
            this.m_lblPhraseRole.Name = "m_lblPhraseRole";
            // 
            // m_comboPhraseRole
            // 
            resources.ApplyResources(this.m_comboPhraseRole, "m_comboPhraseRole");
            this.m_comboPhraseRole.FormattingEnabled = true;
            this.m_comboPhraseRole.Name = "m_comboPhraseRole";
            this.m_comboPhraseRole.SelectionChangeCommitted += new System.EventHandler(this.m_comboPhraseRole_SelectionChangeCommitted);
            // 
            // m_lblCustomClassName
            // 
            resources.ApplyResources(this.m_lblCustomClassName, "m_lblCustomClassName");
            this.m_lblCustomClassName.Name = "m_lblCustomClassName";
            // 
            // m_chkUsed
            // 
            resources.ApplyResources(this.m_chkUsed, "m_chkUsed");
            this.m_chkUsed.Name = "m_chkUsed";
            this.m_chkUsed.UseVisualStyleBackColor = true;
            // 
            // m_chkToDo
            // 
            resources.ApplyResources(this.m_chkToDo, "m_chkToDo");
            this.m_chkToDo.Name = "m_chkToDo";
            this.m_chkToDo.UseVisualStyleBackColor = true;
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
            // m_comboCustomClassName
            // 
            resources.ApplyResources(this.m_comboCustomClassName, "m_comboCustomClassName");
            this.m_comboCustomClassName.FormattingEnabled = true;
            this.m_comboCustomClassName.Name = "m_comboCustomClassName";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_lbParentsList);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // m_lbParentsList
            // 
            resources.ApplyResources(this.m_lbParentsList, "m_lbParentsList");
            this.m_lbParentsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_lbParentsList.FormattingEnabled = true;
            this.m_lbParentsList.Name = "m_lbParentsList";
            this.m_lbParentsList.SelectedIndexChanged += new System.EventHandler(this.m_lbParentsList_SelectedIndexChanged);
            // 
            // m_lblPageNumberDetails
            // 
            resources.ApplyResources(this.m_lblPageNumberDetails, "m_lblPageNumberDetails");
            this.m_lblPageNumberDetails.Name = "m_lblPageNumberDetails";
            // 
            // m_txtPageNumberDetails
            // 
            resources.ApplyResources(this.m_txtPageNumberDetails, "m_txtPageNumberDetails");
            this.m_txtPageNumberDetails.Name = "m_txtPageNumberDetails";
            this.m_txtPageNumberDetails.ReadOnly = true;
            // 
            // m_chkChangePageNumber
            // 
            resources.ApplyResources(this.m_chkChangePageNumber, "m_chkChangePageNumber");
            this.m_chkChangePageNumber.Name = "m_chkChangePageNumber";
            this.m_chkChangePageNumber.UseVisualStyleBackColor = true;
            // 
            // m_lbl_ReferredNote
            // 
            resources.ApplyResources(this.m_lbl_ReferredNote, "m_lbl_ReferredNote");
            this.m_lbl_ReferredNote.Name = "m_lbl_ReferredNote";
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // m_lblCurrentCursorPosition
            // 
            resources.ApplyResources(this.m_lblCurrentCursorPosition, "m_lblCurrentCursorPosition");
            this.m_lblCurrentCursorPosition.Name = "m_lblCurrentCursorPosition";
            // 
            // m_txtCurrentCursorPosition
            // 
            resources.ApplyResources(this.m_txtCurrentCursorPosition, "m_txtCurrentCursorPosition");
            this.m_txtCurrentCursorPosition.Name = "m_txtCurrentCursorPosition";
            this.m_txtCurrentCursorPosition.ReadOnly = true;
            // 
            // PhraseProperties
            // 
            this.AcceptButton = this.m_btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.m_txtCurrentCursorPosition);
            this.Controls.Add(this.m_lblCurrentCursorPosition);
            this.Controls.Add(this.m_lbl_ReferredNote);
            this.Controls.Add(this.m_chkChangePageNumber);
            this.Controls.Add(this.m_txtPageNumberDetails);
            this.Controls.Add(this.m_lblPageNumberDetails);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_comboCustomClassName);
            this.Controls.Add(this.m_txtTimeLength);
            this.Controls.Add(this.m_lblTimeLength);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_chkToDo);
            this.Controls.Add(this.m_chkUsed);
            this.Controls.Add(this.m_lblCustomClassName);
            this.Controls.Add(this.m_comboPhraseRole);
            this.Controls.Add(this.m_lblPhraseRole);
            this.Controls.Add(this.m_txtLocationInsideSection);
            this.Controls.Add(this.m_lblLocationInsideSection);
            this.Controls.Add(this.m_txtParentSection);
            this.Controls.Add(this.m_lblParentSection);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PhraseProperties";
            this.helpProvider1.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PhraseProperties_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblParentSection;
        private System.Windows.Forms.TextBox m_txtParentSection;
        private System.Windows.Forms.Label m_lblLocationInsideSection;
        private System.Windows.Forms.TextBox m_txtLocationInsideSection;
        private System.Windows.Forms.Label m_lblPhraseRole;
        private System.Windows.Forms.ComboBox m_comboPhraseRole;
        private System.Windows.Forms.Label m_lblCustomClassName;
        private System.Windows.Forms.CheckBox m_chkUsed;
        private System.Windows.Forms.CheckBox m_chkToDo;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblTimeLength;
        private System.Windows.Forms.TextBox m_txtTimeLength;
        private System.Windows.Forms.ComboBox m_comboCustomClassName;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox m_lbParentsList;
        private System.Windows.Forms.Label m_lblPageNumberDetails;
        private System.Windows.Forms.TextBox m_txtPageNumberDetails;
        private System.Windows.Forms.CheckBox m_chkChangePageNumber;
        private System.Windows.Forms.Label m_lbl_ReferredNote;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Label m_lblCurrentCursorPosition;
        private System.Windows.Forms.TextBox m_txtCurrentCursorPosition;
    }
}