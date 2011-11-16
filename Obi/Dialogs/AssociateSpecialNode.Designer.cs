namespace Obi.Dialogs
{
    partial class AssociateSpecialNode
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
            this.m_lb_ListOfSpecialNodes = new System.Windows.Forms.ListBox();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_lb_ListOfSpecialNodes
            // 
            this.m_lb_ListOfSpecialNodes.FormattingEnabled = true;
            this.m_lb_ListOfSpecialNodes.Location = new System.Drawing.Point(12, 86);
            this.m_lb_ListOfSpecialNodes.Name = "m_lb_ListOfSpecialNodes";
            this.m_lb_ListOfSpecialNodes.Size = new System.Drawing.Size(268, 108);
            this.m_lb_ListOfSpecialNodes.TabIndex = 0;
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.Location = new System.Drawing.Point(36, 229);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 1;
            this.m_btn_OK.Text = "OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.Location = new System.Drawing.Point(163, 228);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 2;
            this.m_btn_Cancel.Text = "Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(49, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 3;
            // 
            // AssociateSpecialNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_lb_ListOfSpecialNodes);
            this.Name = "AssociateSpecialNode";
            this.Text = "AssociateSpecialNode";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox m_lb_ListOfSpecialNodes;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.TextBox textBox1;
    }
}