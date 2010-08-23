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
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Context_AddSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AddSubsectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_InsertSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_RenameSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_DecreaseSectionLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_IncreaseSectionLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_SectionIsUsedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteBeforeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteInsideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_PropertiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_MergeSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Context_AddSectionMenuItem,
            this.Context_AddSubsectionMenuItem,
            this.Context_InsertSectionMenuItem,
            this.Context_MergeSectionMenuItem,
            this.Context_RenameSectionMenuItem,
            this.Context_DecreaseSectionLevelMenuItem,
            this.Context_IncreaseSectionLevelMenuItem,
            this.Context_SectionIsUsedMenuItem,
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
            this.mContextMenuStrip.Size = new System.Drawing.Size(201, 368);
            // 
            // Context_AddSectionMenuItem
            // 
            this.Context_AddSectionMenuItem.Name = "Context_AddSectionMenuItem";
            this.Context_AddSectionMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_AddSectionMenuItem.Text = "&Add section";
            this.Context_AddSectionMenuItem.Click += new System.EventHandler(this.Context_AddSectionMenuItem_Click);
            // 
            // Context_AddSubsectionMenuItem
            // 
            this.Context_AddSubsectionMenuItem.Name = "Context_AddSubsectionMenuItem";
            this.Context_AddSubsectionMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_AddSubsectionMenuItem.Text = "Add s&ubsection";
            this.Context_AddSubsectionMenuItem.Click += new System.EventHandler(this.Context_AddSubsectionMenuItem_Click);
            // 
            // Context_InsertSectionMenuItem
            // 
            this.Context_InsertSectionMenuItem.Name = "Context_InsertSectionMenuItem";
            this.Context_InsertSectionMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_InsertSectionMenuItem.Text = "&Insert section";
            this.Context_InsertSectionMenuItem.Click += new System.EventHandler(this.Context_InsertSectionMenuItem_Click);
            // 
            // Context_RenameSectionMenuItem
            // 
            this.Context_RenameSectionMenuItem.Name = "Context_RenameSectionMenuItem";
            this.Context_RenameSectionMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_RenameSectionMenuItem.Text = "&Rename section";
            this.Context_RenameSectionMenuItem.Click += new System.EventHandler(this.Context_RenameSectionMenuItem_Click);
            // 
            // Context_DecreaseSectionLevelMenuItem
            // 
            this.Context_DecreaseSectionLevelMenuItem.Name = "Context_DecreaseSectionLevelMenuItem";
            this.Context_DecreaseSectionLevelMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_DecreaseSectionLevelMenuItem.Text = "D&ecrease section level";
            this.Context_DecreaseSectionLevelMenuItem.Click += new System.EventHandler(this.Context_DecreaseSectionLevelMenuItem_Click);
            // 
            // Context_IncreaseSectionLevelMenuItem
            // 
            this.Context_IncreaseSectionLevelMenuItem.Name = "Context_IncreaseSectionLevelMenuItem";
            this.Context_IncreaseSectionLevelMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_IncreaseSectionLevelMenuItem.Text = "I&ncrease section level";
            this.Context_IncreaseSectionLevelMenuItem.Click += new System.EventHandler(this.Context_IncreaseSectionLevelMenuItem_Click);
            // 
            // Context_SectionIsUsedMenuItem
            // 
            this.Context_SectionIsUsedMenuItem.Checked = true;
            this.Context_SectionIsUsedMenuItem.CheckOnClick = true;
            this.Context_SectionIsUsedMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Context_SectionIsUsedMenuItem.Name = "Context_SectionIsUsedMenuItem";
            this.Context_SectionIsUsedMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_SectionIsUsedMenuItem.Text = "Sec&tion is used";
            this.Context_SectionIsUsedMenuItem.CheckedChanged += new System.EventHandler(this.Context_SectionIsUsedMenuItem_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // Context_CutMenuItem
            // 
            this.Context_CutMenuItem.Name = "Context_CutMenuItem";
            this.Context_CutMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_CutMenuItem.Text = "&Cut";
            this.Context_CutMenuItem.Click += new System.EventHandler(this.Context_CutMenuItem_Click);
            // 
            // Context_CopyMenuItem
            // 
            this.Context_CopyMenuItem.Name = "Context_CopyMenuItem";
            this.Context_CopyMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_CopyMenuItem.Text = "Cop&y";
            this.Context_CopyMenuItem.Click += new System.EventHandler(this.Context_CopyMenuItem_Click);
            // 
            // Context_PasteMenuItem
            // 
            this.Context_PasteMenuItem.Name = "Context_PasteMenuItem";
            this.Context_PasteMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_PasteMenuItem.Text = "&Paste";
            this.Context_PasteMenuItem.Click += new System.EventHandler(this.Context_PasteMenuItem_Click);
            // 
            // Context_PasteBeforeMenuItem
            // 
            this.Context_PasteBeforeMenuItem.Name = "Context_PasteBeforeMenuItem";
            this.Context_PasteBeforeMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_PasteBeforeMenuItem.Text = "Paste &before";
            this.Context_PasteBeforeMenuItem.Click += new System.EventHandler(this.Context_PasteBeforeMenuItem_Click);
            // 
            // Context_PasteInsideMenuItem
            // 
            this.Context_PasteInsideMenuItem.Name = "Context_PasteInsideMenuItem";
            this.Context_PasteInsideMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_PasteInsideMenuItem.Text = "Paste &inside";
            this.Context_PasteInsideMenuItem.Click += new System.EventHandler(this.Context_PasteInsideMenuItem_Click);
            // 
            // Context_DeleteMenuItem
            // 
            this.Context_DeleteMenuItem.Name = "Context_DeleteMenuItem";
            this.Context_DeleteMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_DeleteMenuItem.Text = "&Delete";
            this.Context_DeleteMenuItem.Click += new System.EventHandler(this.Context_DeleteMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(197, 6);
            // 
            // Context_PropertiesMenuItem
            // 
            this.Context_PropertiesMenuItem.Name = "Context_PropertiesMenuItem";
            this.Context_PropertiesMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_PropertiesMenuItem.Text = "Pr&operties";
            this.Context_PropertiesMenuItem.Click += new System.EventHandler(this.Context_PropertiesMenuItem_Click);
            // 
            // Context_MergeSectionMenuItem
            // 
            this.Context_MergeSectionMenuItem.Name = "Context_MergeSectionMenuItem";
            this.Context_MergeSectionMenuItem.Size = new System.Drawing.Size(200, 22);
            this.Context_MergeSectionMenuItem.Text = "Mer&ge section with next";
            this.Context_MergeSectionMenuItem.Click += new System.EventHandler(this.Context_MergeSectionMenuItem_Click);
            // 
            // TOCView
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ContextMenuStrip = this.mContextMenuStrip;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FullRowSelect = true;
            this.LabelEdit = true;
            this.LineColor = System.Drawing.Color.Black;
            this.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterCollapse);
            this.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TOCTree_AfterLabelEdit);
            this.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterSelect);
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
        private System.Windows.Forms.ToolStripMenuItem Context_MergeSectionMenuItem;
    }
}
