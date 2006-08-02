namespace Obi.Dialogs
{
    partial class Play
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
            this.components = new System.ComponentModel.Container();
            this.mNameDisplay = new System.Windows.Forms.TextBox();
            this.mTimeDisplay = new System.Windows.Forms.TextBox();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.tmUpdateCurrentTime = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mNameDisplay
            // 
            this.mNameDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mNameDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mNameDisplay.Location = new System.Drawing.Point(12, 24);
            this.mNameDisplay.Name = "mNameDisplay";
            this.mNameDisplay.ReadOnly = true;
            this.mNameDisplay.Size = new System.Drawing.Size(268, 12);
            this.mNameDisplay.TabIndex = 0;
            this.mNameDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mTimeDisplay
            // 
            this.mTimeDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTimeDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTimeDisplay.Location = new System.Drawing.Point(12, 49);
            this.mTimeDisplay.Name = "mTimeDisplay";
            this.mTimeDisplay.ReadOnly = true;
            this.mTimeDisplay.Size = new System.Drawing.Size(268, 12);
            this.mTimeDisplay.TabIndex = 4;
            this.mTimeDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mPlayButton
            // 
            this.mPlayButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mPlayButton.Location = new System.Drawing.Point(68, 101);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(75, 21);
            this.mPlayButton.TabIndex = 2;
            this.mPlayButton.Text = "&Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mStopButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mStopButton.Location = new System.Drawing.Point(149, 101);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 21);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // tmUpdateCurrentTime
            // 
            this.tmUpdateCurrentTime.Interval = 1000;
            this.tmUpdateCurrentTime.Tick += new System.EventHandler(this.tmUpdateCurrentTime_Tick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Currently playing:";
            // 
            // Play
            // 
            this.AcceptButton = this.mStopButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 134);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPlayButton);
            this.Controls.Add(this.mTimeDisplay);
            this.Controls.Add(this.mNameDisplay);
            this.Name = "Play";
            this.Text = "Play audio block";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Play_FormClosing);
            this.Load += new System.EventHandler(this.Play_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mNameDisplay;
        private System.Windows.Forms.TextBox mTimeDisplay;
        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Timer tmUpdateCurrentTime;
        private System.Windows.Forms.Label label1;
    }
}