namespace Obi.Dialogs
{
    partial class Record
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
            this.mRecordButton = new System.Windows.Forms.Button();
            this.mDisplayTimeLable = new System.Windows.Forms.Label();
            this.mTimeTextBox = new System.Windows.Forms.TextBox();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mPhraseMarkerButton = new System.Windows.Forms.Button();
            this.mTimer = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // mRecordButton
            // 
            this.mRecordButton.Location = new System.Drawing.Point(24, 121);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(75, 23);
            this.mRecordButton.TabIndex = 0;
            this.mRecordButton.Text = "&Record";
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.btnRecordAndPause_Click);
            // 
            // mDisplayTimeLable
            // 
            this.mDisplayTimeLable.AutoSize = true;
            this.mDisplayTimeLable.Location = new System.Drawing.Point(145, 23);
            this.mDisplayTimeLable.Name = "mDisplayTimeLable";
            this.mDisplayTimeLable.Size = new System.Drawing.Size(75, 12);
            this.mDisplayTimeLable.TabIndex = 1;
            this.mDisplayTimeLable.Text = "Running &Time";
            // 
            // mTimeTextBox
            // 
            this.mTimeTextBox.Location = new System.Drawing.Point(132, 38);
            this.mTimeTextBox.Name = "mTimeTextBox";
            this.mTimeTextBox.ReadOnly = true;
            this.mTimeTextBox.Size = new System.Drawing.Size(100, 19);
            this.mTimeTextBox.TabIndex = 2;
            // 
            // mStopButton
            // 
            this.mStopButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mStopButton.Location = new System.Drawing.Point(123, 121);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 21);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // mPhraseMarkerButton
            // 
            this.mPhraseMarkerButton.Location = new System.Drawing.Point(238, 122);
            this.mPhraseMarkerButton.Name = "mPhraseMarkerButton";
            this.mPhraseMarkerButton.Size = new System.Drawing.Size(88, 21);
            this.mPhraseMarkerButton.TabIndex = 4;
            this.mPhraseMarkerButton.Text = "Phrase &Marker";
            this.mPhraseMarkerButton.UseVisualStyleBackColor = true;
            this.mPhraseMarkerButton.Click += new System.EventHandler(this.mPhraseMarkerButton_Click_1);
            // 
            // timer1
            // 
            this.mTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Record
            // 
            this.AcceptButton = this.mStopButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 169);
            this.Controls.Add(this.mPhraseMarkerButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mTimeTextBox);
            this.Controls.Add(this.mDisplayTimeLable);
            this.Controls.Add(this.mRecordButton);
            this.Name = "Record";
            this.Text = "Record";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Record_FormClosing);
            this.Load += new System.EventHandler(this.Record_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mRecordButton;
        private System.Windows.Forms.Label mDisplayTimeLable;
        private System.Windows.Forms.TextBox mTimeTextBox;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPhraseMarkerButton;
        private System.Windows.Forms.Timer mTimer;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}