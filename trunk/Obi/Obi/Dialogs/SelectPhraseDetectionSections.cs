using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class SelectPhraseDetectionSections : Form
    {
        private int m_SectionRangeCount;
        List<SectionNode> mOriginalSectionList = new List<SectionNode>();
        List <SectionNode> mSelectedSectionList = new List<SectionNode>();
        
        public SelectPhraseDetectionSections()
        {
            InitializeComponent();
            //for(int i = 1; i<= 360; i++)
            //{
                //mOriginalSectionList.Add(i.ToString());
            //}
                    }

        public SelectPhraseDetectionSections ( List<SectionNode> sectionsList ):this     ()
            {
            mOriginalSectionList = sectionsList;
            m_SectionRangeCount = mOriginalSectionList.Count / 100;
            m_cb_StartRangeForNumberOfSections.Items.AddRange ( new object[] { 1 } );
            for (int i = 1; i <= m_SectionRangeCount; i++)
                {
                m_cb_StartRangeForNumberOfSections.Items.AddRange ( new object[] { i * 100 } );
                m_cb_EndRangeForNumberOfSections.Items.AddRange ( new object[] { i * 100 } );
                }
            m_cb_StartRangeForNumberOfSections.Items.AddRange ( new object[] { mOriginalSectionList.Count } );
            m_cb_EndRangeForNumberOfSections.Items.AddRange ( new object[] { mOriginalSectionList.Count } );

            }
        public List<SectionNode> SelectedSections 
        {
            get { return mSelectedSectionList; }
        }

        private void m_btn_Display_Click(object sender, EventArgs e)
        {
           int startRange = int.Parse(m_cb_StartRangeForNumberOfSections.Items[m_cb_StartRangeForNumberOfSections.SelectedIndex].ToString());
           int endRange =  int.Parse(m_cb_EndRangeForNumberOfSections.Items[m_cb_EndRangeForNumberOfSections.SelectedIndex].ToString());
           if (startRange > endRange) { MessageBox.Show("End value is smaller than start value."); }
           else
           {
               for (int i = startRange; i < endRange; i++)
                   m_lb_listOfSelectedSectionsForPhraseDetection.Items.Add(mOriginalSectionList[ i]);
           }
        }

        private void m_btn_RemoveFromList_Click(object sender, EventArgs e)
        {
            for (int i = m_lb_listOfSelectedSectionsForPhraseDetection.SelectedItems.Count -1; i > 0; i--)
                m_lb_listOfSelectedSectionsForPhraseDetection.Items.Remove(m_lb_listOfSelectedSectionsForPhraseDetection.SelectedItem);              
            m_lb_listOfSelectedSectionsForPhraseDetection.Items.Remove(m_lb_listOfSelectedSectionsForPhraseDetection.SelectedItem);
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
        for (int i = 0; i < m_lb_listOfSelectedSectionsForPhraseDetection.Items.Count; i++)
            {
            mSelectedSectionList.Add ( (SectionNode)m_lb_listOfSelectedSectionsForPhraseDetection.Items[i] );
            }
        DialogResult = DialogResult.OK;
            Close();
        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
        this.DialogResult = DialogResult.Cancel;
            Close();
        }
         
        
    }
}