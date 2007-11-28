namespace Obi.Dialogs
{
    partial class CustomTypes
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
            this.mCustomTypesList = new System.Windows.Forms.ListBox();
            this.mNewCustomType = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mAddButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mCustomTypesList
            // 
            this.mCustomTypesList.FormattingEnabled = true;
            this.mCustomTypesList.Location = new System.Drawing.Point(12, 50);
            this.mCustomTypesList.Name = "mCustomTypesList";
            this.mCustomTypesList.Size = new System.Drawing.Size(179, 108);
            this.mCustomTypesList.TabIndex = 0;
            this.mCustomTypesList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mCustomClassesList_KeyUp);
            // 
            // mNewCustomType
            // 
            this.mNewCustomType.Location = new System.Drawing.Point(11, 14);
            this.mNewCustomType.Name = "mNewCustomType";
            this.mNewCustomType.Size = new System.Drawing.Size(132, 20);
            this.mNewCustomType.TabIndex = 1;
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(68, 167);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 25);
            this.mOKButton.TabIndex = 3;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mAddButton
            // 
            this.mAddButton.Location = new System.Drawing.Point(151, 12);
            this.mAddButton.Name = "mAddButton";
            this.mAddButton.Size = new System.Drawing.Size(40, 23);
            this.mAddButton.TabIndex = 2;
            this.mAddButton.Text = "Add";
            this.mAddButton.UseVisualStyleBackColor = true;
            this.mAddButton.Click += new System.EventHandler(this.mAddButton_Click);
            // 
            // CustomTypes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 197);
            this.Controls.Add(this.mAddButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mNewCustomType);
            this.Controls.Add(this.mCustomTypesList);
            this.Name = "CustomTypes";
            this.Text = "Edit custom types";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox mCustomTypesList;
        private System.Windows.Forms.TextBox mNewCustomType;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mAddButton;
    }
}