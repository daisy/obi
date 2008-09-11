namespace Obi.Dialogs
    {
    partial class WelcomeDialog
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
            this.label1 = new System.Windows.Forms.Label ();
            this.m_btnNewProject = new System.Windows.Forms.Button ();
            this.m_btnOpenProject = new System.Windows.Forms.Button ();
            this.m_btnOpenBlank = new System.Windows.Forms.Button ();
            this.m_btnCancel = new System.Windows.Forms.Button ();
            this.SuspendLayout ();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point ( 10, 10 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size ( 181, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Please choose from following options";
            // 
            // m_btnNewProject
            // 
            this.m_btnNewProject.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnNewProject.Location = new System.Drawing.Point ( 50, 30 );
            this.m_btnNewProject.Name = "m_btnNewProject";
            this.m_btnNewProject.Size = new System.Drawing.Size ( 150, 23 );
            this.m_btnNewProject.TabIndex = 1;
            this.m_btnNewProject.Text = "Create &new project";
            this.m_btnNewProject.UseVisualStyleBackColor = true;
            this.m_btnNewProject.Click += new System.EventHandler ( this.m_btnNewProject_Click );
            // 
            // m_btnOpenProject
            // 
            this.m_btnOpenProject.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOpenProject.Location = new System.Drawing.Point ( 50, 70 );
            this.m_btnOpenProject.Name = "m_btnOpenProject";
            this.m_btnOpenProject.Size = new System.Drawing.Size ( 150, 23 );
            this.m_btnOpenProject.TabIndex = 2;
            this.m_btnOpenProject.Text = "&Open project";
            this.m_btnOpenProject.UseVisualStyleBackColor = true;
            this.m_btnOpenProject.Click += new System.EventHandler ( this.m_btnOpenProject_Click );
            // 
            // m_btnOpenBlank
            // 
            this.m_btnOpenBlank.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOpenBlank.Location = new System.Drawing.Point ( 50, 110 );
            this.m_btnOpenBlank.Name = "m_btnOpenBlank";
            this.m_btnOpenBlank.Size = new System.Drawing.Size ( 150, 23 );
            this.m_btnOpenBlank.TabIndex = 3;
            this.m_btnOpenBlank.Text = "Open &blank";
            this.m_btnOpenBlank.UseVisualStyleBackColor = true;
            this.m_btnOpenBlank.Click += new System.EventHandler ( this.m_btnOpenBlank_Click );
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point ( 50, 139 );
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size ( 75, 23 );
            this.m_btnCancel.TabIndex = 4;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler ( this.m_btnCancel_Click );
            // 
            // WelcomeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size ( 192, 273 );
            this.Controls.Add ( this.m_btnCancel );
            this.Controls.Add ( this.m_btnOpenBlank );
            this.Controls.Add ( this.m_btnOpenProject );
            this.Controls.Add ( this.m_btnNewProject );
            this.Controls.Add ( this.label1 );
            this.Name = "WelcomeDialog";
            this.Text = "Welcome to Obi";
            this.ResumeLayout ( false );
            this.PerformLayout ();

            }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button m_btnNewProject;
        private System.Windows.Forms.Button m_btnOpenProject;
        private System.Windows.Forms.Button m_btnOpenBlank;
        private System.Windows.Forms.Button m_btnCancel;
        }
    }