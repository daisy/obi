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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtendedMessageToAssociate));
            this.m_btn_Yes = new System.Windows.Forms.Button();
            this.m_btn_YesToAll = new System.Windows.Forms.Button();
            this.m_btn_No = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // m_btn_Yes
            // 
            this.m_btn_Yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            resources.ApplyResources(this.m_btn_Yes, "m_btn_Yes");
            this.m_btn_Yes.Name = "m_btn_Yes";
            this.m_btn_Yes.UseVisualStyleBackColor = true;
            this.m_btn_Yes.Click += new System.EventHandler(this.m_btn_Yes_Click);
            // 
            // m_btn_YesToAll
            // 
            resources.ApplyResources(this.m_btn_YesToAll, "m_btn_YesToAll");
            this.m_btn_YesToAll.Name = "m_btn_YesToAll";
            this.m_btn_YesToAll.UseVisualStyleBackColor = true;
            this.m_btn_YesToAll.Click += new System.EventHandler(this.m_btn_YesToAll_Click);
            // 
            // m_btn_No
            // 
            resources.ApplyResources(this.m_btn_No, "m_btn_No");
            this.m_btn_No.Name = "m_btn_No";
            this.m_btn_No.UseVisualStyleBackColor = true;
            this.m_btn_No.Click += new System.EventHandler(this.m_btn_No_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // ExtendedMessageToAssociate
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_btn_No);
            this.Controls.Add(this.m_btn_YesToAll);
            this.Controls.Add(this.m_btn_Yes);
            this.MaximizeBox = false;
            this.Name = "ExtendedMessageToAssociate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btn_Yes;
        private System.Windows.Forms.Button m_btn_YesToAll;
        private System.Windows.Forms.Button m_btn_No;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}