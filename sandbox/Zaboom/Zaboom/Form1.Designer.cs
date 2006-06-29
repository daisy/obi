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
            this.mAssetBox = new System.Windows.Forms.TextBox();
            this.mLoadButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mPlayerStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "&Device:";
            // 
            // mDeviceBox
            // 
            this.mDeviceBox.FormattingEnabled = true;
            this.mDeviceBox.Location = new System.Drawing.Point(60, 12);
            this.mDeviceBox.Name = "mDeviceBox";
            this.mDeviceBox.Size = new System.Drawing.Size(400, 20);
            this.mDeviceBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "&Asset:";
            // 
            // mAssetBox
            // 
            this.mAssetBox.Location = new System.Drawing.Point(60, 38);
            this.mAssetBox.Name = "mAssetBox";
            this.mAssetBox.Size = new System.Drawing.Size(400, 19);
            this.mAssetBox.TabIndex = 6;
            // 
            // mLoadButton
            // 
            this.mLoadButton.Location = new System.Drawing.Point(118, 63);
            this.mLoadButton.Name = "mLoadButton";
            this.mLoadButton.Size = new System.Drawing.Size(75, 75);
            this.mLoadButton.TabIndex = 0;
            this.mLoadButton.Text = "&Load";
            this.mLoadButton.UseVisualStyleBackColor = true;
            this.mLoadButton.Click += new System.EventHandler(this.mLoadButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.Location = new System.Drawing.Point(280, 63);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 75);
            this.mStopButton.TabIndex = 2;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mPlayButton
            // 
            this.mPlayButton.Location = new System.Drawing.Point(199, 63);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(75, 75);
            this.mPlayButton.TabIndex = 1;
            this.mPlayButton.Text = "&Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
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
            // mPlayerStatusLabel
            // 
            this.mPlayerStatusLabel.Name = "mPlayerStatusLabel";
            this.mPlayerStatusLabel.Size = new System.Drawing.Size(57, 17);
            this.mPlayerStatusLabel.Text = "(unknown)";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(107, 17);
            this.toolStripStatusLabel1.Text = "Audio player status:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 172);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mPlayButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mLoadButton);
            this.Controls.Add(this.mAssetBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mDeviceBox);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Zaboom";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox mDeviceBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mAssetBox;
        private System.Windows.Forms.Button mLoadButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel mPlayerStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

