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
        private int mInitialNumber;

        public SetPageNumber(int number, bool renumber): this()
        {
            mInitialNumber = number;
            mNumberBox.Text = number.ToString();
            mRenumber.Checked = renumber;
        }

        public SetPageNumber() { InitializeComponent(); }

        public int Number
        {
            get
            {
                int number = EmptyNode.SafeParsePageNumber(mNumberBox.Text);
                return number > 0 ? number : mInitialNumber;
            } 
        }

        public bool Renumber { get { return mRenumber.Checked; } }
    }
}