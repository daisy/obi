using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Obi.Dialogs
{
    public partial class DownloadFile : Form
    {
        public DownloadFile()
        {
            InitializeComponent();
        }

 

        private void m_linkToDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void m_btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void m_btnDownload_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.daisy.org/obi/download");            
        }

      
        private void m_btnFolder_Click(object sender, EventArgs e)
        {
            string ffmpegWorkingDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Process.Start(ffmpegWorkingDir);
        }
    }
}