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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditRoles));
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
            this.mCustomRolesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mCustomRolesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mCustomRolesList.FormattingEnabled = true;
            this.mCustomRolesList.ItemHeight = 16;
            this.mCustomRolesList.Location = new System.Drawing.Point(16, 64);
            this.mCustomRolesList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mCustomRolesList.Name = "mCustomRolesList";
            this.mCustomRolesList.Size = new System.Drawing.Size(382, 162);
            this.mCustomRolesList.TabIndex = 2;
            this.mCustomRolesList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mCustomRolesList_KeyUp);
            // 
            // mNewCustomRole
            // 
            this.mNewCustomRole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mNewCustomRole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNewCustomRole.Location = new System.Drawing.Point(16, 31);
            this.mNewCustomRole.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mNewCustomRole.Name = "mNewCustomRole";
            this.mNewCustomRole.Size = new System.Drawing.Size(382, 22);
            this.mNewCustomRole.TabIndex = 0;
            this.mNewCustomRole.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mNewCustomRole_KeyUp);
            // 
            // mInstructions
            // 
            this.mInstructions.AutoSize = true;
            this.mInstructions.Location = new System.Drawing.Point(16, 11);
            this.mInstructions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mInstructions.Name = "mInstructions";
            this.mInstructions.Size = new System.Drawing.Size(232, 16);
            this.mInstructions.TabIndex = 3;
            this.mInstructions.Text = "Add a new role or choose from the list.";
            // 
            // mOk
            // 
            this.mOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOk.Location = new System.Drawing.Point(157, 271);
            this.mOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mOk.Name = "mOk";
            this.mOk.Size = new System.Drawing.Size(100, 28);
            this.mOk.TabIndex = 4;
            this.mOk.Text = "&OK";
            this.mOk.UseVisualStyleBackColor = true;
            this.mOk.Click += new System.EventHandler(this.mOk_Click);
            // 
            // mCancel
            // 
            this.mCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancel.Location = new System.Drawing.Point(265, 271);
            this.mCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mCancel.Name = "mCancel";
            this.mCancel.Size = new System.Drawing.Size(100, 28);
            this.mCancel.TabIndex = 5;
            this.mCancel.Text = "&Cancel";
            this.mCancel.UseVisualStyleBackColor = true;
            this.mCancel.Click += new System.EventHandler(this.mCancel_Click);
            // 
            // mAdd
            // 
            this.mAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mAdd.Location = new System.Drawing.Point(407, 27);
            this.mAdd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mAdd.Name = "mAdd";
            this.mAdd.Size = new System.Drawing.Size(100, 28);
            this.mAdd.TabIndex = 1;
            this.mAdd.Text = "&Add";
            this.mAdd.UseVisualStyleBackColor = true;
            this.mAdd.Click += new System.EventHandler(this.mAdd_Click);
            // 
            // mRemove
            // 
            this.mRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRemove.Location = new System.Drawing.Point(407, 64);
            this.mRemove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mRemove.Name = "mRemove";
            this.mRemove.Size = new System.Drawing.Size(100, 28);
            this.mRemove.TabIndex = 3;
            this.mRemove.Text = "&Remove";
            this.mRemove.UseVisualStyleBackColor = true;
            this.mRemove.Click += new System.EventHandler(this.mRemove_Click);
            // 
            // EditRoles
            // 
            this.AcceptButton = this.mOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancel;
            this.ClientSize = new System.Drawing.Size(523, 314);
            this.Controls.Add(this.mRemove);
            this.Controls.Add(this.mAdd);
            this.Controls.Add(this.mCancel);
            this.Controls.Add(this.mOk);
            this.Controls.Add(this.mInstructions);
            this.Controls.Add(this.mNewCustomRole);
            this.Controls.Add(this.mCustomRolesList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
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