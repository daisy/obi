using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Zaboom.UserControls
{
    public partial class DummyBlock : UserControl, Selectable
    {
        private bool selected;
        protected ProjectPanel panel;
        private static readonly Color BACK_COLOR_SELECTED = Color.Aquamarine;
        private static readonly Color BACK_COLOR_UNSELECTED = Color.RoyalBlue;

        public DummyBlock()
        {
            InitializeComponent();
            Selected = false;
            panel = null;
        }

        public ProjectPanel Panel
        {
            set
            {
                if (panel != null) throw new Exception("Panel is already set!");
                if (value == null) throw new urakawa.exception.MethodParameterIsNullException("Null panel!");
                panel = value;
            }
        }

        private void DummyBlock_Click(object sender, EventArgs e)
        {
            Clicked();
        }

        protected void Clicked()
        {
            Selected = !selected;
            if (selected)
            {
                panel.ReplaceSelection(this);
            }
            else
            {
                panel.ModifiedSelection(this);
            }
        }

        #region Selectable Members

        public urakawa.core.TreeNode Node { get { return null; } }

        /// <summary>
        /// Used by the project panel to ask or tell the control when it selected or not.
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    BackColor = selected ? BACK_COLOR_SELECTED : BACK_COLOR_UNSELECTED;
                }
            }
        }

        #endregion
    }
}
