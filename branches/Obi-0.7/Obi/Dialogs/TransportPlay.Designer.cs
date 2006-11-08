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
            this.btnNextPhrase = new System.Windows.Forms.Button();
            this.btnPreviousPhrase = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mPlayButton
            // 
            this.mPlayButton.Location = new System.Drawing.Point(12, 13);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(75, 25);
            this.mPlayButton.TabIndex = 0;
            this.mPlayButton.Text = "&Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.Location = new System.Drawing.Point(93, 13);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 25);
            this.mStopButton.TabIndex = 1;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.Location = new System.Drawing.Point(12, 13);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(75, 25);
            this.mPauseButton.TabIndex = 2;
            this.mPauseButton.Text = "&Pause";
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mCloseButton
            // 
            this.mCloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mCloseButton.Location = new System.Drawing.Point(93, 13);
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.Size = new System.Drawing.Size(75, 25);
            this.mCloseButton.TabIndex = 3;
            this.mCloseButton.Text = "&Close";
            this.mCloseButton.UseVisualStyleBackColor = true;
            // 
            // btnNextPhrase
            // 
            this.btnNextPhrase.Location = new System.Drawing.Point(12, 44);
            this.btnNextPhrase.Name = "btnNextPhrase";
            this.btnNextPhrase.Size = new System.Drawing.Size(75, 23);
            this.btnNextPhrase.TabIndex = 4;
            this.btnNextPhrase.Text = "&Next Phrase";
            this.btnNextPhrase.UseVisualStyleBackColor = true;
            this.btnNextPhrase.Click += new System.EventHandler(this.btnNextPhrase_Click);
            // 
            // btnPreviousPhrase
            // 
            this.btnPreviousPhrase.Location = new System.Drawing.Point(93, 44);
            this.btnPreviousPhrase.Name = "btnPreviousPhrase";
            this.btnPreviousPhrase.Size = new System.Drawing.Size(75, 23);
            this.btnPreviousPhrase.TabIndex = 5;
            this.btnPreviousPhrase.Text = "P&revious Phrase";
            this.btnPreviousPhrase.UseVisualStyleBackColor = true;
            this.btnPreviousPhrase.Click += new System.EventHandler(this.btnPreviousPhrase_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.Location = new System.Drawing.Point(12, 80);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(75, 23);
            this.btnRewind.TabIndex = 6;
            this.btnRewind.Text = "R&ewind";
            this.btnRewind.UseVisualStyleBackColor = true;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(93, 80);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(75, 23);
            this.btnForward.TabIndex = 7;
            this.btnForward.Text = "&Forward";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // TransportPlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 296);
            this.Controls.Add(this.btnForward);
            this.Controls.Add(this.btnRewind);
            this.Controls.Add(this.btnPreviousPhrase);
            this.Controls.Add(this.btnNextPhrase);
            this.Controls.Add(this.mCloseButton);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPlayButton);
            this.Name = "TransportPlay";
            this.Text = "TransportPlay";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransportPlay_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.Button mCloseButton;
        private System.Windows.Forms.Button btnNextPhrase;
        private System.Windows.Forms.Button btnPreviousPhrase;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.Button btnForward;
    }
}