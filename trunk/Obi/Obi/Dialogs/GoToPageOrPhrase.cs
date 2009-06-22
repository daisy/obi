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
            base.label1.Location = new Point(50, 75);
            base.label3.Location = new Point(50, 110);
            base.mNumberBox.Location = new Point(150, 75);
            base.mPageKindComboBox.Location = new Point(150, 108);
            base.Size = new Size(370, 210);
            base.mNumberOfPagesBox.Visible = false;
            base.mRenumber.Visible = false;
            base.mPageKindComboBox.SelectedIndex = 1;
            m_btnOk.Location = new Point(70, 140);
            base.mCancelButton.Location = new Point(200, 140);
            m_radPhrase.Location = new Point(50, 20);
            m_radPage.Location = new Point(180, 20);
            m_radPhrase.TabIndex = 1;
            m_radPage.TabIndex = 1;
            base.mOKButton.Visible = false;
            base.label1.TabIndex = 2;
            base.mNumberBox.TabIndex = 3;
            base.mPageKindComboBox.TabIndex = 4;
            m_btnOk.TabIndex = 5;
            base.mCancelButton.TabIndex = 6;

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