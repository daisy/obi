using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Obi.Dialogs
{
    public partial class AutoPageGeneration : Form
    {
        private ProjectView.ProjectView m_ProjectView;
        private int m_StartingSectionIndex = 0;
        private List<SectionNode> m_sectionsList;
        public AutoPageGeneration(ProjectView.ProjectView ProjectView)
        {
            InitializeComponent();

            m_ProjectView = ProjectView;

            m_sectionsList = ((ObiRootNode)m_ProjectView.Presentation.RootNode).GetListOfAllSections();
            for (int i = 1; i <= m_sectionsList.Count; i++)
            {
                m_cbStartingSectionIndex.Items.Add(i);
            }
            if (m_cbStartingSectionIndex.Items.Count > 0)
            {
                m_cbStartingSectionIndex.SelectedIndex = 0;
            }
            
            
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            
            try
            {
                m_StartingSectionIndex = Convert.ToInt32(m_cbStartingSectionIndex.Text);
                m_StartingSectionIndex = m_StartingSectionIndex - 1;
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Please enter Integer Value");
                return;
            }
            StartingSection();
            AddPage();          
            this.Close();            

        }

        private void StartingSection()
        {
            if (m_ProjectView != null && m_ProjectView.Selection != null )
            {

                m_ProjectView.Selection.Node = m_sectionsList[m_StartingSectionIndex];

             }
     
        }
        private void AddPage()
        {
            int tempGapsInPages;
            try
            {
                tempGapsInPages = Convert.ToInt32(m_txtGapsInPages.Text);
                if (tempGapsInPages <= 0)
                {
                    MessageBox.Show("Please enter Value greater than 0");
                    return;
                }
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Please enter Value greater than 0");
                return;
            }
            tempGapsInPages = (tempGapsInPages * 60 * 1000);
            m_ProjectView.AddPageAutomatically(tempGapsInPages);
            if (m_rbGenerateTTS.Checked)
                m_ProjectView.GenerateSpeechForPage(true);
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
