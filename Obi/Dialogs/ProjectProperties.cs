using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ProjectProperties : Form
    {
        private ProjectView.ProjectView mView;

        public ProjectProperties(ProjectView.ProjectView View)
        {
            InitializeComponent();
            mView = View;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Creating and Working with Projects/Project Properties.htm");
        }

        public string ProjectTitle { get { return m_txtTitle.Text; } }


        private void ProjectStatistics_Load(object sender, EventArgs e)
        {
            m_txtTitle.Text = mView.Presentation.Title;
            m_txtDuration.Text = Program.FormatDuration_Long(((ObiRootNode)mView.Presentation.RootNode).Duration);
            m_txtSectionsCount.Text = ((ObiRootNode)mView.Presentation.RootNode).SectionCount.ToString();
            m_txtPhraseCount.Text = ((ObiRootNode)mView.Presentation.RootNode).PhraseCount.ToString();
            m_txtPageCount.Text = ((ObiRootNode)mView.Presentation.RootNode).PageCount.ToString();
        }
    }
}