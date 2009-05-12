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
        private bool m_GoToPage;

        public SetPageNumber(PageNumber number, bool renumber, bool canSetNumberOfPages): this()
        {
            mInitialNumber = number;
            mNumberOfPages = 1;
            mNumberBox.Text = number.ArabicNumberOrLabel;
            mPageKindComboBox.SelectedIndex = number.Kind == PageKind.Front ? 0 : number.Kind == PageKind.Normal ? 1 : 2;
            mRenumber.Checked = renumber;
            mNumberOfPagesBox.Text = mNumberOfPages.ToString();
            mNumberOfPagesBox.Enabled = canSetNumberOfPages;
            m_GoToPage = false;
        }

        public SetPageNumber (bool goToPage ): this  () 
            {
            if (goToPage)
                {
                m_GoToPage = goToPage;
                this.Text = "Go to page";
                label2.Visible = false;
                label1.Location = new Point(40,22);
                label3.Location = new Point(60, 64);
                this.mNumberBox.Location = new Point(150,20);
                this.mPageKindComboBox.Location = new Point(150,62);
                this.Size = new Size(320,175);
                mNumberOfPagesBox.Visible = false;
                mRenumber.Visible = false;
                mPageKindComboBox.SelectedIndex = 1;
                }
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

        private void mOKButton_Click ( object sender, EventArgs e )
            {
            
                int num = EmptyNode.SafeParsePageNumber ( mNumberBox.Text );
                if ((num == 0 && mPageKindComboBox.SelectedIndex < 2)
                    || (num > 0 && mPageKindComboBox.SelectedIndex == 2))
                    {
                    if (MessageBox.Show ( Localizer.Message ( "PageDialog_InvalidInput" ), Localizer.Message ( "Caption_Error" ), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) == DialogResult.No)
                        return;
                    }
                

            DialogResult = DialogResult.OK;
            Close ();
                        }


    }
}