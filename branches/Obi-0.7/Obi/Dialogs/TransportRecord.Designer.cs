namespace Obi.Dialogs
{
    partial class TransportRecord
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
            this.mRecordButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mRecordButton
            // 
            this.mRecordButton.Location = new System.Drawing.Point(12, 12);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(75, 23);
            this.mRecordButton.TabIndex = 0;
            this.mRecordButton.Text = "&Record";
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.Location = new System.Drawing.Point(12, 12);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(75, 23);
            this.mPauseButton.TabIndex = 1;
            this.mPauseButton.Text = "&Pause";
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mStopButton.Location = new System.Drawing.Point(93, 12);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 23);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // TransportRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mRecordButton);
            this.Name = "TransportRecord";
            this.Text = "TransportRecord";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mRecordButton;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.Button mStopButton;
    }
}