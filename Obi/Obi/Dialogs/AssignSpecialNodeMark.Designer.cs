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
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_cmbBoxSpecialNode
            // 
            this.m_cmbBoxSpecialNode.FormattingEnabled = true;
            this.m_cmbBoxSpecialNode.Items.AddRange(new object[] {
            "Annotation",
            "End note",
            "Footnote",
            "Producer note",
            "Sidebar"});
            this.m_cmbBoxSpecialNode.Location = new System.Drawing.Point(53, 28);
            this.m_cmbBoxSpecialNode.Name = "m_cmbBoxSpecialNode";
            this.m_cmbBoxSpecialNode.Size = new System.Drawing.Size(181, 21);
            this.m_cmbBoxSpecialNode.TabIndex = 0;
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_OK.Location = new System.Drawing.Point(53, 73);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 1;
            this.m_btn_OK.Text = "OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(159, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // AssignSpecialNodeMark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 115);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_cmbBoxSpecialNode);
            this.Name = "AssignSpecialNodeMark";
            this.Text = "Assign note";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox m_cmbBoxSpecialNode;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button button1;
    }
}