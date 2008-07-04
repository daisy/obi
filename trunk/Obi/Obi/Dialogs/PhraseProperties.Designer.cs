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
            this.m_lblParentSection = new System.Windows.Forms.Label();
            this.m_txtParentSection = new System.Windows.Forms.TextBox();
            this.m_lblLocationInsideSection = new System.Windows.Forms.Label();
            this.m_txtLocationInsideSection = new System.Windows.Forms.TextBox();
            this.m_lbParentsList = new System.Windows.Forms.ListBox();
            this.m_lblPhraseRole = new System.Windows.Forms.Label();
            this.m_comboPhraseRole = new System.Windows.Forms.ComboBox();
            this.m_lblCustomClassName = new System.Windows.Forms.Label();
            this.m_txtCustomClassName = new System.Windows.Forms.TextBox();
            this.m_chkUsed = new System.Windows.Forms.CheckBox();
            this.m_chkToDo = new System.Windows.Forms.CheckBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblTimeLength = new System.Windows.Forms.Label();
            this.m_txtTimeLength = new System.Windows.Forms.TextBox();
            this.m_gParentsList = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // m_lblParentSection
            // 
            this.m_lblParentSection.AutoSize = true;
            this.m_lblParentSection.Location = new System.Drawing.Point(0, 10);
            this.m_lblParentSection.Name = "m_lblParentSection";
            this.m_lblParentSection.Size = new System.Drawing.Size(111, 13);
            this.m_lblParentSection.TabIndex = 0;
            this.m_lblParentSection.Text = "Parent &Section Name:";
            // 
            // m_txtParentSection
            // 
            this.m_txtParentSection.AccessibleName = "Parent Section";
            this.m_txtParentSection.Location = new System.Drawing.Point(100, 10);
            this.m_txtParentSection.Name = "m_txtParentSection";
            this.m_txtParentSection.ReadOnly = true;
            this.m_txtParentSection.Size = new System.Drawing.Size(100, 20);
            this.m_txtParentSection.TabIndex = 1;
            // 
            // m_lblLocationInsideSection
            // 
            this.m_lblLocationInsideSection.AutoSize = true;
            this.m_lblLocationInsideSection.Location = new System.Drawing.Point(0, 40);
            this.m_lblLocationInsideSection.Name = "m_lblLocationInsideSection";
            this.m_lblLocationInsideSection.Size = new System.Drawing.Size(139, 13);
            this.m_lblLocationInsideSection.TabIndex = 2;
            this.m_lblLocationInsideSection.Text = "Location &in section phrases:";
            // 
            // m_txtLocationInsideSection
            // 
            this.m_txtLocationInsideSection.AccessibleName = "Location in section phrases:";
            this.m_txtLocationInsideSection.Location = new System.Drawing.Point(100, 40);
            this.m_txtLocationInsideSection.Name = "m_txtLocationInsideSection";
            this.m_txtLocationInsideSection.ReadOnly = true;
            this.m_txtLocationInsideSection.Size = new System.Drawing.Size(100, 20);
            this.m_txtLocationInsideSection.TabIndex = 3;
            // 
            // m_lbParentsList
            // 
            this.m_lbParentsList.AccessibleName = "List of Parent sections";
            this.m_lbParentsList.FormattingEnabled = true;
            this.m_lbParentsList.Location = new System.Drawing.Point(100, 80);
            this.m_lbParentsList.Name = "m_lbParentsList";
            this.m_lbParentsList.Size = new System.Drawing.Size(120, 69);
            this.m_lbParentsList.TabIndex = 4;
            // 
            // m_lblPhraseRole
            // 
            this.m_lblPhraseRole.AutoSize = true;
            this.m_lblPhraseRole.Location = new System.Drawing.Point(0, 210);
            this.m_lblPhraseRole.Name = "m_lblPhraseRole";
            this.m_lblPhraseRole.Size = new System.Drawing.Size(68, 13);
            this.m_lblPhraseRole.TabIndex = 7;
            this.m_lblPhraseRole.Text = "Phrase &Role:";
            // 
            // m_comboPhraseRole
            // 
            this.m_comboPhraseRole.AccessibleName = "Phrase Role:";
            this.m_comboPhraseRole.FormattingEnabled = true;
            this.m_comboPhraseRole.Location = new System.Drawing.Point(100, 210);
            this.m_comboPhraseRole.Name = "m_comboPhraseRole";
            this.m_comboPhraseRole.Size = new System.Drawing.Size(121, 21);
            this.m_comboPhraseRole.TabIndex = 8;
            this.m_comboPhraseRole.SelectionChangeCommitted += new System.EventHandler(this.m_comboPhraseRole_SelectionChangeCommitted);
            // 
            // m_lblCustomClassName
            // 
            this.m_lblCustomClassName.AutoSize = true;
            this.m_lblCustomClassName.Location = new System.Drawing.Point(50, 240);
            this.m_lblCustomClassName.Name = "m_lblCustomClassName";
            this.m_lblCustomClassName.Size = new System.Drawing.Size(103, 13);
            this.m_lblCustomClassName.TabIndex = 9;
            this.m_lblCustomClassName.Text = "C&ustom class Name:";
            // 
            // m_txtCustomClassName
            // 
            this.m_txtCustomClassName.AccessibleName = "Custom class name:";
            this.m_txtCustomClassName.Location = new System.Drawing.Point(150, 240);
            this.m_txtCustomClassName.Name = "m_txtCustomClassName";
            this.m_txtCustomClassName.Size = new System.Drawing.Size(100, 20);
            this.m_txtCustomClassName.TabIndex = 10;
            // 
            // m_chkUsed
            // 
            this.m_chkUsed.AutoSize = true;
            this.m_chkUsed.Location = new System.Drawing.Point(0, 270);
            this.m_chkUsed.Name = "m_chkUsed";
            this.m_chkUsed.Size = new System.Drawing.Size(51, 17);
            this.m_chkUsed.TabIndex = 11;
            this.m_chkUsed.Text = "&Used";
            this.m_chkUsed.UseVisualStyleBackColor = true;
            // 
            // m_chkToDo
            // 
            this.m_chkToDo.AutoSize = true;
            this.m_chkToDo.Location = new System.Drawing.Point(150, 270);
            this.m_chkToDo.Name = "m_chkToDo";
            this.m_chkToDo.Size = new System.Drawing.Size(54, 17);
            this.m_chkToDo.TabIndex = 12;
            this.m_chkToDo.Text = "&To do";
            this.m_chkToDo.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            this.m_btnOk.Location = new System.Drawing.Point(0, 310);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 13;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(100, 310);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 14;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_lblTimeLength
            // 
            this.m_lblTimeLength.AutoSize = true;
            this.m_lblTimeLength.Location = new System.Drawing.Point(0, 180);
            this.m_lblTimeLength.Name = "m_lblTimeLength";
            this.m_lblTimeLength.Size = new System.Drawing.Size(97, 13);
            this.m_lblTimeLength.TabIndex = 5;
            this.m_lblTimeLength.Text = "&Length in seconds:";
            // 
            // m_txtTimeLength
            // 
            this.m_txtTimeLength.AccessibleName = "Length in seconds:";
            this.m_txtTimeLength.Location = new System.Drawing.Point(100, 180);
            this.m_txtTimeLength.Name = "m_txtTimeLength";
            this.m_txtTimeLength.ReadOnly = true;
            this.m_txtTimeLength.Size = new System.Drawing.Size(100, 20);
            this.m_txtTimeLength.TabIndex = 6;
            // 
            // m_gParentsList
            // 
            this.m_gParentsList.Location = new System.Drawing.Point(80, 75);
            this.m_gParentsList.Name = "m_gParentsList";
            this.m_gParentsList.Size = new System.Drawing.Size(140, 85);
            this.m_gParentsList.TabIndex = 4;
            this.m_gParentsList.TabStop = false;
            this.m_gParentsList.Text = "List of parent sections";
            // 
            // PhraseProperties
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 323);
            this.Controls.Add(this.m_gParentsList);
            this.Controls.Add(this.m_txtTimeLength);
            this.Controls.Add(this.m_lblTimeLength);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_chkToDo);
            this.Controls.Add(this.m_chkUsed);
            this.Controls.Add(this.m_txtCustomClassName);
            this.Controls.Add(this.m_lblCustomClassName);
            this.Controls.Add(this.m_comboPhraseRole);
            this.Controls.Add(this.m_lblPhraseRole);
            this.Controls.Add(this.m_lbParentsList);
            this.Controls.Add(this.m_txtLocationInsideSection);
            this.Controls.Add(this.m_lblLocationInsideSection);
            this.Controls.Add(this.m_txtParentSection);
            this.Controls.Add(this.m_lblParentSection);
            this.Name = "PhraseProperties";
            this.Text = "PhraseProperties";
            this.Load += new System.EventHandler(this.PhraseProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblParentSection;
        private System.Windows.Forms.TextBox m_txtParentSection;
        private System.Windows.Forms.Label m_lblLocationInsideSection;
        private System.Windows.Forms.TextBox m_txtLocationInsideSection;
        private System.Windows.Forms.ListBox m_lbParentsList;
        private System.Windows.Forms.Label m_lblPhraseRole;
        private System.Windows.Forms.ComboBox m_comboPhraseRole;
        private System.Windows.Forms.Label m_lblCustomClassName;
        private System.Windows.Forms.TextBox m_txtCustomClassName;
        private System.Windows.Forms.CheckBox m_chkUsed;
        private System.Windows.Forms.CheckBox m_chkToDo;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblTimeLength;
        private System.Windows.Forms.TextBox m_txtTimeLength;
        private System.Windows.Forms.GroupBox m_gParentsList;
    }
}