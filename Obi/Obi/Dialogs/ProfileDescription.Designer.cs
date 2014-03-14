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
            this.m_ProfileDescription_WebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // m_ProfileDescription_WebBrowser
            // 
            this.m_ProfileDescription_WebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_ProfileDescription_WebBrowser.Location = new System.Drawing.Point(0, 0);
            this.m_ProfileDescription_WebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.m_ProfileDescription_WebBrowser.Name = "m_ProfileDescription_WebBrowser";
            this.m_ProfileDescription_WebBrowser.Size = new System.Drawing.Size(606, 517);
            this.m_ProfileDescription_WebBrowser.TabIndex = 0;
           // this.m_ProfileDescription_WebBrowser.Url = new System.Uri(global::Obi.messages_ta.phrase_extra_Plain, System.UriKind.Relative);
            this.m_ProfileDescription_WebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.m_ProfileDescription_WebBrowser_DocumentCompleted);
            // 
            // ProfileDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 517);
            this.Controls.Add(this.m_ProfileDescription_WebBrowser);
            this.Name = "ProfileDescription";
            this.Text = "Phrase Description";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser m_ProfileDescription_WebBrowser;
    }
}