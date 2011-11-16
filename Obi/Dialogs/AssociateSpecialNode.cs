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
            MessageBox.Show(m_View.Selection.ToString());
            textBox1.Text = m_View.Selection.ToString();
            int j = 0;
            int k = 0;
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
                            Console.WriteLine("GOES HERE in FOOTNOTE  " + node);
                            if (j == 0)
                                j = i;
                            else
                                k = i;
                        }
                        if (node.PhraseChild(i).CustomRole == "Sidebar")
                        {
                            Console.WriteLine("GOES HERE  in SIDE BAR  " + node);
                            if (j == 0)
                                j = i;
                            else
                                k = i;
                        }
                    }
                    else if (j != 0)
                    {
                        if (k == 0)
                            k = j;
                        if (node.PhraseChild(j).CustomRole == "Footnote")
                            m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Footnote " + (j + 1) + " to " + (k + 1));
                        if (node.PhraseChild(j).CustomRole == "Sidebar")
                            m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " Sidebar " + (j + 1) + " to " + (k + 1));
                        j = 0;
                        k = 0;
                    }
                }
            }
        }      

    }
}