namespace Zaboom
{
    partial class ZaboomForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mPlayerStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAudioAssetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadXUKFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXUKFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mAssetBox = new System.Windows.Forms.ComboBox();
            this.mPlayAllButton = new System.Windows.Forms.Button();
            this.mPrevButton = new System.Windows.Forms.Button();
            this.mNextButton = new System.Windows.Forms.Button();
            this.deleteAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Asse&t:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.mPlayerStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 131);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(472, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(84, 17);
            this.toolStripStatusLabel1.Text = "Audio player is:";
            // 
            // mPlayerStatusLabel
            // 
            this.mPlayerStatusLabel.Name = "mPlayerStatusLabel";
            this.mPlayerStatusLabel.Size = new System.Drawing.Size(57, 17);
            this.mPlayerStatusLabel.Text = "(unknown)";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.audioToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(472, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importAudioAssetsToolStripMenuItem,
            this.loadXUKFileToolStripMenuItem,
            this.saveXUKFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // importAudioAssetsToolStripMenuItem
            // 
            this.importAudioAssetsToolStripMenuItem.Name = "importAudioAssetsToolStripMenuItem";
            this.importAudioAssetsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.importAudioAssetsToolStripMenuItem.Text = "&Import audio assets";
            this.importAudioAssetsToolStripMenuItem.Click += new System.EventHandler(this.importAudioAssetsToolStripMenuItem_Click);
            // 
            // loadXUKFileToolStripMenuItem
            // 
            this.loadXUKFileToolStripMenuItem.Name = "loadXUKFileToolStripMenuItem";
            this.loadXUKFileToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.loadXUKFileToolStripMenuItem.Text = "&Load XUK file";
            this.loadXUKFileToolStripMenuItem.Visible = false;
            // 
            // saveXUKFileToolStripMenuItem
            // 
            this.saveXUKFileToolStripMenuItem.Name = "saveXUKFileToolStripMenuItem";
            this.saveXUKFileToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.saveXUKFileToolStripMenuItem.Text = "&Save XUK file";
            this.saveXUKFileToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
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
            this.renameAssetToolStripMenuItem,
            this.deleteAssetToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // renameAssetToolStripMenuItem
            // 
            this.renameAssetToolStripMenuItem.Name = "renameAssetToolStripMenuItem";
            this.renameAssetToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.renameAssetToolStripMenuItem.Text = "&Rename asset";
            this.renameAssetToolStripMenuItem.Click += new System.EventHandler(this.renameAssetToolStripMenuItem_Click);
            // 
            // audioToolStripMenuItem
            // 
            this.audioToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outputDeviceToolStripMenuItem});
            this.audioToolStripMenuItem.Name = "audioToolStripMenuItem";
            this.audioToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.audioToolStripMenuItem.Text = "Audi&o";
            // 
            // outputDeviceToolStripMenuItem
            // 
            this.outputDeviceToolStripMenuItem.Name = "outputDeviceToolStripMenuItem";
            this.outputDeviceToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.outputDeviceToolStripMenuItem.Text = "&Output device";
            this.outputDeviceToolStripMenuItem.Click += new System.EventHandler(this.outputDeviceToolStripMenuItem_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.Location = new System.Drawing.Point(280, 27);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 75);
            this.mStopButton.TabIndex = 2;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mPlayButton
            // 
            this.mPlayButton.Location = new System.Drawing.Point(199, 27);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(75, 75);
            this.mPlayButton.TabIndex = 1;
            this.mPlayButton.Text = "&Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mAssetBox
            // 
            this.mAssetBox.FormattingEnabled = true;
            this.mAssetBox.Location = new System.Drawing.Point(55, 108);
            this.mAssetBox.Name = "mAssetBox";
            this.mAssetBox.Size = new System.Drawing.Size(405, 20);
            this.mAssetBox.TabIndex = 4;
            this.mAssetBox.SelectedIndexChanged += new System.EventHandler(this.mAssetBox_SelectedIndexChanged);
            // 
            // mPlayAllButton
            // 
            this.mPlayAllButton.Location = new System.Drawing.Point(118, 27);
            this.mPlayAllButton.Name = "mPlayAllButton";
            this.mPlayAllButton.Size = new System.Drawing.Size(75, 75);
            this.mPlayAllButton.TabIndex = 0;
            this.mPlayAllButton.Text = "Pl&ay*";
            this.mPlayAllButton.UseVisualStyleBackColor = true;
            this.mPlayAllButton.Click += new System.EventHandler(this.mPlayAllButton_Click);
            // 
            // mPrevButton
            // 
            this.mPrevButton.Location = new System.Drawing.Point(37, 27);
            this.mPrevButton.Name = "mPrevButton";
            this.mPrevButton.Size = new System.Drawing.Size(75, 75);
            this.mPrevButton.TabIndex = 9;
            this.mPrevButton.Text = "Pre&v";
            this.mPrevButton.UseVisualStyleBackColor = true;
            this.mPrevButton.Click += new System.EventHandler(this.mPrevButton_Click);
            // 
            // mNextButton
            // 
            this.mNextButton.Location = new System.Drawing.Point(361, 27);
            this.mNextButton.Name = "mNextButton";
            this.mNextButton.Size = new System.Drawing.Size(75, 75);
            this.mNextButton.TabIndex = 10;
            this.mNextButton.Text = "&Next";
            this.mNextButton.UseVisualStyleBackColor = true;
            this.mNextButton.Click += new System.EventHandler(this.mNextButton_Click);
            // 
            // deleteAssetToolStripMenuItem
            // 
            this.deleteAssetToolStripMenuItem.Name = "deleteAssetToolStripMenuItem";
            this.deleteAssetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteAssetToolStripMenuItem.Text = "&Delete asset";
            this.deleteAssetToolStripMenuItem.Click += new System.EventHandler(this.deleteAssetToolStripMenuItem_Click);
            // 
            // ZaboomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 153);
            this.Controls.Add(this.mNextButton);
            this.Controls.Add(this.mPrevButton);
            this.Controls.Add(this.mPlayAllButton);
            this.Controls.Add(this.mAssetBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.mPlayButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.label2);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ZaboomForm";
            this.Text = "Zaboom";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel mPlayerStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadXUKFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveXUKFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.ComboBox mAssetBox;
        private System.Windows.Forms.Button mPlayAllButton;
        private System.Windows.Forms.ToolStripMenuItem audioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importAudioAssetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameAssetToolStripMenuItem;
        private System.Windows.Forms.Button mPrevButton;
        private System.Windows.Forms.Button mNextButton;
        private System.Windows.Forms.ToolStripMenuItem deleteAssetToolStripMenuItem;
    }
}

