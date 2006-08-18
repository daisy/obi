using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    public partial class AudioBlock : AbstractBlock 
    {
        private StructureBlock mStructureBlock;  // corresponding structure block

        #region properties

        public override CoreNode Node
        {
            get
            {
                return mNode;
            }
            set
            {
                mNode = value;
                CoreNode structureNode = Project.GetStructureNode(mNode);
                if (structureNode == null) mStructureBlock.Node = value;
            }
        }

        public override StripManagerPanel Manager
        {
            get
            {
                return mManager;
            }
            set
            {
                mManager = value;
                mStructureBlock.Manager = value;
            }
        }

        public StructureBlock StructureBlock
        {
            get
            {
                return mStructureBlock;
            }
            set
            {
                mStructureBlock = value;
                mStructureBlock.AudioBlock = this;
            }
        }

        public string Label
        {
            get { return mAnnotationLabel.Text; }
            set { mAnnotationLabel.Text = value; }
        }

        public string Time
        {
            set { mTimeLabel.Text = value; }
        }

        #endregion
        
        #region instantiators

        public AudioBlock() : base()
        {
            InitializeComponent();
            this.TabStop = true;  // mg (moved by JQ 20060817)
            mStructureBlock = new StructureBlock();
            mStructureBlock.AudioBlock = this;
        }

        #endregion

        #region AudioBlock (this)

        internal override void MarkDeselected()
        {
            BackColor = Color.MistyRose;
            mStructureBlock.MarkDeselected();
        }

        internal override void MarkSelected()
        {
            BackColor = Color.LightPink;
            mStructureBlock.MarkSelected();
        }

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            mManager.SelectedPhraseNode = mNode; 
        }

        private void AudioBlock_DoubleClick(object sender, EventArgs e)
        {
            mManager.SelectedPhraseNode = mNode;
            ((ObiForm)ParentForm).Play(mNode);
        }

        //mg: for tab navigation et al
        private void AudioBlock_enter(object sender, EventArgs e)
        {
            mManager.SelectedPhraseNode = mNode;
            System.Diagnostics.Debug.Print("Audioblock:tabindex:" + this.TabIndex.ToString());
            // MarkSelected();
        }

        //mg: for tab navigation et al
        private void AudioBlock_leave(object sender, EventArgs e)
        {
            // Removed by JQ--marking an item as selected/deselected is done through properties in the manager panel.
            //MarkDeselected();
        }

        #endregion

        #region Rename Box

        /// <summary>
        /// Bring up the editable text box.
        /// TODO: maybe the labels should be replaced by non-editable text boxes.
        /// There is also a bug that occurs when editing for the second time, when
        /// no text is selected... strange.
        /// </summary>
        internal void StartRenaming()
        {
            mRenameBox.Size = mAnnotationLabel.Size;
            mRenameBox.BackColor = BackColor;
            mRenameBox.Text = "";
            mRenameBox.SelectedText = mAnnotationLabel.Text;
            mRenameBox.Visible = true;
            mRenameBox.Focus();
        }

        /// <summary>
        /// Leaving the text box updates the text property.
        /// </summary>
        private void mRenameBox_Leave(object sender, EventArgs e)
        {
            mRenameBox.Visible = false;
        }

        /// <summary>
        /// Typing return updates the text property; escape cancels the edit.
        /// </summary>
        private void mRenameBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    UpdateText();
                    break;
                case Keys.Escape:
                    mRenameBox.Visible = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Upate the text label from the text box input.
        /// If the input is empty then do not change the text and warn the user.
        /// If the input text is the same as the original text, don't do anything.
        /// The manager is then asked to send a rename event.
        /// The rename event may not happen if there is already an audio block with the same name in the project.
        /// </summary>
        private void UpdateText()
        {
            mRenameBox.Visible = false;
            if (mRenameBox.Text != "" && mRenameBox.Text != mAnnotationLabel.Text)
            {
                mManager.EditedAudioBlockLabel(this, mRenameBox.Text);
            }
            else
            {
                MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                    Localizer.Message("empty_label_warning_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        /// <summary>
        /// Update the size of the structure block when the size changes.
        /// </summary>
        private void AudioBlock_SizeChanged(object sender, EventArgs e)
        {
            mStructureBlock._Width = Width;
        }

        /// <summary>
        /// Edit the page label for the structure block linked to this audio block.
        /// </summary>
        internal void StartEditingPageLabel()
        {
            mStructureBlock.StartEditingPageLabel();
        }
    }
}
