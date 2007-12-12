using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class SetPageNumber : Form
    {
        public SetPageNumber(int number): this()
        {
            mNumberBox.Text = number.ToString();
        }

        public SetPageNumber() { InitializeComponent(); }

        public int Number { get { return Int32.Parse(mNumberBox.Text); } }
    }
}