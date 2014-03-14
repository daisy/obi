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
        public ProfileDescription()
        {
            InitializeComponent();

            m_ProfileDescription_WebBrowser.Url = new System.Uri(System.IO.Path.Combine(
    System.IO.Path.GetDirectoryName(GetType().Assembly.Location),
    Localizer.Message("ProfileDesc_file_name")));

        }

        private void m_ProfileDescription_WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
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