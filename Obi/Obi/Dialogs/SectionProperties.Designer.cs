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
            this.m_lbParentsList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // m_lblName
            // 
            this.m_lblName.AutoSize = true;
            this.m_lblName.Location = new System.Drawing.Point(0, 10);
            this.m_lblName.Name = "m_lblName";
            this.m_lblName.Size = new System.Drawing.Size(77, 13);
            this.m_lblName.TabIndex = 0;
            this.m_lblName.Text = "Section &Name:";
            // 
            // m_txtName
            // 
            this.m_txtName.AccessibleName = "Section Name:";
            this.m_txtName.Location = new System.Drawing.Point(100, 10);
            this.m_txtName.Name = "m_txtName";
            this.m_txtName.Size = new System.Drawing.Size(100, 20);
            this.m_txtName.TabIndex = 1;
            // 
            // m_lblLevel
            // 
            this.m_lblLevel.AutoSize = true;
            this.m_lblLevel.Location = new System.Drawing.Point(0, 40);
            this.m_lblLevel.Name = "m_lblLevel";
            this.m_lblLevel.Size = new System.Drawing.Size(70, 13);
            this.m_lblLevel.TabIndex = 2;
            this.m_lblLevel.Text = "&Level/Depth:";
            // 
            // m_comboLevel
            // 
            this.m_comboLevel.AccessibleName = "Level/Depth:";
            this.m_comboLevel.FormattingEnabled = true;
            this.m_comboLevel.Location = new System.Drawing.Point(100, 40);
            this.m_comboLevel.Name = "m_comboLevel";
            this.m_comboLevel.Size = new System.Drawing.Size(121, 21);
            this.m_comboLevel.TabIndex = 3;
            // 
            // m_lblTimeLength
            // 
            this.m_lblTimeLength.AutoSize = true;
            this.m_lblTimeLength.Location = new System.Drawing.Point(0, 120);
            this.m_lblTimeLength.Name = "m_lblTimeLength";
            this.m_lblTimeLength.Size = new System.Drawing.Size(97, 13);
            this.m_lblTimeLength.TabIndex = 5;
            this.m_lblTimeLength.Text = "Leng&th in seconds:";
            // 
            // m_txtTimeLength
            // 
            this.m_txtTimeLength.AccessibleName = "Length in seconds:";
            this.m_txtTimeLength.Location = new System.Drawing.Point(100, 120);
            this.m_txtTimeLength.Name = "m_txtTimeLength";
            this.m_txtTimeLength.ReadOnly = true;
            this.m_txtTimeLength.Size = new System.Drawing.Size(100, 20);
            this.m_txtTimeLength.TabIndex = 5;
            // 
            // m_lblPhraseCount
            // 
            this.m_lblPhraseCount.AutoSize = true;
            this.m_lblPhraseCount.Location = new System.Drawing.Point(0, 150);
            this.m_lblPhraseCount.Name = "m_lblPhraseCount";
            this.m_lblPhraseCount.Size = new System.Drawing.Size(80, 13);
            this.m_lblPhraseCount.TabIndex = 6;
            this.m_lblPhraseCount.Text = "No. of Phrases:";
            // 
            // m_txtPhraseCount
            // 
            this.m_txtPhraseCount.AccessibleName = "Number of phrases:";
            this.m_txtPhraseCount.Location = new System.Drawing.Point(100, 150);
            this.m_txtPhraseCount.Name = "m_txtPhraseCount";
            this.m_txtPhraseCount.ReadOnly = true;
            this.m_txtPhraseCount.Size = new System.Drawing.Size(100, 20);
            this.m_txtPhraseCount.TabIndex = 7;
            // 
            // m_btnOk
            // 
            this.m_btnOk.Location = new System.Drawing.Point(0, 250);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 9;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(100, 250);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 10;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_chkUsed
            // 
            this.m_chkUsed.AutoSize = true;
            this.m_chkUsed.Location = new System.Drawing.Point(100, 180);
            this.m_chkUsed.Name = "m_chkUsed";
            this.m_chkUsed.Size = new System.Drawing.Size(51, 17);
            this.m_chkUsed.TabIndex = 8;
            this.m_chkUsed.Text = "&Used";
            this.m_chkUsed.UseVisualStyleBackColor = true;
            // 
            // m_lbParentsList
            // 
            this.m_lbParentsList.AccessibleName = "List of parent sections";
            this.m_lbParentsList.FormattingEnabled = true;
            this.m_lbParentsList.Location = new System.Drawing.Point(100, 80);
            this.m_lbParentsList.Name = "m_lbParentsList";
            this.m_lbParentsList.Size = new System.Drawing.Size(120, 69);
            this.m_lbParentsList.TabIndex = 5;
            // 
            // SectionProperties
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.m_lbParentsList);
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
            this.Name = "SectionProperties";
            this.Text = "Section Properties";
            this.Load += new System.EventHandler(this.SectionProperties_Load);
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
        private System.Windows.Forms.ListBox m_lbParentsList;
    }
}