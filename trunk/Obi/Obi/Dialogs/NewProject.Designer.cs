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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProject));
            this.label1 = new System.Windows.Forms.Label();
            this.mTitleBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mFileBox = new System.Windows.Forms.TextBox();
            this.mSelectButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mAutoTitleCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mIDBox = new System.Windows.Forms.TextBox();
            this.mGenerateIDButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Title:";
            // 
            // mTitleBox
            // 
            this.mTitleBox.AccessibleName = "Title:";
            this.mTitleBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTitleBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTitleBox.Location = new System.Drawing.Point(73, 13);
            this.mTitleBox.Name = "mTitleBox";
            this.mTitleBox.Size = new System.Drawing.Size(507, 20);
            this.mTitleBox.TabIndex = 1;
            this.mTitleBox.TextChanged += new System.EventHandler(this.mTitleBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Location:";
            // 
            // mFileBox
            // 
            this.mFileBox.AccessibleName = "Location:";
            this.mFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFileBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mFileBox.Location = new System.Drawing.Point(73, 97);
            this.mFileBox.Name = "mFileBox";
            this.mFileBox.Size = new System.Drawing.Size(426, 20);
            this.mFileBox.TabIndex = 5;
            // 
            // mSelectButton
            // 
            this.mSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mSelectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mSelectButton.Location = new System.Drawing.Point(505, 93);
            this.mSelectButton.Name = "mSelectButton";
            this.mSelectButton.Size = new System.Drawing.Size(75, 25);
            this.mSelectButton.TabIndex = 6;
            this.mSelectButton.Text = "&Select";
            this.mSelectButton.UseVisualStyleBackColor = true;
            this.mSelectButton.Click += new System.EventHandler(this.mSelectButton_Click);
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOKButton.Location = new System.Drawing.Point(218, 154);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 25);
            this.mOKButton.TabIndex = 7;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancelButton.Location = new System.Drawing.Point(299, 154);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 25);
            this.mCancelButton.TabIndex = 8;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mAutoTitleCheckBox
            // 
            this.mAutoTitleCheckBox.AutoSize = true;
            this.mAutoTitleCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mAutoTitleCheckBox.Location = new System.Drawing.Point(73, 39);
            this.mAutoTitleCheckBox.Name = "mAutoTitleCheckBox";
            this.mAutoTitleCheckBox.Size = new System.Drawing.Size(243, 17);
            this.mAutoTitleCheckBox.TabIndex = 2;
            this.mAutoTitleCheckBox.Text = "&Automatically create a title section with this title";
            this.mAutoTitleCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "&ID:";
            // 
            // mIDBox
            // 
            this.mIDBox.AccessibleName = "ID:";
            this.mIDBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mIDBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mIDBox.Location = new System.Drawing.Point(73, 66);
            this.mIDBox.Name = "mIDBox";
            this.mIDBox.Size = new System.Drawing.Size(426, 20);
            this.mIDBox.TabIndex = 3;
            // 
            // mGenerateIDButton
            // 
            this.mGenerateIDButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mGenerateIDButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mGenerateIDButton.Location = new System.Drawing.Point(505, 62);
            this.mGenerateIDButton.Name = "mGenerateIDButton";
            this.mGenerateIDButton.Size = new System.Drawing.Size(75, 25);
            this.mGenerateIDButton.TabIndex = 4;
            this.mGenerateIDButton.Text = "&Generate";
            this.mGenerateIDButton.UseVisualStyleBackColor = true;
            this.mGenerateIDButton.Click += new System.EventHandler(this.mGenerateIDButton_Click);
            // 
            // NewProject
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(592, 191);
            this.Controls.Add(this.mGenerateIDButton);
            this.Controls.Add(this.mIDBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mAutoTitleCheckBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mSelectButton);
            this.Controls.Add(this.mFileBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mTitleBox);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(336, 197);
            this.Name = "NewProject";
            this.ShowInTaskbar = false;
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mIDBox;
        private System.Windows.Forms.Button mGenerateIDButton;
    }
}