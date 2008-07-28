namespace Obi.Dialogs
{
    partial class SetPageNumber
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetPageNumber));
            this.label1 = new System.Windows.Forms.Label();
            this.mNumberBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mRenumber = new System.Windows.Forms.CheckBox();
            this.mNumberOfPagesBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            // mNumberBox
            // 
            this.mNumberBox.AccessibleDescription = null;
            this.mNumberBox.AccessibleName = null;
            resources.ApplyResources(this.mNumberBox, "mNumberBox");
            this.mNumberBox.BackgroundImage = null;
            this.mNumberBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNumberBox.Font = null;
            this.mNumberBox.Name = "mNumberBox";
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
            // mRenumber
            // 
            this.mRenumber.AccessibleDescription = null;
            this.mRenumber.AccessibleName = null;
            resources.ApplyResources(this.mRenumber, "mRenumber");
            this.mRenumber.BackgroundImage = null;
            this.mRenumber.Font = null;
            this.mRenumber.Name = "mRenumber";
            this.mRenumber.UseVisualStyleBackColor = true;
            // 
            // mNumberOfPagesBox
            // 
            this.mNumberOfPagesBox.AccessibleDescription = null;
            this.mNumberOfPagesBox.AccessibleName = null;
            resources.ApplyResources(this.mNumberOfPagesBox, "mNumberOfPagesBox");
            this.mNumberOfPagesBox.BackgroundImage = null;
            this.mNumberOfPagesBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNumberOfPagesBox.Font = null;
            this.mNumberOfPagesBox.Name = "mNumberOfPagesBox";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // SetPageNumber
            // 
            this.AcceptButton = this.mOKButton;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mNumberOfPagesBox);
            this.Controls.Add(this.mRenumber);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mNumberBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetPageNumber";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mNumberBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mRenumber;
        private System.Windows.Forms.TextBox mNumberOfPagesBox;
        private System.Windows.Forms.Label label2;
    }
}