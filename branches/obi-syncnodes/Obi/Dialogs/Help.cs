using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class Help : Form
    {
        public WebBrowser WebBrowser
        {
            get { return mWebBrowser; }
        }

        public Help()
        {
            InitializeComponent();
        }
    }
}