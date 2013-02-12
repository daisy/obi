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
        private SectionNode mSelectedSectionNode;
        private int m_StartRange;
        private int m_EndRange;
       
        public SelectPhraseDetectionSections()
        {
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Phrases/Auto-splitting Phrases of Multiple Sections.htm");
        }

        public SelectPhraseDetectionSections ( List<SectionNode> sectionsList, List<PhraseNode> silencePhraseList,SectionNode selectedSection):this     ()
        {
            m_OriginalSectionList = sectionsList;
            m_SilencePhrases = silencePhraseList;
            mSelectedSectionNode = selectedSection;
            m_SectionRangeCount = m_OriginalSectionList.Count / 10;
           
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
                    int val = i * 10;
                    if (val < m_OriginalSectionList.Count )  m_cb_StartRangeForNumberOfSections.Items.Add(val);
                    if(val <= m_OriginalSectionList.Count) m_cb_EndRangeForNumberOfSections.Items.Add(val);
                    
                }
                if (m_OriginalSectionList.Count > 2)
                {
                    if((m_OriginalSectionList.Count - 1) % 10 != 0 )  m_cb_StartRangeForNumberOfSections.Items.Add( m_OriginalSectionList.Count - 1);
                    m_cb_EndRangeForNumberOfSections.Items.Add( m_OriginalSectionList.Count );
                }
                else                  
                    m_cb_EndRangeForNumberOfSections.Items.Add( m_OriginalSectionList.Count );
            }
        
            m_cb_SilencePhrase.Items.Add (Localizer.Message("UseDefaultValues"));
            for (int i = 0; i < m_SilencePhrases.Count; i++)
                {
                m_cb_SilencePhrase.Items.Add ( (i + 1) + ". Section: " + m_SilencePhrases[i].ParentAs<SectionNode> ().Label + ": " + m_SilencePhrases[i].BaseStringShort());
                }
            m_cb_SilencePhrase.SelectedIndex = 0;
            m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(Localizer.Message("SelectDeselectAllSections"));
            }

        public List<SectionNode> SelectedSections 
        {
            get { return m_SelectedSectionList; }
        }
        private void updateControls()
        {
            m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Clear();
            m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(Localizer.Message("SelectDeselectAllSections"));
            m_cb_StartRangeForNumberOfSections.Enabled = m_rb_loadFromRange.Checked ? true : false;
            m_cb_EndRangeForNumberOfSections.Enabled = m_rb_loadFromRange.Checked ? true : false;
        }
        private void m_btn_Display_Click(object sender, EventArgs e)
        {
            if (m_rb_loadFromRange.Checked && m_cb_StartRangeForNumberOfSections.SelectedIndex == -1 && m_cb_EndRangeForNumberOfSections.SelectedIndex == -1) return;

            updateControls();
            if (m_rb_loadAllSections.Checked)
            {
                foreach (SectionNode sNode in m_OriginalSectionList)
                {
                    ListViewItem item = new ListViewItem(sNode.Label);
                    item.Tag = sNode;
                    m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(item);
                }
            }
            else if (m_rb_loadSelectedOnwards.Checked)
            {
                if (mSelectedSectionNode != null)
                {
                    int lastSectionIndex = m_OriginalSectionList.LastIndexOf(mSelectedSectionNode);
                    for (int i = lastSectionIndex + 1; i <= m_OriginalSectionList.Count; i++)
                    {
                        ListViewItem item = new ListViewItem(m_OriginalSectionList[i - 1].Label);
                        item.Tag = m_OriginalSectionList[i - 1];
                        m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(item);
                    }
                }
            }
            else if (m_rb_loadFromRange.Checked)
            {
                if ((m_cb_EndRangeForNumberOfSections.Items.Count >= 1) &&
                    (m_cb_StartRangeForNumberOfSections.Items.Count >= 1))
                {
                    if ((m_cb_EndRangeForNumberOfSections.SelectedIndex != -1) &&
                        (m_cb_StartRangeForNumberOfSections.SelectedIndex != -1))
                    {
                        m_StartRange =
                            int.Parse(
                                m_cb_StartRangeForNumberOfSections.Items[m_cb_StartRangeForNumberOfSections.SelectedIndex].
                                    ToString());
                        m_EndRange =
                            int.Parse(
                                m_cb_EndRangeForNumberOfSections.Items[m_cb_EndRangeForNumberOfSections.SelectedIndex].
                                    ToString());

                        if (m_StartRange > m_EndRange)
                        {
                            MessageBox.Show(Localizer.Message("SelectPhraseDetection_EndValIsSmallerThanStart"));
                        }

                        else if (m_StartRange < m_EndRange)
                        {
                            for (int i = m_StartRange; i <= m_EndRange; i++)
                            {
                                ListViewItem item = new ListViewItem(m_OriginalSectionList[i - 1].Label);
                                m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(item);
                                item.Tag = m_OriginalSectionList[i - 1];
                            }
                        }
                        else if (m_StartRange == m_EndRange)
                        {
                            m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(
                                m_OriginalSectionList[m_StartRange - 1].Label);
                        }
                    }
                    else
                    {
                        m_cb_EndRangeForNumberOfSections.SelectAll();
                        m_cb_StartRangeForNumberOfSections.SelectAll();
                        if (m_cb_StartRangeForNumberOfSections.SelectedText == "" ||
                            m_cb_EndRangeForNumberOfSections.SelectedText == "")
                        {
                            return;
                        }
                        else
                        {
                            int.TryParse(m_cb_StartRangeForNumberOfSections.SelectedText, out m_StartRange);
                            int.TryParse(m_cb_EndRangeForNumberOfSections.SelectedText, out m_EndRange);
                        }
                        if ((m_StartRange >= 1 && m_StartRange <= m_OriginalSectionList.Count) &&
                            (m_EndRange >= 1 && m_EndRange <= m_OriginalSectionList.Count))
                        {
                            if (m_StartRange > m_EndRange)
                            {
                                MessageBox.Show(Localizer.Message("SelectPhraseDetection_EndValIsSmallerThanStart"));
                            }

                            else if (m_StartRange < m_EndRange)
                            {
                                for (int i = m_StartRange; i <= m_EndRange; i++)
                                    m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(
                                        m_OriginalSectionList[i - 1].Label);
                            }
                            else if (m_StartRange == m_EndRange)
                            {
                                m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Add(
                                    m_OriginalSectionList[m_StartRange - 1].Label);
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
            }
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            if (m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Count <= 1) return;

            for (int i = 0; i < m_lv_ListOfSelectedSectionsForPhraseDetection.CheckedItems.Count; i++)
            {
                if (m_lv_ListOfSelectedSectionsForPhraseDetection.CheckedItems[i] != m_lv_ListOfSelectedSectionsForPhraseDetection.Items[0])
                {
                    m_SelectedSectionList.Add((SectionNode)m_lv_ListOfSelectedSectionsForPhraseDetection.CheckedItems[i].Tag);
                }
            
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
            m_cb_EndRangeForNumberOfSections.Items.Clear();

            for (int i = m_cb_StartRangeForNumberOfSections.SelectedIndex; i < (m_OriginalSectionList.Count / 10); i++)
                    m_cb_EndRangeForNumberOfSections.Items.Add(((i + 1) * 10));

                if ( m_OriginalSectionList.Count % 10 != 0) m_cb_EndRangeForNumberOfSections.Items.Add(m_OriginalSectionList.Count);
            if (m_cb_EndRangeForNumberOfSections.Items.Count == 1)
                m_cb_EndRangeForNumberOfSections.SelectedIndex = 0;                
        }
                       
        private void m_cb_StartRangeForNumberOfSections_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateCombobox();
        }
        public PhraseNode SelectedSilencePhrase { get { return m_SelectedSilencePhrase; } }

        private void m_lv_ListOfSelectedSectionsForPhraseDetection_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Count == 1)
                return;

            if(e.Index == 0)
            {
                for (int i = 1; i < m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Count; i++)
                {
                    m_lv_ListOfSelectedSectionsForPhraseDetection.Items[i].Checked =
                        e.NewValue == CheckState.Checked ? true : false;
                }
            }

            if (m_lv_ListOfSelectedSectionsForPhraseDetection.Items[0].Checked && 
                (m_lv_ListOfSelectedSectionsForPhraseDetection.Items.Count != m_lv_ListOfSelectedSectionsForPhraseDetection.CheckedItems.Count || (e.Index > 0 && e.NewValue == CheckState.Unchecked)))
            {
                
                this.m_lv_ListOfSelectedSectionsForPhraseDetection.ItemCheck -= new System.Windows.Forms.ItemCheckEventHandler(this.m_lv_ListOfSelectedSectionsForPhraseDetection_ItemCheck);
                m_lv_ListOfSelectedSectionsForPhraseDetection.Items[0].Checked = false;
                this.m_lv_ListOfSelectedSectionsForPhraseDetection.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.m_lv_ListOfSelectedSectionsForPhraseDetection_ItemCheck);
            }
        }

        private void m_rb_loadAllSections_CheckedChanged(object sender, EventArgs e)
        {
            updateControls();
        }

        private void m_rb_loadFromRange_CheckedChanged(object sender, EventArgs e)
        {
            updateControls();
        }

        private void m_rb_loadSelectedOnwards_CheckedChanged(object sender, EventArgs e)
        {
            updateControls();
        }
              
      }
}
