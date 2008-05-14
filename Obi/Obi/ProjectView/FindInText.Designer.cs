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
            this.mLabel = new System.Windows.Forms.Label();
            this.mString = new System.Windows.Forms.TextBox();
            this.mPreviousButton = new System.Windows.Forms.Button();
            this.mNextButton = new System.Windows.Forms.Button();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel.Location = new System.Drawing.Point(3, 8);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(51, 13);
            this.mLabel.TabIndex = 0;
            this.mLabel.Text = "Search:";
            // 
            // mString
            // 
            this.mString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mString.Location = new System.Drawing.Point(60, 6);
            this.mString.Name = "mString";
            this.mString.Size = new System.Drawing.Size(206, 20);
            this.mString.TabIndex = 1;
            this.mString.TextChanged += new System.EventHandler(this.mString_TextChanged);
            this.mString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mString_KeyDown);
            this.mString.Enter += new System.EventHandler(this.mString_Enter);
            // 
            // mPreviousButton
            // 
            this.mPreviousButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mPreviousButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPreviousButton.Location = new System.Drawing.Point(272, 3);
            this.mPreviousButton.Name = "mPreviousButton";
            this.mPreviousButton.Size = new System.Drawing.Size(75, 23);
            this.mPreviousButton.TabIndex = 2;
            this.mPreviousButton.Text = "Previo&us";
            this.mPreviousButton.UseVisualStyleBackColor = true;
            // 
            // mNextButton
            // 
            this.mNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mNextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNextButton.Location = new System.Drawing.Point(353, 3);
            this.mNextButton.Name = "mNextButton";
            this.mNextButton.Size = new System.Drawing.Size(75, 23);
            this.mNextButton.TabIndex = 3;
            this.mNextButton.Text = "&Next";
            this.mNextButton.UseVisualStyleBackColor = true;
            // 
            // mCloseButton
            // 
            this.mCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCloseButton.Location = new System.Drawing.Point(434, 3);
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.Size = new System.Drawing.Size(75, 23);
            this.mCloseButton.TabIndex = 4;
            this.mCloseButton.Text = "&Close";
            this.mCloseButton.UseVisualStyleBackColor = true;
            this.mCloseButton.Click += new System.EventHandler(this.mCloseButton_Click);
            // 
            // FindInText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.Controls.Add(this.mCloseButton);
            this.Controls.Add(this.mNextButton);
            this.Controls.Add(this.mPreviousButton);
            this.Controls.Add(this.mString);
            this.Controls.Add(this.mLabel);
            this.Name = "FindInText";
            this.Size = new System.Drawing.Size(512, 29);
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
