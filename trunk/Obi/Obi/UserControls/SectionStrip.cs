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

        #region properties

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
            //mg
            get 
            {
                return mManager;
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

        #endregion

        #region instantiators
        public SectionStrip()
        {
            this.TabStop = true; //mg: not in designer for some reason            
            InitializeComponent();
        }
        #endregion

        #region TextBox (the label strip)

        /// <summary>
        /// The strip has a normally readonly text box at the top.
        /// When renaming, the text box is initialized with the original label.
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
        /// Using this eventhandler to override the TextBox's native ContextMenu with that in StripManagerPanel
        /// </summary>
        ///<remarks>We seem to have to do this at every mousedown, 
        ///else the first pop of contextmenu is that of the textbox 
        ///(windows does the redraw before the event).</remarks>
        private void mTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.mTextBox.ContextMenuStrip = this.Manager.PanelContextMenuStrip;
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
        
        #endregion
        
        #region SectionStrip (this)
        
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
        //mg: for tab navigation et al
        
        private void SectionStrip_leave(object sender, EventArgs e)
        {
            this.MarkDeselected();
        }

        //mg: for tab navigation et al
        private void SectionStrip_enter(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.Print("SectionStrip:tabindex:" + this.TabIndex.ToString());
            mManager.SelectedSection = mNode;            
            this.MarkSelected();
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
        /// Reflows the tab order (tabindex property)
        /// of blocks in this SectionStrip starting from the
        /// inparam block, continuing to the end of the strip.
        /// </summary>
        /// <param name="startBlock">The block to start from</param>
        /// <param name="prevIndex">The tabindex value to preemptively increase before setting on the first Block in this strip</param>
        /// <returns>The last (highest) tabindex added, if startBlock is not in strip, returns -1</returns>
        /// <remarks>Use this to reflow the taborder of a partial strip</remarks>
        //   added by mg 20060803

        internal int ReflowTabOrder(Control startBlock, int prevIndex)
        {
            try
            {
                for (int i = mFlowLayoutPanel.Controls.GetChildIndex(startBlock); i < this.mFlowLayoutPanel.Controls.Count; i++)
                {
                    Control c = this.mFlowLayoutPanel.Controls[i];
                    if (c is AudioBlock) //note: needs to be changed as block types are added
                    {
                        c.TabIndex = ++prevIndex;
                    }
                    else
                    {
                        try
                        {
                            c.TabStop = false;
                        }
                        catch (Exception)
                        {
                            //instead of reflection
                        }
                    }
                }//for
            }
            catch (Exception x)
            {
                //if startBlock was not in ControlCollection
                System.Diagnostics.Debug.Print("SectionStrip.ReflowTabOrder exception: " + x.Message);
                return -1;
            }
            return prevIndex;
        }

        /// <summary>
        /// Reflows the tab order (tabindex property)
        /// of all blocks in this SectionStrip.
        /// </summary>
        /// <param name="prevIndex">The tabindex value to preemptively increase before setting on the first Block in this strip</param>
        /// <returns>The last (highest) tabindex added, if no blocks are in strip, returns the inparam value</returns>
        /// <remarks>Use this to reflow the taborder of an entire strip</remarks>
        //   added by mg 20060803
        internal int ReflowTabOrder(int prevIndex)
        {
            if (mFlowLayoutPanel.Controls.Count > 0)
            {
                return this.ReflowTabOrder(mFlowLayoutPanel.Controls[0], prevIndex);
            }
            return prevIndex;
        }
        #endregion

        #region audio strip

        public void AppendAudioBlock(AudioBlock block)
        {
            mFlowLayoutPanel.Controls.Add(block);
        }

        /// <summary>
        /// Clicking in the audio strip (i.e. the flow layout) selects the strip but unselects the audio block.
        /// </summary>
        private void mFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            mManager.SelectedSection = mNode;
            mManager.SelectedPhrase = null;
        }
        #endregion

    }
}
