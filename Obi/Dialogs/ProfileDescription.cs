using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ProfileDescription : Form
    {
        private int m_ProfileSelectedIndex;
        private bool m_flagFontChange = false; //@fontconfig
        private Settings mSettings;  //@fontconfig
        public ProfileDescription(Settings settings)
        {
            InitializeComponent();
            mSettings = settings; //@fontconfig
            this.m_ProfileDescription_WebBrowser.Height = this.m_ProfileDescription_WebBrowser.Height - (this.m_btnClose.Height + (this.m_btnClose.Height / 2));

            m_ProfileDescription_WebBrowser.Url = new System.Uri(System.IO.Path.Combine(
    System.IO.Path.GetDirectoryName(GetType().Assembly.Location),
    Localizer.Message("ProfileDesc_file_name")));
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
                m_flagFontChange = true;
            }

        }

        private void m_ProfileDescription_WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (m_flagFontChange)
            { //@fontconfig
                m_ProfileDescription_WebBrowser.Document.ExecCommand("SelectAll", false, "null");
                m_ProfileDescription_WebBrowser.Document.ExecCommand("FontName", false, mSettings.ObiFont);
                m_ProfileDescription_WebBrowser.Document.ExecCommand("Unselect", false, "null");
            }
            if (m_ProfileSelectedIndex == 0)
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("Basic").InnerHtml;
            }
            else  if (m_ProfileSelectedIndex == 1)
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("Intermidate").InnerHtml;
            }
            else if (m_ProfileSelectedIndex == 2)
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("Advance").InnerHtml;
            }
            
            else if (m_ProfileSelectedIndex == 3)
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("Profile2").InnerHtml;
            }
            else if (m_ProfileSelectedIndex == 4)
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("VA-Editing").InnerHtml;
            }
            else if (m_ProfileSelectedIndex == 5)
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("VA-Insert").InnerHtml;
            }
            else if (m_ProfileSelectedIndex == 6)
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("VA-Overwrite").InnerHtml;
            }
            else
            {
                m_ProfileDescription_WebBrowser.DocumentText = m_ProfileDescription_WebBrowser.Document.GetElementById("Custom").InnerHtml;
            }
        }

        public int ProfileSelected
        {
            set
            {
                m_ProfileSelectedIndex = value;
            }
        }


    }
}