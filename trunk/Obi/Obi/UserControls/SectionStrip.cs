using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.UserControls
{
    public partial class SectionStrip : UserControl
    {
        private StripManagerPanel mManager;  // the manager for this strip
        private CoreNode mNode;              // the core node for this strip

        public string Label
        {
            get
            {
                return mLabel.Text;
            }
            set
            {
                mLabel.Text = value;
            }
        }

        public StripManagerPanel Manager
        {
            set
            {
                mManager = value;
            }
        }

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
            set
            {
                mNode = value;
            }
        }

        public SectionStrip()
        {
            InitializeComponent();
        }

        private void SectionStrip_Click(object sender, EventArgs e)
        {
            if (mManager.SelectedNode == mNode)
            {
                StartRenaming();
            }
            else
            {
                mManager.SelectedNode = mNode;
            }
        }

        public void MarkSelected()
        {
            BackColor = Color.Gold;
        }

        public void MarkDeselected()
        {
            mTextBox.Visible = false;
            BackColor = Color.PaleGreen;
        }

        /// <summary>
        /// The strip has a normally invisible text box aligned with the label.
        /// When renaming, the text box is shown and initialized with the original label.
        /// The whole text is selected and the text box is given the focus so that the
        /// user can start editing right away.
        /// </summary>
        public void StartRenaming()
        {
            mTextBox.BackColor = BackColor;
            mTextBox.Text = "";
            mTextBox.SelectedText = mLabel.Text;
            mTextBox.Visible = true;
            mFlowLayoutPanel.Focus();
            mTextBox.Focus();
        }

        /// <summary>
        /// Leaving the text box updates the text property.
        /// </summary>
        private void mTextBox_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Leaving the text box...");
            mTextBox.Visible = false;
        }

        /// <summary>
        /// Typing return updates the text property; escape cancels the edit.
        /// </summary>
        private void mTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    UpdateText();
                    break;
                case Keys.Escape:
                    mTextBox.Visible = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Upate the text label from the text box input.
        /// If the input is empty, then do not change the text and warn the user.
        /// The manager is then asked to send a rename event.
        /// </summary>
        private void UpdateText()
        {
            mTextBox.Visible = false;
            if (mTextBox.Text != "")
            {
                mLabel.Text = mTextBox.Text;
                mManager.RenamedSectionStrip(this);
            }
            else
            {
                MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                    Localizer.Message("empty_label_warning_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void AppendAudioBlock(AudioBlock block)
        {
            mFlowLayoutPanel.Controls.Add(block);
        }
    }
}
