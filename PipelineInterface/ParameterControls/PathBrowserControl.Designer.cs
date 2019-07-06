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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathBrowserControl));
            this.mNiceNameLabel = new System.Windows.Forms.Label();
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mNiceNameLabel
            // 
            resources.ApplyResources(this.mNiceNameLabel, "mNiceNameLabel");
            this.mNiceNameLabel.Name = "mNiceNameLabel";
            // 
            // mTextBox
            // 
            resources.ApplyResources(this.mTextBox, "mTextBox");
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTextBox.Name = "mTextBox";
            // 
            // mBrowseButton
            // 
            resources.ApplyResources(this.mBrowseButton, "mBrowseButton");
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.mBrowseButton_Click);
            // 
            // PathBrowserControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mBrowseButton);
            this.Controls.Add(this.mTextBox);
            this.Controls.Add(this.mNiceNameLabel);
            this.Name = "PathBrowserControl";
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
