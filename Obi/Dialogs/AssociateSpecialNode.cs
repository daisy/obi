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
        public AssociateSpecialNode(ProjectView.ProjectView projView)
        {
            InitializeComponent();
            m_View = projView;
            textBox1.Text = m_View.Selection.ToString();
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
                                firstIndex = i;
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Footnote " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }
                        }
                        if (node.PhraseChild(i).CustomRole == "Sidebar")
                        {
                            if (firstIndex == -1)
                                firstIndex = i;
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Sidebar " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }
                        }
                        if (node.PhraseChild(i).CustomRole == "Annotation")
                        {
                            if (firstIndex == -1)
                                firstIndex = i;
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Annotation " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }
                        }
                        if (node.PhraseChild(i).CustomRole == "Producer note")
                        {
                            if (firstIndex == -1)
                                firstIndex = i;
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Producer note " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }
                        }
                        if (node.PhraseChild(i).CustomRole == "End note")
                        {
                            if (firstIndex == -1)
                                firstIndex = i;
                            else
                                lastIndex = i;
                            if (i == node.PhraseChildCount - 1)
                            {
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " End note " + (firstIndex + 1) + " to " + (lastIndex + 1));
                                firstIndex = -1;
                                lastIndex = -1;
                            }
                        } 

                    }
                    else if (firstIndex != -1)
                    {
                        if (lastIndex == -1)
                            lastIndex = firstIndex;
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

                        firstIndex = -1;
                        lastIndex = -1;
                    }
                }
            }
        }      

    }
}

