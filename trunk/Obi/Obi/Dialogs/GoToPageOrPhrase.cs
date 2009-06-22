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
            base.label1.Location = new Point ( 40, 22 );
            base.label3.Location = new Point ( 60, 64 );
            base.mNumberBox.Location = new Point ( 150, 20 );
            base.mPageKindComboBox.Location = new Point ( 150, 62 );
            base.Size = new Size ( 320, 175 );
            base.mNumberOfPagesBox.Visible = false;
            base.mRenumber.Visible = false;
            base.mPageKindComboBox.SelectedIndex = 1;
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


        }
    }