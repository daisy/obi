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
        List<SectionNode> m_OriginalSectionList = null;
        List<SectionNode> m_SelectedSectionList = new List<SectionNode> ();
        List<PhraseNode> m_SilencePhrases = null;
        private PhraseNode m_SelectedSilencePhrase;
        private int m_StartRange;
        private int m_EndRange;
        public SelectPhraseDetectionSections()
        {
            InitializeComponent();
            
        }

        public SelectPhraseDetectionSections ( List<SectionNode> sectionsList, List<PhraseNode> silencePhraseList ):this     ()
            {
            m_OriginalSectionList = sectionsList;
            m_SilencePhrases = silencePhraseList;
            m_SectionRangeCount = m_OriginalSectionList.Count / 100;
           
            if (m_OriginalSectionList.Count == 1)
            {
                m_cb_StartRangeForNumberOfSections.Items.Add(m_OriginalSectionList.Count);
                m_cb_EndRangeForNumberOfSections.Items.Add(m_OriginalSectionList.Count);
            }
            else if (m_OriginalSectionList.Count > 1)
            {
                m_cb_StartRangeForNumberOfSections.Items.Add(1);
                for (int i = 1; i <= m_SectionRangeCount; i++)
                {
                    m_cb_StartRangeForNumberOfSections.Items.Add(i * 100);
                    m_cb_EndRangeForNumberOfSections.Items.Add(i * 100 );
                }
                if (m_OriginalSectionList.Count > 2)
                {
                    m_cb_StartRangeForNumberOfSections.Items.Add( m_OriginalSectionList.Count - 1);
                    m_cb_EndRangeForNumberOfSections.Items.Add( m_OriginalSectionList.Count );
                }
                else                  
                    m_cb_EndRangeForNumberOfSections.Items.Add( m_OriginalSectionList.Count );
            }
        
            m_cb_SilencePhrase.Items.Add ( "Use default values" );
            for (int i = 0; i < m_SilencePhrases.Count; i++)
                {
                m_cb_SilencePhrase.Items.Add ( (i + 1) + ". Section: " + m_SilencePhrases[i].ParentAs<SectionNode> ().Label + ": " + m_SilencePhrases[i].BaseStringShort());
                }
            m_cb_SilencePhrase.SelectedIndex = 0;
            
            }

        public List<SectionNode> SelectedSections 
        {
            get { return m_SelectedSectionList; }
        }

        private void m_btn_Display_Click(object sender, EventArgs e)
        {
            if ((m_cb_EndRangeForNumberOfSections.Items.Count >= 1) && (m_cb_StartRangeForNumberOfSections.Items.Count >= 1)) 
            {
                m_lb_listOfSelectedSectionsForPhraseDetection.Items.Clear();
                if ((m_cb_EndRangeForNumberOfSections.SelectedIndex != -1) && (m_cb_StartRangeForNumberOfSections.SelectedIndex != -1))
                {
                    m_StartRange = int.Parse(m_cb_StartRangeForNumberOfSections.Items[m_cb_StartRangeForNumberOfSections.SelectedIndex].ToString());
                    m_EndRange = int.Parse(m_cb_EndRangeForNumberOfSections.Items[m_cb_EndRangeForNumberOfSections.SelectedIndex].ToString());

                    if (m_StartRange > m_EndRange) { MessageBox.Show(Localizer.Message("SelectPhraseDetection_EndValIsSmallerThanStart")); }

                    else if (m_StartRange < m_EndRange)
                    {
                        for (int i = m_StartRange; i <= m_EndRange; i++)
                            m_lb_listOfSelectedSectionsForPhraseDetection.Items.Add(m_OriginalSectionList[i -1]);
                    }
                    else if (m_StartRange == m_EndRange)
                    {
                        m_lb_listOfSelectedSectionsForPhraseDetection.Items.Add(m_OriginalSectionList[m_StartRange - 1]);
                    }
                }
                else
                {
                    m_cb_EndRangeForNumberOfSections.SelectAll();
                    m_cb_StartRangeForNumberOfSections.SelectAll();
                    if (m_cb_StartRangeForNumberOfSections.SelectedText == "" || m_cb_EndRangeForNumberOfSections.SelectedText == "")
                    {
                       // MessageBox.Show(Localizer.Message("SelectPhraseDetection_MissingStartOrEndVal"));
                        return;
                    }
                    else
                    {
                        int.TryParse(m_cb_StartRangeForNumberOfSections.SelectedText, out m_StartRange);
                        int.TryParse(m_cb_EndRangeForNumberOfSections.SelectedText, out m_EndRange);
                    }
                    if ((m_StartRange >= 1 && m_StartRange <= m_OriginalSectionList.Count) && (m_EndRange >= 1 && m_EndRange <= m_OriginalSectionList.Count))
                    {
                        if (m_StartRange > m_EndRange) { MessageBox.Show(Localizer.Message("SelectPhraseDetection_EndValIsSmallerThanStart")); }

                        else if (m_StartRange < m_EndRange)
                        {
                            for (int i = m_StartRange; i <= m_EndRange; i++)
                                m_lb_listOfSelectedSectionsForPhraseDetection.Items.Add(m_OriginalSectionList[i - 1]);
                        }
                        else if (m_StartRange == m_EndRange)
                        {
                            m_lb_listOfSelectedSectionsForPhraseDetection.Items.Add(m_OriginalSectionList[m_StartRange - 1]);
                        }
                    }
                    else
                    {
                        if (m_EndRange > m_OriginalSectionList.Count || m_StartRange > m_OriginalSectionList.Count)
                            MessageBox.Show(Localizer.Message("SelectPhraseDetection_EndOrStartValMoreThanSectionCount"));
                        else
                            MessageBox.Show(Localizer.Message("InvalidInput"));
                    }
                }               
            }
            m_cb_StartRangeForNumberOfSections.SelectedIndex = -1;
            m_cb_EndRangeForNumberOfSections.SelectedIndex = -1;
            m_btn_RemoveFromList.Enabled = true;
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
        m_SelectedSilencePhrase = m_cb_SilencePhrase.SelectedIndex > 0 ? m_SilencePhrases[m_cb_SilencePhrase.SelectedIndex - 1] : null;
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
                    m_cb_EndRangeForNumberOfSections.Items.Add(((i + 1) * 100));            
                
            m_cb_EndRangeForNumberOfSections.Items.Add( m_OriginalSectionList.Count );
            if (m_cb_EndRangeForNumberOfSections.Items.Count == 1)
                m_cb_EndRangeForNumberOfSections.SelectedIndex = 0;                
        }
                       
        private void m_cb_StartRangeForNumberOfSections_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateCombobox();
        }
        public PhraseNode SelectedSilencePhrase { get { return m_SelectedSilencePhrase; } }
        
    }
}
