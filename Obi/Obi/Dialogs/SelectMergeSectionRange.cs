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
        private int m_selectedIndex;
               
        public SelectMergeSectionRange()
        {
            InitializeComponent();
        }

        public SelectMergeSectionRange(List<SectionNode> sectionsList, int selectedIndexOfSection) : this()
        {
            m_SectionList = sectionsList;
            m_selectedIndex = selectedIndexOfSection;
            populateListboxForSectionsToMerge();
            m_StatusLabelForMergeSection.Text = String.Format("Showing section {0} to {1}. Please select sections to merge. ", m_SectionList[0].Label, m_SectionList[m_SectionList.Count - 1].Label);
        }

        public List<SectionNode> SelectedSections
        {
            get { return  m_SelectedSectionList; }            
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            int totalPhraseCount = 0;
            List<SectionNode> listOfSelectedSections = new List<SectionNode>();
            if (m_lb_listofSectionsToMerge.SelectedItems.Count == 0 || m_lb_listofSectionsToMerge.SelectedItems.Count == 1)
            {
                MessageBox.Show("Please select at least two sections to merge");
                return;
            }
            else
            {
                for (int i = 0; i < m_lb_listofSectionsToMerge.SelectedItems.Count; i++)
                {
                    int k = m_lb_listofSectionsToMerge.SelectedIndices[i];
                    for (int j = 0; j < m_SectionList.Count; j++)
                    {
                        if(k == j)
                        {
                            if (totalPhraseCount < 7000 && m_lb_listofSectionsToMerge.SelectedIndex != -1)
                                {
                                    listOfSelectedSections.Add((SectionNode)m_SectionList[j]);
                                    totalPhraseCount = totalPhraseCount + m_SectionList[i].PhraseChildCount;
                                }
                             else
                                    MessageBox.Show("Total phrase count is more than 7000");
                        }
                     }
               }              
               m_SelectedSectionList = listBoxSelectionIsContinuous(listOfSelectedSections);
            }
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
                    m_lb_listofSectionsToMerge.Items.Add("Section " + m_SectionList[i].Label + " Level " + m_SectionList[i].Level);                 
                else
                    return;
            }
        }

        private void m_lb_listofSectionsToMerge_SelectedIndexChanged(object sender, EventArgs e)
        {
            int totalPhraseCount = 0;
            for (int i = m_lb_listofSectionsToMerge.SelectedIndex; i <= m_lb_listofSectionsToMerge.SelectedItems.Count - 1; i++)
                {
                    if (totalPhraseCount <= 7000 && m_lb_listofSectionsToMerge.SelectedIndex != -1)
                    {
                        totalPhraseCount = totalPhraseCount + m_SectionList[i].PhraseChildCount;
                        m_StatusLabelForMergeSection.Text = String.Format("Selected section {0} to {1} ", m_SectionList[m_lb_listofSectionsToMerge.SelectedIndex].Label, m_SectionList[m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedItems.Count - 1]].Label);
                    }
                    else if (totalPhraseCount > 7000)
                        m_StatusLabelForMergeSection.Text = String.Format("Total number of phrases is {0}.It should be less than 7000", totalPhraseCount);
                }

                if (m_lb_listofSectionsToMerge.SelectedIndices.Count > 0)
                    m_tb_SelectedSection.Text = m_SectionList[m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedItems.Count - 1]].ToString();           
        }

        private List<SectionNode> listBoxSelectionIsContinuous(List<SectionNode> sectionList) 
        {
            int j = 0;
            List<SectionNode> newList = new List<SectionNode>();
            List<List<SectionNode>> listOfContinuousItems = new List<List<SectionNode>>();
            List<SectionNode> subListOfContinuousItems = new List<SectionNode>();
          
           for (int i = 0; i < m_lb_listofSectionsToMerge.SelectedIndices.Count -1; i++)
           {
               int k = m_lb_listofSectionsToMerge.SelectedIndices[i];
               if ((m_lb_listofSectionsToMerge.SelectedIndices[i] == m_lb_listofSectionsToMerge.SelectedIndices[i + 1] - 1))
               {                   
                   if (j == 0)
                   {
                       subListOfContinuousItems.Add(m_SectionList[k]);
                       subListOfContinuousItems.Add(m_SectionList[k + 1]);                       
                       j++;
                   }
                   else
                       subListOfContinuousItems.Add(m_SectionList[k + 1]);                  
               }

              else
               {
                   if (subListOfContinuousItems.Count > 0)
                   listOfContinuousItems.Add(subListOfContinuousItems);
                   subListOfContinuousItems = new List<SectionNode>();
                   j = 0;
               }
               if (subListOfContinuousItems.Count > 0)
                  listOfContinuousItems.Add(subListOfContinuousItems);
           }

           if (listOfContinuousItems.Count > 0)
           {
               newList = listOfContinuousItems[0];
               foreach(List<SectionNode> list in listOfContinuousItems)
               {
                   if (list.Count > newList.Count)
                       newList = list;
               }
               listOfContinuousItems.Clear();
               listOfContinuousItems.Add(newList);
           }

           if (listOfContinuousItems.Count > 0)
           {
               newList = listOfContinuousItems[0];
               m_StatusLabelForMergeSection.Text = (String.Format("Merging sections from {0} to {1} ", newList[0], newList[newList.Count - 1]));
               MessageBox.Show(String.Format("Merged sections will be from {0} to {1} ", newList[0], newList[newList.Count - 1]));               
           }
           else
               MessageBox.Show("There are not enough section to merge. Select at least two continuous sections");
           return newList;
        }

        private void m_btn_SelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_lb_listofSectionsToMerge.Items.Count; i++ )
                m_lb_listofSectionsToMerge.SetSelected(i, true);
        }
    }
}