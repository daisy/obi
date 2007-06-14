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
            this.mNameBox.Size = new System.Drawing.Size(267, 20);
            this.mNameBox.Sorted = true;
            this.mNameBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Content:";
            // 
            // mContentBox
            // 
            this.mContentBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mContentBox.Location = new System.Drawing.Point(56, 32);
            this.mContentBox.Name = "mContentBox";
            this.mContentBox.Size = new System.Drawing.Size(321, 19);
            this.mContentBox.TabIndex = 3;
            // 
            // mDeleteButton
            // 
            this.mDeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mDeleteButton.Location = new System.Drawing.Point(329, 3);
            this.mDeleteButton.Name = "mDeleteButton";
            this.mDeleteButton.Size = new System.Drawing.Size(48, 23);
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
            this.Controls.Add(this.mContentBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mNameBox);
            this.Controls.Add(this.label1);
            this.Name = "MetadataPanel";
            this.Size = new System.Drawing.Size(380, 56);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox mNameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mContentBox;
        private System.Windows.Forms.Button mDeleteButton;
    }
}
