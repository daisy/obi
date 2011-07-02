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
        protected bool m_GoToPage;

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


      public SetPageNumber() { InitializeComponent(); }

        public virtual PageNumber Number
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
                    if (m_GoToPage && number == 0) return null;
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

        protected void mOKButton_Click ( object sender, EventArgs e )
            {
            
                int num = EmptyNode.SafeParsePageNumber ( mNumberBox.Text );
                int numberOfPages = EmptyNode.SafeParsePageNumber(mNumberOfPagesBox.Text);
            // apply a check if dialog is being used for go to page
                if (m_GoToPage && num == 0 && mPageKindComboBox.SelectedIndex < 2)
                    {
                    MessageBox.Show ( Localizer.Message ( "PageNumber_ReEnterValidNumber") );
                    return;
                    }
                    if ((num == 0 && mPageKindComboBox.SelectedIndex < 2) || numberOfPages < 1)
                    //|| (num > 0 && mPageKindComboBox.SelectedIndex == 2)) // message should not appear for assigning special pages.
                    {
                       if (MessageBox.Show(Localizer.Message("PageDialog_InvalidInput"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                          return;
                    }
                

            DialogResult = DialogResult.OK;
            Close ();
                        }


    }
}