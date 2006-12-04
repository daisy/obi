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
            this.label1 = new System.Windows.Forms.Label();
            this.mLeftBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mRightBox = new System.Windows.Forms.TextBox();
            this.mResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Left:";
            // 
            // mLeftBox
            // 
            this.mLeftBox.Location = new System.Drawing.Point(36, 5);
            this.mLeftBox.Name = "mLeftBox";
            this.mLeftBox.Size = new System.Drawing.Size(32, 19);
            this.mLeftBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(74, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Right:";
            // 
            // mRightBox
            // 
            this.mRightBox.Location = new System.Drawing.Point(114, 5);
            this.mRightBox.Name = "mRightBox";
            this.mRightBox.Size = new System.Drawing.Size(32, 19);
            this.mRightBox.TabIndex = 3;
            // 
            // mResetButton
            // 
            this.mResetButton.AutoSize = true;
            this.mResetButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mResetButton.BackColor = System.Drawing.SystemColors.Control;
            this.mResetButton.Location = new System.Drawing.Point(152, 3);
            this.mResetButton.Name = "mResetButton";
            this.mResetButton.Size = new System.Drawing.Size(45, 22);
            this.mResetButton.TabIndex = 4;
            this.mResetButton.Text = "Re&set";
            this.mResetButton.UseVisualStyleBackColor = false;
            // 
            // TextVUMeterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mResetButton);
            this.Controls.Add(this.mRightBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mLeftBox);
            this.Controls.Add(this.label1);
            this.Name = "TextVUMeterPanel";
            this.Size = new System.Drawing.Size(220, 41);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mLeftBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mRightBox;
        private System.Windows.Forms.Button mResetButton;

    }
}
