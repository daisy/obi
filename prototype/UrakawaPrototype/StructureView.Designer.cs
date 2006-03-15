namespace UrakawaPrototype
{
    partial class StructureView
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Part 1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Part 2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Chapter One", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Part 1");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Chapter Two", new System.Windows.Forms.TreeNode[] {
            treeNode4});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StructureView));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addHeadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asSiblingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asChildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSinglePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aRangeOfPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.indentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outdentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.renameLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.LabelEdit = true;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Part 1";
            treeNode2.Name = "Node3";
            treeNode2.Text = "Part 2";
            treeNode3.ImageIndex = 0;
            treeNode3.Name = "Node0";
            treeNode3.Text = "Chapter One";
            treeNode4.Name = "Node5";
            treeNode4.Text = "Part 1";
            treeNode5.Name = "Node4";
            treeNode5.Text = "Chapter Two";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode5});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(301, 347);
            this.treeView1.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addHeadingToolStripMenuItem,
            this.addListToolStripMenuItem,
            this.addPageToolStripMenuItem,
            this.toolStripSeparator1,
            this.indentToolStripMenuItem,
            this.outdentToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparator3,
            this.renameLabelToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 242);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // addHeadingToolStripMenuItem
            // 
            this.addHeadingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asSiblingToolStripMenuItem,
            this.asChildToolStripMenuItem});
            this.addHeadingToolStripMenuItem.Name = "addHeadingToolStripMenuItem";
            this.addHeadingToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.addHeadingToolStripMenuItem.Text = "Add heading";
            // 
            // asSiblingToolStripMenuItem
            // 
            this.asSiblingToolStripMenuItem.Name = "asSiblingToolStripMenuItem";
            this.asSiblingToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.asSiblingToolStripMenuItem.Text = "As sibling";
            this.asSiblingToolStripMenuItem.Click += new System.EventHandler(this.asSiblingToolStripMenuItem_Click);
            // 
            // asChildToolStripMenuItem
            // 
            this.asChildToolStripMenuItem.Name = "asChildToolStripMenuItem";
            this.asChildToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.asChildToolStripMenuItem.Text = "As child";
            this.asChildToolStripMenuItem.Click += new System.EventHandler(this.asChildToolStripMenuItem_Click);
            // 
            // addListToolStripMenuItem
            // 
            this.addListToolStripMenuItem.Name = "addListToolStripMenuItem";
            this.addListToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.addListToolStripMenuItem.Text = "Add list";
            // 
            // addPageToolStripMenuItem
            // 
            this.addPageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aSinglePageToolStripMenuItem,
            this.aRangeOfPagesToolStripMenuItem});
            this.addPageToolStripMenuItem.Name = "addPageToolStripMenuItem";
            this.addPageToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.addPageToolStripMenuItem.Text = "Add page";
            // 
            // aSinglePageToolStripMenuItem
            // 
            this.aSinglePageToolStripMenuItem.Name = "aSinglePageToolStripMenuItem";
            this.aSinglePageToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.aSinglePageToolStripMenuItem.Text = "Single";
            // 
            // aRangeOfPagesToolStripMenuItem
            // 
            this.aRangeOfPagesToolStripMenuItem.Name = "aRangeOfPagesToolStripMenuItem";
            this.aRangeOfPagesToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.aRangeOfPagesToolStripMenuItem.Text = "Range";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(131, 6);
            // 
            // indentToolStripMenuItem
            // 
            this.indentToolStripMenuItem.Name = "indentToolStripMenuItem";
            this.indentToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.indentToolStripMenuItem.Text = "Indent";
            this.indentToolStripMenuItem.Click += new System.EventHandler(this.indentToolStripMenuItem_Click);
            // 
            // outdentToolStripMenuItem
            // 
            this.outdentToolStripMenuItem.Name = "outdentToolStripMenuItem";
            this.outdentToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.outdentToolStripMenuItem.Text = "Outdent";
            this.outdentToolStripMenuItem.Click += new System.EventHandler(this.outdentToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(131, 6);
            // 
            // renameLabelToolStripMenuItem
            // 
            this.renameLabelToolStripMenuItem.Name = "renameLabelToolStripMenuItem";
            this.renameLabelToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.renameLabelToolStripMenuItem.Text = "Rename";
            this.renameLabelToolStripMenuItem.Click += new System.EventHandler(this.renameLabelToolStripMenuItem_Click_1);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.renameToolStripMenuItem.Text = "Edit label...";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(131, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.deleteToolStripMenuItem.Text = "Delete item";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "labelIcon1.jpg");
            // 
            // StructureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Name = "StructureView";
            this.Size = new System.Drawing.Size(301, 347);
            this.Load += new System.EventHandler(this.load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem indentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outdentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSinglePageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aRangeOfPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addHeadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asChildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem renameLabelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asSiblingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ImageList imageList1;
    }
}
