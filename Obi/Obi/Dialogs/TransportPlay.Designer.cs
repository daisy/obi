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
            this.components = new System.ComponentModel.Container();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.btnNextPhrase = new System.Windows.Forms.Button();
            this.btnPreviousPhrase = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.txtOverloadLeft = new System.Windows.Forms.TextBox();
            this.txtOverloadRight = new System.Windows.Forms.TextBox();
            this.lblLeftOverload = new System.Windows.Forms.Label();
            this.lblRithtOverload = new System.Windows.Forms.Label();
            this.tmUpdateAmplitudeText = new System.Windows.Forms.Timer(this.components);
            this.lblAmplitudeLeft = new System.Windows.Forms.Label();
            this.txtAmplitudeLeft = new System.Windows.Forms.TextBox();
            this.lblAmplitudeRight = new System.Windows.Forms.Label();
            this.txtAmplitudeRight = new System.Windows.Forms.TextBox();
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
            this.btnPreviousPhrase.Text = "Pre&vious Phrase";
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
            // txtOverloadLeft
            // 
            this.txtOverloadLeft.AccessibleName = "Left Overload";
            this.txtOverloadLeft.Location = new System.Drawing.Point(73, 110);
            this.txtOverloadLeft.Name = "txtOverloadLeft";
            this.txtOverloadLeft.ReadOnly = true;
            this.txtOverloadLeft.Size = new System.Drawing.Size(100, 20);
            this.txtOverloadLeft.TabIndex = 8;
            // 
            // txtOverloadRight
            // 
            this.txtOverloadRight.AccessibleName = "Right OverLload";
            this.txtOverloadRight.Location = new System.Drawing.Point(255, 110);
            this.txtOverloadRight.Name = "txtOverloadRight";
            this.txtOverloadRight.ReadOnly = true;
            this.txtOverloadRight.Size = new System.Drawing.Size(100, 20);
            this.txtOverloadRight.TabIndex = 9;
            // 
            // lblLeftOverload
            // 
            this.lblLeftOverload.AutoSize = true;
            this.lblLeftOverload.Location = new System.Drawing.Point(0, 112);
            this.lblLeftOverload.Name = "lblLeftOverload";
            this.lblLeftOverload.Size = new System.Drawing.Size(71, 13);
            this.lblLeftOverload.TabIndex = 8;
            this.lblLeftOverload.Text = "&Left Overload";
            // 
            // lblRithtOverload
            // 
            this.lblRithtOverload.AutoSize = true;
            this.lblRithtOverload.Location = new System.Drawing.Point(176, 112);
            this.lblRithtOverload.Name = "lblRithtOverload";
            this.lblRithtOverload.Size = new System.Drawing.Size(78, 13);
            this.lblRithtOverload.TabIndex = 9;
            this.lblRithtOverload.Text = "&Right Overload";
            // 
            // tmUpdateAmplitudeText
            // 
            this.tmUpdateAmplitudeText.Enabled = true;
            this.tmUpdateAmplitudeText.Interval = 2000;
            this.tmUpdateAmplitudeText.Tick += new System.EventHandler(this.tmUpdateAmplitudeText_Tick);
            // 
            // lblAmplitudeLeft
            // 
            this.lblAmplitudeLeft.AutoSize = true;
            this.lblAmplitudeLeft.Location = new System.Drawing.Point(0, 145);
            this.lblAmplitudeLeft.Name = "lblAmplitudeLeft";
            this.lblAmplitudeLeft.Size = new System.Drawing.Size(74, 13);
            this.lblAmplitudeLeft.TabIndex = 10;
            this.lblAmplitudeLeft.Text = "Lef&t Amplitude";
            // 
            // txtAmplitudeLeft
            // 
            this.txtAmplitudeLeft.AccessibleName = "Left Amplitude";
            this.txtAmplitudeLeft.Location = new System.Drawing.Point(75, 144);
            this.txtAmplitudeLeft.Name = "txtAmplitudeLeft";
            this.txtAmplitudeLeft.ReadOnly = true;
            this.txtAmplitudeLeft.Size = new System.Drawing.Size(100, 20);
            this.txtAmplitudeLeft.TabIndex = 11;
            // 
            // lblAmplitudeRight
            // 
            this.lblAmplitudeRight.AutoSize = true;
            this.lblAmplitudeRight.Location = new System.Drawing.Point(177, 146);
            this.lblAmplitudeRight.Name = "lblAmplitudeRight";
            this.lblAmplitudeRight.Size = new System.Drawing.Size(81, 13);
            this.lblAmplitudeRight.TabIndex = 12;
            this.lblAmplitudeRight.Text = "Ri&ght Amplitude";
            // 
            // txtAmplitudeRight
            // 
            this.txtAmplitudeRight.AccessibleName = "Right Amplitude";
            this.txtAmplitudeRight.Location = new System.Drawing.Point(259, 145);
            this.txtAmplitudeRight.Name = "txtAmplitudeRight";
            this.txtAmplitudeRight.ReadOnly = true;
            this.txtAmplitudeRight.Size = new System.Drawing.Size(100, 20);
            this.txtAmplitudeRight.TabIndex = 13;
            // 
            // TransportPlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 296);
            this.Controls.Add(this.txtAmplitudeRight);
            this.Controls.Add(this.lblAmplitudeRight);
            this.Controls.Add(this.txtAmplitudeLeft);
            this.Controls.Add(this.lblAmplitudeLeft);
            this.Controls.Add(this.lblRithtOverload);
            this.Controls.Add(this.lblLeftOverload);
            this.Controls.Add(this.txtOverloadRight);
            this.Controls.Add(this.txtOverloadLeft);
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
            this.PerformLayout();

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
        private System.Windows.Forms.TextBox txtOverloadLeft;
        private System.Windows.Forms.TextBox txtOverloadRight;
        private System.Windows.Forms.Label lblLeftOverload;
        private System.Windows.Forms.Label lblRithtOverload;
        private System.Windows.Forms.Timer tmUpdateAmplitudeText;
        private System.Windows.Forms.Label lblAmplitudeLeft;
        private System.Windows.Forms.TextBox txtAmplitudeLeft;
        private System.Windows.Forms.Label lblAmplitudeRight;
        private System.Windows.Forms.TextBox txtAmplitudeRight;
    }
}