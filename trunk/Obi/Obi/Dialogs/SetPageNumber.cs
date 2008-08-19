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
        private PageNumber mInitialNumber;
        private int mNumberOfPages;

        public SetPageNumber(PageNumber number, bool renumber, bool canSetNumberOfPages): this()
        {
            mInitialNumber = number;
            mNumberOfPages = 1;
            mNumberBox.Text = number.ArabicNumberOrLabel;
            mPageKindComboBox.SelectedIndex = number.Kind == PageKind.Front ? 0 : number.Kind == PageKind.Normal ? 1 : 2;
            mRenumber.Checked = renumber;
            mNumberOfPagesBox.Text = mNumberOfPages.ToString();
            mNumberOfPagesBox.Enabled = canSetNumberOfPages;
        }

        public SetPageNumber() { InitializeComponent(); }

        public PageNumber Number
        {
            get
            {
                if (mPageKindComboBox.SelectedIndex == 2)
                {
                    // Special page
                    return new PageNumber(mNumberBox.Text);
                }
                else
                {
                    int number = EmptyNode.SafeParsePageNumber(mNumberBox.Text);
                    return number > 0 ?
                        new PageNumber(number, mPageKindComboBox.SelectedIndex == 1 ? PageKind.Normal : PageKind.Front) :
                        mInitialNumber.Clone();
                }
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

        public bool Renumber { get { return mRenumber.Checked && mRenumber.Enabled; } }

        private void mPageKindComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mRenumber.Enabled = mPageKindComboBox.SelectedIndex != 2;
        }
    }
}