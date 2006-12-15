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
            this.lblLeft.Location = new System.Drawing.Point(3, 9);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(28, 13);
            this.lblLeft.TabIndex = 0;
            this.lblLeft.Text = "&Left:";
            // 
            // mLeftBox
            // 
            this.mLeftBox.Location = new System.Drawing.Point(36, 5);
            this.mLeftBox.Name = "mLeftBox";
            this.mLeftBox.Size = new System.Drawing.Size(32, 20);
            this.mLeftBox.TabIndex = 1;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(74, 9);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(35, 13);
            this.lblRight.TabIndex = 2;
            this.lblRight.Text = "R&ight:";
            // 
            // mRightBox
            // 
            this.mRightBox.Location = new System.Drawing.Point(114, 5);
            this.mRightBox.Name = "mRightBox";
            this.mRightBox.Size = new System.Drawing.Size(32, 20);
            this.mRightBox.TabIndex = 3;
            // 
            // mResetButton
            // 
            this.mResetButton.AutoSize = true;
            this.mResetButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mResetButton.BackColor = System.Drawing.SystemColors.Control;
            this.mResetButton.Location = new System.Drawing.Point(152, 3);
            this.mResetButton.Name = "mResetButton";
            this.mResetButton.Size = new System.Drawing.Size(45, 23);
            this.mResetButton.TabIndex = 4;
            this.mResetButton.Text = "Re&set";
            this.mResetButton.UseVisualStyleBackColor = false;
            this.mResetButton.Click += new System.EventHandler(this.mResetButton_Click);
            // 
            // tmUpdateText
            // 
            this.tmUpdateText.Enabled = true;
            this.tmUpdateText.Interval = 1500;
            this.tmUpdateText.Tick += new System.EventHandler(this.tmUpdateText_Tick);
            // 
            // TextVUMeterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mResetButton);
            this.Controls.Add(this.mRightBox);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.mLeftBox);
            this.Controls.Add(this.lblLeft);
            this.Name = "TextVUMeterPanel";
            this.Size = new System.Drawing.Size(220, 44);
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
