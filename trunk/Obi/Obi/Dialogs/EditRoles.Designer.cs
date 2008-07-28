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
            this.mCustomRolesList.AccessibleDescription = null;
            this.mCustomRolesList.AccessibleName = null;
            resources.ApplyResources(this.mCustomRolesList, "mCustomRolesList");
            this.mCustomRolesList.BackgroundImage = null;
            this.mCustomRolesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mCustomRolesList.Font = null;
            this.mCustomRolesList.FormattingEnabled = true;
            this.mCustomRolesList.Name = "mCustomRolesList";
            this.mCustomRolesList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mCustomRolesList_KeyUp);
            // 
            // mNewCustomRole
            // 
            this.mNewCustomRole.AccessibleDescription = null;
            this.mNewCustomRole.AccessibleName = null;
            resources.ApplyResources(this.mNewCustomRole, "mNewCustomRole");
            this.mNewCustomRole.BackgroundImage = null;
            this.mNewCustomRole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNewCustomRole.Font = null;
            this.mNewCustomRole.Name = "mNewCustomRole";
            this.mNewCustomRole.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mNewCustomRole_KeyUp);
            // 
            // mInstructions
            // 
            this.mInstructions.AccessibleDescription = null;
            this.mInstructions.AccessibleName = null;
            resources.ApplyResources(this.mInstructions, "mInstructions");
            this.mInstructions.Font = null;
            this.mInstructions.Name = "mInstructions";
            // 
            // mOk
            // 
            this.mOk.AccessibleDescription = null;
            this.mOk.AccessibleName = null;
            resources.ApplyResources(this.mOk, "mOk");
            this.mOk.BackgroundImage = null;
            this.mOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOk.Font = null;
            this.mOk.Name = "mOk";
            this.mOk.UseVisualStyleBackColor = true;
            this.mOk.Click += new System.EventHandler(this.mOk_Click);
            // 
            // mCancel
            // 
            this.mCancel.AccessibleDescription = null;
            this.mCancel.AccessibleName = null;
            resources.ApplyResources(this.mCancel, "mCancel");
            this.mCancel.BackgroundImage = null;
            this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancel.Font = null;
            this.mCancel.Name = "mCancel";
            this.mCancel.UseVisualStyleBackColor = true;
            this.mCancel.Click += new System.EventHandler(this.mCancel_Click);
            // 
            // mAdd
            // 
            this.mAdd.AccessibleDescription = null;
            this.mAdd.AccessibleName = null;
            resources.ApplyResources(this.mAdd, "mAdd");
            this.mAdd.BackgroundImage = null;
            this.mAdd.Font = null;
            this.mAdd.Name = "mAdd";
            this.mAdd.UseVisualStyleBackColor = true;
            this.mAdd.Click += new System.EventHandler(this.mAdd_Click);
            // 
            // mRemove
            // 
            this.mRemove.AccessibleDescription = null;
            this.mRemove.AccessibleName = null;
            resources.ApplyResources(this.mRemove, "mRemove");
            this.mRemove.BackgroundImage = null;
            this.mRemove.Font = null;
            this.mRemove.Name = "mRemove";
            this.mRemove.UseVisualStyleBackColor = true;
            this.mRemove.Click += new System.EventHandler(this.mRemove_Click);
            // 
            // EditRoles
            // 
            this.AcceptButton = this.mOk;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.mCancel;
            this.Controls.Add(this.mRemove);
            this.Controls.Add(this.mAdd);
            this.Controls.Add(this.mCancel);
            this.Controls.Add(this.mOk);
            this.Controls.Add(this.mInstructions);
            this.Controls.Add(this.mNewCustomRole);
            this.Controls.Add(this.mCustomRolesList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditRoles";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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