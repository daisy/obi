namespace Obi.Dialogs
{
    partial class AddAzureKey
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAzureKey));
            m_AddBtn = new System.Windows.Forms.Button();
            m_RegionLbl = new System.Windows.Forms.Label();
            m_KeyLbl = new System.Windows.Forms.Label();
            m_ResgionTB = new System.Windows.Forms.TextBox();
            m_KeyTB = new System.Windows.Forms.TextBox();
            m_CancelBtn = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // m_AddBtn
            // 
            resources.ApplyResources(m_AddBtn, "m_AddBtn");
            m_AddBtn.Name = "m_AddBtn";
            m_AddBtn.UseVisualStyleBackColor = true;
            m_AddBtn.Click += m_AddBtn_Click;
            // 
            // m_RegionLbl
            // 
            resources.ApplyResources(m_RegionLbl, "m_RegionLbl");
            m_RegionLbl.Name = "m_RegionLbl";
            // 
            // m_KeyLbl
            // 
            resources.ApplyResources(m_KeyLbl, "m_KeyLbl");
            m_KeyLbl.Name = "m_KeyLbl";
            // 
            // m_ResgionTB
            // 
            resources.ApplyResources(m_ResgionTB, "m_ResgionTB");
            m_ResgionTB.Name = "m_ResgionTB";
            // 
            // m_KeyTB
            // 
            resources.ApplyResources(m_KeyTB, "m_KeyTB");
            m_KeyTB.Name = "m_KeyTB";
            // 
            // m_CancelBtn
            // 
            m_CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(m_CancelBtn, "m_CancelBtn");
            m_CancelBtn.Name = "m_CancelBtn";
            m_CancelBtn.UseVisualStyleBackColor = true;
            // 
            // AddAzureKey
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(m_CancelBtn);
            Controls.Add(m_AddBtn);
            Controls.Add(m_RegionLbl);
            Controls.Add(m_KeyLbl);
            Controls.Add(m_ResgionTB);
            Controls.Add(m_KeyTB);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddAzureKey";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_AddBtn;
        private System.Windows.Forms.Label m_RegionLbl;
        private System.Windows.Forms.Label m_KeyLbl;
        private System.Windows.Forms.TextBox m_ResgionTB;
        private System.Windows.Forms.TextBox m_KeyTB;
        private System.Windows.Forms.Button m_CancelBtn;
    }
}