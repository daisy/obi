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
                return mTextBox.Text;
            }
            set
            {
                mTextBox.Text = value;
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
            this.TabStop = true; //mg: not in designer for some reason
            InitializeComponent();
        }

        private void SectionStrip_Click(object sender, EventArgs e)
        {
            if (mManager.SelectedSection == mNode)
            {
                //mg: changed renaming to not be 
                //default state at focus:
                //StartRenaming();
            }
            else
            {
                mManager.SelectedSection = mNode;
            }
        }

        public void MarkSelected()
        {
            BackColor = Color.Gold;
            mTextBox.BackColor = Color.Gold;
        }

        public void MarkDeselected()
        {
            BackColor = Color.PaleGreen;
            mTextBox.BackColor = Color.PaleGreen;
        }

        /// <summary>
        /// The strip has a normally invisible text box aligned with the label.
        /// When renaming, the text box is shown and initialized with the original label.
        /// The whole text is selected and the text box is given the focus so that the
        /// user can start editing right away.
        /// </summary>
        public void StartRenaming()
        {
            mTextBox.ReadOnly = false;
            mTextBox.BackColor = BackColor;
            mTextBox.SelectAll();
            mFlowLayoutPanel.Focus();
            mTextBox.Focus();
        }

        /// <summary>
        /// Leaving the text box updates the text property.
        /// </summary>
        private void mTextBox_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Leaving the text box...");
            mTextBox.ReadOnly = true;
        }

        /// <summary>
        /// Typing return updates the text property; escape cancels the edit.
        /// </summary>
        private void mTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    mTextBox.ReadOnly = true;
                    UpdateText();
                    break;
                case Keys.Escape:
                    mTextBox.Text = Project.GetTextMedia(this.Node).getText();
                    mTextBox.ReadOnly = true;
                    break;
                case Keys.F2:
                    if (mTextBox.ReadOnly)
                    {
                        this.StartRenaming();
                    }
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
            if (mTextBox.Text != "")
            {
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

        //mg: for tab navigation et al
        private void SectionStrip_leave(object sender, EventArgs e)
        {
            this.MarkDeselected();
        }

        //mg: for tab navigation et al
        private void SectionStrip_enter(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.Print("SectionStrip:tabindex:"+this.TabIndex.ToString());
            this.MarkSelected();
        } 
    }
}
