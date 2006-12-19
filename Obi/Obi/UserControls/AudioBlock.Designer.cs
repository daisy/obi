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
            this.mPage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mTimeLabel
            // 
            this.mTimeLabel.AutoSize = true;
            this.mTimeLabel.Location = new System.Drawing.Point(3, 23);
            this.mTimeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mTimeLabel.Name = "mTimeLabel";
            this.mTimeLabel.Size = new System.Drawing.Size(32, 13);
            this.mTimeLabel.TabIndex = 2;
            this.mTimeLabel.Text = "(time)";
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
            this.mLabel.Location = new System.Drawing.Point(3, 3);
            this.mLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(35, 13);
            this.mLabel.TabIndex = 1;
            this.mLabel.Text = "(label)";
            this.mLabel.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.mLabel.Click += new System.EventHandler(this.AudioBlock_Click);
            this.mLabel.SizeChanged += new System.EventHandler(this.ContentsSizeChanged);
            // 
            // mPage
            // 
            this.mPage.BackColor = System.Drawing.Color.Pink;
            this.mPage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mPage.Location = new System.Drawing.Point(5, 42);
            this.mPage.Name = "mPage";
            this.mPage.ReadOnly = true;
            this.mPage.Size = new System.Drawing.Size(53, 13);
            this.mPage.TabIndex = 4;
            this.mPage.Text = "(page)";
            this.mPage.Visible = false;
            this.mPage.Leave += new System.EventHandler(this.mPage_Leave);
            this.mPage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mPage_KeyDown);
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Pink;
            this.Controls.Add(this.mPage);
            this.Controls.Add(this.mLabel);
            this.Controls.Add(this.mTimeLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.Name = "AudioBlock";
            this.Size = new System.Drawing.Size(61, 59);
            this.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.Click += new System.EventHandler(this.AudioBlock_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mTimeLabel;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.TextBox mPage;
    }
}
