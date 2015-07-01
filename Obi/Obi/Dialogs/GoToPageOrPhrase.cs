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
        private int m_PhraseCount ;
        
       
        public GoToPageOrPhrase ( int sectionPhraseCount)
            : base ()
            {
            InitializeComponent ();

            m_radPage.Checked = true;
            base.m_GoToPage = true; // To do: remove all go to page dependency from parent set page form
            base.Text = Localizer.Message ( "GoToPageOrPhrase_Title" );
            base.label2.Visible = false;
            //base.label1.Location = new Point ( 50, 96 ); //This Label is used for displaying Phrase Index
           // base.label3.Location = new Point ( 50, 125 );
            //base.mNumberBox.Location = new Point ( 150, 93 );
            //mPhraseIndexComboBox.Location = new Point(150, 93);
            //base.mPageKindComboBox.Location = new Point ( 150, 120 );
            base.Size = new Size ( 370, 210 );
            base.mNumberOfPagesBox.Visible = false;
            base.mRenumber.Visible = false;
            base.m_chkAutoFillPages.Visible = false;
            base.mPageKindComboBox.SelectedIndex = 1;
           // m_btnOk.Location = new Point ( 70, 150 );
            //base.mCancelButton.Location = new Point ( 200, 150 );
           // m_radPhrase.Location = new Point ( 10, 18 );
            //m_radPage.Location = new Point ( 10, 38 );
            //m_radTime.Location = new Point( 10, 58);
            m_radPhrase.TabIndex = 1;
            m_radPage.TabIndex = 1;
            //m_cb_TimeInPhraseOrSection.Location = new Point(150, 91);
            //m_txtBox_TimeInSeconds.Location = new Point(150, 120);
            //m_lbl_Seconds.Location = new Point(255, 123);
            base.mOKButton.Visible = false;
            base.label1.TabIndex = 2;
            base.mNumberBox.TabIndex = 3;
            base.mPageKindComboBox.TabIndex = 4;
            m_btnOk.TabIndex = 5;
            base.mCancelButton.TabIndex = 6;
            m_cb_TimeInPhraseOrSection.SelectedIndex = 0;
            m_PhraseCount = sectionPhraseCount;
            for (int i = 1; i <= (m_PhraseCount/125); i++)
            {
            mPhraseIndexComboBox.Items.AddRange(new object [] {(125 * i) + 1});
            }
            mPhraseIndexComboBox.Items.AddRange(new object [] {m_PhraseCount});
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/Obi Views and Transport Bar/Go to Page or Phrase dialog.htm");
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

/// <summary>
        /// Gets  user's input for phrase index inside selected section in case phrase radio button is checked.
        /// if page radio button is selected or input is not valid, returns null
        /// the returned phrase index starts from 1, so it is to be decremented by one for use urakawa tree.
        /// </summary>
        public int? PhraseIndex
            {
            get
                {
                if (m_radPhrase.Checked)
                    {
                    int phraseIndex = 0;
                    phraseIndex = ExtractPhraseIndexFromCombobox ();
                    if (phraseIndex >= 1)
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

        public double TimeInSeconds
        {
            
            get 
            {
                double time = 0;
                if (m_radTime.Checked)
                   double.TryParse(m_txtBox_TimeInSeconds.Text, out time);
                return time * 1000;                
            }            
        }

        public int SelectedIndex
        {
            get
            {    
                int i;
                if (m_cb_TimeInPhraseOrSection.SelectedIndex == 0)
                    i = 0;
                else if (m_cb_TimeInPhraseOrSection.SelectedIndex == 1)
                    i = 1;
                else
                {
                    i = 2;
                }
                return i;
            }
        }

        private void m_btnOk_Click ( object sender, EventArgs e )
            {
            int phraseIndex = 0;
            int.TryParse(base.mNumberBox.Text, out phraseIndex);
            
            if (m_radPage.Checked && phraseIndex < 1 && this.mPageKindComboBox.SelectedIndex < 2)
                {
                MessageBox.Show ( Localizer.Message ( "InvalidInput" ) );
                return;
                }
            else if (m_radPhrase.Checked)
                {
                phraseIndex = ExtractPhraseIndexFromCombobox ();
                if (phraseIndex < 1)
                    {
                    MessageBox.Show ( Localizer.Message ( "InvalidInput" ) );
                    return;
                    }
                else
                    {
                    DialogResult = DialogResult.OK;
                    Close ();
                    }
                }
                else if (m_radTime.Checked)
                {
                    if (TimeInSeconds < 1)
                    {
                        MessageBox.Show(Localizer.Message("InvalidInput"));
                        return;
                    }
                    else                   
                    {
                    DialogResult = DialogResult.OK;
                    Close ();
                    }
                }
                else
                {
                    base.mOKButton_Click(sender, e);
                }
            }

        private void m_radPage_CheckedChanged ( object sender, EventArgs e )
            {
            if (m_radPage.Checked)
                {
                    base.label1.Visible = true;
                    label4.Visible = false;
                    //base.label4.Location = new Point(50, 96);
                base.label1.Text = Localizer.Message("GoToPageOrPhrase_PageNumberLabel");
                base.mNumberBox.AccessibleName = base.label1.Text.Replace ("&", "") ;
                base.label3.Visible = true;
                base.mNumberBox.Visible = true;
                mPhraseIndexComboBox.Visible = false;
                base.mPageKindComboBox.Visible = true;
                m_cb_TimeInPhraseOrSection.Visible = false;
                m_lbl_Seconds.Visible = false;
                m_txtBox_TimeInSeconds.Visible = false;
                m_lbl_Time.Visible = false; 
                                }
            }

        private void m_radPhrase_CheckedChanged ( object sender, EventArgs e )
            {
            if (m_radPhrase.Checked)
                {
                    base.label1.Visible = true;
                    label4.Visible = false;
                   // base.label4.Location = new Point(50, 96);
                    base.label1.Text = Localizer.Message("GoToPageOrPhrase_PhraseIndexLabel");
                    mPhraseIndexComboBox.AccessibleName = base.label1.Text.Replace("&", "");
                base.label3.Visible = false;
                base.mPageKindComboBox.Visible = false;
                base.mNumberBox.Visible = false;
                mPhraseIndexComboBox.Visible = true;
                m_cb_TimeInPhraseOrSection.Visible = false;
                m_lbl_Seconds.Visible = false;
                m_txtBox_TimeInSeconds.Visible = false;
                m_lbl_Time.Visible = false; 
                                }
            }

        public int ExtractPhraseIndexFromCombobox()
        {
        int phraseIndex = 0;
            if(mPhraseIndexComboBox.Items.Count >=1)
            {
                if(mPhraseIndexComboBox.SelectedIndex != -1)
                {
                    int.TryParse(mPhraseIndexComboBox.Items[mPhraseIndexComboBox.SelectedIndex].ToString(), out phraseIndex);    
                }
                else
                {
                    mPhraseIndexComboBox.SelectAll();
                    
                    if (mPhraseIndexComboBox.SelectedText == "")   
                        MessageBox.Show(Localizer.Message ( "GotoPage_MissingVal")); 
                                         
                    else 
                        int.TryParse(mPhraseIndexComboBox.SelectedText, out phraseIndex);
                }
                }
                return phraseIndex;
            }

        private void m_radTime_CheckedChanged(object sender, EventArgs e)
        {
            m_lbl_Time.Visible = true;
            label4.Visible = true;
            base.label1.Visible = false;
            label4.Text = Localizer.Message("GoToPageOrPhrase_Time");
            base.label1.Text = Localizer.Message("GoToPageOrPhrase_Time");
            //label4.Location = new Point(90, 93);
          //  m_lbl_Time.Location = new Point(100, 120);
            base.mNumberBox.AccessibleName = base.label1.Text.Replace("&", "");            
            base.label3.Visible = false;
            base.mPageKindComboBox.Visible = false;
            mNumberBox.Visible = false;
            mPhraseIndexComboBox.Visible = false;
            base.mPageKindComboBox.Visible = false;
            m_txtBox_TimeInSeconds.Visible = true;
            m_lbl_Seconds.Visible = true;
            m_cb_TimeInPhraseOrSection.Visible = true;
        }
           // mPhraseIndexComboBox.SelectedIndex = -1;
            
        }

        
    }