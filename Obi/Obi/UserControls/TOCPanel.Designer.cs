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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mAddSectionAtSameLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAddSubSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mCutSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCopySectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPasteSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEditLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mShowInStripViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTocTree
            // 
            this.mTocTree.ContextMenuStrip = this.contextMenuStrip1;
            this.mTocTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTocTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTocTree.ForeColor = System.Drawing.Color.SteelBlue;
            this.mTocTree.LabelEdit = true;
            this.mTocTree.Location = new System.Drawing.Point(0, 0);
            this.mTocTree.Name = "mTocTree";
            this.mTocTree.Size = new System.Drawing.Size(129, 123);
            this.mTocTree.TabIndex = 0;
            this.mTocTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tocTree_NodeMouseDoubleClick);
            this.mTocTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tocTree_AfterLabelEdit);
            this.mTocTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tocTree_NodeMouseClick);
            this.mTocTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tocTree_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddSectionAtSameLevelToolStripMenuItem,
            this.mAddSubSectionToolStripMenuItem,
            this.mEditLabelToolStripMenuItem,
            this.mMoveOutToolStripMenuItem,
            this.mMoveInToolStripMenuItem,
            this.toolStripSeparator1,
            this.mCutSectionToolStripMenuItem,
            this.mCopySectionToolStripMenuItem,
            this.mPasteSectionToolStripMenuItem,
            this.mDeleteSectionToolStripMenuItem,
            this.toolStripSeparator3,
            this.mShowInStripViewToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(207, 258);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mAddSectionAtSameLevelToolStripMenuItem
            // 
            this.mAddSectionAtSameLevelToolStripMenuItem.Name = "mAddSectionAtSameLevelToolStripMenuItem";
            this.mAddSectionAtSameLevelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mAddSectionAtSameLevelToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mAddSectionAtSameLevelToolStripMenuItem.Text = "&Add section";
            this.mAddSectionAtSameLevelToolStripMenuItem.Click += new System.EventHandler(this.mAddSectionToolStripMenuItem_Click);
            // 
            // mAddSubSectionToolStripMenuItem
            // 
            this.mAddSubSectionToolStripMenuItem.Name = "mAddSubSectionToolStripMenuItem";
            this.mAddSubSectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.mAddSubSectionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mAddSubSectionToolStripMenuItem.Text = "Add &sub-section";
            this.mAddSubSectionToolStripMenuItem.Click += new System.EventHandler(this.mAddSubSectionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
            // 
            // mCutSectionToolStripMenuItem
            // 
            this.mCutSectionToolStripMenuItem.Name = "mCutSectionToolStripMenuItem";
            this.mCutSectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.X)));
            this.mCutSectionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mCutSectionToolStripMenuItem.Text = "Cu&t section";
            this.mCutSectionToolStripMenuItem.Click += new System.EventHandler(this.mCutSectionToolStripMenuItem_Click);
            // 
            // mCopySectionToolStripMenuItem
            // 
            this.mCopySectionToolStripMenuItem.Name = "mCopySectionToolStripMenuItem";
            this.mCopySectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.C)));
            this.mCopySectionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mCopySectionToolStripMenuItem.Text = "&Copy section";
            this.mCopySectionToolStripMenuItem.Click += new System.EventHandler(this.copySectionToolStripMenuItem_Click);
            // 
            // mPasteSectionToolStripMenuItem
            // 
            this.mPasteSectionToolStripMenuItem.Name = "mPasteSectionToolStripMenuItem";
            this.mPasteSectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.V)));
            this.mPasteSectionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mPasteSectionToolStripMenuItem.Text = "&Paste section";
            this.mPasteSectionToolStripMenuItem.Click += new System.EventHandler(this.mPasteSectionToolStripMenuItem_Click);
            // 
            // mMoveOutToolStripMenuItem
            // 
            this.mMoveOutToolStripMenuItem.Name = "mMoveOutToolStripMenuItem";
            this.mMoveOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Left)));
            this.mMoveOutToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mMoveOutToolStripMenuItem.Text = "Move &out";
            this.mMoveOutToolStripMenuItem.Click += new System.EventHandler(this.decreaseLevelToolStripMenuItem_Click);
            // 
            // mMoveInToolStripMenuItem
            // 
            this.mMoveInToolStripMenuItem.Name = "mMoveInToolStripMenuItem";
            this.mMoveInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Right)));
            this.mMoveInToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mMoveInToolStripMenuItem.Text = "Move &in";
            this.mMoveInToolStripMenuItem.Click += new System.EventHandler(this.increaseLevelToolStripMenuItem_Click);
            // 
            // mDeleteSectionToolStripMenuItem
            // 
            this.mDeleteSectionToolStripMenuItem.Name = "mDeleteSectionToolStripMenuItem";
            this.mDeleteSectionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mDeleteSectionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mDeleteSectionToolStripMenuItem.Text = "&Delete section";
            this.mDeleteSectionToolStripMenuItem.Click += new System.EventHandler(this.mDeleteSectionToolStripMenuItem_Click);
            // 
            // mEditLabelToolStripMenuItem
            // 
            this.mEditLabelToolStripMenuItem.Name = "mEditLabelToolStripMenuItem";
            this.mEditLabelToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mEditLabelToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mEditLabelToolStripMenuItem.Text = "Re&name section";
            this.mEditLabelToolStripMenuItem.Click += new System.EventHandler(this.mRenameToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(203, 6);
            // 
            // mShowInStripViewToolStripMenuItem
            // 
            this.mShowInStripViewToolStripMenuItem.Name = "mShowInStripViewToolStripMenuItem";
            this.mShowInStripViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
            this.mShowInStripViewToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mShowInStripViewToolStripMenuItem.Text = "Show in strip &view";
            this.mShowInStripViewToolStripMenuItem.Click += new System.EventHandler(this.mShowInStripViewToolStripMenuItem_Click);
            // 
            // mToolTip
            // 
            this.mToolTip.AutomaticDelay = 3000;
            this.mToolTip.AutoPopDelay = 4000;
            this.mToolTip.InitialDelay = 3000;
            this.mToolTip.IsBalloon = true;
            this.mToolTip.ReshowDelay = 600;
            this.mToolTip.ToolTipTitle = "Table of Contents View";
            // 
            // TOCPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mTocTree);
            this.Name = "TOCPanel";
            this.Size = new System.Drawing.Size(129, 123);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView mTocTree;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mAddSectionAtSameLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAddSubSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mEditLabelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mShowInStripViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCutSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCopySectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPasteSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveOutToolStripMenuItem;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
