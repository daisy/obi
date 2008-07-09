using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ProjectStatistics : Form
    {
        private ProjectView.ProjectView m_View;

        public ProjectStatistics( ProjectView.ProjectView View )
        {
            InitializeComponent();

            m_View = View;
                    }

        private void ProjectStatistics_Load(object sender, EventArgs e)
        {
            m_txtTitle.Text = m_View.Presentation.Title;
            LoadProjectDuration();
            LoadSectionsAndPhraseCount();
                    }

        private void LoadProjectDuration()
        {
            uint seconds = Convert.ToUInt32 ( m_View.Presentation.RootNode.Duration / 1000 );
            uint minutes = seconds / 60;
            uint hours = minutes / 60;
            seconds = seconds % 60;
            m_txtDuration.Text = hours.ToString() + " hours, " + minutes.ToString() + " minutes, " + seconds.ToString() + " seconds";
        }

        private void LoadSectionsAndPhraseCount ()
        {
            SectionNode node     = m_View.Presentation.FirstSection ;
            EmptyNode FirstEmptyNode = null;
            uint SectionCount = 0 ;
            if ( node != null )
            {
                SectionCount++ ;

                while (node.FollowingSection!= null)
                {
                    SectionCount++;
                    if (FirstEmptyNode == null &&  node.PhraseChildCount > 0) FirstEmptyNode = node.PhraseChild(0);
                    node = node.FollowingSection;
                }
                            }
            m_txtSectionsCount.Text = SectionCount.ToString();

            if (FirstEmptyNode != null )
            LoadPhraseAndPageCount(FirstEmptyNode);
            }

        private void LoadPhraseAndPageCount(EmptyNode node  )
        {
                        uint PhraseCount = 0;
            uint PageCount = 0;

            if (node != null)
            {
                PhraseCount++;
                if (node.NodeKind == EmptyNode.Kind.Page) PageCount++;
                                while (node.FollowingNode != null)
                {
                                                        PhraseCount++;
                    if (node.NodeKind == EmptyNode.Kind.Page) PageCount++;
                    node =(EmptyNode) node.FollowingNode;
                }
            }
            m_txtPhraseCount.Text = PhraseCount.ToString() ;
            m_txtPageCount.Text = PageCount.ToString();
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}