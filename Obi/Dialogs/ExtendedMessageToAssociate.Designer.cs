namespace Obi.Dialogs
{
    partial class ExtendedMessageToAssociate
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
            this.m_btn_Yes = new System.Windows.Forms.Button();
            this.m_btn_YesToAll = new System.Windows.Forms.Button();
            this.m_btn_No = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_btn_Yes
            // 
            this.m_btn_Yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_btn_Yes.Location = new System.Drawing.Point(12, 77);
            this.m_btn_Yes.Name = "m_btn_Yes";
            this.m_btn_Yes.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Yes.TabIndex = 0;
            this.m_btn_Yes.Text = "Yes";
            this.m_btn_Yes.UseVisualStyleBackColor = true;
            this.m_btn_Yes.Click += new System.EventHandler(this.m_btn_Yes_Click);
            // 
            // m_btn_YesToAll
            // 
            this.m_btn_YesToAll.Location = new System.Drawing.Point(124, 77);
            this.m_btn_YesToAll.Name = "m_btn_YesToAll";
            this.m_btn_YesToAll.Size = new System.Drawing.Size(75, 23);
            this.m_btn_YesToAll.TabIndex = 1;
            this.m_btn_YesToAll.Text = "Yes to all";
            this.m_btn_YesToAll.UseVisualStyleBackColor = true;
            this.m_btn_YesToAll.Click += new System.EventHandler(this.m_btn_YesToAll_Click);
            // 
            // m_btn_No
            // 
            this.m_btn_No.Location = new System.Drawing.Point(234, 77);
            this.m_btn_No.Name = "m_btn_No";
            this.m_btn_No.Size = new System.Drawing.Size(75, 23);
            this.m_btn_No.TabIndex = 2;
            this.m_btn_No.Text = "Abort";
            this.m_btn_No.UseVisualStyleBackColor = true;
            this.m_btn_No.Click += new System.EventHandler(this.m_btn_No_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(298, 40);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "The special node chunk already contain custom phrase. Do you want to convert them" +
                " all into current custom role?";
            // 
            // ExtendedMessageToAssociate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 117);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_btn_No);
            this.Controls.Add(this.m_btn_YesToAll);
            this.Controls.Add(this.m_btn_Yes);
            this.Name = "ExtendedMessageToAssociate";
            this.Text = "Extended message box";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btn_Yes;
        private System.Windows.Forms.Button m_btn_YesToAll;
        private System.Windows.Forms.Button m_btn_No;
        private System.Windows.Forms.TextBox textBox1;
    }
}