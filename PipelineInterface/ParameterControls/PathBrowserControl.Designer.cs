using System.Drawing;
namespace PipelineInterface.ParameterControls
{
    partial class PathBrowserControl
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
            this.mNiceNameLabel = new System.Windows.Forms.Label();
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mNiceNameLabel
            // 
            this.mNiceNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mNiceNameLabel.AutoSize = true;
            this.mNiceNameLabel.Location = new System.Drawing.Point(4, 26);
            this.mNiceNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mNiceNameLabel.Name = "mNiceNameLabel";
            this.mNiceNameLabel.Size = new System.Drawing.Size(39, 16);
            this.mNiceNameLabel.TabIndex = 1;
            this.mNiceNameLabel.Text = "Nice:";
            // 
            // mTextBox
            // 
            this.mTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTextBox.Location = new System.Drawing.Point(51, 24);
            this.mTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.Size = new System.Drawing.Size(262, 22);
            this.mTextBox.TabIndex = 2;
            // 
            // mBrowseButton
            // 
            this.mBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBrowseButton.Location = new System.Drawing.Point(321, 20);
            this.mBrowseButton.Margin = new System.Windows.Forms.Padding(4);
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.Size = new System.Drawing.Size(100, 28);
            this.mBrowseButton.TabIndex = 3;
            this.mBrowseButton.Text = "Browse";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.mBrowseButton_Click);
            // 
            // PathBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mBrowseButton);
            this.Controls.Add(this.mTextBox);
            this.Controls.Add(this.mNiceNameLabel);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "PathBrowserControl";
            this.Size = new System.Drawing.Size(425, 52);
            this.Controls.SetChildIndex(this.mLabel, 0);
            this.Controls.SetChildIndex(this.mNiceNameLabel, 0);
            this.Controls.SetChildIndex(this.mTextBox, 0);
            this.Controls.SetChildIndex(this.mBrowseButton, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mNiceNameLabel;
        private System.Windows.Forms.TextBox mTextBox;
        private System.Windows.Forms.Button mBrowseButton;
    }
}
