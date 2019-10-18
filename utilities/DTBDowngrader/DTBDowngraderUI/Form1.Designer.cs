namespace DTBDowngraderUI
    {
    partial class Form1
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog ();
            this.m_btnOpen = new System.Windows.Forms.Button ();
            this.m_btnSaveDir = new System.Windows.Forms.Button ();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog ();
            this.SuspendLayout ();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // m_btnOpen
            // 
            this.m_btnOpen.Location = new System.Drawing.Point ( 0, 0 );
            this.m_btnOpen.Name = "m_btnOpen";
            this.m_btnOpen.Size = new System.Drawing.Size ( 75, 23 );
            this.m_btnOpen.TabIndex = 0;
            this.m_btnOpen.Text = "Open";
            this.m_btnOpen.UseVisualStyleBackColor = true;
            this.m_btnOpen.Click += new System.EventHandler ( this.m_btnOpen_Click );
            // 
            // m_btnSaveDir
            // 
            this.m_btnSaveDir.Location = new System.Drawing.Point ( 0, 43 );
            this.m_btnSaveDir.Name = "m_btnSaveDir";
            this.m_btnSaveDir.Size = new System.Drawing.Size ( 75, 23 );
            this.m_btnSaveDir.TabIndex = 1;
            this.m_btnSaveDir.Text = "Save dir";
            this.m_btnSaveDir.UseVisualStyleBackColor = true;
            this.m_btnSaveDir.Click += new System.EventHandler ( this.m_btnSaveDir_Click );
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size ( 292, 273 );
            this.Controls.Add ( this.m_btnSaveDir );
            this.Controls.Add ( this.m_btnOpen );
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout ( false );

            }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button m_btnOpen;
        private System.Windows.Forms.Button m_btnSaveDir;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        }
    }

