namespace Obi.UserControls
{
    partial class AudioBlock
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mTimeLabel = new System.Windows.Forms.Label();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mAssetName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mTimeLabel
            // 
            this.mTimeLabel.AutoSize = true;
            this.mTimeLabel.Location = new System.Drawing.Point(3, 27);
            this.mTimeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mTimeLabel.Name = "mTimeLabel";
            this.mTimeLabel.Size = new System.Drawing.Size(32, 13);
            this.mTimeLabel.TabIndex = 2;
            this.mTimeLabel.Text = "(time)";
            this.mTimeLabel.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.mTimeLabel.Click += new System.EventHandler(this.AudioBlock_Click);
            // 
            // mToolTip
            // 
            this.mToolTip.AutomaticDelay = 3000;
            this.mToolTip.AutoPopDelay = 4000;
            this.mToolTip.InitialDelay = 3000;
            this.mToolTip.IsBalloon = true;
            this.mToolTip.ReshowDelay = 600;
            this.mToolTip.ToolTipTitle = "Audio Block";
            // 
            // mAssetName
            // 
            this.mAssetName.AutoSize = true;
            this.mAssetName.Location = new System.Drawing.Point(3, 0);
            this.mAssetName.Name = "mAssetName";
            this.mAssetName.Size = new System.Drawing.Size(38, 13);
            this.mAssetName.TabIndex = 1;
            this.mAssetName.Text = "(asset)";
            this.mAssetName.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.mAssetName.Click += new System.EventHandler(this.AudioBlock_Click);
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.mAssetName);
            this.Controls.Add(this.mTimeLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.Name = "AudioBlock";
            this.Size = new System.Drawing.Size(42, 51);
            this.Enter += new System.EventHandler(this.AudioBlock_enter);
            this.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.Click += new System.EventHandler(this.AudioBlock_Click);
            this.Leave += new System.EventHandler(this.AudioBlock_leave);
            this.SizeChanged += new System.EventHandler(this.AudioBlock_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mTimeLabel;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.Label mAssetName;
    }
}
