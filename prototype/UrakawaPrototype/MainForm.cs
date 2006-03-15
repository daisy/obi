using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace UrakawaPrototype
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
    {
        private ToolStrip audioToolStrip;
        private ToolStrip markerToolStrip;
        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem customizeToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem contentsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripButton pauseButton;
        private ToolStripButton recordButton;
        private ToolStripButton previousMarkerButton;
        private ToolStripButton nextMarkerButton;
        private ToolStripButton addMarkerButton;
        private ToolStripButton deleteMarkerButton;
        private ToolStripButton rewindButton;
        private ToolStripButton fastForwardButton;
        private ToolStrip phraseNavToolStrip;
        private ToolStripButton previousBlockButton;
        private ToolStripButton nextBlockButton;
        private ToolStrip pageToolStrip;
        private ToolStripButton previousPageButton;
        private ToolStripButton nextPageButton;
        private ToolStripTextBox goToPageTextBox;
        private ToolStripButton goToPageButton;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem indexToolStripMenuItem;
        private ToolStripMenuItem structureToolStripMenuItem;
        private ToolStripMenuItem addHeadingToolStripMenuItem;
        private ToolStripMenuItem addListsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem indentHeadingToolStripMenuItem;
        private ToolStripMenuItem outdentHeadingToolStripMenuItem;
        private ToolStripMenuItem addPageToolStripMenuItem;
        private ToolStripMenuItem editLabelToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem toggleViewToolStripMenuItem;
        private ToolStripMenuItem increaseFontSizeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem blocksToolStripMenuItem;
        private ToolStripMenuItem moveToolStripMenuItem;
        private ToolStripMenuItem moveRightToolStripMenuItem;
        private ToolStripMenuItem resizeToolStripMenuItem;
        private ToolStripMenuItem makeLongerToolStripMenuItem;
        private ToolStripMenuItem makeShorterToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem nextSectionToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem splitToolStripMenuItem;
        private ToolStripMenuItem mergeAdjacentToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem isPartOfPresentationToolStripMenuItem;
        private ToolStripMenuItem markersToolStripMenuItem;
        private ToolStripMenuItem addMarkerToolStripMenuItem;
        private ToolStripMenuItem deleteMarkerToolStripMenuItem1;
        private ToolStripMenuItem previousMarkerToolStripMenuItem;
        private ToolStripMenuItem nextMarkerToolStripMenuItem2;
        private ToolStripMenuItem importAssetsToolStripMenuItem;
        private ToolStripMenuItem cleanProjectToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem beneathCurrentToolStripMenuItem;
        private ToolStripMenuItem beforeCurrentToolStripMenuItem;
        private ToolStripMenuItem addSinglePageToolStripMenuItem;
        private ToolStripMenuItem addARangeOfPagesToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private SplitContainer structureAndBlocks;
        private StructureView structureView1;
        private SplitContainer blocksAndDesc;
        private BlocksView blocksView1;
        private TextBox textBox1;
        private ToolStripMenuItem editMetadataToolStripMenuItem;
        private ToolStripSplitButton playButton;
        private ToolStripMenuItem playAllToolStripMenuItem;
        private ToolStripMenuItem playSelectedToolStripMenuItem;
        private ToolStripMenuItem playFromCursorToolStripMenuItem;
        private ToolStrip structureNavToolStrip;
        private ToolStripButton previousSectionButton;
        private ToolStripButton nextSectionButton;
        private ToolStripMenuItem moveUpToolStripMenuItem;
        private ToolStripMenuItem moveDownToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem previousBlocToolStripMenuItem;
        private ToolStripMenuItem nextBlockToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem renameItemToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripContainer toolStripContainer1;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.ToolStripMenuItem previousSectionToolStripMenuItem;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.structureAndBlocks = new System.Windows.Forms.SplitContainer();
            this.blocksAndDesc = new System.Windows.Forms.SplitContainer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.structureNavToolStrip = new System.Windows.Forms.ToolStrip();
            this.previousSectionButton = new System.Windows.Forms.ToolStripButton();
            this.nextSectionButton = new System.Windows.Forms.ToolStripButton();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.importAssetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.structureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.increaseFontSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.nextSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.addHeadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beneathCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beforeCurrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addListsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSinglePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addARangeOfPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.indentHeadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outdentHeadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.renameItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.resizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeLongerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeShorterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeAdjacentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.previousBlocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.isPartOfPresentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.previousMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextMarkerToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteMarkerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pageToolStrip = new System.Windows.Forms.ToolStrip();
            this.previousPageButton = new System.Windows.Forms.ToolStripButton();
            this.nextPageButton = new System.Windows.Forms.ToolStripButton();
            this.goToPageTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.goToPageButton = new System.Windows.Forms.ToolStripButton();
            this.audioToolStrip = new System.Windows.Forms.ToolStrip();
            this.playButton = new System.Windows.Forms.ToolStripSplitButton();
            this.playAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playFromCursorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseButton = new System.Windows.Forms.ToolStripButton();
            this.recordButton = new System.Windows.Forms.ToolStripButton();
            this.rewindButton = new System.Windows.Forms.ToolStripButton();
            this.fastForwardButton = new System.Windows.Forms.ToolStripButton();
            this.phraseNavToolStrip = new System.Windows.Forms.ToolStrip();
            this.previousBlockButton = new System.Windows.Forms.ToolStripButton();
            this.nextBlockButton = new System.Windows.Forms.ToolStripButton();
            this.markerToolStrip = new System.Windows.Forms.ToolStrip();
            this.previousMarkerButton = new System.Windows.Forms.ToolStripButton();
            this.nextMarkerButton = new System.Windows.Forms.ToolStripButton();
            this.addMarkerButton = new System.Windows.Forms.ToolStripButton();
            this.deleteMarkerButton = new System.Windows.Forms.ToolStripButton();
            this.structureView1 = new UrakawaPrototype.StructureView();
            this.blocksView1 = new UrakawaPrototype.BlocksView();
            previousSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.LeftToolStripPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.structureAndBlocks.Panel1.SuspendLayout();
            this.structureAndBlocks.Panel2.SuspendLayout();
            this.structureAndBlocks.SuspendLayout();
            this.blocksAndDesc.Panel1.SuspendLayout();
            this.blocksAndDesc.Panel2.SuspendLayout();
            this.blocksAndDesc.SuspendLayout();
            this.structureNavToolStrip.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.pageToolStrip.SuspendLayout();
            this.audioToolStrip.SuspendLayout();
            this.phraseNavToolStrip.SuspendLayout();
            this.markerToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // previousSectionToolStripMenuItem
            // 
            previousSectionToolStripMenuItem.Name = "previousSectionToolStripMenuItem";
            previousSectionToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            previousSectionToolStripMenuItem.Text = "Previous item";
            previousSectionToolStripMenuItem.Click += new System.EventHandler(this.previousToolStripMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.structureAndBlocks);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(568, 371);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.Controls.Add(this.structureNavToolStrip);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(592, 442);
            this.toolStripContainer1.TabIndex = 9;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.pageToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.markerToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.mainMenuStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.audioToolStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.phraseNavToolStrip);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(592, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // structureAndBlocks
            // 
            this.structureAndBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structureAndBlocks.Location = new System.Drawing.Point(0, 0);
            this.structureAndBlocks.Name = "structureAndBlocks";
            // 
            // structureAndBlocks.Panel1
            // 
            this.structureAndBlocks.Panel1.Controls.Add(this.structureView1);
            // 
            // structureAndBlocks.Panel2
            // 
            this.structureAndBlocks.Panel2.Controls.Add(this.blocksAndDesc);
            this.structureAndBlocks.Size = new System.Drawing.Size(568, 371);
            this.structureAndBlocks.SplitterDistance = 188;
            this.structureAndBlocks.TabIndex = 1;
            // 
            // blocksAndDesc
            // 
            this.blocksAndDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blocksAndDesc.Location = new System.Drawing.Point(0, 0);
            this.blocksAndDesc.Name = "blocksAndDesc";
            this.blocksAndDesc.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // blocksAndDesc.Panel1
            // 
            this.blocksAndDesc.Panel1.Controls.Add(this.blocksView1);
            this.blocksAndDesc.Panel1MinSize = 340;
            // 
            // blocksAndDesc.Panel2
            // 
            this.blocksAndDesc.Panel2.Controls.Add(this.textBox1);
            this.blocksAndDesc.Size = new System.Drawing.Size(376, 371);
            this.blocksAndDesc.SplitterDistance = 340;
            this.blocksAndDesc.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(376, 27);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = "Phrase 23, in Chapter 2\r\n15 seconds long\r\nAudio source: c:\\bookproject\\file.wav f" +
                "rom 2s to 17s\r\nThis block is also a structure point";
            // 
            // structureNavToolStrip
            // 
            this.structureNavToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.structureNavToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previousSectionButton,
            this.nextSectionButton});
            this.structureNavToolStrip.Location = new System.Drawing.Point(0, 3);
            this.structureNavToolStrip.Name = "structureNavToolStrip";
            this.structureNavToolStrip.Size = new System.Drawing.Size(24, 55);
            this.structureNavToolStrip.TabIndex = 6;
            // 
            // previousSectionButton
            // 
            this.previousSectionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previousSectionButton.Image = global::UrakawaPrototype.Properties.Resources.previousSection;
            this.previousSectionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previousSectionButton.Name = "previousSectionButton";
            this.previousSectionButton.Size = new System.Drawing.Size(22, 20);
            this.previousSectionButton.Text = "Previous Section";
            this.previousSectionButton.Click += new System.EventHandler(this.previousSectionClick);
            // 
            // nextSectionButton
            // 
            this.nextSectionButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextSectionButton.Image = global::UrakawaPrototype.Properties.Resources.nextSection;
            this.nextSectionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextSectionButton.Name = "nextSectionButton";
            this.nextSectionButton.Size = new System.Drawing.Size(22, 20);
            this.nextSectionButton.Text = "Next Section";
            this.nextSectionButton.Click += new System.EventHandler(this.nextSection_Click);
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.structureToolStripMenuItem,
            this.blocksToolStripMenuItem,
            this.markersToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(592, 24);
            this.mainMenuStrip.TabIndex = 3;
            this.mainMenuStrip.Text = "Main Menu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator,
            this.importAssetsToolStripMenuItem,
            this.editMetadataToolStripMenuItem,
            this.cleanProjectToolStripMenuItem,
            this.toolStripSeparator6,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(150, 6);
            // 
            // importAssetsToolStripMenuItem
            // 
            this.importAssetsToolStripMenuItem.Name = "importAssetsToolStripMenuItem";
            this.importAssetsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.importAssetsToolStripMenuItem.Text = "Import audio...";
            this.importAssetsToolStripMenuItem.Click += new System.EventHandler(this.importAssetsToolStripMenuItem_Click);
            // 
            // editMetadataToolStripMenuItem
            // 
            this.editMetadataToolStripMenuItem.Name = "editMetadataToolStripMenuItem";
            this.editMetadataToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.editMetadataToolStripMenuItem.Text = "Edit Metadata...";
            this.editMetadataToolStripMenuItem.Click += new System.EventHandler(this.editMetadataToolStripMenuItem_Click);
            // 
            // cleanProjectToolStripMenuItem
            // 
            this.cleanProjectToolStripMenuItem.Name = "cleanProjectToolStripMenuItem";
            this.cleanProjectToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.cleanProjectToolStripMenuItem.Text = "Clean project";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(150, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(150, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.redoToolStripMenuItem.Text = "&Redo";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(114, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(114, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // structureToolStripMenuItem
            // 
            this.structureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleViewToolStripMenuItem,
            this.increaseFontSizeToolStripMenuItem,
            this.toolStripSeparator10,
            previousSectionToolStripMenuItem,
            this.nextSectionToolStripMenuItem,
            this.toolStripSeparator7,
            this.addHeadingToolStripMenuItem,
            this.addListsToolStripMenuItem,
            this.addPageToolStripMenuItem,
            this.toolStripSeparator1,
            this.indentHeadingToolStripMenuItem,
            this.outdentHeadingToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparator11,
            this.renameItemToolStripMenuItem,
            this.editLabelToolStripMenuItem});
            this.structureToolStripMenuItem.Name = "structureToolStripMenuItem";
            this.structureToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.structureToolStripMenuItem.Text = "Structure";
            // 
            // toggleViewToolStripMenuItem
            // 
            this.toggleViewToolStripMenuItem.Checked = true;
            this.toggleViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleViewToolStripMenuItem.Name = "toggleViewToolStripMenuItem";
            this.toggleViewToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.toggleViewToolStripMenuItem.Text = "Toggle view";
            this.toggleViewToolStripMenuItem.Click += new System.EventHandler(this.toggleViewToolStripMenuItem_Click);
            // 
            // increaseFontSizeToolStripMenuItem
            // 
            this.increaseFontSizeToolStripMenuItem.Name = "increaseFontSizeToolStripMenuItem";
            this.increaseFontSizeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.increaseFontSizeToolStripMenuItem.Text = "Increase font size";
            this.increaseFontSizeToolStripMenuItem.Click += new System.EventHandler(this.increaseFontSizeToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(157, 6);
            // 
            // nextSectionToolStripMenuItem
            // 
            this.nextSectionToolStripMenuItem.Name = "nextSectionToolStripMenuItem";
            this.nextSectionToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.nextSectionToolStripMenuItem.Text = "Next item";
            this.nextSectionToolStripMenuItem.Click += new System.EventHandler(this.nextSectionToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(157, 6);
            // 
            // addHeadingToolStripMenuItem
            // 
            this.addHeadingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.beneathCurrentToolStripMenuItem,
            this.beforeCurrentToolStripMenuItem});
            this.addHeadingToolStripMenuItem.Name = "addHeadingToolStripMenuItem";
            this.addHeadingToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addHeadingToolStripMenuItem.Text = "Add heading";
            // 
            // beneathCurrentToolStripMenuItem
            // 
            this.beneathCurrentToolStripMenuItem.Name = "beneathCurrentToolStripMenuItem";
            this.beneathCurrentToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.beneathCurrentToolStripMenuItem.Text = "As sibling";
            // 
            // beforeCurrentToolStripMenuItem
            // 
            this.beforeCurrentToolStripMenuItem.Name = "beforeCurrentToolStripMenuItem";
            this.beforeCurrentToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.beforeCurrentToolStripMenuItem.Text = "As child";
            // 
            // addListsToolStripMenuItem
            // 
            this.addListsToolStripMenuItem.Name = "addListsToolStripMenuItem";
            this.addListsToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addListsToolStripMenuItem.Text = "Add list";
            // 
            // addPageToolStripMenuItem
            // 
            this.addPageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSinglePageToolStripMenuItem,
            this.addARangeOfPagesToolStripMenuItem});
            this.addPageToolStripMenuItem.Name = "addPageToolStripMenuItem";
            this.addPageToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addPageToolStripMenuItem.Text = "Add page";
            // 
            // addSinglePageToolStripMenuItem
            // 
            this.addSinglePageToolStripMenuItem.Name = "addSinglePageToolStripMenuItem";
            this.addSinglePageToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.addSinglePageToolStripMenuItem.Text = "Single";
            // 
            // addARangeOfPagesToolStripMenuItem
            // 
            this.addARangeOfPagesToolStripMenuItem.Name = "addARangeOfPagesToolStripMenuItem";
            this.addARangeOfPagesToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.addARangeOfPagesToolStripMenuItem.Text = "Range";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // indentHeadingToolStripMenuItem
            // 
            this.indentHeadingToolStripMenuItem.Name = "indentHeadingToolStripMenuItem";
            this.indentHeadingToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.indentHeadingToolStripMenuItem.Text = "Indent";
            this.indentHeadingToolStripMenuItem.Click += new System.EventHandler(this.indentHeadingToolStripMenuItem_Click);
            // 
            // outdentHeadingToolStripMenuItem
            // 
            this.outdentHeadingToolStripMenuItem.Name = "outdentHeadingToolStripMenuItem";
            this.outdentHeadingToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.outdentHeadingToolStripMenuItem.Text = "Outdent";
            this.outdentHeadingToolStripMenuItem.Click += new System.EventHandler(this.outdentHeadingToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(157, 6);
            // 
            // renameItemToolStripMenuItem
            // 
            this.renameItemToolStripMenuItem.Name = "renameItemToolStripMenuItem";
            this.renameItemToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.renameItemToolStripMenuItem.Text = "Rename item";
            this.renameItemToolStripMenuItem.Click += new System.EventHandler(this.renameItemToolStripMenuItem_Click);
            // 
            // editLabelToolStripMenuItem
            // 
            this.editLabelToolStripMenuItem.Name = "editLabelToolStripMenuItem";
            this.editLabelToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.editLabelToolStripMenuItem.Text = "Edit label...";
            this.editLabelToolStripMenuItem.Click += new System.EventHandler(this.editLabelToolStripMenuItem_Click);
            // 
            // blocksToolStripMenuItem
            // 
            this.blocksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToolStripMenuItem,
            this.moveRightToolStripMenuItem,
            this.toolStripSeparator9,
            this.resizeToolStripMenuItem,
            this.splitToolStripMenuItem,
            this.mergeAdjacentToolStripMenuItem,
            this.toolStripSeparator8,
            this.previousBlocToolStripMenuItem,
            this.nextBlockToolStripMenuItem,
            this.toolStripSeparator12,
            this.isPartOfPresentationToolStripMenuItem});
            this.blocksToolStripMenuItem.Name = "blocksToolStripMenuItem";
            this.blocksToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.blocksToolStripMenuItem.Text = "Phrase Blocks";
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.moveToolStripMenuItem.Text = "Move left";
            // 
            // moveRightToolStripMenuItem
            // 
            this.moveRightToolStripMenuItem.Name = "moveRightToolStripMenuItem";
            this.moveRightToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.moveRightToolStripMenuItem.Text = "Move right";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(203, 6);
            // 
            // resizeToolStripMenuItem
            // 
            this.resizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeLongerToolStripMenuItem,
            this.makeShorterToolStripMenuItem});
            this.resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
            this.resizeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.resizeToolStripMenuItem.Text = "Resize";
            // 
            // makeLongerToolStripMenuItem
            // 
            this.makeLongerToolStripMenuItem.Name = "makeLongerToolStripMenuItem";
            this.makeLongerToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.makeLongerToolStripMenuItem.Text = "Move starting point";
            // 
            // makeShorterToolStripMenuItem
            // 
            this.makeShorterToolStripMenuItem.Name = "makeShorterToolStripMenuItem";
            this.makeShorterToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.makeShorterToolStripMenuItem.Text = "Move ending point";
            // 
            // splitToolStripMenuItem
            // 
            this.splitToolStripMenuItem.Name = "splitToolStripMenuItem";
            this.splitToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.splitToolStripMenuItem.Text = "Split";
            // 
            // mergeAdjacentToolStripMenuItem
            // 
            this.mergeAdjacentToolStripMenuItem.Name = "mergeAdjacentToolStripMenuItem";
            this.mergeAdjacentToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mergeAdjacentToolStripMenuItem.Text = "Merge adjacent";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(203, 6);
            // 
            // previousBlocToolStripMenuItem
            // 
            this.previousBlocToolStripMenuItem.Name = "previousBlocToolStripMenuItem";
            this.previousBlocToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.previousBlocToolStripMenuItem.Text = "Previous block";
            // 
            // nextBlockToolStripMenuItem
            // 
            this.nextBlockToolStripMenuItem.Name = "nextBlockToolStripMenuItem";
            this.nextBlockToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.nextBlockToolStripMenuItem.Text = "Next block";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(203, 6);
            // 
            // isPartOfPresentationToolStripMenuItem
            // 
            this.isPartOfPresentationToolStripMenuItem.Name = "isPartOfPresentationToolStripMenuItem";
            this.isPartOfPresentationToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.isPartOfPresentationToolStripMenuItem.Text = "Excluded from presentation";
            this.isPartOfPresentationToolStripMenuItem.Click += new System.EventHandler(this.isPartOfPresentationToolStripMenuItem_Click);
            // 
            // markersToolStripMenuItem
            // 
            this.markersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMarkerToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripSeparator13,
            this.previousMarkerToolStripMenuItem,
            this.nextMarkerToolStripMenuItem2,
            this.toolStripSeparator14,
            this.deleteMarkerToolStripMenuItem1});
            this.markersToolStripMenuItem.Name = "markersToolStripMenuItem";
            this.markersToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.markersToolStripMenuItem.Text = "Markers";
            // 
            // addMarkerToolStripMenuItem
            // 
            this.addMarkerToolStripMenuItem.Name = "addMarkerToolStripMenuItem";
            this.addMarkerToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.addMarkerToolStripMenuItem.Text = "Add";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(112, 6);
            // 
            // previousMarkerToolStripMenuItem
            // 
            this.previousMarkerToolStripMenuItem.Name = "previousMarkerToolStripMenuItem";
            this.previousMarkerToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.previousMarkerToolStripMenuItem.Text = "Previous";
            // 
            // nextMarkerToolStripMenuItem2
            // 
            this.nextMarkerToolStripMenuItem2.Name = "nextMarkerToolStripMenuItem2";
            this.nextMarkerToolStripMenuItem2.Size = new System.Drawing.Size(115, 22);
            this.nextMarkerToolStripMenuItem2.Text = "Next";
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(112, 6);
            // 
            // deleteMarkerToolStripMenuItem1
            // 
            this.deleteMarkerToolStripMenuItem1.Name = "deleteMarkerToolStripMenuItem1";
            this.deleteMarkerToolStripMenuItem1.Size = new System.Drawing.Size(115, 22);
            this.deleteMarkerToolStripMenuItem1.Text = "Delete";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(115, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // pageToolStrip
            // 
            this.pageToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pageToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previousPageButton,
            this.nextPageButton,
            this.goToPageTextBox,
            this.goToPageButton});
            this.pageToolStrip.Location = new System.Drawing.Point(230, 24);
            this.pageToolStrip.Name = "pageToolStrip";
            this.pageToolStrip.Size = new System.Drawing.Size(101, 25);
            this.pageToolStrip.TabIndex = 5;
            // 
            // previousPageButton
            // 
            this.previousPageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previousPageButton.Image = global::UrakawaPrototype.Properties.Resources.previousPage;
            this.previousPageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previousPageButton.Name = "previousPageButton";
            this.previousPageButton.Size = new System.Drawing.Size(23, 22);
            this.previousPageButton.Text = "Previous Page";
            // 
            // nextPageButton
            // 
            this.nextPageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextPageButton.Image = global::UrakawaPrototype.Properties.Resources.nextPage;
            this.nextPageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextPageButton.Name = "nextPageButton";
            this.nextPageButton.Size = new System.Drawing.Size(23, 22);
            this.nextPageButton.Text = "Next Page";
            // 
            // goToPageTextBox
            // 
            this.goToPageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.goToPageTextBox.Name = "goToPageTextBox";
            this.goToPageTextBox.Size = new System.Drawing.Size(20, 25);
            // 
            // goToPageButton
            // 
            this.goToPageButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goToPageButton.Image = global::UrakawaPrototype.Properties.Resources.gotopage;
            this.goToPageButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goToPageButton.Name = "goToPageButton";
            this.goToPageButton.Size = new System.Drawing.Size(23, 22);
            this.goToPageButton.Text = "Go To Page";
            // 
            // audioToolStrip
            // 
            this.audioToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.audioToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playButton,
            this.pauseButton,
            this.recordButton,
            this.rewindButton,
            this.fastForwardButton});
            this.audioToolStrip.Location = new System.Drawing.Point(3, 24);
            this.audioToolStrip.Name = "audioToolStrip";
            this.audioToolStrip.Size = new System.Drawing.Size(165, 25);
            this.audioToolStrip.TabIndex = 0;
            this.audioToolStrip.EndDrag += new System.EventHandler(this.endDragAudioToolstrip);
            // 
            // playButton
            // 
            this.playButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.playButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playAllToolStripMenuItem,
            this.playSelectedToolStripMenuItem,
            this.playFromCursorToolStripMenuItem});
            this.playButton.Image = global::UrakawaPrototype.Properties.Resources.play;
            this.playButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(32, 22);
            this.playButton.Text = "toolStripDropDownButton1";
            // 
            // playAllToolStripMenuItem
            // 
            this.playAllToolStripMenuItem.Name = "playAllToolStripMenuItem";
            this.playAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.playAllToolStripMenuItem.Text = "Play all";
            // 
            // playSelectedToolStripMenuItem
            // 
            this.playSelectedToolStripMenuItem.Name = "playSelectedToolStripMenuItem";
            this.playSelectedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.playSelectedToolStripMenuItem.Text = "Play selected";
            // 
            // playFromCursorToolStripMenuItem
            // 
            this.playFromCursorToolStripMenuItem.Name = "playFromCursorToolStripMenuItem";
            this.playFromCursorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.playFromCursorToolStripMenuItem.Text = "Play from cursor";
            // 
            // pauseButton
            // 
            this.pauseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseButton.Image = global::UrakawaPrototype.Properties.Resources.pause;
            this.pauseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(23, 22);
            this.pauseButton.Text = "Pause";
            // 
            // recordButton
            // 
            this.recordButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.recordButton.Image = global::UrakawaPrototype.Properties.Resources.record;
            this.recordButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(23, 22);
            this.recordButton.Text = "Record";
            // 
            // rewindButton
            // 
            this.rewindButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rewindButton.Image = global::UrakawaPrototype.Properties.Resources.rewind;
            this.rewindButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rewindButton.Name = "rewindButton";
            this.rewindButton.Size = new System.Drawing.Size(23, 22);
            this.rewindButton.Text = "Rewind";
            // 
            // fastForwardButton
            // 
            this.fastForwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fastForwardButton.Image = global::UrakawaPrototype.Properties.Resources.fastforward;
            this.fastForwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fastForwardButton.Name = "fastForwardButton";
            this.fastForwardButton.Size = new System.Drawing.Size(23, 22);
            this.fastForwardButton.Text = "Fast Forward";
            // 
            // phraseNavToolStrip
            // 
            this.phraseNavToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.phraseNavToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previousBlockButton,
            this.nextBlockButton});
            this.phraseNavToolStrip.Location = new System.Drawing.Point(171, 24);
            this.phraseNavToolStrip.Name = "phraseNavToolStrip";
            this.phraseNavToolStrip.Size = new System.Drawing.Size(56, 25);
            this.phraseNavToolStrip.TabIndex = 4;
            // 
            // previousBlockButton
            // 
            this.previousBlockButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previousBlockButton.Image = global::UrakawaPrototype.Properties.Resources.previousBlock;
            this.previousBlockButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previousBlockButton.Name = "previousBlockButton";
            this.previousBlockButton.Size = new System.Drawing.Size(23, 22);
            this.previousBlockButton.Text = "Previous Block";
            // 
            // nextBlockButton
            // 
            this.nextBlockButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextBlockButton.Image = global::UrakawaPrototype.Properties.Resources.nextBlock;
            this.nextBlockButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextBlockButton.Name = "nextBlockButton";
            this.nextBlockButton.Size = new System.Drawing.Size(23, 22);
            this.nextBlockButton.Text = "Next Block";
            // 
            // markerToolStrip
            // 
            this.markerToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.markerToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previousMarkerButton,
            this.nextMarkerButton,
            this.addMarkerButton,
            this.deleteMarkerButton});
            this.markerToolStrip.Location = new System.Drawing.Point(335, 24);
            this.markerToolStrip.Name = "markerToolStrip";
            this.markerToolStrip.Size = new System.Drawing.Size(102, 25);
            this.markerToolStrip.TabIndex = 1;
            // 
            // previousMarkerButton
            // 
            this.previousMarkerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previousMarkerButton.Image = global::UrakawaPrototype.Properties.Resources.previousMarker;
            this.previousMarkerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.previousMarkerButton.Name = "previousMarkerButton";
            this.previousMarkerButton.Size = new System.Drawing.Size(23, 22);
            this.previousMarkerButton.Text = "Previous Marker";
            // 
            // nextMarkerButton
            // 
            this.nextMarkerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextMarkerButton.Image = global::UrakawaPrototype.Properties.Resources.nextMarker;
            this.nextMarkerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextMarkerButton.Name = "nextMarkerButton";
            this.nextMarkerButton.Size = new System.Drawing.Size(23, 22);
            this.nextMarkerButton.Text = "Next Marker";
            // 
            // addMarkerButton
            // 
            this.addMarkerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addMarkerButton.Image = global::UrakawaPrototype.Properties.Resources.addMarker;
            this.addMarkerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addMarkerButton.Name = "addMarkerButton";
            this.addMarkerButton.Size = new System.Drawing.Size(23, 22);
            this.addMarkerButton.Text = "Add Marker";
            // 
            // deleteMarkerButton
            // 
            this.deleteMarkerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteMarkerButton.Image = global::UrakawaPrototype.Properties.Resources.deleteMarker;
            this.deleteMarkerButton.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(204)))));
            this.deleteMarkerButton.Name = "deleteMarkerButton";
            this.deleteMarkerButton.Size = new System.Drawing.Size(23, 22);
            this.deleteMarkerButton.Text = "Delete Marker";
            // 
            // structureView1
            // 
            this.structureView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structureView1.Location = new System.Drawing.Point(0, 0);
            this.structureView1.Name = "structureView1";
            this.structureView1.Size = new System.Drawing.Size(188, 371);
            this.structureView1.TabIndex = 0;
            // 
            // blocksView1
            // 
            this.blocksView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blocksView1.Location = new System.Drawing.Point(0, 0);
            this.blocksView1.Name = "blocksView1";
            this.blocksView1.Size = new System.Drawing.Size(376, 340);
            this.blocksView1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(592, 442);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "MainForm";
            this.Text = "Happy Fun DAISY Time";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.showMainForm);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.LeftToolStripPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.structureAndBlocks.Panel1.ResumeLayout(false);
            this.structureAndBlocks.Panel2.ResumeLayout(false);
            this.structureAndBlocks.ResumeLayout(false);
            this.blocksAndDesc.Panel1.ResumeLayout(false);
            this.blocksAndDesc.Panel2.ResumeLayout(false);
            this.blocksAndDesc.Panel2.PerformLayout();
            this.blocksAndDesc.ResumeLayout(false);
            this.structureNavToolStrip.ResumeLayout(false);
            this.structureNavToolStrip.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.pageToolStrip.ResumeLayout(false);
            this.pageToolStrip.PerformLayout();
            this.audioToolStrip.ResumeLayout(false);
            this.audioToolStrip.PerformLayout();
            this.phraseNavToolStrip.ResumeLayout(false);
            this.phraseNavToolStrip.PerformLayout();
            this.markerToolStrip.ResumeLayout(false);
            this.markerToolStrip.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}


        private void appendSegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.ShowDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsDialog settings = new SettingsDialog();
            settings.ShowDialog();
        }

        private void toggleViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if the left panel is small (less than 10% of screen space)
            //then make it bigger
            //else make it smaller

           
            if (this.structureAndBlocks.SplitterDistance > 25)
            {
                this.structureAndBlocks.SplitterDistance = 25;

                this.toggleViewToolStripMenuItem.Checked = false;
            }
            else
            {
                this.structureAndBlocks.SplitterDistance = (int)((double)this.Width * .25);
                this.toggleViewToolStripMenuItem.Checked = true;
            }

            
        }

        private void showMainForm(object sender, EventArgs e)
        {
            this.structureAndBlocks.SplitterDistance = (int)((double)this.Width * .25);
        }

        private void editMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MetadataEditorDialog meta = new MetadataEditorDialog();
            meta.ShowDialog();
        }

        private void importAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AudioImportDialog dlg = new AudioImportDialog();
            dlg.ShowDialog();
        }

        private void editLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.editLabel();
        }

        private void isPartOfPresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //toggle the value
            //no real functionality
            isPartOfPresentationToolStripMenuItem.Checked =
                !(isPartOfPresentationToolStripMenuItem.Checked);

        }

        private void indentHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.indent();
        }

        private void previousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.previous();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.delete();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.structureView1.Focused == true)
            {
                this.structureView1.selectAll();
            }
        }

        private void nextSection_Click(object sender, EventArgs e)
        {
            this.structureView1.next();
        }

        private void previousSectionClick(object sender, EventArgs e)
        {
            this.structureView1.previous();
        }

        private void nextSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.next();

        }

        private void outdentHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.outdent();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.moveUp();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.moveDown();
        }

        private void renameItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.rename();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.structureView1.resizeFont();
        }

        private void endDragAudioToolstrip(object sender, EventArgs e)
        {
            
        }

        
	}
}
