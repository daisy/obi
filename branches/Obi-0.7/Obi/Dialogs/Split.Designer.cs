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
            this.txtDisplayAsset = new System.Windows.Forms.TextBox();
            this.txtDisplayTime = new System.Windows.Forms.TextBox();
            this.tmUpdateTimePosition = new System.Windows.Forms.Timer(this.components);
            this.btnFastRewind = new System.Windows.Forms.Button();
            this.btnFastForward = new System.Windows.Forms.Button();
            this.btnFineRewind = new System.Windows.Forms.Button();
            this.btnFineForward = new System.Windows.Forms.Button();
            this.btnSplit = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.txtSplitTime = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.labelAssetName = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelSplitTime = new System.Windows.Forms.Label();
            this.AudioTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AudioTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(12, 43);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(118, 23);
            this.btnPreview.TabIndex = 2;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // txtDisplayAsset
            // 
            this.txtDisplayAsset.Location = new System.Drawing.Point(136, 12);
            this.txtDisplayAsset.Name = "txtDisplayAsset";
            this.txtDisplayAsset.ReadOnly = true;
            this.txtDisplayAsset.Size = new System.Drawing.Size(127, 20);
            this.txtDisplayAsset.TabIndex = 1;
            // 
            // txtDisplayTime
            // 
            this.txtDisplayTime.Location = new System.Drawing.Point(136, 72);
            this.txtDisplayTime.Name = "txtDisplayTime";
            this.txtDisplayTime.ReadOnly = true;
            this.txtDisplayTime.Size = new System.Drawing.Size(127, 20);
            this.txtDisplayTime.TabIndex = 4;
            // 
            // tmUpdateTimePosition
            // 
            this.tmUpdateTimePosition.Enabled = true;
            this.tmUpdateTimePosition.Interval = 1000;
            this.tmUpdateTimePosition.Tick += new System.EventHandler(this.tmUpdateTimePosition_Tick);
            // 
            // btnFastRewind
            // 
            this.btnFastRewind.Location = new System.Drawing.Point(18, 17);
            this.btnFastRewind.Name = "btnFastRewind";
            this.btnFastRewind.Size = new System.Drawing.Size(100, 23);
            this.btnFastRewind.TabIndex = 6;
            this.btnFastRewind.Text = "Fast Rewind";
            this.btnFastRewind.UseVisualStyleBackColor = true;
            this.btnFastRewind.Click += new System.EventHandler(this.btnFastRewind_Click);
            // 
            // btnFastForward
            // 
            this.btnFastForward.Location = new System.Drawing.Point(124, 17);
            this.btnFastForward.Name = "btnFastForward";
            this.btnFastForward.Size = new System.Drawing.Size(105, 23);
            this.btnFastForward.TabIndex = 7;
            this.btnFastForward.Text = "Fast Forward";
            this.btnFastForward.UseVisualStyleBackColor = true;
            this.btnFastForward.Click += new System.EventHandler(this.btnFastForward_Click);
            // 
            // btnFineRewind
            // 
            this.btnFineRewind.Location = new System.Drawing.Point(18, 46);
            this.btnFineRewind.Name = "btnFineRewind";
            this.btnFineRewind.Size = new System.Drawing.Size(100, 23);
            this.btnFineRewind.TabIndex = 5;
            this.btnFineRewind.Text = "Fine Rewind";
            this.btnFineRewind.UseVisualStyleBackColor = true;
            this.btnFineRewind.Click += new System.EventHandler(this.btnFineRewind_Click);
            // 
            // btnFineForward
            // 
            this.btnFineForward.Location = new System.Drawing.Point(124, 46);
            this.btnFineForward.Name = "btnFineForward";
            this.btnFineForward.Size = new System.Drawing.Size(105, 23);
            this.btnFineForward.TabIndex = 9;
            this.btnFineForward.Text = "Fine Forward";
            this.btnFineForward.UseVisualStyleBackColor = true;
            this.btnFineForward.Click += new System.EventHandler(this.btnFineForward_Click);
            // 
            // btnSplit
            // 
            this.btnSplit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSplit.Location = new System.Drawing.Point(12, 213);
            this.btnSplit.Name = "btnSplit";
            this.btnSplit.Size = new System.Drawing.Size(118, 23);
            this.btnSplit.TabIndex = 10;
            this.btnSplit.Text = "Split";
            this.btnSplit.UseVisualStyleBackColor = true;
            this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(136, 213);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(127, 23);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(136, 43);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(127, 23);
            this.btnPause.TabIndex = 3;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // txtSplitTime
            // 
            this.txtSplitTime.Location = new System.Drawing.Point(136, 97);
            this.txtSplitTime.Name = "txtSplitTime";
            this.txtSplitTime.Size = new System.Drawing.Size(127, 20);
            this.txtSplitTime.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.btnFineRewind);
            this.groupBox1.Controls.Add(this.btnFineForward);
            this.groupBox1.Controls.Add(this.btnFastRewind);
            this.groupBox1.Controls.Add(this.btnFastForward);
            this.groupBox1.Location = new System.Drawing.Point(12, 123);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 84);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "navigate";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Fine Rewind";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnFineRewind_Click);
            // 
            // labelAssetName
            // 
            this.labelAssetName.AutoSize = true;
            this.labelAssetName.Location = new System.Drawing.Point(68, 12);
            this.labelAssetName.Name = "labelAssetName";
            this.labelAssetName.Size = new System.Drawing.Size(62, 13);
            this.labelAssetName.TabIndex = 11;
            this.labelAssetName.Text = "Asset name";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(83, 75);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(48, 13);
            this.labelTime.TabIndex = 12;
            this.labelTime.Text = "play time";
            // 
            // labelSplitTime
            // 
            this.labelSplitTime.AutoSize = true;
            this.labelSplitTime.Location = new System.Drawing.Point(83, 97);
            this.labelSplitTime.Name = "labelSplitTime";
            this.labelSplitTime.Size = new System.Drawing.Size(47, 13);
            this.labelSplitTime.TabIndex = 13;
            this.labelSplitTime.Text = "split time";
            // 
            // AudioTrackBar
            // 
            this.AudioTrackBar.AccessibleName = "Audio Slider";
            this.AudioTrackBar.LargeChange = 80;
            this.AudioTrackBar.Location = new System.Drawing.Point(12, 242);
            this.AudioTrackBar.Maximum = 20000;
            this.AudioTrackBar.Name = "AudioTrackBar";
            this.AudioTrackBar.Size = new System.Drawing.Size(104, 42);
            this.AudioTrackBar.SmallChange = 5;
            this.AudioTrackBar.TabIndex = 14;
            this.AudioTrackBar.ValueChanged += new System.EventHandler(this.AudioTrackBar_ValueChanged);
            // 
            // Split
            // 
            this.AcceptButton = this.btnSplit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(275, 273);
            this.Controls.Add(this.AudioTrackBar);
            this.Controls.Add(this.labelSplitTime);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelAssetName);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtSplitTime);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnSplit);
            this.Controls.Add(this.txtDisplayTime);
            this.Controls.Add(this.txtDisplayAsset);
            this.Controls.Add(this.btnPreview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Split";
            this.Text = "Split";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Split_FormClosing);
            this.Load += new System.EventHandler(this.Split_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AudioTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.TextBox txtDisplayAsset;
        private System.Windows.Forms.TextBox txtDisplayTime;
        private System.Windows.Forms.Timer tmUpdateTimePosition;
        private System.Windows.Forms.Button btnFastRewind;
        private System.Windows.Forms.Button btnFastForward;
        private System.Windows.Forms.Button btnFineRewind;
        private System.Windows.Forms.Button btnFineForward;
        private System.Windows.Forms.Button btnSplit;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.TextBox txtSplitTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelSplitTime;
        private System.Windows.Forms.TrackBar AudioTrackBar;
    }
}