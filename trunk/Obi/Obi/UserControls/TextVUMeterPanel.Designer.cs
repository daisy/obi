namespace Obi.UserControls
{
    partial class TextVUMeterPanel
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
            this.lblLeft = new System.Windows.Forms.Label();
            this.mLeftBox = new System.Windows.Forms.TextBox();
            this.lblRight = new System.Windows.Forms.Label();
            this.mRightBox = new System.Windows.Forms.TextBox();
            this.mResetButton = new System.Windows.Forms.Button();
            this.tmUpdateText = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(-3, 6);
            this.lblLeft.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(18, 16);
            this.lblLeft.TabIndex = 0;
            this.lblLeft.Text = "&L:";
            // 
            // mLeftBox
            // 
            this.mLeftBox.AccessibleName = "Left";
            this.mLeftBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mLeftBox.Location = new System.Drawing.Point(23, 4);
            this.mLeftBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mLeftBox.Name = "mLeftBox";
            this.mLeftBox.ReadOnly = true;
            this.mLeftBox.Size = new System.Drawing.Size(42, 22);
            this.mLeftBox.TabIndex = 1;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(73, 6);
            this.lblRight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(21, 16);
            this.lblRight.TabIndex = 2;
            this.lblRight.Text = "&R:";
            // 
            // mRightBox
            // 
            this.mRightBox.AccessibleName = "Right";
            this.mRightBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mRightBox.Location = new System.Drawing.Point(102, 4);
            this.mRightBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mRightBox.Name = "mRightBox";
            this.mRightBox.ReadOnly = true;
            this.mRightBox.Size = new System.Drawing.Size(42, 22);
            this.mRightBox.TabIndex = 3;
            // 
            // mResetButton
            // 
            this.mResetButton.AutoSize = true;
            this.mResetButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mResetButton.BackColor = System.Drawing.SystemColors.Control;
            this.mResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mResetButton.Location = new System.Drawing.Point(152, 0);
            this.mResetButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mResetButton.Name = "mResetButton";
            this.mResetButton.Size = new System.Drawing.Size(56, 28);
            this.mResetButton.TabIndex = 4;
            this.mResetButton.Text = "R&eset";
            this.mResetButton.UseVisualStyleBackColor = false;
            this.mResetButton.Click += new System.EventHandler(this.mResetButton_Click);
            // 
            // tmUpdateText
            // 
            this.tmUpdateText.Enabled = true;
            this.tmUpdateText.Interval = 600;
            this.tmUpdateText.Tick += new System.EventHandler(this.tmUpdateText_Tick);
            // 
            // TextVUMeterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.mResetButton);
            this.Controls.Add(this.mRightBox);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.mLeftBox);
            this.Controls.Add(this.lblLeft);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TextVUMeterPanel";
            this.Size = new System.Drawing.Size(208, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.TextBox mLeftBox;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.TextBox mRightBox;
        private System.Windows.Forms.Button mResetButton;
        private System.Windows.Forms.Timer tmUpdateText;

    }
}
