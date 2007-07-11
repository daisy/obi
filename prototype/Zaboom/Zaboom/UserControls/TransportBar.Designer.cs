namespace Zaboom.UserControls
{
    partial class TransportBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransportBar));
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mStatusLabel = new System.Windows.Forms.Label();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mPlayButton
            // 
            this.mPlayButton.FlatAppearance.BorderSize = 0;
            this.mPlayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPlayButton.Image = ((System.Drawing.Image)(resources.GetObject("mPlayButton.Image")));
            this.mPlayButton.Location = new System.Drawing.Point(3, 3);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(32, 32);
            this.mPlayButton.TabIndex = 0;
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.FlatAppearance.BorderSize = 0;
            this.mStopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mStopButton.Image = ((System.Drawing.Image)(resources.GetObject("mStopButton.Image")));
            this.mStopButton.Location = new System.Drawing.Point(41, 3);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(32, 32);
            this.mStopButton.TabIndex = 1;
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // mStatusLabel
            // 
            this.mStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mStatusLabel.AutoSize = true;
            this.mStatusLabel.Location = new System.Drawing.Point(246, 13);
            this.mStatusLabel.Name = "mStatusLabel";
            this.mStatusLabel.Size = new System.Drawing.Size(23, 12);
            this.mStatusLabel.TabIndex = 2;
            this.mStatusLabel.Text = "***";
            this.mStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mPauseButton
            // 
            this.mPauseButton.FlatAppearance.BorderSize = 0;
            this.mPauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPauseButton.Image = ((System.Drawing.Image)(resources.GetObject("mPauseButton.Image")));
            this.mPauseButton.Location = new System.Drawing.Point(79, 3);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(32, 32);
            this.mPauseButton.TabIndex = 3;
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // TransportBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mStatusLabel);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPlayButton);
            this.Name = "TransportBar";
            this.Size = new System.Drawing.Size(272, 38);
            this.Load += new System.EventHandler(this.TransportBar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Label mStatusLabel;
        private System.Windows.Forms.Button mPauseButton;
    }
}
