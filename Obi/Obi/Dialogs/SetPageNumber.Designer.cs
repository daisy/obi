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
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Set page number for block:";
            // 
            // mNumberBox
            // 
            this.mNumberBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNumberBox.Location = new System.Drawing.Point(153, 12);
            this.mNumberBox.Name = "mNumberBox";
            this.mNumberBox.Size = new System.Drawing.Size(127, 20);
            this.mNumberBox.TabIndex = 1;
            // 
            // mOKButton
            // 
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOKButton.Location = new System.Drawing.Point(123, 116);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 4;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancelButton.Location = new System.Drawing.Point(204, 116);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 5;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mRenumber
            // 
            this.mRenumber.AutoSize = true;
            this.mRenumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRenumber.Location = new System.Drawing.Point(12, 64);
            this.mRenumber.Name = "mRenumber";
            this.mRenumber.Size = new System.Drawing.Size(148, 17);
            this.mRenumber.TabIndex = 3;
            this.mRenumber.Text = "&Renumber following pages";
            this.mRenumber.UseVisualStyleBackColor = true;
            // 
            // mNumberOfPagesBox
            // 
            this.mNumberOfPagesBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNumberOfPagesBox.Location = new System.Drawing.Point(153, 38);
            this.mNumberOfPagesBox.Name = "mNumberOfPagesBox";
            this.mNumberOfPagesBox.Size = new System.Drawing.Size(127, 20);
            this.mNumberOfPagesBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "&Number of pages:";
            // 
            // SetPageNumber
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(292, 151);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mNumberOfPagesBox);
            this.Controls.Add(this.mRenumber);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mNumberBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetPageNumber";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Set page number";
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