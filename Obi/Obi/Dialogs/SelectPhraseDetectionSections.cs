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
        List<SectionNode> m_OriginalSectionList = new List<SectionNode>();
        List <SectionNode> m_SelectedSectionList = new List<SectionNode>();
        List<string> m_SilencePhrases = new List<string>();
        private int m_StartRange;
        private int m_EndRange;
        public SelectPhraseDetectionSections()
        {
            InitializeComponent();
            for(int i = 1; i<= 36; i++)
            {
                m_SilencePhrases.Add(i.ToString());
            }
        }

        public SelectPhraseDetectionSections ( List<SectionNode> sectionsList ):this     ()
            {
            m_OriginalSectionList = sectionsList;
            m_SectionRangeCount = m_OriginalSectionList.Count / 100;
            m_cb_StartRangeForNumberOfSections.Items.AddRange ( new object[] { 1 } );
            for (int i = 1; i <= m_SectionRangeCount; i++)
                {
                m_cb_StartRangeForNumberOfSections.Items.AddRange ( new object[] { i * 100 } );
                m_cb_EndRangeForNumberOfSections.Items.AddRange ( new object[] { i * 100 } );
                }
            m_cb_StartRangeForNumberOfSections.Items.AddRange ( new object[] { m_OriginalSectionList.Count -1 } );
            m_cb_EndRangeForNumberOfSections.Items.AddRange ( new object[] { m_OriginalSectionList.Count } );
        //    updateCombobox();
            }

        public List<SectionNode> SelectedSections 
        {
            get { return m_SelectedSectionList; }
        }

        public List<string> SilencePhrases
        {
            get { return m_SilencePhrases; }
        }

        private void m_btn_Display_Click(object sender, EventArgs e)
        { 
            if (m_cb_EndRangeForNumberOfSections.Items.Count >= 1 && m_cb_StartRangeForNumberOfSections.Items.Count >= 1) 
            {
                m_lb_listOfSelectedSectionsForPhraseDetection.Items.Clear();
                if((m_cb_EndRangeForNumberOfSections.SelectedIndex != -1) && (m_cb_StartRangeForNumberOfSections.SelectedIndex != -1))
                {
                m_StartRange = int.Parse(m_cb_StartRangeForNumberOfSections.Items[m_cb_StartRangeForNumberOfSections.SelectedIndex].ToString());
                m_EndRange = int.Parse(m_cb_EndRangeForNumberOfSections.Items[m_cb_EndRangeForNumberOfSections.SelectedIndex].ToString());
                if (m_StartRange > m_EndRange) { MessageBox.Show("End value is smaller than start value."); }
            
                else
                {
                    for (int i = m_StartRange; i < m_EndRange; i++)
                       m_lb_listOfSelectedSectionsForPhraseDetection.Items.Add(m_OriginalSectionList[i]);                                            
                }
                }
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
            m_SelectedSectionList.Add ( (SectionNode)m_lb_listOfSelectedSectionsForPhraseDetection.Items[i] );
            }
        DialogResult = DialogResult.OK;
            Close();
        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
        this.DialogResult = DialogResult.Cancel;
            Close();
        }

        public void updateCombobox()
        {
            
          /*  int startSelectedIndex;
            
            m_cb_StartRangeForNumberOfSections.SelectAll();
           
            if (m_cb_StartRangeForNumberOfSections.SelectedText == "" && m_cb_StartRangeForNumberOfSections.SelectedIndex != -1)
                startSelectedIndex = int.Parse(m_cb_StartRangeForNumberOfSections.Items[m_cb_StartRangeForNumberOfSections.SelectedIndex].ToString());
                                
            else if (m_cb_StartRangeForNumberOfSections.SelectedText != "")
                {
                    startSelectedIndex = int.Parse(m_cb_StartRangeForNumberOfSections.SelectedText);
                    m_cb_StartRangeForNumberOfSections.ResetText();
                }
            else
                return;*/
            m_cb_EndRangeForNumberOfSections.Items.Clear();

            for (int i = m_cb_StartRangeForNumberOfSections.SelectedIndex; i < (m_OriginalSectionList.Count / 100); i++)
                    m_cb_EndRangeForNumberOfSections.Items.AddRange(new object[] { ((i + 1) * 100) });                   
                
            m_cb_EndRangeForNumberOfSections.Items.AddRange(new object[] { m_OriginalSectionList.Count });
            if (m_cb_EndRangeForNumberOfSections.Items.Count == 1)
                m_cb_EndRangeForNumberOfSections.SelectedIndex = 0;                
        }
                       
        private void m_cb_StartRangeForNumberOfSections_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateCombobox();
        }

        private void m_btn_ShowSilencePhrases_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < m_SilencePhrases.Count ; i ++)
               m_cb_SilencePhrase.Items.AddRange(new object[] {i});
        }
    }
}
