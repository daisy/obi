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
        }

        public string ProjectTitle { get { return m_txtTitle.Text; } }


        private void ProjectStatistics_Load(object sender, EventArgs e)
        {
            m_txtTitle.Text = mView.Presentation.Title;
            m_txtDuration.Text = Program.FormatDuration_Long(mView.Presentation.RootNode.Duration);
            m_txtSectionsCount.Text = mView.Presentation.RootNode.SectionCount.ToString();
            m_txtPhraseCount.Text = mView.Presentation.RootNode.PhraseCount.ToString();
            m_txtPageCount.Text = mView.Presentation.RootNode.PageCount.ToString();
        }
    }
}