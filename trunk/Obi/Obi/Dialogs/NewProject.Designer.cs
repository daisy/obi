namespace Obi.Dialogs
{
    partial class NewProject
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
            this.label1 = new System.Windows.Forms.Label();
            this.mTitleBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mFileBox = new System.Windows.Forms.TextBox();
            this.mSelectButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mAutoTitleCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Title:";
            // 
            // mTitleBox
            // 
            this.mTitleBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTitleBox.Location = new System.Drawing.Point(48, 12);
            this.mTitleBox.Name = "mTitleBox";
            this.mTitleBox.Size = new System.Drawing.Size(532, 19);
            this.mTitleBox.TabIndex = 1;
            this.mTitleBox.TextChanged += new System.EventHandler(this.mTitleBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "&File:";
            // 
            // mFileBox
            // 
            this.mFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFileBox.Location = new System.Drawing.Point(48, 59);
            this.mFileBox.Name = "mFileBox";
            this.mFileBox.Size = new System.Drawing.Size(451, 19);
            this.mFileBox.TabIndex = 4;
            // 
            // mSelectButton
            // 
            this.mSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mSelectButton.Location = new System.Drawing.Point(505, 57);
            this.mSelectButton.Name = "mSelectButton";
            this.mSelectButton.Size = new System.Drawing.Size(75, 23);
            this.mSelectButton.TabIndex = 5;
            this.mSelectButton.Text = "&Select";
            this.mSelectButton.UseVisualStyleBackColor = true;
            this.mSelectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(218, 113);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 6;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(299, 113);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 7;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mAutoTitleCheckBox
            // 
            this.mAutoTitleCheckBox.AutoSize = true;
            this.mAutoTitleCheckBox.Location = new System.Drawing.Point(48, 37);
            this.mAutoTitleCheckBox.Name = "mAutoTitleCheckBox";
            this.mAutoTitleCheckBox.Size = new System.Drawing.Size(277, 16);
            this.mAutoTitleCheckBox.TabIndex = 2;
            this.mAutoTitleCheckBox.Text = "&Automatically create a title section with this title";
            this.mAutoTitleCheckBox.UseVisualStyleBackColor = true;
            // 
            // NewProject
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(592, 148);
            this.Controls.Add(this.mAutoTitleCheckBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mSelectButton);
            this.Controls.Add(this.mFileBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mTitleBox);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(8, 162);
            this.Name = "NewProject";
            this.Text = "Create a new project";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewProject_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mTitleBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mFileBox;
        private System.Windows.Forms.Button mSelectButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mAutoTitleCheckBox;
    }
}