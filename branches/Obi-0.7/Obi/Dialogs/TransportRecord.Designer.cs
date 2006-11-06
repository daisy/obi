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
            this.btnPhraseMark = new System.Windows.Forms.Button();
            this.btnBeginSection = new System.Windows.Forms.Button();
            this.btnPageMark = new System.Windows.Forms.Button();
            this.combRecordingSelect = new System.Windows.Forms.ComboBox();
            this.txtCommitInterval = new System.Windows.Forms.TextBox();
            this.lblCommitInterval = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mRecordButton
            // 
            this.mRecordButton.Location = new System.Drawing.Point(93, 13);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(75, 25);
            this.mRecordButton.TabIndex = 0;
            this.mRecordButton.Text = "&Record";
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.Location = new System.Drawing.Point(174, 13);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(75, 25);
            this.mPauseButton.TabIndex = 1;
            this.mPauseButton.Text = "&Pause";
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mStopButton.Location = new System.Drawing.Point(205, 13);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 25);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.Text = "&Stop";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // btnPhraseMark
            // 
            this.btnPhraseMark.Location = new System.Drawing.Point(93, 44);
            this.btnPhraseMark.Name = "btnPhraseMark";
            this.btnPhraseMark.Size = new System.Drawing.Size(75, 23);
            this.btnPhraseMark.TabIndex = 4;
            this.btnPhraseMark.Text = "Phrase &mark";
            this.btnPhraseMark.UseVisualStyleBackColor = true;
            this.btnPhraseMark.Click += new System.EventHandler(this.btnPhraseMark_Click);
            // 
            // btnBeginSection
            // 
            this.btnBeginSection.Location = new System.Drawing.Point(174, 44);
            this.btnBeginSection.Name = "btnBeginSection";
            this.btnBeginSection.Size = new System.Drawing.Size(75, 23);
            this.btnBeginSection.TabIndex = 5;
            this.btnBeginSection.Text = "Begin Se&ction";
            this.btnBeginSection.UseVisualStyleBackColor = true;
            this.btnBeginSection.Click += new System.EventHandler(this.btnBeginSection_Click);
            // 
            // btnPageMark
            // 
            this.btnPageMark.Location = new System.Drawing.Point(205, 44);
            this.btnPageMark.Name = "btnPageMark";
            this.btnPageMark.Size = new System.Drawing.Size(75, 23);
            this.btnPageMark.TabIndex = 6;
            this.btnPageMark.Text = "P&age Mark";
            this.btnPageMark.UseVisualStyleBackColor = true;
            this.btnPageMark.Click += new System.EventHandler(this.btnPageMark_Click);
            // 
            // combRecordingSelect
            // 
            this.combRecordingSelect.FormattingEnabled = true;
            this.combRecordingSelect.Location = new System.Drawing.Point(12, 15);
            this.combRecordingSelect.Name = "combRecordingSelect";
            this.combRecordingSelect.Size = new System.Drawing.Size(121, 21);
            this.combRecordingSelect.TabIndex = 0;
            // 
            // txtCommitInterval
            // 
            this.txtCommitInterval.AccessibleName = "Commit interval in Seconds";
            this.txtCommitInterval.Location = new System.Drawing.Point(93, 74);
            this.txtCommitInterval.Name = "txtCommitInterval";
            this.txtCommitInterval.Size = new System.Drawing.Size(100, 20);
            this.txtCommitInterval.TabIndex = 7;
            this.txtCommitInterval.Text = "300";
            // 
            // lblCommitInterval
            // 
            this.lblCommitInterval.AutoSize = true;
            this.lblCommitInterval.Location = new System.Drawing.Point(12, 74);
            this.lblCommitInterval.Name = "lblCommitInterval";
            this.lblCommitInterval.Size = new System.Drawing.Size(110, 13);
            this.lblCommitInterval.TabIndex = 7;
            this.lblCommitInterval.Text = "Commit &Interval in sec";
            // 
            // TransportRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 296);
            this.Controls.Add(this.lblCommitInterval);
            this.Controls.Add(this.txtCommitInterval);
            this.Controls.Add(this.combRecordingSelect);
            this.Controls.Add(this.btnPageMark);
            this.Controls.Add(this.btnBeginSection);
            this.Controls.Add(this.btnPhraseMark);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mRecordButton);
            this.Name = "TransportRecord";
            this.Text = "TransportRecord";
            this.Load += new System.EventHandler(this.TransportRecord_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mRecordButton;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button btnPhraseMark;
        private System.Windows.Forms.Button btnBeginSection;
        private System.Windows.Forms.Button btnPageMark;
        private System.Windows.Forms.ComboBox combRecordingSelect;
        private System.Windows.Forms.TextBox txtCommitInterval;
        private System.Windows.Forms.Label lblCommitInterval;
    }
}