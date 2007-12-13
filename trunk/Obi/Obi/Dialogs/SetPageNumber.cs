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
        private int mNumberOfPages;

        public SetPageNumber(int number, bool renumber, bool canSetNumberOfPages): this()
        {
            mInitialNumber = number;
            mNumberOfPages = 1;
            mNumberBox.Text = number.ToString();
            mRenumber.Checked = renumber;
            mNumberOfPagesBox.Text = mNumberOfPages.ToString();
            mNumberOfPagesBox.Enabled = canSetNumberOfPages;
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

        public int NumberOfPages
        {
            get
            {
                int number = EmptyNode.SafeParsePageNumber(mNumberOfPagesBox.Text);
                return number > 0 ? number : mNumberOfPages;
            }
        }

        public bool Renumber { get { return mRenumber.Checked; } }
    }
}