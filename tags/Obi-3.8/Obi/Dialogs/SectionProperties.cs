using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class SectionProperties : Form
    {
        private SectionNode mNode;              // section node being described
        private ProjectView.ProjectView mView;  // current view
        private double m_TotalSectionTime;

 
        /// <summary>
        /// Create a section properties dialog to be shown by ShowDialog() for the given view.
        /// </summary>
        public SectionProperties(ProjectView.ProjectView View)
        {
            InitializeComponent();
            
            mView = View;
            mNode = View.SelectedNodeAs<SectionNode>();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Sections/Section Properties.htm");
        }


        /// <summary>
        /// Get the new section title from the combo box.
        /// </summary>
        public string Label { get { return m_txtName.Text; } }

        /// <summary>
        /// Get the new level from the combo box.
        /// </summary>
        public int Level 
            { 
            get 
                {
                if (m_comboLevel.SelectedIndex >= 0 && m_comboLevel.SelectedIndex < m_comboLevel.Items.Count )
                    {
                                        return Convert.ToInt32 ( m_comboLevel.Items[m_comboLevel.SelectedIndex] );
                    }
                else
                    return mNode.Level;
                } 
            }

        /// <summary>
        /// Get the section node for which the information is displayed.
        /// </summary>
        public SectionNode Node { get { return mNode; } }

        /// <summary>
        /// Get the used checkbox value.
        /// </summary>
        public bool Used { get { return m_chkUsed.Checked; } }


        // Fill in the fields of the view when the form is loaded.
        private void SectionProperties_Load(object sender, EventArgs e)
        {
            m_txtName.Text = mNode.Label;
            int maxLevel = mNode.PrecedingSection == null ? 1 : mNode.PrecedingSection.Level + 1;
            for (int i = 1; i <= maxLevel; ++i) m_comboLevel.Items.Add(i);
            m_comboLevel.SelectedIndex = mNode.Level - 1;
            if (mNode.Level == 1)
            {
                m_lbParentsList.Items.Insert(0, Localizer.Message("top_level_section"));
            }
            else
            {
                for (SectionNode parent = mNode.ParentAs<SectionNode>(); parent != null; parent = parent.ParentAs<SectionNode>())
                {
                    m_lbParentsList.Items.Insert(0, string.Format(Localizer.Message("section_level"),
                        parent.Label, parent.Level));
                }
            }            
            m_txtTimeLength.Text = Program.FormatDuration_Long(mNode.Duration);
            m_txtPhraseCount.Text = mNode.PhraseChildCount.ToString();
            m_chkUsed.Checked = mNode.Used;

            if (mView != null && mView.Selection != null && mView.Selection.Node is SectionNode)
            {
                SectionNode secNode = (SectionNode)mView.Selection.Node;
                if (secNode != null && secNode.PrecedingNode != null && secNode.PrecedingSection is SectionNode)
                {
                    CalculateSectionTime((SectionNode)secNode.PrecedingSection);
                }
            }
            m_txtSectionTimePosition.Text = Program.FormatDuration_Long(m_TotalSectionTime);
        }
        private void CalculateSectionTime(SectionNode secNode)
        {

            m_TotalSectionTime += secNode.Duration;
            if (secNode.PrecedingNode != null && secNode.PrecedingSection is SectionNode)
            {
                CalculateSectionTime((SectionNode)secNode.PrecedingSection);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
