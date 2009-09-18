namespace Obi.ProjectView
{
    partial class EditableLabel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditableLabel));
            this.mLabel = new System.Windows.Forms.Label();
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            resources.ApplyResources(this.mLabel, "mLabel");
            this.mLabel.Name = "mLabel";
            this.mLabel.UseMnemonic = false;
            this.mLabel.Click += new System.EventHandler(this.mLabel_Click);
            this.mLabel.Enter += new System.EventHandler(this.mLabel_Enter);
            // 
            // mTextBox
            // 
            resources.ApplyResources(this.mTextBox, "mTextBox");
            this.mTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mTextBox_KeyDown);
            // 
            // mOKButton
            // 
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // EditableLabel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Thistle;
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mTextBox);
            this.Controls.Add(this.mLabel);
            this.Name = "EditableLabel";
            this.Tag = "";
            this.Leave += new System.EventHandler(this.EditableLabel_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.TextBox mTextBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
    }
}
