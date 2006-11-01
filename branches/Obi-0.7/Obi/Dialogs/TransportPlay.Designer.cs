namespace Obi.Dialogs
{
    partial class TransportPlay
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
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mPlayButton
            // 
            this.mPlayButton.Location = new System.Drawing.Point(12, 12);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(75, 23);
            this.mPlayButton.TabIndex = 0;
            this.mPlayButton.Text = "&Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.Location = new System.Drawing.Point(93, 12);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 23);
            this.mStopButton.TabIndex = 1;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.Location = new System.Drawing.Point(12, 12);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(75, 23);
            this.mPauseButton.TabIndex = 2;
            this.mPauseButton.Text = "&Pause";
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mCloseButton
            // 
            this.mCloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mCloseButton.Location = new System.Drawing.Point(93, 12);
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.Size = new System.Drawing.Size(75, 23);
            this.mCloseButton.TabIndex = 3;
            this.mCloseButton.Text = "&Close";
            this.mCloseButton.UseVisualStyleBackColor = true;
            // 
            // TransportPlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.mCloseButton);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPlayButton);
            this.Name = "TransportPlay";
            this.Text = "TransportPlay";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.Button mCloseButton;
    }
}