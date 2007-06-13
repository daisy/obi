namespace Obi.UserControls
{
    partial class MetadataPanel
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
            this.mNameBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mContentBox = new System.Windows.Forms.TextBox();
            this.mEditButton = new System.Windows.Forms.Button();
            this.mDeleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Name:";
            // 
            // mNameBox
            // 
            this.mNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mNameBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mNameBox.FormattingEnabled = true;
            this.mNameBox.Location = new System.Drawing.Point(56, 5);
            this.mNameBox.Name = "mNameBox";
            this.mNameBox.Size = new System.Drawing.Size(240, 20);
            this.mNameBox.Sorted = true;
            this.mNameBox.TabIndex = 1;
            this.mNameBox.SelectionChangeCommitted += new System.EventHandler(this.mNameBox_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Content:";
            // 
            // mContentBox
            // 
            this.mContentBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mContentBox.Location = new System.Drawing.Point(56, 34);
            this.mContentBox.Name = "mContentBox";
            this.mContentBox.Size = new System.Drawing.Size(240, 19);
            this.mContentBox.TabIndex = 3;
            // 
            // mEditButton
            // 
            this.mEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mEditButton.Location = new System.Drawing.Point(302, 3);
            this.mEditButton.Name = "mEditButton";
            this.mEditButton.Size = new System.Drawing.Size(75, 23);
            this.mEditButton.TabIndex = 4;
            this.mEditButton.Text = "&Edit";
            this.mEditButton.UseVisualStyleBackColor = true;
            this.mEditButton.Click += new System.EventHandler(this.mEditButton_Click);
            // 
            // mDeleteButton
            // 
            this.mDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mDeleteButton.Location = new System.Drawing.Point(302, 32);
            this.mDeleteButton.Name = "mDeleteButton";
            this.mDeleteButton.Size = new System.Drawing.Size(75, 23);
            this.mDeleteButton.TabIndex = 5;
            this.mDeleteButton.Text = "&Delete";
            this.mDeleteButton.UseVisualStyleBackColor = true;
            this.mDeleteButton.Click += new System.EventHandler(this.mDeleteButton_Click);
            // 
            // MetadataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mDeleteButton);
            this.Controls.Add(this.mEditButton);
            this.Controls.Add(this.mContentBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mNameBox);
            this.Controls.Add(this.label1);
            this.Name = "MetadataPanel";
            this.Size = new System.Drawing.Size(380, 58);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox mNameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mContentBox;
        private System.Windows.Forms.Button mEditButton;
        private System.Windows.Forms.Button mDeleteButton;
    }
}
