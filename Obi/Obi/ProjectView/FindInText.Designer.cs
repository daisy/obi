namespace Obi.ProjectView
{
    partial class FindInText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindInText));
            this.mLabel = new System.Windows.Forms.Label();
            this.mString = new System.Windows.Forms.TextBox();
            this.mPreviousButton = new System.Windows.Forms.Button();
            this.mNextButton = new System.Windows.Forms.Button();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            this.mLabel.AccessibleDescription = null;
            this.mLabel.AccessibleName = null;
            resources.ApplyResources(this.mLabel, "mLabel");
            this.mLabel.Name = "mLabel";
            // 
            // mString
            // 
            this.mString.AccessibleDescription = null;
            this.mString.AccessibleName = null;
            resources.ApplyResources(this.mString, "mString");
            this.mString.BackgroundImage = null;
            this.mString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mString.Font = null;
            this.mString.Name = "mString";
            this.mString.TextChanged += new System.EventHandler(this.mString_TextChanged);
            this.mString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mString_KeyDown);
            this.mString.Enter += new System.EventHandler(this.mString_Enter);
            // 
            // mPreviousButton
            // 
            this.mPreviousButton.AccessibleDescription = null;
            this.mPreviousButton.AccessibleName = null;
            resources.ApplyResources(this.mPreviousButton, "mPreviousButton");
            this.mPreviousButton.BackgroundImage = null;
            this.mPreviousButton.Font = null;
            this.mPreviousButton.Name = "mPreviousButton";
            this.mPreviousButton.UseVisualStyleBackColor = true;
            // 
            // mNextButton
            // 
            this.mNextButton.AccessibleDescription = null;
            this.mNextButton.AccessibleName = null;
            resources.ApplyResources(this.mNextButton, "mNextButton");
            this.mNextButton.BackgroundImage = null;
            this.mNextButton.Font = null;
            this.mNextButton.Name = "mNextButton";
            this.mNextButton.UseVisualStyleBackColor = true;
            // 
            // mCloseButton
            // 
            this.mCloseButton.AccessibleDescription = null;
            this.mCloseButton.AccessibleName = null;
            resources.ApplyResources(this.mCloseButton, "mCloseButton");
            this.mCloseButton.BackgroundImage = null;
            this.mCloseButton.Font = null;
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.UseVisualStyleBackColor = true;
            this.mCloseButton.Click += new System.EventHandler(this.mCloseButton_Click);
            // 
            // FindInText
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.BackgroundImage = null;
            this.Controls.Add(this.mCloseButton);
            this.Controls.Add(this.mNextButton);
            this.Controls.Add(this.mPreviousButton);
            this.Controls.Add(this.mString);
            this.Controls.Add(this.mLabel);
            this.Font = null;
            this.Name = "FindInText";
            this.Leave += new System.EventHandler(this.FindInText_Leave);
            this.Enter += new System.EventHandler(this.FindInText_Enter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.TextBox mString;
        private System.Windows.Forms.Button mPreviousButton;
        private System.Windows.Forms.Button mNextButton;
        private System.Windows.Forms.Button mCloseButton;
    }
}
