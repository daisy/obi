namespace Obi.Dialogs
{
    partial class SpecialPhraseList
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
            this.m_grpSelectSpecialPhrases = new System.Windows.Forms.GroupBox();
            this.m_cb_SpecialPhrases = new System.Windows.Forms.ComboBox();
            this.m_btnFind = new System.Windows.Forms.Button();
            this.m_lbSpecialPhrasesList = new System.Windows.Forms.ListBox();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_grpSelectSpecialPhrases.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_grpSelectSpecialPhrases
            // 
            this.m_grpSelectSpecialPhrases.AccessibleName = "Select special phrases to show in list box";
            this.m_grpSelectSpecialPhrases.Controls.Add(this.m_cb_SpecialPhrases);
            this.m_grpSelectSpecialPhrases.Controls.Add(this.m_btnFind);
            this.m_grpSelectSpecialPhrases.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_grpSelectSpecialPhrases.Location = new System.Drawing.Point(12, 12);
            this.m_grpSelectSpecialPhrases.Name = "m_grpSelectSpecialPhrases";
            this.m_grpSelectSpecialPhrases.Size = new System.Drawing.Size(400, 89);
            this.m_grpSelectSpecialPhrases.TabIndex = 0;
            this.m_grpSelectSpecialPhrases.TabStop = false;
            this.m_grpSelectSpecialPhrases.Text = "Select Special Phrases to show in List Box";
            // 
            // m_cb_SpecialPhrases
            // 
            this.m_cb_SpecialPhrases.FormattingEnabled = true;
            this.m_cb_SpecialPhrases.Items.AddRange(new object[] {
            "Todo Marked Phrases",
            "Heading",
            "Silence",
            "All Pages",
            "Front Pages",
            "Normal Pages",
            "Special Pages",
            "Anchor"});
            this.m_cb_SpecialPhrases.Location = new System.Drawing.Point(6, 42);
            this.m_cb_SpecialPhrases.Name = "m_cb_SpecialPhrases";
            this.m_cb_SpecialPhrases.Size = new System.Drawing.Size(264, 24);
            this.m_cb_SpecialPhrases.TabIndex = 1;
            this.m_cb_SpecialPhrases.Text = "Click to Select..";
            // 
            // m_btnFind
            // 
            this.m_btnFind.AccessibleName = "Find";
            this.m_btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnFind.Location = new System.Drawing.Point(288, 39);
            this.m_btnFind.Margin = new System.Windows.Forms.Padding(4);
            this.m_btnFind.Name = "m_btnFind";
            this.m_btnFind.Size = new System.Drawing.Size(100, 28);
            this.m_btnFind.TabIndex = 0;
            this.m_btnFind.Text = "&Find";
            this.m_btnFind.UseVisualStyleBackColor = true;
            this.m_btnFind.Click += new System.EventHandler(this.m_btnFind_Click);
            // 
            // m_lbSpecialPhrasesList
            // 
            this.m_lbSpecialPhrasesList.FormattingEnabled = true;
            this.m_lbSpecialPhrasesList.HorizontalScrollbar = true;
            this.m_lbSpecialPhrasesList.Location = new System.Drawing.Point(12, 122);
            this.m_lbSpecialPhrasesList.Name = "m_lbSpecialPhrasesList";
            this.m_lbSpecialPhrasesList.Size = new System.Drawing.Size(400, 238);
            this.m_lbSpecialPhrasesList.TabIndex = 1;
            this.m_lbSpecialPhrasesList.SelectedIndexChanged += new System.EventHandler(this.m_lbSpecialPhrasesList_SelectedIndexChanged);
            // 
            // m_btnOK
            // 
            this.m_btnOK.AccessibleName = "OK";
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnOK.Location = new System.Drawing.Point(80, 372);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(100, 28);
            this.m_btnOK.TabIndex = 2;
            this.m_btnOK.Text = "&OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.AccessibleName = "Cancel";
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCancel.Location = new System.Drawing.Point(205, 372);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(100, 28);
            this.m_btnCancel.TabIndex = 3;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // SpecialPhraseList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 410);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_lbSpecialPhrasesList);
            this.Controls.Add(this.m_grpSelectSpecialPhrases);
            this.MaximizeBox = false;
            this.Name = "SpecialPhraseList";
            this.Text = "SpecialPhraseList";
            this.m_grpSelectSpecialPhrases.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox m_grpSelectSpecialPhrases;
        private System.Windows.Forms.ComboBox m_cb_SpecialPhrases;
        private System.Windows.Forms.Button m_btnFind;
        private System.Windows.Forms.ListBox m_lbSpecialPhrasesList;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
    }
}