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
            this.tocTree = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSectionAtSameLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSubSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.increaseLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decreaseLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testShallowDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tocTree
            // 
            this.tocTree.ContextMenuStrip = this.contextMenuStrip1;
            this.tocTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tocTree.LabelEdit = true;
            this.tocTree.Location = new System.Drawing.Point(0, 0);
            this.tocTree.Name = "tocTree";
            this.tocTree.Size = new System.Drawing.Size(129, 133);
            this.tocTree.TabIndex = 0;
            this.tocTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tocTree_AfterLabelEdit);
            this.tocTree.Click += new System.EventHandler(this.tocTree_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSectionAtSameLevelToolStripMenuItem,
            this.addSubSectionToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteSectionToolStripMenuItem,
            this.testShallowDeleteToolStripMenuItem,
            this.editLabelToolStripMenuItem,
            this.toolStripSeparator2,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.increaseLevelToolStripMenuItem,
            this.decreaseLevelToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(167, 236);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // addSectionAtSameLevelToolStripMenuItem
            // 
            this.addSectionAtSameLevelToolStripMenuItem.Name = "addSectionAtSameLevelToolStripMenuItem";
            this.addSectionAtSameLevelToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.addSectionAtSameLevelToolStripMenuItem.Text = "&Add section";
            this.addSectionAtSameLevelToolStripMenuItem.Click += new System.EventHandler(this.addSectionAtSameLevelToolStripMenuItem_Click);
            // 
            // addSubSectionToolStripMenuItem
            // 
            this.addSubSectionToolStripMenuItem.Name = "addSubSectionToolStripMenuItem";
            this.addSubSectionToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.addSubSectionToolStripMenuItem.Text = "Add &sub-section";
            this.addSubSectionToolStripMenuItem.Click += new System.EventHandler(this.addSubSectionToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // increaseLevelToolStripMenuItem
            // 
            this.increaseLevelToolStripMenuItem.Name = "increaseLevelToolStripMenuItem";
            this.increaseLevelToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.increaseLevelToolStripMenuItem.Text = "Increase level";
            this.increaseLevelToolStripMenuItem.Click += new System.EventHandler(this.increaseLevelToolStripMenuItem_Click);
            // 
            // decreaseLevelToolStripMenuItem
            // 
            this.decreaseLevelToolStripMenuItem.Name = "decreaseLevelToolStripMenuItem";
            this.decreaseLevelToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.decreaseLevelToolStripMenuItem.Text = "Decrease level";
            this.decreaseLevelToolStripMenuItem.Click += new System.EventHandler(this.decreaseLevelToolStripMenuItem_Click);
            // 
            // editLabelToolStripMenuItem
            // 
            this.editLabelToolStripMenuItem.Name = "editLabelToolStripMenuItem";
            this.editLabelToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.editLabelToolStripMenuItem.Text = "Rena&me";
            this.editLabelToolStripMenuItem.Click += new System.EventHandler(this.editLabelToolStripMenuItem_Click);
            // 
            // deleteSectionToolStripMenuItem
            // 
            this.deleteSectionToolStripMenuItem.Name = "deleteSectionToolStripMenuItem";
            this.deleteSectionToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.deleteSectionToolStripMenuItem.Text = "&Delete section";
            this.deleteSectionToolStripMenuItem.Click += new System.EventHandler(this.deleteSectionToolStripMenuItem_Click);
            // 
            // testShallowDeleteToolStripMenuItem
            // 
            this.testShallowDeleteToolStripMenuItem.Name = "testShallowDeleteToolStripMenuItem";
            this.testShallowDeleteToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.testShallowDeleteToolStripMenuItem.Text = "Test shallow delete";
            this.testShallowDeleteToolStripMenuItem.Click += new System.EventHandler(this.testShallowDeleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(193, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(193, 6);
            // 
            // TOCPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tocTree);
            this.Name = "TOCPanel";
            this.Size = new System.Drawing.Size(129, 133);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tocTree;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addSectionAtSameLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSubSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem increaseLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decreaseLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editLabelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testShallowDeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
