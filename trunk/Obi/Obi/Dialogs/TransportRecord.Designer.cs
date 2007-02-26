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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransportRecord));
            this.mRecordButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.btnPhraseMark = new System.Windows.Forms.Button();
            this.btnBeginSection = new System.Windows.Forms.Button();
            this.btnPageMark = new System.Windows.Forms.Button();
            this.tmDisplayTime = new System.Windows.Forms.Timer(this.components);
            this.txtDisplayTime = new System.Windows.Forms.TextBox();
            this.mTextVuMeter = new Obi.UserControls.TextVUMeterPanel();
            this.SuspendLayout();
            // 
            // mRecordButton
            // 
            this.mRecordButton.Location = new System.Drawing.Point(12, 11);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(75, 23);
            this.mRecordButton.TabIndex = 0;
            this.mRecordButton.Text = "&Record";
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.Location = new System.Drawing.Point(12, 11);
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
            this.mStopButton.Location = new System.Drawing.Point(195, 40);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 23);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.Text = "&Stop/Close";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // btnPhraseMark
            // 
            this.btnPhraseMark.Location = new System.Drawing.Point(12, 40);
            this.btnPhraseMark.Name = "btnPhraseMark";
            this.btnPhraseMark.Size = new System.Drawing.Size(75, 21);
            this.btnPhraseMark.TabIndex = 4;
            this.btnPhraseMark.Text = "Phrase &mark";
            this.btnPhraseMark.UseVisualStyleBackColor = true;
            this.btnPhraseMark.Click += new System.EventHandler(this.btnPhraseMark_Click);
            // 
            // btnBeginSection
            // 
            this.btnBeginSection.Location = new System.Drawing.Point(105, 40);
            this.btnBeginSection.Name = "btnBeginSection";
            this.btnBeginSection.Size = new System.Drawing.Size(75, 21);
            this.btnBeginSection.TabIndex = 5;
            this.btnBeginSection.Text = "Begin Se&ction";
            this.btnBeginSection.UseVisualStyleBackColor = true;
            this.btnBeginSection.Click += new System.EventHandler(this.btnBeginSection_Click);
            // 
            // btnPageMark
            // 
            this.btnPageMark.Location = new System.Drawing.Point(195, 11);
            this.btnPageMark.Name = "btnPageMark";
            this.btnPageMark.Size = new System.Drawing.Size(75, 21);
            this.btnPageMark.TabIndex = 6;
            this.btnPageMark.Text = "P&age Mark";
            this.btnPageMark.UseVisualStyleBackColor = true;
            this.btnPageMark.Click += new System.EventHandler(this.btnPageMark_Click);
            // 
            // tmDisplayTime
            // 
            this.tmDisplayTime.Interval = 333;
            this.tmDisplayTime.Tick += new System.EventHandler(this.tmDisplayTime_Tick);
            // 
            // txtDisplayTime
            // 
            this.txtDisplayTime.AccessibleName = "Record Time:";
            this.txtDisplayTime.Location = new System.Drawing.Point(105, 11);
            this.txtDisplayTime.Name = "txtDisplayTime";
            this.txtDisplayTime.ReadOnly = true;
            this.txtDisplayTime.Size = new System.Drawing.Size(75, 19);
            this.txtDisplayTime.TabIndex = 8;
            // 
            // mTextVuMeter
            // 
            this.mTextVuMeter.BackColor = System.Drawing.Color.Transparent;
            this.mTextVuMeter.Location = new System.Drawing.Point(12, 66);
            this.mTextVuMeter.Name = "mTextVuMeter";
            this.mTextVuMeter.PlayListObj = null;
            this.mTextVuMeter.RecordingSessionObj = null;
            this.mTextVuMeter.Size = new System.Drawing.Size(205, 24);
            this.mTextVuMeter.TabIndex = 7;
            // 
            // TransportRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 102);
            this.Controls.Add(this.txtDisplayTime);
            this.Controls.Add(this.mTextVuMeter);
            this.Controls.Add(this.btnPageMark);
            this.Controls.Add(this.btnBeginSection);
            this.Controls.Add(this.btnPhraseMark);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mRecordButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TransportRecord";
            this.Text = "Record";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransportRecord_FormClosing);
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
        private Obi.UserControls.TextVUMeterPanel mTextVuMeter;
        private System.Windows.Forms.Timer tmDisplayTime;
        private System.Windows.Forms.TextBox txtDisplayTime;
    }
}