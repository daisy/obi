namespace Obi.Dialogs
{
    partial class Split
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Split));
            this.btnPreview = new System.Windows.Forms.Button();
            this.txtDisplayTime = new System.Windows.Forms.TextBox();
            this.tmUpdateTimePosition = new System.Windows.Forms.Timer(this.components);
            this.btnFineRewind = new System.Windows.Forms.Button();
            this.btnFineForward = new System.Windows.Forms.Button();
            this.btnSplit = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.txtSplitTime = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelSplitTime = new System.Windows.Forms.Label();
            this.lblStepSize = new System.Windows.Forms.Label();
            this.txtStepSize = new System.Windows.Forms.TextBox();
            this.btnStepSizeIncrement = new System.Windows.Forms.Button();
            this.btnStepSizeDecrement = new System.Windows.Forms.Button();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(136, 40);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(127, 21);
            this.btnPreview.TabIndex = 2;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // txtDisplayTime
            // 
            this.txtDisplayTime.AccessibleName = "Play Time";
            this.txtDisplayTime.Location = new System.Drawing.Point(136, 66);
            this.txtDisplayTime.Name = "txtDisplayTime";
            this.txtDisplayTime.ReadOnly = true;
            this.txtDisplayTime.Size = new System.Drawing.Size(127, 19);
            this.txtDisplayTime.TabIndex = 4;
            // 
            // tmUpdateTimePosition
            // 
            this.tmUpdateTimePosition.Enabled = true;
            this.tmUpdateTimePosition.Interval = 1000;
            this.tmUpdateTimePosition.Tick += new System.EventHandler(this.tmUpdateTimePosition_Tick);
            // 
            // btnFineRewind
            // 
            this.btnFineRewind.Location = new System.Drawing.Point(18, 42);
            this.btnFineRewind.Name = "btnFineRewind";
            this.btnFineRewind.Size = new System.Drawing.Size(100, 21);
            this.btnFineRewind.TabIndex = 5;
            this.btnFineRewind.Text = "Rewind";
            this.btnFineRewind.UseVisualStyleBackColor = true;
            this.btnFineRewind.Click += new System.EventHandler(this.btnFineRewind_Click);
            // 
            // btnFineForward
            // 
            this.btnFineForward.Location = new System.Drawing.Point(124, 42);
            this.btnFineForward.Name = "btnFineForward";
            this.btnFineForward.Size = new System.Drawing.Size(105, 21);
            this.btnFineForward.TabIndex = 9;
            this.btnFineForward.Text = "Forward";
            this.btnFineForward.UseVisualStyleBackColor = true;
            this.btnFineForward.Click += new System.EventHandler(this.btnFineForward_Click);
            // 
            // btnSplit
            // 
            this.btnSplit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSplit.Location = new System.Drawing.Point(12, 222);
            this.btnSplit.Name = "btnSplit";
            this.btnSplit.Size = new System.Drawing.Size(118, 21);
            this.btnSplit.TabIndex = 17;
            this.btnSplit.Text = "Split";
            this.btnSplit.UseVisualStyleBackColor = true;
            this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(136, 222);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(127, 21);
            this.btnStop.TabIndex = 18;
            this.btnStop.Text = "Stop/Close";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.Location = new System.Drawing.Point(12, 40);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(118, 21);
            this.mPauseButton.TabIndex = 2;
            this.mPauseButton.Text = "&Pause";
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // txtSplitTime
            // 
            this.txtSplitTime.AccessibleName = "Split time in seconds";
            this.txtSplitTime.Location = new System.Drawing.Point(136, 90);
            this.txtSplitTime.Name = "txtSplitTime";
            this.txtSplitTime.Size = new System.Drawing.Size(127, 19);
            this.txtSplitTime.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnFineRewind);
            this.groupBox1.Controls.Add(this.btnFineForward);
            this.groupBox1.Location = new System.Drawing.Point(12, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 78);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "navigate";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(83, 69);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(52, 12);
            this.labelTime.TabIndex = 4;
            this.labelTime.Text = "play &time";
            // 
            // labelSplitTime
            // 
            this.labelSplitTime.AutoSize = true;
            this.labelSplitTime.Location = new System.Drawing.Point(83, 90);
            this.labelSplitTime.Name = "labelSplitTime";
            this.labelSplitTime.Size = new System.Drawing.Size(53, 12);
            this.labelSplitTime.TabIndex = 5;
            this.labelSplitTime.Text = "&split time";
            // 
            // lblStepSize
            // 
            this.lblStepSize.AutoSize = true;
            this.lblStepSize.Location = new System.Drawing.Point(9, 195);
            this.lblStepSize.Name = "lblStepSize";
            this.lblStepSize.Size = new System.Drawing.Size(95, 12);
            this.lblStepSize.TabIndex = 10;
            this.lblStepSize.Text = "Step s&ize in sec\'s";
            // 
            // txtStepSize
            // 
            this.txtStepSize.AccessibleName = "Step Size in Seconds";
            this.txtStepSize.Location = new System.Drawing.Point(135, 194);
            this.txtStepSize.Name = "txtStepSize";
            this.txtStepSize.Size = new System.Drawing.Size(50, 19);
            this.txtStepSize.TabIndex = 11;
            this.txtStepSize.Text = "0.5";
            this.txtStepSize.TextChanged += new System.EventHandler(this.txtStepSize_TextChanged);
            // 
            // btnStepSizeIncrement
            // 
            this.btnStepSizeIncrement.AccessibleName = "Step Size +";
            this.btnStepSizeIncrement.Location = new System.Drawing.Point(190, 188);
            this.btnStepSizeIncrement.Name = "btnStepSizeIncrement";
            this.btnStepSizeIncrement.Size = new System.Drawing.Size(25, 14);
            this.btnStepSizeIncrement.TabIndex = 15;
            this.btnStepSizeIncrement.Text = "+";
            this.btnStepSizeIncrement.UseVisualStyleBackColor = true;
            this.btnStepSizeIncrement.Click += new System.EventHandler(this.btnStepSizeIncrement_Click);
            // 
            // btnStepSizeDecrement
            // 
            this.btnStepSizeDecrement.AccessibleName = "Step Size Minus";
            this.btnStepSizeDecrement.Location = new System.Drawing.Point(190, 199);
            this.btnStepSizeDecrement.Name = "btnStepSizeDecrement";
            this.btnStepSizeDecrement.Size = new System.Drawing.Size(25, 14);
            this.btnStepSizeDecrement.TabIndex = 16;
            this.btnStepSizeDecrement.Text = "-";
            this.btnStepSizeDecrement.UseVisualStyleBackColor = true;
            this.btnStepSizeDecrement.Click += new System.EventHandler(this.btnStepSizeDecrement_Click);
            // 
            // mPlayButton
            // 
            this.mPlayButton.Location = new System.Drawing.Point(12, 39);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(118, 23);
            this.mPlayButton.TabIndex = 19;
            this.mPlayButton.Text = "&Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            // 
            // Split
            // 
            this.AcceptButton = this.btnSplit;
            this.AccessibleName = "Split Dialog";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(275, 307);
            this.Controls.Add(this.mPlayButton);
            this.Controls.Add(this.btnStepSizeDecrement);
            this.Controls.Add(this.btnStepSizeIncrement);
            this.Controls.Add(this.txtStepSize);
            this.Controls.Add(this.lblStepSize);
            this.Controls.Add(this.labelSplitTime);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtSplitTime);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnSplit);
            this.Controls.Add(this.txtDisplayTime);
            this.Controls.Add(this.btnPreview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Split";
            this.Text = "Split";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Split_FormClosing);
            this.Load += new System.EventHandler(this.Split_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.TextBox txtDisplayTime;
        private System.Windows.Forms.Timer tmUpdateTimePosition;
        private System.Windows.Forms.Button btnFineRewind;
        private System.Windows.Forms.Button btnFineForward;
        private System.Windows.Forms.Button btnSplit;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.TextBox txtSplitTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelSplitTime;
        private System.Windows.Forms.Label lblStepSize;
        private System.Windows.Forms.TextBox txtStepSize;
        private System.Windows.Forms.Button btnStepSizeIncrement;
        private System.Windows.Forms.Button btnStepSizeDecrement;
        private System.Windows.Forms.Button mPlayButton;
    }
}