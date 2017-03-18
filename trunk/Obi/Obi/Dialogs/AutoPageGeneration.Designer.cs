namespace Obi.Dialogs
{
    partial class AutoPageGeneration
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
            this.label1 = new System.Windows.Forms.Label();
            this.m_txtGapsInPages = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_cbStartingSectionIndex = new System.Windows.Forms.ComboBox();
            this.m_rbGenerateTTS = new System.Windows.Forms.RadioButton();
            this.m_rbKeepEmptyPages = new System.Windows.Forms.RadioButton();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gaps in pages (mins):";
            // 
            // m_txtGapsInPages
            // 
            this.m_txtGapsInPages.Location = new System.Drawing.Point(140, 35);
            this.m_txtGapsInPages.Name = "m_txtGapsInPages";
            this.m_txtGapsInPages.Size = new System.Drawing.Size(100, 20);
            this.m_txtGapsInPages.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Starting section index:";
            // 
            // m_cbStartingSectionIndex
            // 
            this.m_cbStartingSectionIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cbStartingSectionIndex.FormattingEnabled = true;
            this.m_cbStartingSectionIndex.Location = new System.Drawing.Point(140, 80);
            this.m_cbStartingSectionIndex.Name = "m_cbStartingSectionIndex";
            this.m_cbStartingSectionIndex.Size = new System.Drawing.Size(121, 21);
            this.m_cbStartingSectionIndex.TabIndex = 3;
            // 
            // m_rbGenerateTTS
            // 
            this.m_rbGenerateTTS.AutoSize = true;
            this.m_rbGenerateTTS.Location = new System.Drawing.Point(15, 141);
            this.m_rbGenerateTTS.Name = "m_rbGenerateTTS";
            this.m_rbGenerateTTS.Size = new System.Drawing.Size(212, 17);
            this.m_rbGenerateTTS.TabIndex = 4;
            this.m_rbGenerateTTS.Text = "Generate synthetic speech for page no.";
            this.m_rbGenerateTTS.UseVisualStyleBackColor = true;
            // 
            // m_rbKeepEmptyPages
            // 
            this.m_rbKeepEmptyPages.AutoSize = true;
            this.m_rbKeepEmptyPages.Checked = true;
            this.m_rbKeepEmptyPages.Location = new System.Drawing.Point(15, 183);
            this.m_rbKeepEmptyPages.Name = "m_rbKeepEmptyPages";
            this.m_rbKeepEmptyPages.Size = new System.Drawing.Size(113, 17);
            this.m_rbKeepEmptyPages.TabIndex = 5;
            this.m_rbKeepEmptyPages.TabStop = true;
            this.m_rbKeepEmptyPages.Text = "Keep empty pages";
            this.m_rbKeepEmptyPages.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.Location = new System.Drawing.Point(45, 226);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 6;
            this.m_btnOk.Text = "OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(152, 226);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 7;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // AutoPageGeneration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_rbKeepEmptyPages);
            this.Controls.Add(this.m_rbGenerateTTS);
            this.Controls.Add(this.m_cbStartingSectionIndex);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_txtGapsInPages);
            this.Controls.Add(this.label1);
            this.Name = "AutoPageGeneration";
            this.Text = "AutoPageGeneration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_txtGapsInPages;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox m_cbStartingSectionIndex;
        private System.Windows.Forms.RadioButton m_rbGenerateTTS;
        private System.Windows.Forms.RadioButton m_rbKeepEmptyPages;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
    }
}