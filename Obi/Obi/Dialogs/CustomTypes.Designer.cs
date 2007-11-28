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
            this.mInstructions = new System.Windows.Forms.Label();
            this.mOk = new System.Windows.Forms.Button();
            this.mCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mCustomTypesList
            // 
            this.mCustomTypesList.FormattingEnabled = true;
            this.mCustomTypesList.Location = new System.Drawing.Point(6, 50);
            this.mCustomTypesList.Name = "mCustomTypesList";
            this.mCustomTypesList.Size = new System.Drawing.Size(179, 108);
            this.mCustomTypesList.TabIndex = 1;
            this.mCustomTypesList.SelectedIndexChanged += new System.EventHandler(this.mCustomTypesList_SelectedIndexChanged);
            this.mCustomTypesList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mCustomTypesList_KeyUp);
            // 
            // mNewCustomType
            // 
            this.mNewCustomType.Location = new System.Drawing.Point(6, 20);
            this.mNewCustomType.Name = "mNewCustomType";
            this.mNewCustomType.Size = new System.Drawing.Size(179, 20);
            this.mNewCustomType.TabIndex = 0;
            this.mNewCustomType.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mNewCustomType_KeyUp);
            // 
            // mInstructions
            // 
            this.mInstructions.AutoSize = true;
            this.mInstructions.Location = new System.Drawing.Point(3, 4);
            this.mInstructions.Name = "mInstructions";
            this.mInstructions.Size = new System.Drawing.Size(190, 13);
            this.mInstructions.TabIndex = 3;
            this.mInstructions.Text = "Add a new type or choose from the list.";
            // 
            // mOk
            // 
            this.mOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOk.Location = new System.Drawing.Point(11, 164);
            this.mOk.Name = "mOk";
            this.mOk.Size = new System.Drawing.Size(75, 23);
            this.mOk.TabIndex = 2;
            this.mOk.Text = "&OK";
            this.mOk.UseVisualStyleBackColor = true;
            this.mOk.Click += new System.EventHandler(this.mOk_Click);
            // 
            // mCancel
            // 
            this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancel.Location = new System.Drawing.Point(105, 164);
            this.mCancel.Name = "mCancel";
            this.mCancel.Size = new System.Drawing.Size(75, 23);
            this.mCancel.TabIndex = 3;
            this.mCancel.Text = "&Cancel";
            this.mCancel.UseVisualStyleBackColor = true;
            this.mCancel.Click += new System.EventHandler(this.mCancel_Click);
            // 
            // CustomTypes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(193, 189);
            this.Controls.Add(this.mCancel);
            this.Controls.Add(this.mOk);
            this.Controls.Add(this.mInstructions);
            this.Controls.Add(this.mNewCustomType);
            this.Controls.Add(this.mCustomTypesList);
            this.Name = "CustomTypes";
            this.Text = "Edit custom types";
            this.Load += new System.EventHandler(this.CustomTypes_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox mCustomTypesList;
        private System.Windows.Forms.TextBox mNewCustomType;
        private System.Windows.Forms.Label mInstructions;
        private System.Windows.Forms.Button mOk;
        private System.Windows.Forms.Button mCancel;
    }
}