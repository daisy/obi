namespace Obi.Dialogs
    {
    partial class AssignNewCustomRole
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose ();
                }
            base.Dispose ( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
            {
            this.m_lblCustomRoleName = new System.Windows.Forms.Label ();
            this.m_txtCustomRoleName = new System.Windows.Forms.TextBox ();
            this.m_btnOk = new System.Windows.Forms.Button ();
            this.m_btnCancel = new System.Windows.Forms.Button ();
            this.SuspendLayout ();
            // 
            // m_lblCustomRoleName
            // 
            this.m_lblCustomRoleName.AutoSize = true;
            this.m_lblCustomRoleName.Location = new System.Drawing.Point ( 10, 12 );
            this.m_lblCustomRoleName.Name = "m_lblCustomRoleName";
            this.m_lblCustomRoleName.Size = new System.Drawing.Size ( 91, 13 );
            this.m_lblCustomRoleName.TabIndex = 0;
            this.m_lblCustomRoleName.Text = "Custom role &name";
            // 
            // m_txtCustomRoleName
            // 
            this.m_txtCustomRoleName.Location = new System.Drawing.Point ( 100, 10 );
            this.m_txtCustomRoleName.Name = "m_txtCustomRoleName";
            this.m_txtCustomRoleName.Size = new System.Drawing.Size ( 130, 20 );
            this.m_txtCustomRoleName.TabIndex = 1;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.Location = new System.Drawing.Point ( 80, 50 );
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size ( 75, 23 );
            this.m_btnOk.TabIndex = 2;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler ( this.m_btnOk_Click );
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point ( 160, 50 );
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size ( 75, 23 );
            this.m_btnCancel.TabIndex = 3;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler ( this.m_btnCancel_Click );
            // 
            // AssignNewCustomRole
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size ( 232, 50 );
            this.Controls.Add ( this.m_btnCancel );
            this.Controls.Add ( this.m_btnOk );
            this.Controls.Add ( this.m_txtCustomRoleName );
            this.Controls.Add ( this.m_lblCustomRoleName );
            this.Name = "AssignNewCustomRole";
            this.Text = "Assign new custom role";
            this.ResumeLayout ( false );
            this.PerformLayout ();

            }

        #endregion

        private System.Windows.Forms.Label m_lblCustomRoleName;
        private System.Windows.Forms.TextBox m_txtCustomRoleName;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        }
    }