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
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Set page number for block:";
            // 
            // mNumberBox
            // 
            this.mNumberBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mNumberBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNumberBox.Location = new System.Drawing.Point(189, 13);
            this.mNumberBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mNumberBox.Name = "mNumberBox";
            this.mNumberBox.Size = new System.Drawing.Size(187, 22);
            this.mNumberBox.TabIndex = 1;
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOKButton.Location = new System.Drawing.Point(90, 137);
            this.mOKButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(100, 28);
            this.mOKButton.TabIndex = 4;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancelButton.Location = new System.Drawing.Point(198, 137);
            this.mCancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(100, 28);
            this.mCancelButton.TabIndex = 5;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mRenumber
            // 
            this.mRenumber.AutoSize = true;
            this.mRenumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRenumber.Location = new System.Drawing.Point(189, 73);
            this.mRenumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mRenumber.Name = "mRenumber";
            this.mRenumber.Size = new System.Drawing.Size(184, 20);
            this.mRenumber.TabIndex = 3;
            this.mRenumber.Text = "&Renumber following pages";
            this.mRenumber.UseVisualStyleBackColor = true;
            // 
            // mNumberOfPagesBox
            // 
            this.mNumberOfPagesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mNumberOfPagesBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNumberOfPagesBox.Location = new System.Drawing.Point(189, 43);
            this.mNumberOfPagesBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mNumberOfPagesBox.Name = "mNumberOfPagesBox";
            this.mNumberOfPagesBox.Size = new System.Drawing.Size(187, 22);
            this.mNumberOfPagesBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "&Number of pages:";
            // 
            // SetPageNumber
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(389, 178);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mNumberOfPagesBox);
            this.Controls.Add(this.mRenumber);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mNumberBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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