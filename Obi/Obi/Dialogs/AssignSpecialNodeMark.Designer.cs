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
            this.m_cmbBoxSpecialNode = new System.Windows.Forms.ComboBox();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_rdb_btn_SpecialPhrase = new System.Windows.Forms.RadioButton();
            this.m_rdb_btn_RenumberPages = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // m_cmbBoxSpecialNode
            // 
            this.m_cmbBoxSpecialNode.FormattingEnabled = true;
            this.m_cmbBoxSpecialNode.Items.AddRange(new object[] {
            "annotation",
            "endnote",
            "footnote",
            "note",
            "prodnote",
            "sidebar"});
            this.m_cmbBoxSpecialNode.Location = new System.Drawing.Point(53, 46);
            this.m_cmbBoxSpecialNode.Name = "m_cmbBoxSpecialNode";
            this.m_cmbBoxSpecialNode.Size = new System.Drawing.Size(181, 21);
            this.m_cmbBoxSpecialNode.TabIndex = 0;
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_OK.Location = new System.Drawing.Point(53, 89);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 1;
            this.m_btn_OK.Text = "OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Cancel.Location = new System.Drawing.Point(159, 89);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 2;
            this.m_btn_Cancel.Text = "Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // m_rdb_btn_SpecialPhrase
            // 
            this.m_rdb_btn_SpecialPhrase.AutoSize = true;
            this.m_rdb_btn_SpecialPhrase.Checked = true;
            this.m_rdb_btn_SpecialPhrase.Location = new System.Drawing.Point(42, 12);
            this.m_rdb_btn_SpecialPhrase.Name = "m_rdb_btn_SpecialPhrase";
            this.m_rdb_btn_SpecialPhrase.Size = new System.Drawing.Size(101, 17);
            this.m_rdb_btn_SpecialPhrase.TabIndex = 3;
            this.m_rdb_btn_SpecialPhrase.TabStop = true;
            this.m_rdb_btn_SpecialPhrase.Text = "Skippable notes";
            this.m_rdb_btn_SpecialPhrase.UseVisualStyleBackColor = true;
            this.m_rdb_btn_SpecialPhrase.CheckedChanged += new System.EventHandler(this.m_rdb_btn_SpecialPhrase_CheckedChanged);
            // 
            // m_rdb_btn_RenumberPages
            // 
            this.m_rdb_btn_RenumberPages.AutoSize = true;
            this.m_rdb_btn_RenumberPages.Location = new System.Drawing.Point(163, 12);
            this.m_rdb_btn_RenumberPages.Name = "m_rdb_btn_RenumberPages";
            this.m_rdb_btn_RenumberPages.Size = new System.Drawing.Size(106, 17);
            this.m_rdb_btn_RenumberPages.TabIndex = 4;
            this.m_rdb_btn_RenumberPages.TabStop = true;
            this.m_rdb_btn_RenumberPages.Text = "Renumber pages";
            this.m_rdb_btn_RenumberPages.UseVisualStyleBackColor = true;
            this.m_rdb_btn_RenumberPages.CheckedChanged += new System.EventHandler(this.m_rdb_btn_RenumberPages_CheckedChanged);
            // 
            // AssignSpecialNodeMark
            // 
            this.AcceptButton = this.m_btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.ClientSize = new System.Drawing.Size(292, 124);
            this.Controls.Add(this.m_rdb_btn_RenumberPages);
            this.Controls.Add(this.m_rdb_btn_SpecialPhrase);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_cmbBoxSpecialNode);
            this.Name = "AssignSpecialNodeMark";
            this.Text = "Assign note/ Renumber pages";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox m_cmbBoxSpecialNode;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.RadioButton m_rdb_btn_SpecialPhrase;
        private System.Windows.Forms.RadioButton m_rdb_btn_RenumberPages;
    }
}