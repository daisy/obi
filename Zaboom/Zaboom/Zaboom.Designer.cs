namespace Zaboom
{
    partial class Zaboom
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
            this.importAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mAssBox = new System.Windows.Forms.ComboBox();
            this.mPlayOneButton = new System.Windows.Forms.Button();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mStopButton = new System.Windows.Forms.Button();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeAssetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detectphrasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.moveAssetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveAssetDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.controlsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(312, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importAssetToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // importAssetToolStripMenuItem
            // 
            this.importAssetToolStripMenuItem.Name = "importAssetToolStripMenuItem";
            this.importAssetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.importAssetToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.importAssetToolStripMenuItem.Text = "&Import asset";
            this.importAssetToolStripMenuItem.Click += new System.EventHandler(this.importAssetToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 144);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(312, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mAssBox
            // 
            this.mAssBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mAssBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mAssBox.FormattingEnabled = true;
            this.mAssBox.Location = new System.Drawing.Point(12, 27);
            this.mAssBox.Name = "mAssBox";
            this.mAssBox.Size = new System.Drawing.Size(288, 20);
            this.mAssBox.TabIndex = 2;
            this.mAssBox.SelectedIndexChanged += new System.EventHandler(this.mAssBox_SelectedIndexChanged);
            // 
            // mPlayOneButton
            // 
            this.mPlayOneButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mPlayOneButton.Location = new System.Drawing.Point(78, 53);
            this.mPlayOneButton.Name = "mPlayOneButton";
            this.mPlayOneButton.Size = new System.Drawing.Size(75, 75);
            this.mPlayOneButton.TabIndex = 3;
            this.mPlayOneButton.Text = "&Play";
            this.mPlayOneButton.UseVisualStyleBackColor = true;
            this.mPlayOneButton.Click += new System.EventHandler(this.mPlayOneButton_Click);
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.controlsToolStripMenuItem.Text = "&Controls";
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Space)));
            this.playToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.playToolStripMenuItem.Text = "&Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(62, 17);
            this.toolStripStatusLabel1.Text = "Initializing...";
            // 
            // mStopButton
            // 
            this.mStopButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.mStopButton.Location = new System.Drawing.Point(159, 53);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 75);
            this.mStopButton.TabIndex = 4;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.stopToolStripMenuItem.Text = "&Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameAssetToolStripMenuItem,
            this.moveAssetupToolStripMenuItem,
            this.moveAssetDownToolStripMenuItem,
            this.toolStripSeparator3,
            this.mergeAssetsToolStripMenuItem,
            this.splitAssetToolStripMenuItem,
            this.detectphrasesToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteAssetToolStripMenuItem,
            this.enableAssetToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // deleteAssetToolStripMenuItem
            // 
            this.deleteAssetToolStripMenuItem.Name = "deleteAssetToolStripMenuItem";
            this.deleteAssetToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteAssetToolStripMenuItem.Text = "&Delete asset";
            // 
            // enableAssetToolStripMenuItem
            // 
            this.enableAssetToolStripMenuItem.CheckOnClick = true;
            this.enableAssetToolStripMenuItem.Name = "enableAssetToolStripMenuItem";
            this.enableAssetToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.enableAssetToolStripMenuItem.Text = "S&kip asset";
            // 
            // splitAssetToolStripMenuItem
            // 
            this.splitAssetToolStripMenuItem.Name = "splitAssetToolStripMenuItem";
            this.splitAssetToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.splitAssetToolStripMenuItem.Text = "&Split asset";
            // 
            // mergeAssetsToolStripMenuItem
            // 
            this.mergeAssetsToolStripMenuItem.Name = "mergeAssetsToolStripMenuItem";
            this.mergeAssetsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.mergeAssetsToolStripMenuItem.Text = "&Merge assets";
            // 
            // detectphrasesToolStripMenuItem
            // 
            this.detectphrasesToolStripMenuItem.Name = "detectphrasesToolStripMenuItem";
            this.detectphrasesToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.detectphrasesToolStripMenuItem.Text = "Detect &phrases";
            // 
            // renameAssetToolStripMenuItem
            // 
            this.renameAssetToolStripMenuItem.Name = "renameAssetToolStripMenuItem";
            this.renameAssetToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.renameAssetToolStripMenuItem.Text = "&Rename asset";
            this.renameAssetToolStripMenuItem.Click += new System.EventHandler(this.renameAssetToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(156, 6);
            // 
            // moveAssetupToolStripMenuItem
            // 
            this.moveAssetupToolStripMenuItem.Name = "moveAssetupToolStripMenuItem";
            this.moveAssetupToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.moveAssetupToolStripMenuItem.Text = "Move asset &up";
            // 
            // moveAssetDownToolStripMenuItem
            // 
            this.moveAssetDownToolStripMenuItem.Name = "moveAssetDownToolStripMenuItem";
            this.moveAssetDownToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.moveAssetDownToolStripMenuItem.Text = "Move asset do&wn";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(156, 6);
            // 
            // Zaboom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 166);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPlayOneButton);
            this.Controls.Add(this.mAssBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Zaboom";
            this.Text = "Zaboom";
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
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem importAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ComboBox mAssBox;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.Button mPlayOneButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeAssetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detectphrasesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveAssetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveAssetDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}

