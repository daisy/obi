using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class PhraseProperties : Form
    {
        private EmptyNode mNode;                // the node to show the information for.
        private ProjectView.ProjectView mView;  // the current view.


        /// <summary>
        /// Create the dialog to be shown by ShowDialog() for the given view.
        /// </summary>
        public PhraseProperties(ProjectView.ProjectView view)
        {
            InitializeComponent();
            mView = view;
            mNode = view.SelectedNodeAs<EmptyNode>();
        }

        
        /// <summary>
        /// Get the custom class value input by the user. Should be used only when role is set to custom.
        /// </summary>
        public string CustomClass { get { return m_txtCustomClassName.Text; } }

        /// <summary>
        /// Get the node under inspection.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }

        /// <summary>
        /// Get the role chosen from the drop-down menu.
        /// </summary>
        public EmptyNode.Kind Role { get { return (EmptyNode.Kind)m_comboPhraseRole.SelectedItem; } }

        /// <summary>
        /// Get the TODO flag from the checkbox.
        /// </summary>
        public bool TODO { get { return m_chkToDo.Checked; } }

        /// <summary>
        /// Get the used status from the checkbox.
        /// </summary>
        public bool Used { get { return m_chkUsed.Checked; } }


        // Fill out the fields when the form loads.
        private void PhraseProperties_Load(object sender, EventArgs e)
        {
            m_txtParentSection.Text = mNode.AncestorAs<SectionNode>().Label;
            m_txtLocationInsideSection.Text = string.Format(Localizer.Message("node_position"),
                mNode.Index + 1, mNode.ParentAs<ObiNode>().PhraseChildCount);
            for (SectionNode parent = mNode.AncestorAs<SectionNode>(); parent != null; parent = parent.ParentAs<SectionNode>())
            {
                m_lbParentsList.Items.Insert(0, string.Format(Localizer.Message("section_level"),
                    parent.Label, parent.Level));
            }
            m_txtTimeLength.Text = Program.FormatDuration_Long(mNode.Duration);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Heading);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Page);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Plain);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Silence);
            m_comboPhraseRole.Items.Add(EmptyNode.Kind.Custom);
            m_comboPhraseRole.SelectedItem = mNode.NodeKind;
            m_txtCustomClassName.Text = mNode.NodeKind == EmptyNode.Kind.Custom ? mNode.CustomClass : "";
            m_chkUsed.Checked = mNode.Used;
            m_chkToDo.Checked = mNode.IsTo_Do;
            EnableCustomClassField();
        }

        // Turn the custom class field on/off depending on the role (i.e. turned on only for custom classes.)
        private void EnableCustomClassField()
        {
            m_txtCustomClassName.Enabled =
                (EmptyNode.Kind)m_comboPhraseRole.SelectedItem == EmptyNode.Kind.Custom;
        }

        // Update the custom class text box when the role changes.
        private void m_comboPhraseRole_SelectionChangeCommitted(object sender, EventArgs e)
        {
            EnableCustomClassField();
        }
    }
}