namespace Zaboom
{
    partial class SplitForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mPreviewButton = new System.Windows.Forms.Button();
            this.mBeforeButton = new System.Windows.Forms.Button();
            this.mAfterButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mOKButton
            // 
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(12, 179);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 75);
            this.mOKButton.TabIndex = 0;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(93, 179);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 75);
            this.mCancelButton.TabIndex = 1;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mPreviewButton
            // 
            this.mPreviewButton.Location = new System.Drawing.Point(12, 98);
            this.mPreviewButton.Name = "mPreviewButton";
            this.mPreviewButton.Size = new System.Drawing.Size(75, 75);
            this.mPreviewButton.TabIndex = 2;
            this.mPreviewButton.Text = "&Preview";
            this.mPreviewButton.UseVisualStyleBackColor = true;
            this.mPreviewButton.Click += new System.EventHandler(this.mPreviewButton_Click);
            // 
            // mBeforeButton
            // 
            this.mBeforeButton.Location = new System.Drawing.Point(93, 98);
            this.mBeforeButton.Name = "mBeforeButton";
            this.mBeforeButton.Size = new System.Drawing.Size(75, 75);
            this.mBeforeButton.TabIndex = 3;
            this.mBeforeButton.Text = "&Before";
            this.mBeforeButton.UseVisualStyleBackColor = true;
            this.mBeforeButton.Click += new System.EventHandler(this.mBeforeButton_Click);
            // 
            // mAfterButton
            // 
            this.mAfterButton.Location = new System.Drawing.Point(174, 98);
            this.mAfterButton.Name = "mAfterButton";
            this.mAfterButton.Size = new System.Drawing.Size(75, 75);
            this.mAfterButton.TabIndex = 4;
            this.mAfterButton.Text = "&After";
            this.mAfterButton.UseVisualStyleBackColor = true;
            this.mAfterButton.Click += new System.EventHandler(this.mAfterButton_Click);
            // 
            // SplitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.mAfterButton);
            this.Controls.Add(this.mBeforeButton);
            this.Controls.Add(this.mPreviewButton);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Name = "SplitForm";
            this.Text = "Splitting";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Button mPreviewButton;
        private System.Windows.Forms.Button mBeforeButton;
        private System.Windows.Forms.Button mAfterButton;
    }
}