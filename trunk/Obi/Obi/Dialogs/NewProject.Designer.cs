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
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // mTitleBox
            // 
            this.mTitleBox.AccessibleDescription = null;
            resources.ApplyResources(this.mTitleBox, "mTitleBox");
            this.mTitleBox.BackgroundImage = null;
            this.mTitleBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTitleBox.Font = null;
            this.mTitleBox.Name = "mTitleBox";
            this.mTitleBox.TextChanged += new System.EventHandler(this.mTitleBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // mFileBox
            // 
            this.mFileBox.AccessibleDescription = null;
            resources.ApplyResources(this.mFileBox, "mFileBox");
            this.mFileBox.BackgroundImage = null;
            this.mFileBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mFileBox.Font = null;
            this.mFileBox.Name = "mFileBox";
            // 
            // mSelectButton
            // 
            this.mSelectButton.AccessibleDescription = null;
            this.mSelectButton.AccessibleName = null;
            resources.ApplyResources(this.mSelectButton, "mSelectButton");
            this.mSelectButton.BackgroundImage = null;
            this.mSelectButton.Font = null;
            this.mSelectButton.Name = "mSelectButton";
            this.mSelectButton.UseVisualStyleBackColor = true;
            this.mSelectButton.Click += new System.EventHandler(this.mSelectButton_Click);
            // 
            // mOKButton
            // 
            this.mOKButton.AccessibleDescription = null;
            this.mOKButton.AccessibleName = null;
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.BackgroundImage = null;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Font = null;
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.AccessibleDescription = null;
            this.mCancelButton.AccessibleName = null;
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.BackgroundImage = null;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Font = null;
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mAutoTitleCheckBox
            // 
            this.mAutoTitleCheckBox.AccessibleDescription = null;
            this.mAutoTitleCheckBox.AccessibleName = null;
            resources.ApplyResources(this.mAutoTitleCheckBox, "mAutoTitleCheckBox");
            this.mAutoTitleCheckBox.BackgroundImage = null;
            this.mAutoTitleCheckBox.Font = null;
            this.mAutoTitleCheckBox.Name = "mAutoTitleCheckBox";
            this.mAutoTitleCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // mIDBox
            // 
            this.mIDBox.AccessibleDescription = null;
            resources.ApplyResources(this.mIDBox, "mIDBox");
            this.mIDBox.BackgroundImage = null;
            this.mIDBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mIDBox.Font = null;
            this.mIDBox.Name = "mIDBox";
            // 
            // mGenerateIDButton
            // 
            this.mGenerateIDButton.AccessibleDescription = null;
            this.mGenerateIDButton.AccessibleName = null;
            resources.ApplyResources(this.mGenerateIDButton, "mGenerateIDButton");
            this.mGenerateIDButton.BackgroundImage = null;
            this.mGenerateIDButton.Font = null;
            this.mGenerateIDButton.Name = "mGenerateIDButton";
            this.mGenerateIDButton.UseVisualStyleBackColor = true;
            this.mGenerateIDButton.Click += new System.EventHandler(this.mGenerateIDButton_Click);
            // 
            // NewProject
            // 
            this.AcceptButton = this.mOKButton;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.mCancelButton;
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
            this.MaximizeBox = false;
            this.Name = "NewProject";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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