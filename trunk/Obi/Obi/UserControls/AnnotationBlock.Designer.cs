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
            this.mRenameBox.BackColor = System.Drawing.Color.PowderBlue;
            this.mRenameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mRenameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mRenameBox.Location = new System.Drawing.Point(3, 3);
            this.mRenameBox.Margin = new System.Windows.Forms.Padding(0);
            this.mRenameBox.Name = "mRenameBox";
            this.mRenameBox.Size = new System.Drawing.Size(0, 14);
            this.mRenameBox.TabIndex = 3;
            this.mRenameBox.TabStop = false;
            this.mRenameBox.Visible = false;
            this.mRenameBox.Enter += new System.EventHandler(this.mRenameBox_Enter);
            this.mRenameBox.Leave += new System.EventHandler(this.mRenameBox_Leave);
            this.mRenameBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mRenameBox_KeyUp);
            this.mRenameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mRenameBox_KeyDown);
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel.Location = new System.Drawing.Point(3, 3);
            this.mLabel.Margin = new System.Windows.Forms.Padding(0);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(0, 15);
            this.mLabel.TabIndex = 4;
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
            this.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Size = new System.Drawing.Size(189, 21);
            this.Enter += new System.EventHandler(this.AnnotationBlock_Enter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mRenameBox;
        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
