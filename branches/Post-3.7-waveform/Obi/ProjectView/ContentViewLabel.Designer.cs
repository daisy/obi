namespace Obi.ProjectView
{
    partial class ContentViewLabel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContentViewLabel));
            this.m_lblStaticLabel = new System.Windows.Forms.Label();
            this.m_lblSectionName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_lblStaticLabel
            // 
            resources.ApplyResources(this.m_lblStaticLabel, "m_lblStaticLabel");
            this.m_lblStaticLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.m_lblStaticLabel.Name = "m_lblStaticLabel";
            // 
            // m_lblSectionName
            // 
            resources.ApplyResources(this.m_lblSectionName, "m_lblSectionName");
            this.m_lblSectionName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.m_lblSectionName.Name = "m_lblSectionName";
            // 
            // ContentViewLabel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.m_lblSectionName);
            this.Controls.Add(this.m_lblStaticLabel);
            this.Name = "ContentViewLabel";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ContentViewLabel_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblStaticLabel;
        private System.Windows.Forms.Label m_lblSectionName;
    }
}
