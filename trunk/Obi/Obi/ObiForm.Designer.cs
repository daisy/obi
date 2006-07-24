namespace Obi
{
    partial class ObiForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mNewProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenRecentProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openrecentSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mClearListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mSaveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSaveProjectasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDiscardChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCloseProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.touchProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mTocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowhideTableOfCOntentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mAddSectionAtSameLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAddsubsectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mStripsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mRenameSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAddStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mProjectPanel = new Obi.UserControls.ProjectPanel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.mTocToolStripMenuItem,
            this.mStripsToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(775, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mNewProjectToolStripMenuItem,
            this.mOpenProjectToolStripMenuItem,
            this.mOpenRecentProjectToolStripMenuItem,
            this.toolStripSeparator2,
            this.mSaveProjectToolStripMenuItem,
            this.mSaveProjectasToolStripMenuItem,
            this.mDiscardChangesToolStripMenuItem,
            this.mCloseProjectToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // mNewProjectToolStripMenuItem
            // 
            this.mNewProjectToolStripMenuItem.Name = "mNewProjectToolStripMenuItem";
            this.mNewProjectToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mNewProjectToolStripMenuItem.Text = "&New project";
            this.mNewProjectToolStripMenuItem.Click += new System.EventHandler(this.mNewProjectToolStripMenuItem_Click);
            // 
            // mOpenProjectToolStripMenuItem
            // 
            this.mOpenProjectToolStripMenuItem.Name = "mOpenProjectToolStripMenuItem";
            this.mOpenProjectToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mOpenProjectToolStripMenuItem.Text = "&Open project";
            this.mOpenProjectToolStripMenuItem.Click += new System.EventHandler(this.mOpenProjectToolStripMenuItem_Click);
            // 
            // mOpenRecentProjectToolStripMenuItem
            // 
            this.mOpenRecentProjectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openrecentSeparator,
            this.mClearListToolStripMenuItem});
            this.mOpenRecentProjectToolStripMenuItem.Name = "mOpenRecentProjectToolStripMenuItem";
            this.mOpenRecentProjectToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mOpenRecentProjectToolStripMenuItem.Text = "Open &recent project";
            // 
            // openrecentSeparator
            // 
            this.openrecentSeparator.Name = "openrecentSeparator";
            this.openrecentSeparator.Size = new System.Drawing.Size(114, 6);
            // 
            // mClearListToolStripMenuItem
            // 
            this.mClearListToolStripMenuItem.Name = "mClearListToolStripMenuItem";
            this.mClearListToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.mClearListToolStripMenuItem.Text = "&Clear list";
            this.mClearListToolStripMenuItem.Click += new System.EventHandler(this.mClearListToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(168, 6);
            // 
            // mSaveProjectToolStripMenuItem
            // 
            this.mSaveProjectToolStripMenuItem.Name = "mSaveProjectToolStripMenuItem";
            this.mSaveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mSaveProjectToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mSaveProjectToolStripMenuItem.Text = "&Save project";
            this.mSaveProjectToolStripMenuItem.Click += new System.EventHandler(this.mSaveProjectToolStripMenuItem_Click);
            // 
            // mSaveProjectasToolStripMenuItem
            // 
            this.mSaveProjectasToolStripMenuItem.Name = "mSaveProjectasToolStripMenuItem";
            this.mSaveProjectasToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mSaveProjectasToolStripMenuItem.Text = "Save project &as";
            this.mSaveProjectasToolStripMenuItem.Click += new System.EventHandler(this.mSaveProjectasToolStripMenuItem_Click);
            // 
            // mDiscardChangesToolStripMenuItem
            // 
            this.mDiscardChangesToolStripMenuItem.Name = "mDiscardChangesToolStripMenuItem";
            this.mDiscardChangesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mDiscardChangesToolStripMenuItem.Text = "&Discard changes";
            this.mDiscardChangesToolStripMenuItem.Click += new System.EventHandler(this.mDiscardChangesToolStripMenuItem_Click);
            // 
            // mCloseProjectToolStripMenuItem
            // 
            this.mCloseProjectToolStripMenuItem.Name = "mCloseProjectToolStripMenuItem";
            this.mCloseProjectToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mCloseProjectToolStripMenuItem.Text = "&Close project";
            this.mCloseProjectToolStripMenuItem.Click += new System.EventHandler(this.mCloseProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(168, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.metadataToolStripMenuItem,
            this.touchProjectToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // metadataToolStripMenuItem
            // 
            this.metadataToolStripMenuItem.Name = "metadataToolStripMenuItem";
            this.metadataToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.metadataToolStripMenuItem.Text = "&Metadata";
            this.metadataToolStripMenuItem.Click += new System.EventHandler(this.metadataToolStripMenuItem_Click);
            // 
            // touchProjectToolStripMenuItem
            // 
            this.touchProjectToolStripMenuItem.Name = "touchProjectToolStripMenuItem";
            this.touchProjectToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.touchProjectToolStripMenuItem.Text = "&Touch project";
            this.touchProjectToolStripMenuItem.Click += new System.EventHandler(this.touchProjectToolStripMenuItem_Click);
            // 
            // mTocToolStripMenuItem
            // 
            this.mTocToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mShowhideTableOfCOntentsToolStripMenuItem,
            this.toolStripSeparator1,
            this.mAddSectionAtSameLevelToolStripMenuItem,
            this.mAddsubsectionToolStripMenuItem,
            this.mRenameSectionToolStripMenuItem,
            this.mDeleteSectionToolStripMenuItem});
            this.mTocToolStripMenuItem.Name = "mTocToolStripMenuItem";
            this.mTocToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.mTocToolStripMenuItem.Text = "TO&C";
            // 
            // mShowhideTableOfCOntentsToolStripMenuItem
            // 
            this.mShowhideTableOfCOntentsToolStripMenuItem.Name = "mShowhideTableOfCOntentsToolStripMenuItem";
            this.mShowhideTableOfCOntentsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mShowhideTableOfCOntentsToolStripMenuItem.Text = "Show &table of contents ";
            this.mShowhideTableOfCOntentsToolStripMenuItem.Click += new System.EventHandler(this.mShowhideTableOfContentsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
            // 
            // mAddSectionAtSameLevelToolStripMenuItem
            // 
            this.mAddSectionAtSameLevelToolStripMenuItem.Name = "mAddSectionAtSameLevelToolStripMenuItem";
            this.mAddSectionAtSameLevelToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mAddSectionAtSameLevelToolStripMenuItem.Text = "&Add section at same level";
            // 
            // mAddsubsectionToolStripMenuItem
            // 
            this.mAddsubsectionToolStripMenuItem.Name = "mAddsubsectionToolStripMenuItem";
            this.mAddsubsectionToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mAddsubsectionToolStripMenuItem.Text = "Add &sub-section";
            // 
            // mDeleteSectionToolStripMenuItem
            // 
            this.mDeleteSectionToolStripMenuItem.Name = "mDeleteSectionToolStripMenuItem";
            this.mDeleteSectionToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mDeleteSectionToolStripMenuItem.Text = "&Delete section";
            // 
            // mStripsToolStripMenuItem
            // 
            this.mStripsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddStripToolStripMenuItem});
            this.mStripsToolStripMenuItem.Name = "mStripsToolStripMenuItem";
            this.mStripsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.mStripsToolStripMenuItem.Text = "Strips";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.userSettingsToolStripMenuItem,
            this.preferencesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // userSettingsToolStripMenuItem
            // 
            this.userSettingsToolStripMenuItem.Name = "userSettingsToolStripMenuItem";
            this.userSettingsToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.userSettingsToolStripMenuItem.Text = "&User profile";
            this.userSettingsToolStripMenuItem.Click += new System.EventHandler(this.userSettingsToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 480);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(775, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // mRenameSectionToolStripMenuItem
            // 
            this.mRenameSectionToolStripMenuItem.Name = "mRenameSectionToolStripMenuItem";
            this.mRenameSectionToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.mRenameSectionToolStripMenuItem.Text = "&Rename section";
            // 
            // mAddStripToolStripMenuItem
            // 
            this.mAddStripToolStripMenuItem.Name = "mAddStripToolStripMenuItem";
            this.mAddStripToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mAddStripToolStripMenuItem.Text = "&Add strip";
            // 
            // mProjectPanel
            // 
            this.mProjectPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mProjectPanel.BackColor = System.Drawing.Color.White;
            this.mProjectPanel.Location = new System.Drawing.Point(4, 27);
            this.mProjectPanel.Name = "mProjectPanel";
            this.mProjectPanel.Project = null;
            this.mProjectPanel.Size = new System.Drawing.Size(767, 450);
            this.mProjectPanel.TabIndex = 4;
            // 
            // ObiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(775, 502);
            this.Controls.Add(this.mProjectPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ObiForm";
            this.Text = "Obi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObiForm_FormClosing);
            this.Load += new System.EventHandler(this.ObiForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem mNewProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mOpenProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSaveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSaveProjectasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCloseProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem touchProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mOpenRecentProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem mDiscardChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator openrecentSeparator;
        private System.Windows.Forms.ToolStripMenuItem mClearListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metadataToolStripMenuItem;
        private Obi.UserControls.ProjectPanel mProjectPanel;
        private System.Windows.Forms.ToolStripMenuItem mTocToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mStripsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mShowhideTableOfCOntentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAddSectionAtSameLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAddsubsectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRenameSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAddStripToolStripMenuItem;


    }
}

