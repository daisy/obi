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
        private double m_TotalCursorTime;
        private bool m_IsPause = false;
        

        /// <summary>
        /// Create the dialog to be shown by ShowDialog() for the given view.
        /// </summary>
        public PhraseProperties(ProjectView.ProjectView view, bool SetCustomClass, bool showAdvanceProperties)
        {
            InitializeComponent();
            m_txtFileName.Visible = showAdvanceProperties;
            m_lbl_FileName.Visible = showAdvanceProperties;
            mView = view;
            mNode = view.SelectedNodeAs<EmptyNode>();

            m_IsSetCustomClass = SetCustomClass;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Phrases/Phrase Properties.htm");
            if (mView.TransportBar.IsPlayerActive)
            {
                m_IsPause = true;
            }
            if (mView.ObiForm.Settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(view.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
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
        public bool PageChange 
            { 
            get 
                { 
                return m_chkChangePageNumber.Checked
                    ||    ( m_comboPhraseRole.SelectedItem == EmptyNode.LOCALIZED_PAGE &&  mNode.Role_ != EmptyNode.Role.Page );
                } 
            }

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
            // SectionNode section = mView.Selection.Node.ParentAs<SectionNode>();

            if (mView != null && mView.Selection != null)
            {
              
                if (mView.Selection.Node is PhraseNode)
                {
                    PhraseNode phraseNode = (PhraseNode)mView.Selection.Node;

                    if (mView.Selection is AudioSelection)
                    {
                        if (((AudioSelection)mView.Selection).AudioRange != null && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
                        {
                            if (!m_IsPause)
                            {
                                m_TotalCursorTime += ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                            }
                        }
                    }

                    if (m_IsPause)
                    {
                        m_TotalCursorTime += mView.TransportBar.CurrentPlaylist.CurrentTimeInAsset;
                        m_IsPause = false;
                    }

                    if (phraseNode.PrecedingNode != null && phraseNode.Parent == phraseNode.PrecedingNode.Parent)
                    {
                        if (phraseNode.PrecedingNode is PhraseNode)
                            CalculateCursorTime((PhraseNode)phraseNode.PrecedingNode);
                        else if (phraseNode.PrecedingNode is EmptyNode)
                        {
                            FindPhraseNode((EmptyNode)phraseNode.PrecedingNode);
                        }
                    }

                    if (phraseNode.Parent is SectionNode)
                    {
                        if (((SectionNode)phraseNode.Parent).PrecedingSection != null)
                        {
                            CalculateSectionTime(((SectionNode)phraseNode.Parent).PrecedingSection);
                        }
                    }

                    //SectionNode secNode = (SectionNode)mView.Selection.Node;
                    //if (secNode != null)
                    //{
                    //    CalculateCursorTime((SectionNode)secNode.PrecedingNode);
                    //}
                }
                else if (mView.Selection.Node is EmptyNode)
                {

                    EmptyNode emptyNode = (EmptyNode)mView.Selection.Node;
                    FindPhraseNode(emptyNode);

                    if (emptyNode.Parent is SectionNode)
                    {
                        if (((SectionNode)emptyNode.Parent).PrecedingSection != null)
                        {
                            CalculateSectionTime(((SectionNode)emptyNode.Parent).PrecedingSection);
                        }
                    }
                }
            }
            // m_txtCurrentCursorPosition.Text = mView.TransportBar.CalculateTimeElapsedInSectionForProperties().ToString();
            m_txtCurrentCursorPosition.Text = Program.FormatDuration_Long(m_TotalCursorTime);
            if (mView.CanAssignHeadingRole || mNode.Role_ == EmptyNode.Role.Heading) m_comboPhraseRole.Items.Add(EmptyNode.LOCALIZED_HEADING);
            if (mView.CanAssignARole || mNode.Role_ == EmptyNode.Role.Page) m_comboPhraseRole.Items.Add(EmptyNode.LOCALIZED_PAGE);
            if (mView.CanAssignPlainRole || mNode.Role_ == EmptyNode.Role.Plain) m_comboPhraseRole.Items.Add(EmptyNode.LOCALIZED_PLAIN);
            if (mView.CanAssignSilenceRole || mNode.Role_ == EmptyNode.Role.Silence) m_comboPhraseRole.Items.Add(EmptyNode.LOCALIZED_SILENCE);
            if (mView.CanAssignAnchorRole || mNode.Role_ == EmptyNode.Role.Anchor) m_comboPhraseRole.Items.Add(EmptyNode.LOCALIZED_ANCHOR);
            if (mView.CanAssignARole) m_comboPhraseRole.Items.Add(EmptyNode.LOCALIZED_CUSTOM);

            m_comboPhraseRole.SelectedItem = EmptyNode.LocalizedRoleFor(mNode.Role_);

            // load custom class combobox
            foreach (string customType in mView.Presentation.CustomClasses) m_comboCustomClassName.Items.Add(customType);

            m_comboCustomClassName.Text = mNode.Role_ == EmptyNode.Role.Custom ? mNode.CustomRole : "";
            m_chkUsed.Checked = mNode.Used;
            m_chkToDo.Checked = mNode.TODO;

            if (mNode.Role_ == EmptyNode.Role.Page)
            {
                m_lblPageNumberDetails.Visible = true;
                m_txtPageNumberDetails.AccessibleName = m_lblPageNumberDetails.Text.Replace("&", "");
                m_txtPageNumberDetails.Visible = true;
                m_txtPageNumberDetails.Text = mNode.PageNumber.Kind.ToString() + ", #" + mNode.PageNumber.ToString();
                m_chkChangePageNumber.Visible = true;
            }

            else if (mNode.Role_ == EmptyNode.Role.Anchor)
            {
                m_lbl_ReferredNote.Visible = true;
                m_txtPageNumberDetails.AccessibleName = m_lbl_ReferredNote.Text.Replace("&", "");
                m_txtPageNumberDetails.Visible = true;
                m_txtPageNumberDetails.Text = mNode.AssociatedNode != null ? mNode.AssociatedNode.ParentAs<SectionNode>().Label + ", " + mNode.AssociatedNode.ToString() : "?";
            }

            EnableCustomClassField();
            if (m_IsSetCustomClass) SetCustomClassOnLoad();

            try
            {
                if (mNode is PhraseNode)
                {//1
                    PhraseNode phrase = (PhraseNode)mNode;
                    if (phrase.Audio.AudioMediaData is urakawa.media.data.audio.codec.WavAudioMediaData)
                    {//2
                        urakawa.media.data.audio.codec.WavAudioMediaData wavMedia = (urakawa.media.data.audio.codec.WavAudioMediaData)phrase.Audio.AudioMediaData;
                        string textboxString = "";
                        for (int i = 0; i < wavMedia.mWavClips.Count; i++)
                        {//3
                            if (i > 0) textboxString += ", ";
                            textboxString += ((urakawa.data.FileDataProvider)wavMedia.mWavClips[i].DataProvider).DataFileRelativePath;
                        }//-3
                        m_txtFileName.Text = textboxString;
                    }//-2
                }//-1
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FindPhraseNode(EmptyNode emptyNode)
        {
            //EmptyNode emptyNode = (EmptyNode)mView.Selection.Node;
            while (emptyNode.PrecedingNode != null && emptyNode.PrecedingNode is EmptyNode && (emptyNode.PrecedingNode.Parent == emptyNode.Parent))
            {
                emptyNode = (EmptyNode)emptyNode.PrecedingNode;
                if (emptyNode is PhraseNode)
                {
                    CalculateCursorTime((PhraseNode)emptyNode);
                    break;
                }
            }
        }
        private void CalculateCursorTime(PhraseNode phraseNode)
        {
            //if (m_IsPause && mView.Selection != null && mView.Selection is AudioSelection)
            //{
            //    AudioSelection audioSelection = (AudioSelection)mView.Selection;
            //    m_TotalCursorTime += audioSelection.AudioRange.CursorTime;
            //    m_IsPause = false;
            //}
            //else
            {
                m_TotalCursorTime += phraseNode.Duration;
            }
            if (phraseNode.PrecedingNode != null && phraseNode.PrecedingNode is PhraseNode && (phraseNode.PrecedingNode.Parent == phraseNode.Parent))
            {
                CalculateCursorTime((PhraseNode) phraseNode.PrecedingNode);
            }
            else if (phraseNode.PrecedingNode != null && phraseNode.PrecedingNode is EmptyNode && (phraseNode.PrecedingNode.Parent == phraseNode.Parent))
            {
                FindPhraseNode((EmptyNode)phraseNode.PrecedingNode);
            }

        }
        private void CalculateSectionTime(SectionNode secNode)
        {

            m_TotalCursorTime += secNode.Duration;
            if (secNode.PrecedingSection != null && secNode.PrecedingSection is SectionNode)
            {
                CalculateSectionTime((SectionNode)secNode.PrecedingSection);
            }
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
                m_txtPageNumberDetails.Visible = true;
                m_chkChangePageNumber.Visible = true;
                }
            else
                {
                m_lblPageNumberDetails.Visible = false;
                m_txtPageNumberDetails.Visible = false;
                m_chkChangePageNumber.Visible = false;
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


        }
}