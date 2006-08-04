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
            this.mPhraseMarkerBuutton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mVolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.mVolumeTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // mRecordButton
            // 
            this.mRecordButton.Location = new System.Drawing.Point(30, 65);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(75, 25);
            this.mRecordButton.TabIndex = 0;
            this.mRecordButton.Text = "&Record";
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.btnRecordAndPause_Click);
            // 
            // mDisplayTimeLable
            // 
            this.mDisplayTimeLable.AutoSize = true;
            this.mDisplayTimeLable.Location = new System.Drawing.Point(37, 9);
            this.mDisplayTimeLable.Name = "mDisplayTimeLable";
            this.mDisplayTimeLable.Size = new System.Drawing.Size(73, 13);
            this.mDisplayTimeLable.TabIndex = 1;
            this.mDisplayTimeLable.Text = "Running &Time";
            // 
            // mTimeTextBox
            // 
            this.mTimeTextBox.Location = new System.Drawing.Point(39, 30);
            this.mTimeTextBox.Name = "mTimeTextBox";
            this.mTimeTextBox.ReadOnly = true;
            this.mTimeTextBox.Size = new System.Drawing.Size(100, 20);
            this.mTimeTextBox.TabIndex = 2;
            // 
            // mStopButton
            // 
            this.mStopButton.Location = new System.Drawing.Point(123, 67);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 23);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // mPhraseMarkerBuutton
            // 
            this.mPhraseMarkerBuutton.Location = new System.Drawing.Point(221, 67);
            this.mPhraseMarkerBuutton.Name = "mPhraseMarkerBuutton";
            this.mPhraseMarkerBuutton.Size = new System.Drawing.Size(75, 23);
            this.mPhraseMarkerBuutton.TabIndex = 4;
            this.mPhraseMarkerBuutton.Text = "Phrase &Marker";
            this.mPhraseMarkerBuutton.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mVolumeTrackBar
            // 
            this.mVolumeTrackBar.Location = new System.Drawing.Point(4, 121);
            this.mVolumeTrackBar.Name = "mVolumeTrackBar";
            this.mVolumeTrackBar.Size = new System.Drawing.Size(290, 48);
            this.mVolumeTrackBar.TabIndex = 5;
            // 
            // Record
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 183);
            this.Controls.Add(this.mVolumeTrackBar);
            this.Controls.Add(this.mPhraseMarkerBuutton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mTimeTextBox);
            this.Controls.Add(this.mDisplayTimeLable);
            this.Controls.Add(this.mRecordButton);
            this.Name = "Record";
            this.Text = "Record";
            this.Load += new System.EventHandler(this.Record_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mVolumeTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mRecordButton;
        private System.Windows.Forms.Label mDisplayTimeLable;
        private System.Windows.Forms.TextBox mTimeTextBox;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPhraseMarkerBuutton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar mVolumeTrackBar;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}