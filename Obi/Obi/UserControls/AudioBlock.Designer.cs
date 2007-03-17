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
            this.mLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mTimeLabel
            // 
            this.mTimeLabel.AutoSize = true;
            this.mTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTimeLabel.Location = new System.Drawing.Point(7, 28);
            this.mTimeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mTimeLabel.Name = "mTimeLabel";
            this.mTimeLabel.Size = new System.Drawing.Size(0, 15);
            this.mTimeLabel.TabIndex = 2;
            this.mTimeLabel.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.mTimeLabel.Click += new System.EventHandler(this.AudioBlock_Click);
            this.mTimeLabel.SizeChanged += new System.EventHandler(this.ContentsSizeChanged);
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
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel.Location = new System.Drawing.Point(7, 7);
            this.mLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(0, 15);
            this.mLabel.TabIndex = 1;
            this.mLabel.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.mLabel.Click += new System.EventHandler(this.AudioBlock_Click);
            this.mLabel.SizeChanged += new System.EventHandler(this.ContentsSizeChanged);
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Pink;
            this.Controls.Add(this.mLabel);
            this.Controls.Add(this.mTimeLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.Name = "AudioBlock";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(105, 54);
            this.Enter += new System.EventHandler(this.AudioBlock_Enter);
            this.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.Click += new System.EventHandler(this.AudioBlock_Click);
            this.SizeChanged += new System.EventHandler(this.AudioBlock_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mTimeLabel;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.Label mLabel;
    }
}
