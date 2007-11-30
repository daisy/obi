using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class ContainerBlock : Block
    {
        public ContainerBlock(PhraseNode node, Strip strip): base(node, strip)
        {
            this.InitializeComponent();
        }

        public void AddBlocksForPhrase(PhraseNode phrase)
        {
            for (int i = 0; i < phrase.getChildCount(); i++)
            {
                AddBlockForPhrase(phrase.PhraseChild(i));
            }
        }
        public Block AddBlockForPhrase(PhraseNode phrase)
        {
            Block block = null;
            if (phrase.Audio == null) block = new Block(phrase, this);
            else block = new AudioBlock(phrase, Strip);
            mBlocksPanel.Controls.Add(block);
            mBlocksPanel.Controls.SetChildIndex(block, phrase.Index);
            UpdateWidth();
            UpdateTime();
            return block;
        }

        private void UpdateTime()
        {
            double time = 0;
            for (int i = 0; i < Node.PhraseChildCount; i++)
                time += Node.PhraseChild(i).Audio.getDuration().getTimeDeltaAsMillisecondFloat();
            TimeLabel = String.Format("{0:0.00}s", time / 1000);
        }

        private void UpdateWidth()
        {
            int w = 0;
            foreach (Control c in mBlocksPanel.Controls) w += c.Width + c.Margin.Right;
            if (mBlocksPanel.Controls.Count > 0) w -= mBlocksPanel.Controls[mBlocksPanel.Controls.Count - 1].Margin.Right;
            if (w > mBlocksPanel.Width) mBlocksPanel.Size = new Size(w, mBlocksPanel.Height);
            w += mBlocksPanel.Location.X + mBlocksPanel.Margin.Right;
            if (w > MinimumSize.Width) MinimumSize = new Size(w, MinimumSize.Height);
        }

        public Block FindBlock(PhraseNode phrase)
        {
            foreach (Control c in mBlocksPanel.Controls)
            {
                if (c is Block && ((Block)c).Node == phrase) return (Block)c;
            }
            return null;
        }
    }
}
