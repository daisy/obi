using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public delegate void SectionsManipulationDelegate (object sender, EventArgs e);
    public partial class SelectMergeSectionRange : Form
    {
        List<SectionNode> m_SectionList = null;
        List<SectionNode> m_SelectedSectionList = new List<SectionNode>();        
        List<List<int>> m_ListOfContinuousItems = new List<List<int>>();
        List<SectionNode> listOfLargestNumberOfSections = new List<SectionNode>();
        List<int> m_RemainingIndexes = new List<int>();
        private int m_SelectedIndex;
        private bool m_IsDeselected = false;
        private bool m_Merge = false;
        private ProjectView.ProjectView m_ProjectView;
        private ProjectView.ContentView m_ContentView;
        private SectionNode m_tempSectionNode;
        private List<SectionNode> m_SelectedSectionListForDecreaseLevel = new List<SectionNode>();
        private List<SectionNode> m_SelectedSectionListForIncreaseLevel = new List<SectionNode>();
        private List<int> m_IndexOfSectionSelected = new List<int>();
        private List<SectionNode> m_SelectedSectionListToMerge = new List<SectionNode>();
        private bool m_FlagMerge = false;

        public event SectionsManipulationDelegate LevelIncrementEvent;
        public event SectionsManipulationDelegate LevelDecrementEvent;
        public event SectionsManipulationDelegate MergeSectionEvent ;
        public event SectionsManipulationDelegate UndoChangeEvent;
    


        public SelectMergeSectionRange()
        {
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Working with Sections\\Multi-section Operations.htm");
        }

        public SelectMergeSectionRange(Obi.ProjectView.ProjectView projectView,Obi.ProjectView.ContentView contentview)
            : this()
        {
            m_ProjectView = projectView;
            m_ContentView = contentview;
            InitializeList();
            populateListboxForSectionsToMerge();
            m_tb_SectionsSelected.Text = m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("StatusForMergeSection"), m_SectionList[0].Label, m_SectionList[m_SectionList.Count - 1].Label);
            m_ProjectView.TransportBar.StateChanged += new AudioLib.AudioPlayer.StateChangedHandler(State_Changed_Player);
            toolTip.SetToolTip(m_btn_IncreaseSectionLevel, Localizer.Message("MergeOptions_IncreaseLevel"));
            toolTip.SetToolTip(m_btn_DecreaseSectionLevel, Localizer.Message("MergeOptions_DecreaseLevel"));
            toolTip.SetToolTip(m_btn_Merge,Localizer.Message("MergeOptions_Merge"));

        }
        private void UpdateList()
        {
            List<SectionNode> listOfSections = ((Obi.ObiRootNode)(m_ProjectView.RootNode)).GetListOfAllSections();
            if (m_SelectedIndex > 0) listOfSections.RemoveRange(0, m_SelectedIndex);
            m_SectionList = listOfSections;
        }
        private void InitializeList()
        {
            List<SectionNode> listOfSections = ((Obi.ObiRootNode)(m_ProjectView.RootNode)).GetListOfAllSections();
            int selectedSectionIndex = listOfSections.IndexOf(m_ProjectView.GetSelectedPhraseSection);
            if (selectedSectionIndex > 0) listOfSections.RemoveRange(0, selectedSectionIndex);
            m_SelectedIndex = selectedSectionIndex;
            m_SectionList = listOfSections;
        }
        public void State_Changed_Player(object sender, AudioLib.AudioPlayer.StateChangedEventArgs e)
        {
            UpdateButtons();
        }
        private void UpdateButtons()
        {
            if (m_ProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {
                m_btnPause.Enabled = true;
                m_btnPause.Visible = true;
                if (m_btn_Play.ContainsFocus) m_btnPause.Focus();
                m_btn_Stop.Enabled = true;
                m_btn_Play.Visible = false;
            }
            else if (m_ProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused)
            {
                m_btn_Play.Enabled = true;
                m_btn_Play.Visible = true;
                if (m_btnPause.ContainsFocus) m_btn_Play.Focus();
                m_btnPause.Visible = false;
            }
            else if (m_ProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)
            {
                m_btn_Play.Visible = true;
                m_btn_Play.Enabled = true;
                m_btnPause.Visible = false;
            }
        }

        
        public List<SectionNode> SelectedSections
        {
            get { return m_SelectedSectionListToMerge; }
        }
        public List<SectionNode> ListOfSections
        {
            get { return m_SectionList; }
        }
        public List<SectionNode> SelectedSectionsForIncreaseLevel
        {
            get { return m_SelectedSectionListForIncreaseLevel; }
        }
        public List<SectionNode> SelectedSectionsForDecreaseLevel
        {
            get { return m_SelectedSectionListForDecreaseLevel; }
        }
   

        private void populateListboxForSectionsToMerge()
        {
            SectionNode firstSection = m_SectionList[0];
            for (int i = 0; i <= (m_SectionList.Count - 1); i++)
            {
             // if (m_SectionList[i].Level >= firstSection.Level)
                {
                    m_SectionList[i].Label = m_SectionList[i].Label.Replace("\n", string.Empty);
                    m_lb_listofSectionsToMerge.Items.Add(m_SectionList[i].Label + " " + Localizer.Message("MergeOptions_LevelString") + m_SectionList[i].Level);
                   // m_SectionListAfterLevelChange.Add(m_SectionList[i]);
                }
                //else
                //    return;
            }
        }
      

        private void populateListboxForSectionsAfterLevelchange()
        {
            int j = 0;


            m_lb_listofSectionsToMerge.Items.Clear();
            SectionNode firstSection = m_SectionList[0];
            m_lb_listofSectionsToMerge.Refresh();
        

            for (int i = 0; i <= (m_SectionList.Count - 1); i++)
            {
             //  if (m_SectionListAfterLevelChange.Contains(m_SectionList[i]))
                {
                    if ((m_SectionList[i].IsRooted))
                    {
                        m_SectionList[i].Label = m_SectionList[i].Label.Replace("\n", string.Empty);
                        m_lb_listofSectionsToMerge.Items.Add(m_SectionList[i].Label + " " + Localizer.Message("MergeOptions_LevelString") + m_SectionList[i].Level);
                        if (m_IndexOfSectionSelected.Contains(i))
                        {
                            m_lb_listofSectionsToMerge.SelectedIndex = i;
                        }
                    }
                }
            }
            m_IndexOfSectionSelected.Clear();

        }
        private void populateListboxForSectionsOnUndo()
        {
            int j = 0;


            m_lb_listofSectionsToMerge.Items.Clear();
            SectionNode firstSection = m_SectionList[0];
            m_lb_listofSectionsToMerge.Refresh();


            for (int i = 0; i <= (m_SectionList.Count - 1); i++)
            {
                if ((m_SectionList[i].IsRooted))
                {
                    int n = m_SectionList.IndexOf(m_SectionList[i]);
                    m_SectionList[n].Label = m_SectionList[n].Label.Replace("\n", string.Empty);
                    m_lb_listofSectionsToMerge.Items.Add(m_SectionList[n].Label + " " + Localizer.Message("MergeOptions_LevelString") + m_SectionList[n].Level);
                    if (m_SectionList[i] == m_ProjectView.GetSelectedPhraseSection)
                    {
                        m_lb_listofSectionsToMerge.SelectedIndex = i;
                    }

                }
            }
        }

        private void populateListboxForSectionsAfterMerge()
        {
            int j = 0;


            m_lb_listofSectionsToMerge.Items.Clear();
            SectionNode firstSection = m_SectionList[0];
            bool flag = true;
            m_lb_listofSectionsToMerge.Refresh();

            List<SectionNode> tempSectionIndex = new List<SectionNode>();
            for (int i = 0; i <= (m_SectionList.Count - 1); i++)
            {
                if (!m_SelectedSectionListToMerge.Contains(m_SectionList[i]))
                {

                    if (m_SectionList[i].IsRooted)
                    {
                        m_SectionList[i].Label = m_SectionList[i].Label.Replace("\n", string.Empty);
                        m_lb_listofSectionsToMerge.Items.Add(m_SectionList[i].Label + " " + Localizer.Message("MergeOptions_LevelString") + m_SectionList[i].Level);

                    }
                    if (m_IndexOfSectionSelected[0] == i)
                    {
                        m_lb_listofSectionsToMerge.SelectedIndex = i;
                    }

                }
                else
                {
                    tempSectionIndex.Add(m_SectionList[i]);
                }
             }
             foreach (SectionNode node in tempSectionIndex)
             {
                 if (m_SectionList.Contains(node))
                 {
                     m_SectionList.Remove(node);
                 }
             }

            m_IndexOfSectionSelected.Clear();

        }

        private void m_lb_listofSectionsToMerge_SelectedIndexChanged(object sender, EventArgs e)
        {    
            if (!m_IsDeselected)
            {               
                List<SectionNode> tempList = new List<SectionNode>();
                tempList = listBoxSelectionIsContinuous();
                if (tempList.Count != 0)
                {
                    int LevelOfFirstSelectedSection = tempList[0].Level;

                    foreach (SectionNode secNode in tempList)
                    {
                        if (secNode.Level < LevelOfFirstSelectedSection)
                        {
                            MessageBox.Show(Localizer.Message("MergeOptions_CannotSelect"), Localizer.Message("Caption_Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            m_lb_listofSectionsToMerge.SelectedItems.Clear();
                            return;
                        }

                    }
                }
              //  List<SectionNode> listOfSelectedSections = new List<SectionNode>();
                SectionNode FirstSection = null;
                if (m_lb_listofSectionsToMerge.SelectedItems.Count != 0)
                {
                    int k = m_lb_listofSectionsToMerge.SelectedIndices[0];
                    if (k == 0)
                    {
                        FirstSection = m_SectionList[0];
                    }
                }

                if (FirstSection != null && FirstSection == m_ProjectView.FirstSectionOfTOCView)
                {
                    m_btn_IncreaseSectionLevel.Enabled = false;
                    m_btn_DecreaseSectionLevel.Enabled = false;
                }
                else
                {

                    m_btn_IncreaseSectionLevel.Enabled = true;
                    m_btn_DecreaseSectionLevel.Enabled = true;
                }
                if (m_RemainingIndexes.Count < 1)
                { return; }
                else
                {
                    for (int l = 0; l < m_RemainingIndexes.Count; l++)
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
                m_tb_SectionsSelected.Text = m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("select_one_more_section"), m_lb_listofSectionsToMerge.SelectedItem);               
            }

            //if (m_lb_listofSectionsToMerge.SelectedIndices.Count > 0)
            //    m_tb_SelectedSection.Text = m_SectionList[m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedItems.Count - 1]].ToString();

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
                foreach (List<int> list in m_ListOfContinuousItems)
                {
                    if (list.Count > listOfIndexOfLargestNumberOfSection.Count)
                        listOfIndexOfLargestNumberOfSection = list;
                }

                if (listOfIndexOfLargestNumberOfSection.Count == 1)
                { }
                else
                    m_ListOfContinuousItems.Clear();


                for (int i = 0; i <= listOfIndexOfLargestNumberOfSection.Count - 1; i++)
                { listOfLargestNumberOfSections.Add(m_SectionList[listOfIndexOfLargestNumberOfSection[i]]); }

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
                        m_tb_SectionsSelected.Text = m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("select_one_more_section"), m_lb_listofSectionsToMerge.SelectedItem);
                    }
                    else
                    {
                        m_tb_SectionsSelected.Text = m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("merged_sections"), listOfLargestNumberOfSections[0].Label, listOfLargestNumberOfSections[listOfLargestNumberOfSections.Count - 1].Label);
                      
                    }

                    //    MessageBox.Show(String.Format("Merged sections will be from {0} to {1} ", newList[0], newList[newList.Count - 1]));
                }
                else
                {
                    MessageBox.Show(Localizer.Message("phrase_count_more_than_7000"));
                    m_tb_SectionsSelected.Text = m_StatusLabelForMergeSection.Text = String.Format(Localizer.Message("phrase_count_more_than_7000"));
                    listOfLargestNumberOfSections = null;
                }
            }

            //if (m_lb_listofSectionsToMerge.SelectedIndices.Count > 0)
            //    m_tb_SelectedSection.Text = m_SectionList[m_lb_listofSectionsToMerge.SelectedIndices[m_lb_listofSectionsToMerge.SelectedItems.Count - 1]].ToString();
            return listOfLargestNumberOfSections;
        }

        private void m_btn_SelectAll_Click(object sender, EventArgs e)
        {
            int totalPhraseCount = 0;

            m_ListOfContinuousItems.Clear();
            if (m_ProjectView.TransportBar.IsPlayerActive)
            {
                m_ProjectView.TransportBar.Stop();
            }
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
            m_tb_SectionsSelected.AccessibleName = m_StatusLabelForMergeSection.AccessibleName = String.Format(Localizer.Message("merged_sections"), m_SectionList[0].Label, m_SectionList[m_lb_listofSectionsToMerge.SelectedItems.Count - 1].Label);

            if (m_SectionList.Contains(m_ProjectView.FirstSectionOfTOCView))
            {
                m_btn_IncreaseSectionLevel.Enabled = false;
                m_btn_DecreaseSectionLevel.Enabled = false;
            }
            else
            {

                m_btn_IncreaseSectionLevel.Enabled = true;
                m_btn_DecreaseSectionLevel.Enabled = true;
            }

        }

        private void m_btnMerge_Click(object sender, EventArgs e)
        {
            m_Merge = true;
           SectionsSelected();        
        }

        private void m_btn_IncreaseSectionLevel_Click(object sender, EventArgs e)
        {
            m_Merge = false;
            SectionsSelected();
            m_SelectedSectionListForIncreaseLevel = m_SelectedSectionList;
            if (LevelIncrementEvent != null)
            {
                LevelIncrementEvent(this, new EventArgs());
            }
            populateListboxForSectionsAfterLevelchange();        
        }
        private void SectionsSelected()
        {
            List<SectionNode> listOfSelectedSections = new List<SectionNode>();
            for (int i = 0; i < m_lb_listofSectionsToMerge.SelectedItems.Count; i++)
            {              
                int k = m_lb_listofSectionsToMerge.SelectedIndices[i];
                for (int j = 0; j < m_SectionList.Count; j++)
                {
                    if (k == j)
                    {
                        listOfSelectedSections.Add((SectionNode)m_SectionList[j]);
                        m_IndexOfSectionSelected.Add(j);
                    }
                }
            }
            if (m_Merge)
            {               
                m_SelectedSectionList = listBoxSelectionIsContinuous();
                m_SelectedSectionListToMerge = m_SelectedSectionList;
            }
            else
            {
                m_SelectedSectionList = listOfSelectedSections;
                if (listOfSelectedSections.Count < 1)
                {
                    MessageBox.Show(Localizer.Message("MergeOptions_NoSelectionForLevelChange"));
                }
            }



            if (m_SelectedSectionListToMerge != null)
            {
                if (m_Merge)
                {
                    if (m_SelectedSectionListToMerge.Count <= 1)
                    {
                        MessageBox.Show(Localizer.Message("not_enough_sections_to_merge"));
                        m_FlagMerge = false;
                        return;
                    }
                    m_FlagMerge = true;
                    MessageBox.Show(String.Format(Localizer.Message("merged_sections"), m_SelectedSectionListToMerge[0].Label, m_SelectedSectionListToMerge[m_SelectedSectionListToMerge.Count - 1].Label));
                }
            }
            else
                return;
        }

        private void m_btn_DecreaseSectionLevel_Click(object sender, EventArgs e)
        {
            m_Merge = false;
            SectionsSelected();
            m_SelectedSectionListForDecreaseLevel = m_SelectedSectionList;
            if (LevelDecrementEvent != null)
            {
                LevelDecrementEvent(this, new EventArgs());
            }
            populateListboxForSectionsAfterLevelchange();
        }

        private void m_btn_Play_Click(object sender, EventArgs e)
        {
            m_Merge = false;
            SectionsSelected();
            if (m_ProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused)
            {
                m_ProjectView.TransportBar.PlayOrResume();
            }
            else
            {
                if (m_SelectedSectionList.Count != 0)
                {
                     m_tempSectionNode = m_SelectedSectionList[m_SelectedSectionList.Count - 1];
                    //     m_ProjectView.Selection = new NodeSelection(m_tempSectionNode, m_ContentView);
                    m_ProjectView.Selection = null;
                  //  m_ProjectView.TransportBar.PlayOrResume(m_SelectedSectionList[m_SelectedSectionList.Count - 1]);
                    m_ProjectView.TransportBar.PlayHeadingPhrase(m_SelectedSectionList[m_SelectedSectionList.Count - 1]);
                }
            }
            if (m_SelectedSectionList.Count != 0 && m_SelectedSectionList[m_SelectedSectionList.Count - 1] != null
                && m_SelectedSectionList[m_SelectedSectionList.Count - 1].Duration > 0)
            {
                m_btnPause.Visible = true;
                m_btn_Play.Visible = false;
            }
        }

        private void m_btn_Stop_Click(object sender, EventArgs e)
        {
            if (m_ProjectView.TransportBar.CanStop) m_ProjectView.TransportBar.Stop();

            m_Merge = false;
            m_btnPause.Visible = false;
            m_btn_Play.Visible = true;
        }

        private void m_btnPause_Click(object sender, EventArgs e)
        {
            if (m_tempSectionNode != null && m_ContentView != null)
            {
                m_ProjectView.Selection = new NodeSelection(m_tempSectionNode, m_ContentView);
            }
            m_ProjectView.TransportBar.Pause();
            m_btnPause.Visible = false;
            m_btn_Play.Visible = true;
        }

     

        private void m_btn_Undo_Click(object sender, EventArgs e)
        {
           if (UndoChangeEvent != null)
            {
                UndoChangeEvent(this, new EventArgs());
                UpdateList();                   
            }
            populateListboxForSectionsOnUndo();
            if (m_lb_listofSectionsToMerge.SelectedIndex == -1)
            {
                m_StatusLabelForMergeSection.Text = "";
                m_tb_SectionsSelected.Text = "";
            }
        }

        private void m_btn_Merge_Click(object sender, EventArgs e)
        {
            m_Merge = true;  
            if (m_ProjectView.TransportBar.IsPlayerActive)
            {
                m_ProjectView.TransportBar.Stop();
            }
            SectionsSelected();
            if (m_FlagMerge)
            {
                if (MergeSectionEvent != null)
                {
                    MergeSectionEvent(this, new EventArgs());
                    UpdateList();
                }
                int count = 0;
                populateListboxForSectionsAfterMerge();
            }
            if (m_lb_listofSectionsToMerge.SelectedIndex == -1)
            {
                m_StatusLabelForMergeSection.Text = "";
                m_tb_SectionsSelected.Text = "";
            }
        }

        private void m_btn_Close_Click(object sender, EventArgs e)
        {
            if (m_ProjectView.TransportBar.CanStop) m_ProjectView.TransportBar.Stop();
            Close();
        }
    }
}