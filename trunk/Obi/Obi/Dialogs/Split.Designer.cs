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
            this.SuspendLayout();
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(30, 26);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 21);
            this.btnPreview.TabIndex = 0;
            this.btnPreview.Text = "Pre&view";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // txtDisplayAsset
            // 
            this.txtDisplayAsset.Location = new System.Drawing.Point(30, 0);
            this.txtDisplayAsset.Name = "txtDisplayAsset";
            this.txtDisplayAsset.ReadOnly = true;
            this.txtDisplayAsset.Size = new System.Drawing.Size(100, 19);
            this.txtDisplayAsset.TabIndex = 1;
            // 
            // txtDisplayTime
            // 
            this.txtDisplayTime.Location = new System.Drawing.Point(30, 55);
            this.txtDisplayTime.Name = "txtDisplayTime";
            this.txtDisplayTime.ReadOnly = true;
            this.txtDisplayTime.Size = new System.Drawing.Size(100, 19);
            this.txtDisplayTime.TabIndex = 2;
            // 
            // tmUpdateTimePosition
            // 
            this.tmUpdateTimePosition.Enabled = true;
            this.tmUpdateTimePosition.Interval = 1000;
            this.tmUpdateTimePosition.Tick += new System.EventHandler(this.tmUpdateTimePosition_Tick);
            // 
            // btnFastRewind
            // 
            this.btnFastRewind.Location = new System.Drawing.Point(30, 83);
            this.btnFastRewind.Name = "btnFastRewind";
            this.btnFastRewind.Size = new System.Drawing.Size(75, 21);
            this.btnFastRewind.TabIndex = 3;
            this.btnFastRewind.Text = "Fast Rewind";
            this.btnFastRewind.UseVisualStyleBackColor = true;
            this.btnFastRewind.Click += new System.EventHandler(this.btnFastRewind_Click);
            // 
            // btnFastForward
            // 
            this.btnFastForward.Location = new System.Drawing.Point(120, 83);
            this.btnFastForward.Name = "btnFastForward";
            this.btnFastForward.Size = new System.Drawing.Size(75, 21);
            this.btnFastForward.TabIndex = 4;
            this.btnFastForward.Text = "Fast Forward";
            this.btnFastForward.UseVisualStyleBackColor = true;
            this.btnFastForward.Click += new System.EventHandler(this.btnFastForward_Click);
            // 
            // btnFineRewind
            // 
            this.btnFineRewind.Location = new System.Drawing.Point(30, 138);
            this.btnFineRewind.Name = "btnFineRewind";
            this.btnFineRewind.Size = new System.Drawing.Size(75, 21);
            this.btnFineRewind.TabIndex = 5;
            this.btnFineRewind.Text = "Fine Rewind";
            this.btnFineRewind.UseVisualStyleBackColor = true;
            this.btnFineRewind.Click += new System.EventHandler(this.btnFineRewind_Click);
            // 
            // btnFineForward
            // 
            this.btnFineForward.Location = new System.Drawing.Point(120, 138);
            this.btnFineForward.Name = "btnFineForward";
            this.btnFineForward.Size = new System.Drawing.Size(75, 21);
            this.btnFineForward.TabIndex = 6;
            this.btnFineForward.Text = "Fine Forward";
            this.btnFineForward.UseVisualStyleBackColor = true;
            this.btnFineForward.Click += new System.EventHandler(this.btnFineForward_Click);
            // 
            // btnSplit
            // 
            this.btnSplit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSplit.Location = new System.Drawing.Point(30, 166);
            this.btnSplit.Name = "btnSplit";
            this.btnSplit.Size = new System.Drawing.Size(75, 21);
            this.btnSplit.TabIndex = 7;
            this.btnSplit.Text = "Split";
            this.btnSplit.UseVisualStyleBackColor = true;
            this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(120, 166);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 21);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(150, 28);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 21);
            this.btnPause.TabIndex = 0;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // txtSplitTime
            // 
            this.txtSplitTime.Location = new System.Drawing.Point(200, 231);
            this.txtSplitTime.Name = "txtSplitTime";
            this.txtSplitTime.Size = new System.Drawing.Size(100, 19);
            this.txtSplitTime.TabIndex = 9;
            // 
            // Split
            // 
            this.AcceptButton = this.btnSplit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(292, 252);
            this.Controls.Add(this.txtSplitTime);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnSplit);
            this.Controls.Add(this.btnFineForward);
            this.Controls.Add(this.btnFineRewind);
            this.Controls.Add(this.btnFastForward);
            this.Controls.Add(this.btnFastRewind);
            this.Controls.Add(this.txtDisplayTime);
            this.Controls.Add(this.txtDisplayAsset);
            this.Controls.Add(this.btnPreview);
            this.Name = "Split";
            this.Text = "Split";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Split_FormClosing);
            this.Load += new System.EventHandler(this.Split_Load);
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
    }
}