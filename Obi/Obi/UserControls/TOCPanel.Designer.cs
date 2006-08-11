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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mDeleteSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEditLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMoveOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mShowInStripViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tESTShallowDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.mTocTree.Size = new System.Drawing.Size(129, 133);
            this.mTocTree.TabIndex = 0;
            this.mTocTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tocTree_NodeMouseDoubleClick);
            this.mTocTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tocTree_AfterLabelEdit);
            this.mTocTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mTocTree_AfterSelect);
            this.mTocTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tocTree_NodeMouseClick);
            this.mTocTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tocTree_BeforeSelect);
            this.mTocTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tocTree_KeyDown);
            this.mTocTree.Leave += new System.EventHandler(this.mTocTree_Leave);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddSectionAtSameLevelToolStripMenuItem,
            this.mAddSubSectionToolStripMenuItem,
            this.toolStripSeparator1,
            this.mCutSectionToolStripMenuItem,
            this.mCopySectionToolStripMenuItem,
            this.mPasteSectionToolStripMenuItem,
            this.toolStripSeparator2,
            this.mDeleteSectionToolStripMenuItem,
            this.mEditLabelToolStripMenuItem,
            this.mMoveToolStripMenuItem,
            this.toolStripSeparator3,
            this.mShowInStripViewToolStripMenuItem,
            this.toolStripSeparator4,
            this.tESTShallowDeleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(170, 270);
            // 
            // mAddSectionAtSameLevelToolStripMenuItem
            // 
            this.mAddSectionAtSameLevelToolStripMenuItem.Name = "mAddSectionAtSameLevelToolStripMenuItem";
            this.mAddSectionAtSameLevelToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mAddSectionAtSameLevelToolStripMenuItem.Text = "&Add section";
            this.mAddSectionAtSameLevelToolStripMenuItem.Click += new System.EventHandler(this.mAddSectionToolStripMenuItem_Click);
            // 
            // mAddSubSectionToolStripMenuItem
            // 
            this.mAddSubSectionToolStripMenuItem.Name = "mAddSubSectionToolStripMenuItem";
            this.mAddSubSectionToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mAddSubSectionToolStripMenuItem.Text = "Add &sub-section";
            this.mAddSubSectionToolStripMenuItem.Click += new System.EventHandler(this.mAddSubSectionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
            // 
            // mCutSectionToolStripMenuItem
            // 
            this.mCutSectionToolStripMenuItem.Name = "mCutSectionToolStripMenuItem";
            this.mCutSectionToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mCutSectionToolStripMenuItem.Text = "Cu&t section";
            this.mCutSectionToolStripMenuItem.Click += new System.EventHandler(this.cutSectionToolStripMenuItem_Click);
            // 
            // mCopySectionToolStripMenuItem
            // 
            this.mCopySectionToolStripMenuItem.Name = "mCopySectionToolStripMenuItem";
            this.mCopySectionToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mCopySectionToolStripMenuItem.Text = "&Copy section";
            this.mCopySectionToolStripMenuItem.Click += new System.EventHandler(this.copySectionToolStripMenuItem_Click);
            // 
            // mPasteSectionToolStripMenuItem
            // 
            this.mPasteSectionToolStripMenuItem.Name = "mPasteSectionToolStripMenuItem";
            this.mPasteSectionToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mPasteSectionToolStripMenuItem.Text = "&Paste section";
            this.mPasteSectionToolStripMenuItem.Click += new System.EventHandler(this.mPasteSectionToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(166, 6);
            // 
            // mDeleteSectionToolStripMenuItem
            // 
            this.mDeleteSectionToolStripMenuItem.Name = "mDeleteSectionToolStripMenuItem";
            this.mDeleteSectionToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mDeleteSectionToolStripMenuItem.Text = "&Delete section";
            this.mDeleteSectionToolStripMenuItem.Click += new System.EventHandler(this.mDeleteSectionToolStripMenuItem_Click);
            // 
            // mEditLabelToolStripMenuItem
            // 
            this.mEditLabelToolStripMenuItem.Name = "mEditLabelToolStripMenuItem";
            this.mEditLabelToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mEditLabelToolStripMenuItem.Text = "Re&name section";
            this.mEditLabelToolStripMenuItem.Click += new System.EventHandler(this.mRenameToolStripMenuItem_Click);
            // 
            // mMoveToolStripMenuItem
            // 
            this.mMoveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mMoveUpToolStripMenuItem,
            this.mMoveDownToolStripMenuItem,
            this.mMoveInToolStripMenuItem,
            this.mMoveOutToolStripMenuItem});
            this.mMoveToolStripMenuItem.Name = "mMoveToolStripMenuItem";
            this.mMoveToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mMoveToolStripMenuItem.Text = "&Move section...";
            // 
            // mMoveUpToolStripMenuItem
            // 
            this.mMoveUpToolStripMenuItem.Name = "mMoveUpToolStripMenuItem";
            this.mMoveUpToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.mMoveUpToolStripMenuItem.Text = "&Up";
            this.mMoveUpToolStripMenuItem.Click += new System.EventHandler(this.mMoveUpToolStripMenuItem_Click);
            // 
            // mMoveDownToolStripMenuItem
            // 
            this.mMoveDownToolStripMenuItem.Name = "mMoveDownToolStripMenuItem";
            this.mMoveDownToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.mMoveDownToolStripMenuItem.Text = "&Down";
            this.mMoveDownToolStripMenuItem.Click += new System.EventHandler(this.mMoveDownToolStripMenuItem_Click);
            // 
            // mMoveInToolStripMenuItem
            // 
            this.mMoveInToolStripMenuItem.Name = "mMoveInToolStripMenuItem";
            this.mMoveInToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.mMoveInToolStripMenuItem.Text = "&In";
            this.mMoveInToolStripMenuItem.Click += new System.EventHandler(this.increaseLevelToolStripMenuItem_Click);
            // 
            // mMoveOutToolStripMenuItem
            // 
            this.mMoveOutToolStripMenuItem.Name = "mMoveOutToolStripMenuItem";
            this.mMoveOutToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.mMoveOutToolStripMenuItem.Text = "&Out";
            this.mMoveOutToolStripMenuItem.Click += new System.EventHandler(this.decreaseLevelToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(166, 6);
            // 
            // mShowInStripViewToolStripMenuItem
            // 
            this.mShowInStripViewToolStripMenuItem.Name = "mShowInStripViewToolStripMenuItem";
            this.mShowInStripViewToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.mShowInStripViewToolStripMenuItem.Text = "Show in strip &view";
            this.mShowInStripViewToolStripMenuItem.Click += new System.EventHandler(this.mShowInStripViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(166, 6);
            // 
            // tESTShallowDeleteToolStripMenuItem
            // 
            this.tESTShallowDeleteToolStripMenuItem.Name = "tESTShallowDeleteToolStripMenuItem";
            this.tESTShallowDeleteToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.tESTShallowDeleteToolStripMenuItem.Text = "TEST shallow delete";
            this.tESTShallowDeleteToolStripMenuItem.Click += new System.EventHandler(this.tESTShallowDeleteToolStripMenuItem_Click_1);
            // 
            // TOCPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mTocTree);
            this.Name = "TOCPanel";
            this.Size = new System.Drawing.Size(129, 133);
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
        private System.Windows.Forms.ToolStripMenuItem mMoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMoveOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCutSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCopySectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPasteSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem tESTShallowDeleteToolStripMenuItem;
    }
}
