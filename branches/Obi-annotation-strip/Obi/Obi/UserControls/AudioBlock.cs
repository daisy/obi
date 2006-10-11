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
        //md 20061009 - added annotation blocks to replace annotation text field on audio blocks
        private AnnotationBlock mAnnotationBlock;

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
                mAnnotationBlock.Manager = value;
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

        public AnnotationBlock AnnotationBlock
        {
            get
            {
                return mAnnotationBlock;
            }
            set
            {
                mAnnotationBlock = value;
                mAnnotationBlock.AudioBlock = this;
            }
        }

        public string Time
        {
            set { mTimeLabel.Text = value; }
        }

        //md20061011
        public int _Width
        {
            set { Size = new Size(value, Size.Height); }
        }

        #endregion
        
        #region instantiators

        public AudioBlock() : base()
        {
            InitializeComponent();
            this.TabStop = true;  // mg (moved by JQ 20060817)
            mStructureBlock = new StructureBlock();
            mStructureBlock.AudioBlock = this;
            InitializeToolTips();
            mAnnotationBlock = new AnnotationBlock();
            mAnnotationBlock.AudioBlock = this;
        }

        #endregion

        #region AudioBlock (this)

        internal override void MarkDeselected()
        {
            BackColor = Color.MistyRose;
            mStructureBlock.MarkDeselected();
            mAnnotationBlock.MarkDeselected();
        }

        internal override void MarkSelected()
        {
            BackColor = Color.LightPink;
            mStructureBlock.MarkSelected();
            mAnnotationBlock.MarkSelected();
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
            mAnnotationBlock.StartRenaming();
        }

        #endregion

        /// <summary>
        /// Update the size of the structure block when the size changes.
        /// </summary>
        private void AudioBlock_SizeChanged(object sender, EventArgs e)
        {
         //md testing
         //resizing is now done by the annotation block
         /*
            if (mStructureBlock != null && mAnnotationBlock != null)
            {
                mStructureBlock._Width = Width;
                mAnnotationBlock._Width = Width;
            }
          */
        }

        /// <summary>
        /// Edit the page label for the structure block linked to this audio block.
        /// </summary>
        internal void StartEditingPageNumber()
        {
            mStructureBlock.StartEditingPageNumber();
        }

        //md 20061009
        private void InitializeToolTips()
        {
            this.mToolTip.SetToolTip(this, Localizer.Message("audio_block_tooltip"));
            this.mToolTip.SetToolTip(this.mTimeLabel, Localizer.Message("audio_block_duration_tooltip"));
        }
    }
}
