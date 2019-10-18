namespace Obi.Dialogs
{
    partial class ProfileDescription
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileDescription));
            this.m_ProfileDescription_WebBrowser = new System.Windows.Forms.WebBrowser();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_ProfileDescription_WebBrowser
            // 
            resources.ApplyResources(this.m_ProfileDescription_WebBrowser, "m_ProfileDescription_WebBrowser");
            this.m_ProfileDescription_WebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.m_ProfileDescription_WebBrowser.Name = "m_ProfileDescription_WebBrowser";
            this.m_ProfileDescription_WebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.m_ProfileDescription_WebBrowser_DocumentCompleted);
            // 
            // m_btnClose
            // 
            resources.ApplyResources(this.m_btnClose, "m_btnClose");
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.UseVisualStyleBackColor = true;
            // 
            // ProfileDescription
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnClose;
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_ProfileDescription_WebBrowser);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProfileDescription";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser m_ProfileDescription_WebBrowser;
        private System.Windows.Forms.Button m_btnClose;
    }
}