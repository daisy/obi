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
            this.mAnnotationLabel = new System.Windows.Forms.Label();
            this.mTimeLabel = new System.Windows.Forms.Label();
            this.mRenameBox = new System.Windows.Forms.TextBox();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mAnnotationLabel
            // 
            this.mAnnotationLabel.AutoSize = true;
            this.mAnnotationLabel.Location = new System.Drawing.Point(3, 3);
            this.mAnnotationLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mAnnotationLabel.Name = "mAnnotationLabel";
            this.mAnnotationLabel.Size = new System.Drawing.Size(63, 13);
            this.mAnnotationLabel.TabIndex = 0;
            this.mAnnotationLabel.Text = "(annotation)";
            this.mAnnotationLabel.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.mAnnotationLabel.Click += new System.EventHandler(this.AudioBlock_Click);
            // 
            // mTimeLabel
            // 
            this.mTimeLabel.AutoSize = true;
            this.mTimeLabel.Location = new System.Drawing.Point(3, 23);
            this.mTimeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mTimeLabel.Name = "mTimeLabel";
            this.mTimeLabel.Size = new System.Drawing.Size(32, 13);
            this.mTimeLabel.TabIndex = 1;
            this.mTimeLabel.Text = "(time)";
            this.mTimeLabel.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.mTimeLabel.Click += new System.EventHandler(this.AudioBlock_Click);
            // 
            // mRenameBox
            // 
            this.mRenameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mRenameBox.Location = new System.Drawing.Point(5, 3);
            this.mRenameBox.Name = "mRenameBox";
            this.mRenameBox.Size = new System.Drawing.Size(100, 13);
            this.mRenameBox.TabIndex = 2;
            this.mRenameBox.TabStop = false;
            this.mRenameBox.Visible = false;
            this.mRenameBox.Leave += new System.EventHandler(this.mRenameBox_Leave);
            this.mRenameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mRenameBox_KeyDown);
            // 
            // mToolTip
            // 
            this.mToolTip.AutomaticDelay = 1000;
            this.mToolTip.IsBalloon = true;
            this.mToolTip.ToolTipTitle = "Audio Block";
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.mRenameBox);
            this.Controls.Add(this.mTimeLabel);
            this.Controls.Add(this.mAnnotationLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.Name = "AudioBlock";
            this.Size = new System.Drawing.Size(206, 163);
            this.Enter += new System.EventHandler(this.AudioBlock_enter);
            this.DoubleClick += new System.EventHandler(this.AudioBlock_DoubleClick);
            this.Click += new System.EventHandler(this.AudioBlock_Click);
            this.Leave += new System.EventHandler(this.AudioBlock_leave);
            this.SizeChanged += new System.EventHandler(this.AudioBlock_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mAnnotationLabel;
        private System.Windows.Forms.Label mTimeLabel;
        private System.Windows.Forms.TextBox mRenameBox;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
