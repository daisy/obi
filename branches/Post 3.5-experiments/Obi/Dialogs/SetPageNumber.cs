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
        private bool m_IsRenumber;

        public SetPageNumber(PageNumber number, bool renumber, bool canSetNumberOfPages,Settings settings): this()
        {
            mInitialNumber = number;
            mNumberOfPages = 1;
            if (mInitialNumber != null)
            {
                mNumberBox.Text = number.ArabicNumberOrLabel;
                mPageKindComboBox.SelectedIndex = number.Kind == PageKind.Front ? 0 : number.Kind == PageKind.Normal ? 1 : 2;
            }
            else
            {
                mNumberBox.Text = "1";
                mPageKindComboBox.SelectedIndex = 1;
            }
            
            mRenumber.Checked = renumber;
            mNumberOfPagesBox.Text = mNumberOfPages.ToString();
            mNumberOfPagesBox.Enabled = canSetNumberOfPages;
            m_GoToPage = false;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Phrases/Assigning a page role.htm");

            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
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

        public bool IsRenumberChecked
        {
            get { return m_IsRenumber; }
            set
            {
                m_IsRenumber = value; 
                if (m_IsRenumber)
                {
                    mPageKindComboBox.Items.RemoveAt(2);
                    mRenumber.Checked = true;
                    if(mInitialNumber != null)
                    mNumberBox.Text = (mInitialNumber.Number + 1).ToString();
                }
            }
        }

        public bool EnableRenumberCheckBox
        {
            get { return mRenumber.Enabled; }
            set { mRenumber.Enabled = value; }
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
                if (string.IsNullOrEmpty(mNumberBox.Text.Trim())) return;
                int num = EmptyNode.SafeParsePageNumber ( mNumberBox.Text );
                int numberOfPages = EmptyNode.SafeParsePageNumber(mNumberOfPagesBox.Text);
            // apply a check if dialog is being used for go to page
                if (m_GoToPage && num == 0 && mPageKindComboBox.SelectedIndex < 2)
                    {
                    MessageBox.Show ( Localizer.Message ( "PageNumber_ReEnterValidNumber") );
                    return;
                    }
                    if ((num == 0 && mPageKindComboBox.SelectedIndex < 2) || (!m_GoToPage && numberOfPages < 1))
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