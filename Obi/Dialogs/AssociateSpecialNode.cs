using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AssociateSpecialNode : Form
    {
        private ProjectView.ProjectView m_View;
        private int m_AnchorNodeIndex = 0;
        private int m_SpecialNodeIndex = 0;
        private bool m_IsShowAll = false;
        List<EmptyNode> listOfFirstNodeOfSpecialNodes = new List<EmptyNode>();
       
        public AssociateSpecialNode(ProjectView.ProjectView projView)
        {
            m_View = projView;
            InitializeComponent();
            m_txtBox_SectionName.Text = m_View.Selection.ToString();
            AddToListBox();           
        }

        private void m_btn_ShowAll_Click(object sender, EventArgs e)
        {
            m_IsShowAll = true;
            m_lb_listOfAllAnchorNodes.Visible = true;
            AddToListBox();            
        }

        public void AddToListBox()
        {
            int firstIndex = -1;
            int lastIndex = -1;
            List<SectionNode> listOfAllSections = new List<SectionNode>();
            listOfAllSections = ((ObiRootNode)m_View.Presentation.RootNode).GetListOfAllSections();
            foreach (SectionNode node in listOfAllSections)
            {
                for (int i = 0; i < node.PhraseChildCount; i++)
                {
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Custom)
                    {
                        if (node.PhraseChild(i).CustomRole == "Footnote")
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(firstIndex));
                            }
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1 )
                            {
                                if(m_IsShowAll)
                                    m_lb_listOfAllAnchorNodes.Items.Add(node.Label);
                                else
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Footnote " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }
                            
                        }
                        if (node.PhraseChild(i).CustomRole == "Sidebar")
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(firstIndex));
                            }
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                             
                                if (m_IsShowAll)
                                    m_lb_listOfAllAnchorNodes.Items.Add(node.Label);
                                else
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Sidebar " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }                            
                        }
                        if (node.PhraseChild(i).CustomRole == "Annotation")
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(firstIndex));
                            }
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {

                                if (m_IsShowAll)
                                    m_lb_listOfAllAnchorNodes.Items.Add(node.Label); 
                                else
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Annotation " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }                            
                        }
                        if (node.PhraseChild(i).CustomRole == "Producer note")
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(firstIndex));
                            }
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                                if (m_IsShowAll)
                                    m_lb_listOfAllAnchorNodes.Items.Add(node.Label);
                                else
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Producer note " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }
                        }
                        if (node.PhraseChild(i).CustomRole == "End note")
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(firstIndex));
                            }
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                                if (m_IsShowAll)
                                    m_lb_listOfAllAnchorNodes.Items.Add(node.Label);
                                else
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " End note " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }                            
                        }

                    }
                    else if (firstIndex != -1)
                    {
                        if (lastIndex == -1)
                        {
                            lastIndex = firstIndex;
                            listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(firstIndex));
                            
                        }
                        if (!m_IsShowAll)
                        {
                            if (node.PhraseChild(firstIndex).CustomRole == "Footnote")
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Footnote " + (firstIndex + 1) + " to " + (lastIndex + 1));
                            if (node.PhraseChild(firstIndex).CustomRole == "Sidebar")
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Sidebar " + (firstIndex + 1) + " to " + (lastIndex + 1));
                            if (node.PhraseChild(firstIndex).CustomRole == "Annotation")
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Annotation " + (firstIndex + 1) + " to " + (lastIndex + 1));
                            if (node.PhraseChild(firstIndex).CustomRole == "Producer note")
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Producer note " + (firstIndex + 1) + " to " + (lastIndex + 1));
                            if (node.PhraseChild(firstIndex).CustomRole == "End note")
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " End note " + (firstIndex + 1) + " to " + (lastIndex + 1));
                        }
                        if(m_IsShowAll)
                        m_lb_listOfAllAnchorNodes.Items.Add(node.Label);

                        firstIndex = -1;
                        lastIndex = -1;
                    }
                }
            }
        }

        private void m_btn_Associate_Click(object sender, EventArgs e)
        {
            ((EmptyNode)m_View.Selection.Node).AssociatedNode = listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex];
                                         
            if (m_lb_ListOfSpecialNodes.Items[m_AnchorNodeIndex] == "")
            {
                m_lb_ListOfSpecialNodes.Items.Remove(m_lb_ListOfSpecialNodes.Items[m_AnchorNodeIndex]);
                m_lb_ListOfSpecialNodes.Items.Insert(m_AnchorNodeIndex, m_lb_ListOfSpecialNodes.Items[m_SpecialNodeIndex - 1]);               
            }
        }

        private void m_btn_Deassociate_Click(object sender, EventArgs e)
        {
             ((EmptyNode)m_View.Selection.Node).AssociatedNode = null;
             m_lb_ListOfSpecialNodes.Items.Insert(m_lb_ListOfSpecialNodes.SelectedIndex, ""); 
             m_lb_ListOfSpecialNodes.Items.Remove(m_lb_ListOfSpecialNodes.SelectedItem);
        }

        private void m_lb_ListOfSpecialNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_SpecialNodeIndex = m_lb_ListOfSpecialNodes.SelectedIndex;            
        }

        private void m_lb_listOfAllAnchorNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_AnchorNodeIndex = m_lb_listOfAllAnchorNodes.SelectedIndex;
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            m_View.SelectedBlockNode = ((EmptyNode)m_View.Selection.Node).AssociatedNode ;
        }
    }
}