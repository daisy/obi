using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class SelectMergeSectionRange : Form
    {
        List<SectionNode> m_SectionList = null;
        List<SectionNode> m_SelectedSectionList = new List<SectionNode>();
        List<List<int>> m_ListOfContinuousItems = new List<List<int>>();
        List<SectionNode> listOfLargestNumberOfSections = new List<SectionNode>();
        List<int> m_RemainingIndexes = new List<int>();
        private int m_SelectedIndex;
        private bool m_IsDeselected = false;
       
        public SelectMergeSectionRange()
        {
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this,"HTML Files\\Creating a DTB\\Working with Sections\\Merging multiple sections.htm");                  
        }

        public SelectMergeSectionRange(List<SectionNode> sectionsList, int selectedIndexOfSection) : this()
        {
            m_SectionList = sectionsList;
            m_SelectedIndex = selectedIndexOfSection;
            populateListboxForSectionsToMerge();          
            m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("StatusForMergeSection"), m_SectionList[0].Label, m_SectionList[m_SectionList.Count - 1].Label);
            m_StatusLabelForMergeSection.AccessibleName = String.Format(Localizer.Message("StatusForMergeSection"), m_SectionList[0].Label, m_SectionList[m_SectionList.Count - 1].Label);
        }

        public List<SectionNode> SelectedSections
        {
            get { return  m_SelectedSectionList; }            
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            int totalPhraseCount = 0;
            List<SectionNode> listOfSelectedSections = new List<SectionNode>();

            for (int i = 0; i < m_lb_listofSectionsToMerge.SelectedItems.Count; i++)
            {
                int k = m_lb_listofSectionsToMerge.SelectedIndices[i];
                for (int j = 0; j < m_SectionList.Count; j++)
                {
                    if (k == j)
                       listOfSelectedSections.Add((SectionNode)m_SectionList[j]);
                }
            } 
            m_SelectedSectionList = listBoxSelectionIsContinuous();

            if (m_SelectedSectionList != null)
            {
                if (m_SelectedSectionList.Count <= 1)
                {
                    MessageBox.Show(Localizer.Message("not_enough_sections_to_merge"));
                    return;
                }
                MessageBox.Show(String.Format(Localizer.Message("merged_sections"), m_SelectedSectionList[0].Label, m_SelectedSectionList[m_SelectedSectionList.Count - 1].Label));
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                return;               
        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void populateListboxForSectionsToMerge()
        {
            SectionNode firstSection = m_SectionList[0];
            for (int i = 0; i <= (m_SectionList.Count - 1); i++)
            {
                if (m_SectionList[i].Level >= firstSection.Level)
                {
                    m_lb_listofSectionsToMerge.Items.Add("Section " + m_SectionList[i].Label + " Level " + m_SectionList[i].Level);
                }
                else
                    return;
            }
        }

        private void m_lb_listofSectionsToMerge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_IsDeselected)
            {
                List<SectionNode> tempList = new List<SectionNode>();
                tempList = listBoxSelectionIsContinuous();
                
                if (m_RemainingIndexes.Count < 1)
                {return;}
                else
                {
                    for (int l = 0; l < m_RemainingIndexes.Count; l++ )
                    {
                       /* for (int i = 0; i < m_lb_listofSectionsToMerge.SelectedIndices.Count - 1; i++)
                        {
                            if ((m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedIndices.Count - 1] != m_lb_listofSectionsToMerge.SelectedIndices[i + 1] - 1))
                            {
                                m_StatusLabelForMergeSection.Text = "The selection is not continuous";
                            }
                            else
                                m_StatusLabelForMergeSection.Text = "";
                        } */ 
                        m_IsDeselected = true;
                        m_lb_listofSectionsToMerge.SetSelected(m_RemainingIndexes[l], false);                   
                        }
                    }
                //     int totalPhraseCount = 0;
                /* for (int i = m_lb_listofSectionsToMerge.SelectedIndex; i <= m_lb_listofSectionsToMerge.SelectedItems.Count - 1; i++)
                     {
                         if (totalPhraseCount <= 7000 && m_lb_listofSectionsToMerge.SelectedIndex != -1)
                         {
                             totalPhraseCount = totalPhraseCount + m_SectionList[i].PhraseChildCount;
                             m_StatusLabelForMergeSection.Text = String.Format("Selected section {0} to {1} ", m_SectionList[m_lb_listofSectionsToMerge.SelectedIndex].Label, m_SectionList[m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedItems.Count - 1]].Label);
                         }
                         else if (totalPhraseCount > 7000)
                             m_StatusLabelForMergeSection.Text = String.Format("Total number of phrases is {0}.It should be less than 7000", totalPhraseCount);
                     }*/                                  
            }
            if (m_lb_listofSectionsToMerge.SelectedItems.Count == 1)
            {
                m_StatusLabelForMergeSection.Text = String.Format("Selected {0}. Select at least one more section to merge", m_lb_listofSectionsToMerge.SelectedItem);                
            }

            if (m_lb_listofSectionsToMerge.SelectedIndices.Count > 0)
                m_tb_SelectedSection.Text = m_SectionList[m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedItems.Count - 1]].ToString();
            m_IsDeselected = false;
        }

        private List<SectionNode> listBoxSelectionIsContinuous() 
        {
            int j = 0;
            int totalPhraseCount = 0;
            List<SectionNode> listOfLargestNumberOfSections = new List<SectionNode>();
            bool IsSameIndex = false;
            List<int> indexOfSublist = new List<int>();
            List<int> listOfIndexOfLargestNumberOfSection = new List<int>();
            m_RemainingIndexes.Clear();

            if (m_lb_listofSectionsToMerge.SelectedIndices.Count > 1)
            {
                for (int i = 0; i < m_lb_listofSectionsToMerge.SelectedIndices.Count - 1; i++)
                {
                    int k = m_lb_listofSectionsToMerge.SelectedIndices[i];
                    if ((m_lb_listofSectionsToMerge.SelectedIndices[i] == m_lb_listofSectionsToMerge.SelectedIndices[i + 1] - 1))
                    {
                        if (j == 0)
                        {
                            indexOfSublist.Add(k);
                            indexOfSublist.Add(k + 1);
                            j++;
                        }
                        else
                            indexOfSublist.Add(k + 1);
                    }
                    else
                    {
                        if (indexOfSublist.Count > 0)
                            m_ListOfContinuousItems.Add(indexOfSublist);
                        indexOfSublist = new List<int>();
                        j = 0;
                    }
                    if (indexOfSublist.Count > 0)
                        m_ListOfContinuousItems.Add(indexOfSublist);
                }
            }

            else
            {
                if (m_lb_listofSectionsToMerge.SelectedIndices.Count >= 1)
                {
                    indexOfSublist.Add(m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedIndices.Count - 1]);
                    m_ListOfContinuousItems.Add(indexOfSublist);
                }
            }

           if (m_ListOfContinuousItems.Count > 0)
           {
               listOfIndexOfLargestNumberOfSection = m_ListOfContinuousItems[0];
               foreach(List<int> list in m_ListOfContinuousItems)
               {
                   if (list.Count > listOfIndexOfLargestNumberOfSection.Count)
                      listOfIndexOfLargestNumberOfSection = list;
               }

               if (listOfIndexOfLargestNumberOfSection.Count == 1)
               { }
               else
                   m_ListOfContinuousItems.Clear();


               for (int i = 0; i <= listOfIndexOfLargestNumberOfSection.Count -1; i++)
               {   listOfLargestNumberOfSections.Add(m_SectionList[listOfIndexOfLargestNumberOfSection[i]]);  }

               for (int m = 0; m < m_lb_listofSectionsToMerge.SelectedItems.Count; m++)
               {
                   for (int i = 0; i < listOfIndexOfLargestNumberOfSection.Count; i++)
                   {
                       if (m_lb_listofSectionsToMerge.SelectedIndices[m] == listOfIndexOfLargestNumberOfSection[i])
                       {
                           IsSameIndex = true;
                           break;
                       }  
                   }

                if (!IsSameIndex && (m_lb_listofSectionsToMerge.SelectedItems.Count > 1))
                    m_RemainingIndexes.Add(m_lb_listofSectionsToMerge.SelectedIndices[m]);
                if (m_lb_listofSectionsToMerge.SelectedIndices[m] > listOfIndexOfLargestNumberOfSection[listOfIndexOfLargestNumberOfSection.Count - 1] && (m_lb_listofSectionsToMerge.SelectedItems.Count > 1))
                   m_RemainingIndexes.Add(m_lb_listofSectionsToMerge.SelectedIndices[m]);                 
               }    
           }

           if (listOfLargestNumberOfSections.Count > 0)
           {
               for (int k = 0; k < listOfLargestNumberOfSections.Count; k++)
               { totalPhraseCount += listOfLargestNumberOfSections[k].PhraseChildCount; }

               if (totalPhraseCount < 7000)
               {
                   if (m_lb_listofSectionsToMerge.SelectedItems.Count == 1)
                   {
                       m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("select_one_more_section"), m_lb_listofSectionsToMerge.SelectedItem);                       
                   }
                   else
                   {
                       m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("merged_sections"), listOfLargestNumberOfSections[0].Label, listOfLargestNumberOfSections[listOfLargestNumberOfSections.Count - 1].Label);                      
                   }

               //    MessageBox.Show(String.Format("Merged sections will be from {0} to {1} ", newList[0], newList[newList.Count - 1]));
               }
               else
               {
                   MessageBox.Show(Localizer.Message("phrase_count_more_than_7000"));
                   m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("phrase_count_more_than_7000"));
                   listOfLargestNumberOfSections = null;
               }
           }          

           if (m_lb_listofSectionsToMerge.SelectedIndices.Count > 0)
               m_tb_SelectedSection.Text = m_SectionList[m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedItems.Count - 1]].ToString();
           return listOfLargestNumberOfSections;
        }

        private void m_btn_SelectAll_Click(object sender, EventArgs e)
        {
             int totalPhraseCount = 0;
            
            m_ListOfContinuousItems.Clear();
            for (int i = 0; i < m_lb_listofSectionsToMerge.Items.Count; i++)
            {
                totalPhraseCount += m_SectionList[i].PhraseChildCount;
                if (totalPhraseCount < 7000)
                {
                    m_IsDeselected = true;
                    m_lb_listofSectionsToMerge.SetSelected(i, true);
                    
                }
            }
            if (totalPhraseCount > 7000)
                MessageBox.Show(String.Format(Localizer.Message("limited_sections_merge"), m_SectionList[m_lb_listofSectionsToMerge.SelectedItems.Count - 1].Label));           
            m_StatusLabelForMergeSection.AccessibleName = String.Format(Localizer.Message("merged_sections"), m_SectionList[0].Label, m_SectionList[m_lb_listofSectionsToMerge.SelectedItems.Count - 1].Label);
        
        }
    }
}