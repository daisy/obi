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

        private void m_btnOpen_Click ( object sender, EventArgs e )
            {
            if (openFileDialog1.ShowDialog () == DialogResult.OK)
                {
                DTBDowngrader.Daisy3To202 d = new Daisy3To202 ( openFileDialog1.FileName, System.AppDomain.CurrentDomain.BaseDirectory );
                }
            }
        }
    }