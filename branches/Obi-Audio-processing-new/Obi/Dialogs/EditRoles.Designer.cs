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
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // mCustomRolesList
            // 
            resources.ApplyResources(this.mCustomRolesList, "mCustomRolesList");
            this.mCustomRolesList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mCustomRolesList.FormattingEnabled = true;
            this.mCustomRolesList.Name = "mCustomRolesList";
            this.mCustomRolesList.SelectedIndexChanged += new System.EventHandler(this.mCustomRolesList_SelectedIndexChanged);
            this.mCustomRolesList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mCustomRolesList_KeyUp);
            // 
            // mNewCustomRole
            // 
            resources.ApplyResources(this.mNewCustomRole, "mNewCustomRole");
            this.mNewCustomRole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNewCustomRole.Name = "mNewCustomRole";
            this.mNewCustomRole.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mNewCustomRole_KeyUp);
            // 
            // mInstructions
            // 
            resources.ApplyResources(this.mInstructions, "mInstructions");
            this.mInstructions.Name = "mInstructions";
            // 
            // mOk
            // 
            resources.ApplyResources(this.mOk, "mOk");
            this.mOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOk.Name = "mOk";
            this.mOk.UseVisualStyleBackColor = true;
            this.mOk.Click += new System.EventHandler(this.mOk_Click);
            // 
            // mCancel
            // 
            resources.ApplyResources(this.mCancel, "mCancel");
            this.mCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancel.Name = "mCancel";
            this.mCancel.UseVisualStyleBackColor = true;
            this.mCancel.Click += new System.EventHandler(this.mCancel_Click);
            // 
            // mAdd
            // 
            resources.ApplyResources(this.mAdd, "mAdd");
            this.mAdd.Name = "mAdd";
            this.mAdd.UseVisualStyleBackColor = true;
            this.mAdd.Click += new System.EventHandler(this.mAdd_Click);
            // 
            // mRemove
            // 
            resources.ApplyResources(this.mRemove, "mRemove");
            this.mRemove.Name = "mRemove";
            this.mRemove.UseVisualStyleBackColor = true;
            this.mRemove.Click += new System.EventHandler(this.mRemove_Click);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // EditRoles
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}