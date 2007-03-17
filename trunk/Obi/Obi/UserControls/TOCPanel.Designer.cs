namespace Obi.UserControls
{
    partial class TOCPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mTocTree = new System.Windows.Forms.TreeView();
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mAddSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAddSubSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRenameSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCutSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCopySectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPasteSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMarkSectionAsUnusedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowInStripViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTocTree
            // 
            this.mTocTree.ContextMenuStrip = this.mContextMenuStrip;
            this.mTocTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTocTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTocTree.ForeColor = System.Drawing.Color.SteelBlue;
            this.mTocTree.LabelEdit = true;
            this.mTocTree.Location = new System.Drawing.Point(0, 0);
            this.mTocTree.Name = "mTocTree";
            this.mTocTree.Size = new System.Drawing.Size(129, 123);
            this.mTocTree.TabIndex = 0;
            this.mTocTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tocTree_NodeMouseDoubleClick);
            this.mTocTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.mTocTree_AfterLabelEdit);
            this.mTocTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mTocTree_AfterSelect);
            this.mTocTree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.mTocTree_BeforeLabelEdit);
            this.mTocTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tocTree_KeyDown);
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddSectionToolStripMenuItem,
            this.mAddSubSectionToolStripMenuItem,
            this.mRenameSectionToolStripMenuItem,
            this.mMoveOutToolStripMenuItem,
            this.mMoveInToolStripMenuItem,
            this.mCutSectionToolStripMenuItem,
            this.mCopySectionToolStripMenuItem,
            this.mPasteSectionToolStripMenuItem,
            this.mDeleteSectionToolStripMenuItem,
            this.mMarkSectionAsUnusedToolStripMenuItem,
            this.mShowInStripViewToolStripMenuItem});
            this.mContextMenuStrip.Name = "contextMenuStrip1";
            this.mContextMenuStrip.Size = new System.Drawing.Size(230, 246);
            this.mContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.mContextMenuStrip_Opening);
            // 
            // mAddSectionToolStripMenuItem
            // 
            this.mAddSectionToolStripMenuItem.Name = "mAddSectionToolStripMenuItem";
            this.mAddSectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mAddSectionToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mAddSectionToolStripMenuItem.Text = "&Insert section";
            this.mAddSectionToolStripMenuItem.Click += new System.EventHandler(this.mInsertSectionToolStripMenuItem_Click);
            // 
            // mAddSubSectionToolStripMenuItem
            // 
            this.mAddSubSectionToolStripMenuItem.Name = "mAddSubSectionToolStripMenuItem";
            this.mAddSubSectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.mAddSubSectionToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mAddSubSectionToolStripMenuItem.Text = "Add &sub-section";
            this.mAddSubSectionToolStripMenuItem.Click += new System.EventHandler(this.mAddSubSectionToolStripMenuItem_Click);
            // 
            // mRenameSectionToolStripMenuItem
            // 
            this.mRenameSectionToolStripMenuItem.Name = "mRenameSectionToolStripMenuItem";
            this.mRenameSectionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mRenameSectionToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mRenameSectionToolStripMenuItem.Text = "Rena&me section";
            this.mRenameSectionToolStripMenuItem.Click += new System.EventHandler(this.mRenameToolStripMenuItem_Click);
            // 
            // mMoveOutToolStripMenuItem
            // 
            this.mMoveOutToolStripMenuItem.Name = "mMoveOutToolStripMenuItem";
            this.mMoveOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Left)));
            this.mMoveOutToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mMoveOutToolStripMenuItem.Text = "Move &out";
            this.mMoveOutToolStripMenuItem.Click += new System.EventHandler(this.mMoveOutToolStripMenuItem_Click);
            // 
            // mMoveInToolStripMenuItem
            // 
            this.mMoveInToolStripMenuItem.Name = "mMoveInToolStripMenuItem";
            this.mMoveInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Right)));
            this.mMoveInToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mMoveInToolStripMenuItem.Text = "Move i&n";
            this.mMoveInToolStripMenuItem.Click += new System.EventHandler(this.mMoveInToolStripMenuItem_Click);
            // 
            // mCutSectionToolStripMenuItem
            // 
            this.mCutSectionToolStripMenuItem.Name = "mCutSectionToolStripMenuItem";
            this.mCutSectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.X)));
            this.mCutSectionToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mCutSectionToolStripMenuItem.Text = "Cu&t section";
            this.mCutSectionToolStripMenuItem.Click += new System.EventHandler(this.mCutSectionToolStripMenuItem_Click);
            // 
            // mCopySectionToolStripMenuItem
            // 
            this.mCopySectionToolStripMenuItem.Name = "mCopySectionToolStripMenuItem";
            this.mCopySectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.C)));
            this.mCopySectionToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mCopySectionToolStripMenuItem.Text = "&Copy section";
            this.mCopySectionToolStripMenuItem.Click += new System.EventHandler(this.mCopySectionToolStripMenuItem_Click);
            // 
            // mPasteSectionToolStripMenuItem
            // 
            this.mPasteSectionToolStripMenuItem.Name = "mPasteSectionToolStripMenuItem";
            this.mPasteSectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.V)));
            this.mPasteSectionToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mPasteSectionToolStripMenuItem.Text = "&Paste section";
            this.mPasteSectionToolStripMenuItem.Click += new System.EventHandler(this.mPasteSectionToolStripMenuItem_Click);
            // 
            // mDeleteSectionToolStripMenuItem
            // 
            this.mDeleteSectionToolStripMenuItem.Name = "mDeleteSectionToolStripMenuItem";
            this.mDeleteSectionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mDeleteSectionToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mDeleteSectionToolStripMenuItem.Text = "&Delete section";
            this.mDeleteSectionToolStripMenuItem.Click += new System.EventHandler(this.mDeleteSectionToolStripMenuItem_Click);
            // 
            // mMarkSectionAsUnusedToolStripMenuItem
            // 
            this.mMarkSectionAsUnusedToolStripMenuItem.Name = "mMarkSectionAsUnusedToolStripMenuItem";
            this.mMarkSectionAsUnusedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.mMarkSectionAsUnusedToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mMarkSectionAsUnusedToolStripMenuItem.Text = "Mar&k section as unused";
            this.mMarkSectionAsUnusedToolStripMenuItem.Click += new System.EventHandler(this.mMarkSectionAsUnusedToolStripMenuItem_Click);
            // 
            // mShowInStripViewToolStripMenuItem
            // 
            this.mShowInStripViewToolStripMenuItem.Name = "mShowInStripViewToolStripMenuItem";
            this.mShowInStripViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
            this.mShowInStripViewToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.mShowInStripViewToolStripMenuItem.Text = "Show in strip &view";
            this.mShowInStripViewToolStripMenuItem.Click += new System.EventHandler(this.mShowInStripViewToolStripMenuItem_Click);
            // 
            // mToolTip
            // 
            this.mToolTip.IsBalloon = true;
            this.mToolTip.ToolTipTitle = "Table of Contents View";
            // 
            // TOCPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mTocTree);
            this.Name = "TOCPanel";
            this.Size = new System.Drawing.Size(129, 123);
            this.mContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView mTocTree;
        private System.Windows.Forms.ContextMenuStrip mContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mAddSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAddSubSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRenameSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mShowInStripViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCutSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCopySectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPasteSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveOutToolStripMenuItem;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.ToolStripMenuItem mMarkSectionAsUnusedToolStripMenuItem;
    }
}
