namespace Obi.Dialogs
{
    partial class AdvancePreferencesSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancePreferencesSettings));
            this.m_CheckBoxListView = new System.Windows.Forms.ListView();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_tbDialogDesc = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_CheckBoxListView
            // 
            resources.ApplyResources(this.m_CheckBoxListView, "m_CheckBoxListView");
            this.m_CheckBoxListView.CheckBoxes = true;
            this.m_CheckBoxListView.Name = "m_CheckBoxListView";
            this.m_CheckBoxListView.UseCompatibleStateImageBehavior = false;
            this.m_CheckBoxListView.View = System.Windows.Forms.View.List;
            // 
            // m_btnOk
            // 
            this.m_btnOk.AccessibleDescription = global::Obi.messages.Executing__0_;
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.AccessibleDescription = global::Obi.messages.Executing__0_;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // m_tbDialogDesc
            // 
            resources.ApplyResources(this.m_tbDialogDesc, "m_tbDialogDesc");
            this.m_tbDialogDesc.Name = "m_tbDialogDesc";
            this.m_tbDialogDesc.ReadOnly = true;
            // 
            // AdvancePreferencesSettings
            // 
            this.AcceptButton = this.m_btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.m_tbDialogDesc);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_CheckBoxListView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvancePreferencesSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView m_CheckBoxListView;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.TextBox m_tbDialogDesc;




    }
}