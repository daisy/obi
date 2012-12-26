using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DTBDowngrader;

namespace DTBDowngraderUI
    {
    public partial class Form1 : Form
        {
        public Form1 ()
            {
            InitializeComponent ();
            }
        string m_inputPath;
        private void m_btnOpen_Click ( object sender, EventArgs e )
            {
            if (openFileDialog1.ShowDialog () == DialogResult.OK)
                {
                m_inputPath = openFileDialog1.FileName;
                
                }
            }

        private void m_btnSaveDir_Click ( object sender, EventArgs e )
            {
            //folderBrowserDialog1.SelectedPath = "C:\\Downloads\\mp3Export1" ;
            if (folderBrowserDialog1.ShowDialog () == DialogResult.OK)
                {
                DTBDowngrader.Daisy3To202 d = new Daisy3To202 ( m_inputPath, folderBrowserDialog1.SelectedPath);
                }
            }
        }
    }