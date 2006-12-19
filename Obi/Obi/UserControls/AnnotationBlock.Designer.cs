namespace Obi.UserControls
{
    partial class AnnotationBlock
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
            this.mRenameBox = new System.Windows.Forms.TextBox();
            this.mLabel = new System.Windows.Forms.Label();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mRenameBox
            // 
            this.mRenameBox.BackColor = System.Drawing.Color.LightYellow;
            this.mRenameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mRenameBox.Location = new System.Drawing.Point(3, 3);
            this.mRenameBox.Margin = new System.Windows.Forms.Padding(0);
            this.mRenameBox.Name = "mRenameBox";
            this.mRenameBox.Size = new System.Drawing.Size(66, 12);
            this.mRenameBox.TabIndex = 3;
            this.mRenameBox.TabStop = false;
            this.mRenameBox.Visible = false;
            this.mRenameBox.Click += new System.EventHandler(this.AnnotationBlock_Click);
            this.mRenameBox.Leave += new System.EventHandler(this.mRenameBox_Leave);
            this.mRenameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mRenameBox_KeyDown);
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Location = new System.Drawing.Point(3, 3);
            this.mLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(66, 12);
            this.mLabel.TabIndex = 4;
            this.mLabel.Text = "(annotation)";
            this.mLabel.Click += new System.EventHandler(this.AnnotationBlock_Click);
            this.mLabel.SizeChanged += new System.EventHandler(this.mLabel_SizeChanged);
            // 
            // mToolTip
            // 
            this.mToolTip.IsBalloon = true;
            // 
            // AnnotationBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.Controls.Add(this.mLabel);
            this.Controls.Add(this.mRenameBox);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.Name = "AnnotationBlock";
            this.Size = new System.Drawing.Size(72, 18);
            this.Load += new System.EventHandler(this.AnnotationBlock_Load);
            this.Click += new System.EventHandler(this.AnnotationBlock_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mRenameBox;
        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
