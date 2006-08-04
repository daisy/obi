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
        //private StripManagerPanel mManager;  // the manager for this block
        //private CoreNode mNode;              // the phrase node for this block

        #region properties
        //public StripManagerPanel Manager
        //{
        //    set
        //    {
        //        mManager = value;
        //    }
        //}

        //public CoreNode Node
        //{
        //    get
        //    {
        //        return mNode;
        //    }
        //    set
        //    {
        //        mNode = value;
        //    }
        //}

        public string Label
        {
            get
            {
                return mAnnotationLabel.Text;
            }
            set
            {
                mAnnotationLabel.Text = value;
            }
        }

        public string Time
        {
            set
            {
                mTimeLabel.Text = value;
            }
        }
#endregion
        
        #region instantiators
        public AudioBlock() : base()
        {
            //mg:
            this.TabStop = true;  
            InitializeComponent();
        }
        #endregion

        #region AudioBlock (this)
        internal void MarkDeselected()
        {
            BackColor = Color.MistyRose;
        }

        internal void MarkSelected()
        {
            BackColor = Color.LightPink;
        }

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            mManager.SelectedPhrase = mNode; 
        }

        private void AudioBlock_DoubleClick(object sender, EventArgs e)
        {
            mManager.SelectedPhrase = mNode;
            Dialogs.Play dialog = new Dialogs.Play(mNode);
            dialog.ShowDialog();
        }

        //mg: for tab navigation et al
        private void AudioBlock_enter(object sender, EventArgs e)
        {
            mManager.SelectedPhrase = mNode;
            System.Diagnostics.Debug.Print("Audioblock:tabindex:" + this.TabIndex.ToString());
            MarkSelected();
        }

        //mg: for tab navigation et al
        private void AudioBlock_leave(object sender, EventArgs e)
        {
            MarkDeselected();
        }
#endregion

        #region Rename Box

        internal void StartRenaming()
        {
            mRenameBox.BackColor = BackColor;
            mRenameBox.Text = "";
            mRenameBox.SelectedText = ((TextMedia)Project.GetMediaForChannel(mNode, Project.AnnotationChannel)).getText();
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
        /// If the input is empty, then do not change the text and warn the user.
        /// The manager is then asked to send a rename event.
        /// </summary>
        private void UpdateText()
        {
            mRenameBox.Visible = false;
            if (mRenameBox.Text != "")
            {
                mAnnotationLabel.Text = mRenameBox.Text;
                mManager.RenamedAudioBlock(this);
            }
            else
            {
                MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                    Localizer.Message("empty_label_warning_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion
        

    }
}
