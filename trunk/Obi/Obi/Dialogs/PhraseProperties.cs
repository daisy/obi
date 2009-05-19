using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Obi.Dialogs
{
    public partial class PhraseProperties : Form
        {
        private EmptyNode mNode;                // the node to show the information for.
        private ProjectView.ProjectView mView;  // the current view.
        private bool m_IsSetCustomClass;
        private bool m_IsPageChange ;

        /// <summary>
        /// Create the dialog to be shown by ShowDialog() for the given view.
        /// </summary>
        public PhraseProperties(ProjectView.ProjectView view, bool SetCustomClass)
        {
            InitializeComponent();
            
            mView = view;
            mNode = view.SelectedNodeAs<EmptyNode>();

            m_IsSetCustomClass = SetCustomClass;
            m_IsPageChange = false;
        }


        /// <summary>
        /// Get the custom class value input by the user. Should be used only when role is set to custom.
        /// </summary>
        public string CustomClass
            {
            get
                {
                return m_comboCustomClassName.Text;
                }
            }

        /// <summary>
        /// Get the node under inspection.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }

        /// <summary>
        /// Get the role chosen from the drop-down menu.
        /// </summary>
        public EmptyNode.Role Role 
            { 
                        get 
                        {
                            if (m_comboPhraseRole.SelectedItem != null)
                                return ((LocalizedRole)m_comboPhraseRole.SelectedItem).Role;
                            else
                                return mNode.Role_ ;
                        } 
                    }

        /// <summary>
        /// Is True if page change is to be invoked
        /// </summary>
        public bool PageChange { get { return m_IsPageChange; } }

        /// <summary>
        /// Get the TODO flag from the checkbox.
        /// </summary>
        public bool TODO { get { return m_chkToDo.Checked; } }

        /// <summary>
        /// Get the used status from the checkbox.
        /// </summary>
        public bool Used { get { return m_chkUsed.Checked; } }


        // Fill out the fields when the form loads.
        private void PhraseProperties_Load ( object sender, EventArgs e )
            {
            m_txtParentSection.Text = mNode.AncestorAs<SectionNode> ().Label;
            m_txtLocationInsideSection.Text = string.Format ( Localizer.Message ( "node_position" ),
                mNode.Index + 1, mNode.ParentAs<ObiNode> ().PhraseChildCount );
            for (SectionNode parent = mNode.AncestorAs<SectionNode> (); parent != null; parent = parent.ParentAs<SectionNode> ())
                {
                m_lbParentsList.Items.Insert ( 0, string.Format ( Localizer.Message ( "section_level" ),
                    parent.Label, parent.Level ) );
                }
            m_txtTimeLength.Text = Program.FormatDuration_Long ( mNode.Duration );
            if (mView.CanAssignHeadingRole || mNode.Role_ == EmptyNode.Role.Heading) m_comboPhraseRole.Items.Add ( EmptyNode.LOCALIZED_HEADING );
            if (mView.CanAssignARole || mNode.Role_ == EmptyNode.Role.Page ) m_comboPhraseRole.Items.Add ( EmptyNode.LOCALIZED_PAGE );
            if ( mView.CanAssignPlainRole || mNode.Role_ == EmptyNode.Role.Plain)  m_comboPhraseRole.Items.Add ( EmptyNode.LOCALIZED_PLAIN );
            if ( mView.CanAssignSilenceRole || mNode.Role_ == EmptyNode.Role.Silence)  m_comboPhraseRole.Items.Add ( EmptyNode.LOCALIZED_SILENCE );
            if ( mView.CanAssignARole ) m_comboPhraseRole.Items.Add ( EmptyNode.LOCALIZED_CUSTOM );
            m_comboPhraseRole.SelectedItem = EmptyNode.LocalizedRoleFor ( mNode.Role_ );
            
            // load custom class combobox
            foreach (string customType in mView.Presentation.CustomClasses) m_comboCustomClassName.Items.Add ( customType );

            m_comboCustomClassName.Text = mNode.Role_ == EmptyNode.Role.Custom ? mNode.CustomRole : "";
            m_chkUsed.Checked = mNode.Used;
            m_chkToDo.Checked = mNode.TODO;

            if (mNode.Role_ == EmptyNode.Role.Page)
                {
                m_lblPageNumberDetails.Visible = true;
                m_comboPageNumberDetails.Visible = true;
                m_comboPageNumberDetails.Items.Add ( string.Concat ( mNode.PageNumber.Kind.ToString (), ": ", mNode.PageNumber.Number.ToString () ) );
                m_comboPageNumberDetails.Items.Add ( Localizer.Message("PhraseProperties_ChangePage"));
                m_comboPageNumberDetails.SelectedIndex = 0;
                }

            EnableCustomClassField ();
            if (m_IsSetCustomClass) SetCustomClassOnLoad ();
                        }

        // Turn the custom class field on/off depending on the role (i.e. turned on only for custom classes.)
        private void EnableCustomClassField ()
            {
            m_comboCustomClassName.Enabled =
                m_comboPhraseRole.SelectedItem == EmptyNode.LOCALIZED_CUSTOM;
            }

        private void SetCustomClassOnLoad ()
            {
            m_comboPhraseRole.SelectedIndex = m_comboPhraseRole.Items.Count - 1;
            m_comboCustomClassName.Enabled = true;

            backgroundWorker1.RunWorkerAsync ();
                                    }

        // Update the custom class text box when the role changes.
        private void m_comboPhraseRole_SelectionChangeCommitted ( object sender, EventArgs e )
            {
            EnableCustomClassField ();

            if (m_comboPhraseRole.SelectedItem == EmptyNode.LOCALIZED_PAGE && mNode.Role_ == EmptyNode.Role.Page)
                {
                m_lblPageNumberDetails.Visible = true;
                m_comboPageNumberDetails.Visible = true;
                m_comboPageNumberDetails.SelectedIndex = 0;
                }
            else
                {
                m_lblPageNumberDetails.Visible = false;
                m_comboPageNumberDetails.Visible = false;

                if (m_comboPhraseRole.SelectedItem == EmptyNode.LOCALIZED_PAGE)
                    {
                    m_IsPageChange = true;
                    }
                }
                        }

        private void backgroundWorker1_RunWorkerCompleted ( object sender, RunWorkerCompletedEventArgs e )
            {
            Thread.Sleep ( 500 );
            m_comboCustomClassName.Focus ();
            }

        private void m_lbParentsList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void m_comboPageNumberDetails_SelectionChangeCommitted ( object sender, EventArgs e )
            {
            m_IsPageChange = 
                m_comboPageNumberDetails.SelectedIndex == 1 ? true : false;
            }


        }
}