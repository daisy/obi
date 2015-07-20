namespace Obi.ProjectView
{
    partial class TOCView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TOCView));
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Context_ShowContentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AddSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AddSubsectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_InsertSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_MergeWithNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_MultipleOperationsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_RenameSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_DecreaseSectionLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_IncreaseSectionLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_SectionIsUsedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AddEmptyPagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteBeforeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteInsideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_PropertiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mContextMenuStrip
            // 
            resources.ApplyResources(this.mContextMenuStrip, "mContextMenuStrip");
            this.mContextMenuStrip.AllowDrop = true;
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Context_ShowContentsMenuItem,
            this.Context_AddSectionMenuItem,
            this.Context_AddSubsectionMenuItem,
            this.Context_InsertSectionMenuItem,
            this.Context_MergeWithNextMenuItem,
            this.Context_MultipleOperationsMenuItem,
            this.Context_RenameSectionMenuItem,
            this.Context_DecreaseSectionLevelMenuItem,
            this.Context_IncreaseSectionLevelMenuItem,
            this.Context_SectionIsUsedMenuItem,
            this.Context_AddEmptyPagesMenuItem,
            this.toolStripSeparator1,
            this.Context_CutMenuItem,
            this.Context_CopyMenuItem,
            this.Context_PasteMenuItem,
            this.Context_PasteBeforeMenuItem,
            this.Context_PasteInsideMenuItem,
            this.Context_DeleteMenuItem,
            this.toolStripSeparator2,
            this.Context_PropertiesMenuItem});
            this.mContextMenuStrip.Name = "mContextMenu";
            // 
            // Context_ShowContentsMenuItem
            // 
            resources.ApplyResources(this.Context_ShowContentsMenuItem, "Context_ShowContentsMenuItem");
            this.Context_ShowContentsMenuItem.Name = "Context_ShowContentsMenuItem";
            this.Context_ShowContentsMenuItem.Click += new System.EventHandler(this.Context_ShowContentsMenuItem_Click);
            // 
            // Context_AddSectionMenuItem
            // 
            this.Context_AddSectionMenuItem.Name = "Context_AddSectionMenuItem";
            resources.ApplyResources(this.Context_AddSectionMenuItem, "Context_AddSectionMenuItem");
            this.Context_AddSectionMenuItem.Click += new System.EventHandler(this.Context_AddSectionMenuItem_Click);
            // 
            // Context_AddSubsectionMenuItem
            // 
            this.Context_AddSubsectionMenuItem.Name = "Context_AddSubsectionMenuItem";
            resources.ApplyResources(this.Context_AddSubsectionMenuItem, "Context_AddSubsectionMenuItem");
            this.Context_AddSubsectionMenuItem.Click += new System.EventHandler(this.Context_AddSubsectionMenuItem_Click);
            // 
            // Context_InsertSectionMenuItem
            // 
            this.Context_InsertSectionMenuItem.Name = "Context_InsertSectionMenuItem";
            resources.ApplyResources(this.Context_InsertSectionMenuItem, "Context_InsertSectionMenuItem");
            this.Context_InsertSectionMenuItem.Click += new System.EventHandler(this.Context_InsertSectionMenuItem_Click);
            // 
            // Context_MergeWithNextMenuItem
            // 
            resources.ApplyResources(this.Context_MergeWithNextMenuItem, "Context_MergeWithNextMenuItem");
            this.Context_MergeWithNextMenuItem.Name = "Context_MergeWithNextMenuItem";
            // 
            // Context_MultipleOperationsMenuItem
            // 
            resources.ApplyResources(this.Context_MultipleOperationsMenuItem, "Context_MultipleOperationsMenuItem");
            this.Context_MultipleOperationsMenuItem.Name = "Context_MultipleOperationsMenuItem";
            // 
            // Context_RenameSectionMenuItem
            // 
            this.Context_RenameSectionMenuItem.Name = "Context_RenameSectionMenuItem";
            resources.ApplyResources(this.Context_RenameSectionMenuItem, "Context_RenameSectionMenuItem");
            this.Context_RenameSectionMenuItem.Click += new System.EventHandler(this.Context_RenameSectionMenuItem_Click);
            // 
            // Context_DecreaseSectionLevelMenuItem
            // 
            this.Context_DecreaseSectionLevelMenuItem.Name = "Context_DecreaseSectionLevelMenuItem";
            resources.ApplyResources(this.Context_DecreaseSectionLevelMenuItem, "Context_DecreaseSectionLevelMenuItem");
            this.Context_DecreaseSectionLevelMenuItem.Click += new System.EventHandler(this.Context_DecreaseSectionLevelMenuItem_Click);
            // 
            // Context_IncreaseSectionLevelMenuItem
            // 
            this.Context_IncreaseSectionLevelMenuItem.Name = "Context_IncreaseSectionLevelMenuItem";
            resources.ApplyResources(this.Context_IncreaseSectionLevelMenuItem, "Context_IncreaseSectionLevelMenuItem");
            this.Context_IncreaseSectionLevelMenuItem.Click += new System.EventHandler(this.Context_IncreaseSectionLevelMenuItem_Click);
            // 
            // Context_SectionIsUsedMenuItem
            // 
            this.Context_SectionIsUsedMenuItem.Checked = true;
            this.Context_SectionIsUsedMenuItem.CheckOnClick = true;
            this.Context_SectionIsUsedMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Context_SectionIsUsedMenuItem.Name = "Context_SectionIsUsedMenuItem";
            resources.ApplyResources(this.Context_SectionIsUsedMenuItem, "Context_SectionIsUsedMenuItem");
            this.Context_SectionIsUsedMenuItem.CheckedChanged += new System.EventHandler(this.Context_SectionIsUsedMenuItem_CheckedChanged);
            // 
            // Context_AddEmptyPagesMenuItem
            // 
            resources.ApplyResources(this.Context_AddEmptyPagesMenuItem, "Context_AddEmptyPagesMenuItem");
            this.Context_AddEmptyPagesMenuItem.Name = "Context_AddEmptyPagesMenuItem";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // Context_CutMenuItem
            // 
            this.Context_CutMenuItem.Name = "Context_CutMenuItem";
            resources.ApplyResources(this.Context_CutMenuItem, "Context_CutMenuItem");
            this.Context_CutMenuItem.Click += new System.EventHandler(this.Context_CutMenuItem_Click);
            // 
            // Context_CopyMenuItem
            // 
            this.Context_CopyMenuItem.Name = "Context_CopyMenuItem";
            resources.ApplyResources(this.Context_CopyMenuItem, "Context_CopyMenuItem");
            this.Context_CopyMenuItem.Click += new System.EventHandler(this.Context_CopyMenuItem_Click);
            // 
            // Context_PasteMenuItem
            // 
            this.Context_PasteMenuItem.Name = "Context_PasteMenuItem";
            resources.ApplyResources(this.Context_PasteMenuItem, "Context_PasteMenuItem");
            this.Context_PasteMenuItem.Click += new System.EventHandler(this.Context_PasteMenuItem_Click);
            // 
            // Context_PasteBeforeMenuItem
            // 
            this.Context_PasteBeforeMenuItem.Name = "Context_PasteBeforeMenuItem";
            resources.ApplyResources(this.Context_PasteBeforeMenuItem, "Context_PasteBeforeMenuItem");
            this.Context_PasteBeforeMenuItem.Click += new System.EventHandler(this.Context_PasteBeforeMenuItem_Click);
            // 
            // Context_PasteInsideMenuItem
            // 
            this.Context_PasteInsideMenuItem.Name = "Context_PasteInsideMenuItem";
            resources.ApplyResources(this.Context_PasteInsideMenuItem, "Context_PasteInsideMenuItem");
            this.Context_PasteInsideMenuItem.Click += new System.EventHandler(this.Context_PasteInsideMenuItem_Click);
            // 
            // Context_DeleteMenuItem
            // 
            this.Context_DeleteMenuItem.Name = "Context_DeleteMenuItem";
            resources.ApplyResources(this.Context_DeleteMenuItem, "Context_DeleteMenuItem");
            this.Context_DeleteMenuItem.Click += new System.EventHandler(this.Context_DeleteMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // Context_PropertiesMenuItem
            // 
            this.Context_PropertiesMenuItem.Name = "Context_PropertiesMenuItem";
            resources.ApplyResources(this.Context_PropertiesMenuItem, "Context_PropertiesMenuItem");
            this.Context_PropertiesMenuItem.Click += new System.EventHandler(this.Context_PropertiesMenuItem_Click);
            // 
            // TOCView
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ContextMenuStrip = this.mContextMenuStrip;
            resources.ApplyResources(this, "$this");
            this.FullRowSelect = true;
            this.LabelEdit = true;
            this.LineColor = System.Drawing.Color.Black;
            this.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterCollapse);
            this.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TOCTree_AfterLabelEdit);
            this.DoubleClick += new System.EventHandler(this.TOCView_DoubleClick);
            this.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterSelect);
            this.Leave += new System.EventHandler(this.TOCView_Leave);
            this.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TOCTree_BeforeLabelEdit);
            this.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.TOCTree_BeforeSelect);
            this.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterExpand);
            this.mContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip mContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Context_AddSectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AddSubsectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_InsertSectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_DecreaseSectionLevelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_IncreaseSectionLevelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_SectionIsUsedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_ShowContentsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_CutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_RenameSectionMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Context_CopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PasteBeforeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PasteInsideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_DeleteMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem Context_PropertiesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_MultipleOperationsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_MergeWithNextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AddEmptyPagesMenuItem;
    }
}
