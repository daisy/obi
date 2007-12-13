namespace Obi.Dialogs
{
    partial class EditRoles
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
            this.mCustomRolesList = new System.Windows.Forms.ListBox();
            this.mNewCustomRole = new System.Windows.Forms.TextBox();
            this.mInstructions = new System.Windows.Forms.Label();
            this.mOk = new System.Windows.Forms.Button();
            this.mCancel = new System.Windows.Forms.Button();
            this.mAdd = new System.Windows.Forms.Button();
            this.mRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mCustomRolesList
            // 
            this.mCustomRolesList.FormattingEnabled = true;
            this.mCustomRolesList.Location = new System.Drawing.Point(6, 50);
            this.mCustomRolesList.Name = "mCustomRolesList";
            this.mCustomRolesList.Size = new System.Drawing.Size(179, 108);
            this.mCustomRolesList.TabIndex = 2;
            this.mCustomRolesList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mCustomRolesList_KeyUp);
            // 
            // mNewCustomRole
            // 
            this.mNewCustomRole.Location = new System.Drawing.Point(6, 20);
            this.mNewCustomRole.Name = "mNewCustomRole";
            this.mNewCustomRole.Size = new System.Drawing.Size(179, 20);
            this.mNewCustomRole.TabIndex = 0;
            this.mNewCustomRole.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mNewCustomRole_KeyUp);
            // 
            // mInstructions
            // 
            this.mInstructions.AutoSize = true;
            this.mInstructions.Location = new System.Drawing.Point(3, 4);
            this.mInstructions.Name = "mInstructions";
            this.mInstructions.Size = new System.Drawing.Size(187, 13);
            this.mInstructions.TabIndex = 3;
            this.mInstructions.Text = "Add a new role or choose from the list.";
            // 
            // mOk
            // 
            this.mOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOk.Location = new System.Drawing.Point(11, 164);
            this.mOk.Name = "mOk";
            this.mOk.Size = new System.Drawing.Size(75, 23);
            this.mOk.TabIndex = 4;
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
            this.mCancel.TabIndex = 5;
            this.mCancel.Text = "&Cancel";
            this.mCancel.UseVisualStyleBackColor = true;
            this.mCancel.Click += new System.EventHandler(this.mCancel_Click);
            // 
            // mAdd
            // 
            this.mAdd.Location = new System.Drawing.Point(205, 20);
            this.mAdd.Name = "mAdd";
            this.mAdd.Size = new System.Drawing.Size(75, 23);
            this.mAdd.TabIndex = 1;
            this.mAdd.Text = "&Add";
            this.mAdd.UseVisualStyleBackColor = true;
            this.mAdd.Click += new System.EventHandler(this.mAdd_Click);
            // 
            // mRemove
            // 
            this.mRemove.Location = new System.Drawing.Point(205, 50);
            this.mRemove.Name = "mRemove";
            this.mRemove.Size = new System.Drawing.Size(75, 23);
            this.mRemove.TabIndex = 3;
            this.mRemove.Text = "&Remove";
            this.mRemove.UseVisualStyleBackColor = true;
            this.mRemove.Click += new System.EventHandler(this.mRemove_Click);
            // 
            // EditRoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 189);
            this.Controls.Add(this.mRemove);
            this.Controls.Add(this.mAdd);
            this.Controls.Add(this.mCancel);
            this.Controls.Add(this.mOk);
            this.Controls.Add(this.mInstructions);
            this.Controls.Add(this.mNewCustomRole);
            this.Controls.Add(this.mCustomRolesList);
            this.MaximizeBox = false;
            this.Name = "EditRoles";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Edit roles";
            this.Load += new System.EventHandler(this.CustomRoles_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox mCustomRolesList;
        private System.Windows.Forms.TextBox mNewCustomRole;
        private System.Windows.Forms.Label mInstructions;
        private System.Windows.Forms.Button mOk;
        private System.Windows.Forms.Button mCancel;
        private System.Windows.Forms.Button mAdd;
        private System.Windows.Forms.Button mRemove;
    }
}