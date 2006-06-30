namespace Zaboom
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.mDeviceBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mPlayerStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadXUKFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXUKFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newAudioAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAssetBox = new System.Windows.Forms.TextBox();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mLoadButton = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "&Device:";
            // 
            // mDeviceBox
            // 
            this.mDeviceBox.FormattingEnabled = true;
            this.mDeviceBox.Location = new System.Drawing.Point(60, 27);
            this.mDeviceBox.Name = "mDeviceBox";
            this.mDeviceBox.Size = new System.Drawing.Size(400, 20);
            this.mDeviceBox.TabIndex = 4;
            this.mDeviceBox.SelectedIndexChanged += new System.EventHandler(this.mDeviceBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "&Asset:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.mPlayerStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 150);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(472, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(107, 17);
            this.toolStripStatusLabel1.Text = "Audio player status:";
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
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(472, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadXUKFileToolStripMenuItem,
            this.saveXUKFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadXUKFileToolStripMenuItem
            // 
            this.loadXUKFileToolStripMenuItem.Name = "loadXUKFileToolStripMenuItem";
            this.loadXUKFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadXUKFileToolStripMenuItem.Text = "&Load XUK file";
            this.loadXUKFileToolStripMenuItem.Visible = false;
            // 
            // saveXUKFileToolStripMenuItem
            // 
            this.saveXUKFileToolStripMenuItem.Name = "saveXUKFileToolStripMenuItem";
            this.saveXUKFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveXUKFileToolStripMenuItem.Text = "&Save XUK file";
            this.saveXUKFileToolStripMenuItem.Visible = false;
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            this.toolStripSeparator1.Visible = false;
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newAudioAssetToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // newAudioAssetToolStripMenuItem
            // 
            this.newAudioAssetToolStripMenuItem.Name = "newAudioAssetToolStripMenuItem";
            this.newAudioAssetToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.newAudioAssetToolStripMenuItem.Text = "&New audio asset";
            this.newAudioAssetToolStripMenuItem.Click += new System.EventHandler(this.newAudioAssetToolStripMenuItem_Click);
            // 
            // mAssetBox
            // 
            this.mAssetBox.Location = new System.Drawing.Point(60, 53);
            this.mAssetBox.Name = "mAssetBox";
            this.mAssetBox.Size = new System.Drawing.Size(400, 19);
            this.mAssetBox.TabIndex = 6;
            // 
            // mStopButton
            // 
            this.mStopButton.Location = new System.Drawing.Point(280, 78);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 22);
            this.mStopButton.TabIndex = 2;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mPlayButton
            // 
            this.mPlayButton.Location = new System.Drawing.Point(199, 78);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(75, 22);
            this.mPlayButton.TabIndex = 1;
            this.mPlayButton.Text = "&Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mLoadButton
            // 
            this.mLoadButton.Location = new System.Drawing.Point(118, 78);
            this.mLoadButton.Name = "mLoadButton";
            this.mLoadButton.Size = new System.Drawing.Size(75, 22);
            this.mLoadButton.TabIndex = 0;
            this.mLoadButton.Text = "&Load";
            this.mLoadButton.UseVisualStyleBackColor = true;
            this.mLoadButton.Click += new System.EventHandler(this.mLoadButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 172);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.mPlayButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mLoadButton);
            this.Controls.Add(this.mAssetBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mDeviceBox);
            this.Controls.Add(this.label1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
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

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox mDeviceBox;
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
        private System.Windows.Forms.ToolStripMenuItem newAudioAssetToolStripMenuItem;
        private System.Windows.Forms.TextBox mAssetBox;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mLoadButton;
    }
}

