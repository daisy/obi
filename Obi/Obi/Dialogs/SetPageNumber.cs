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
        private ObiNode m_SelectedNode;

        public SetPageNumber(PageNumber number, bool renumber, bool canSetNumberOfPages,ObiNode selectedNode)
            : this(number, renumber, canSetNumberOfPages)
        {
            m_SelectedNode = selectedNode;
        }

            public SetPageNumber(PageNumber number, bool renumber, bool canSetNumberOfPages): this()
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

        public bool AutoFillPages
        {
            get
            {
                return m_chkAutoFillPages.Checked;
            }
        }
        public bool AutoFillPagesEnable
        {
            set
            {
                m_chkAutoFillPages.Enabled = value;
            }
        }

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

        private void m_chkAutoFillPages_CheckedChanged(object sender, EventArgs e)
        {
            if (m_SelectedNode != null)
            {
                
                EmptyNode startingPage = null;
                EmptyNode lastPage = null;
                // find previous page node
                for (ObiNode n = m_SelectedNode;
                    n != null ;
                    n = n.PrecedingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                    {
                        startingPage = (EmptyNode) n;
                        break;
                    }
                }

                // Find the next page node
                for (ObiNode n = m_SelectedNode.FollowingNode;
                    n != null;
                    n = n.FollowingNode)
                {
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                    {
                        lastPage = (EmptyNode)n;
                        break;
                    }
                }

                if (startingPage == null)
                {
                    if (m_chkAutoFillPages.Checked)
                    {
                        MessageBox.Show("No preceeding page found. Please key in the values.");
                        m_chkAutoFillPages.Checked = false;
                    }
                    return;
                }
                if (lastPage == null)
                {
                    if (m_chkAutoFillPages.Checked)
                    {
                        MessageBox.Show("Unable to find next page. Please key in the values.");
                        m_chkAutoFillPages.Checked = false;
                    }
                    return;
                }

                if (startingPage.PageNumber.Number >= lastPage.PageNumber.Number
                    || startingPage.PageNumber.Kind != lastPage.PageNumber.Kind)
                {
                    if (m_chkAutoFillPages.Checked)
                    {
                        MessageBox.Show("Can not proceed. The preceeding page number and the next page number are out of order");
                        m_chkAutoFillPages.Checked = false;
                    }
                    return ;
                }

                if (startingPage.PageNumber.Number == lastPage.PageNumber.Number - 1)
                {
                    if (m_chkAutoFillPages.Checked)
                    {
                        MessageBox.Show("Cannot proceed. Pages are already consecutive.");
                        m_chkAutoFillPages.Checked = false;
                    }
                        return;
                }
                                // fill in the page count
                int pageCount = lastPage.PageNumber.Number - startingPage.PageNumber.Number - 1;
                mNumberOfPagesBox.Text = pageCount.ToString();
                if (m_chkAutoFillPages.Checked)
                {
                    mRenumber.Checked = false;
                }
            }
        }


    }
}