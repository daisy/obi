using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
    {
    public partial class GoToPageOrPhrase : SetPageNumber 
        {
        
        public GoToPageOrPhrase ():base ()
            {
            InitializeComponent ();

            m_radPage.Checked = true;
            base.m_GoToPage = true;
            base.Text = "Go to page";
            base.label2.Visible = false;
            base.label1.Location = new Point(50, 56);
            base.label3.Location = new Point(50, 94);
            base.mNumberBox.Location = new Point(150, 56);
            base.mPageKindComboBox.Location = new Point(150, 92);
            base.Size = new Size(370, 200);
            base.mNumberOfPagesBox.Visible = false;
            base.mRenumber.Visible = false;
            base.mPageKindComboBox.SelectedIndex = 1;
            m_btnOk.Location = new Point(70, 130);
            base.mCancelButton.Location = new Point(200, 130);
            m_radPhrase.Location = new Point(50, 20);
            m_radPage.Location = new Point(180, 20);
            base.mOKButton.Visible = false;
            }

        public override PageNumber Number
            {
            get
                {
                if (m_radPage.Checked)
                    {
                    return base.Number;
                    }
                else
                    {
                    return null;
                    }
                }
            }


        public int? PhraseIndex
            {
            get
                {
                if (m_radPhrase.Checked)
                    {
                    int phraseIndex = 0 ;
                    int.TryParse ( base.mNumberBox.Text , out phraseIndex);
                    if (phraseIndex > 1)
                        return phraseIndex;
                    else
                        return null;
                    }
                else
                    {
                    return null;
                    }
                }
            }

        private void m_btnOk_Click ( object sender, EventArgs e )
            {
            base.mOKButton_Click ( sender, e );
            }

        private void m_radPage_CheckedChanged(object sender, EventArgs e)
        {
            base.label1.Text = "Page Number";
            base.label3.Visible = true;
            base.mPageKindComboBox.Visible = true;
        }

        private void m_radPhrase_CheckedChanged(object sender, EventArgs e)
        {
            base.label1.Text = "Phrase Index";
            base.label3.Visible = false;
            base.mPageKindComboBox.Visible = false;
        }

        }
    }