namespace Obi.UserControls
{
    partial class StructureBlock
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
            this.mLabelBox = new System.Windows.Forms.TextBox();
            this.mLabel = new System.Windows.Forms.Label();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mLabelBox
            // 
            this.mLabelBox.BackColor = System.Drawing.Color.PowderBlue;
            this.mLabelBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mLabelBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mLabelBox.Location = new System.Drawing.Point(3, 3);
            this.mLabelBox.Name = "mLabelBox";
            this.mLabelBox.Size = new System.Drawing.Size(144, 13);
            this.mLabelBox.TabIndex = 0;
            this.mLabelBox.TabStop = false;
            this.mLabelBox.Visible = false;
            this.mLabelBox.Click += new System.EventHandler(this.mLabelBox_Click);
            this.mLabelBox.Leave += new System.EventHandler(this.mLabelBox_Leave);
            this.mLabelBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mLabelBox_KeyDown);
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Location = new System.Drawing.Point(3, 3);
            this.mLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(0, 13);
            this.mLabel.TabIndex = 1;
            this.mLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // mToolTip
            // 
            this.mToolTip.AutomaticDelay = 3000;
            this.mToolTip.AutoPopDelay = 4000;
            this.mToolTip.InitialDelay = 3000;
            this.mToolTip.IsBalloon = true;
            this.mToolTip.ReshowDelay = 600;
            this.mToolTip.ToolTipTitle = "Structure Block";
            // 
            // StructureBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.Controls.Add(this.mLabelBox);
            this.Controls.Add(this.mLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.Name = "StructureBlock";
            this.Size = new System.Drawing.Size(150, 20);
            this.Click += new System.EventHandler(this.StructureBlock_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mLabelBox;
        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.ToolTip mToolTip;

    }
}
