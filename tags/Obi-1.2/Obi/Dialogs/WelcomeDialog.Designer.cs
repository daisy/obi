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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( WelcomeDialog ) );
            this.mNewProjectButton = new System.Windows.Forms.Button ();
            this.mOpenProjectButton = new System.Windows.Forms.Button ();
            this.mOpenLastProjectButton = new System.Windows.Forms.Button ();
            this.mOpenEmptyButton = new System.Windows.Forms.Button ();
            this.mImportButton = new System.Windows.Forms.Button ();
            this.mViewManualButton = new System.Windows.Forms.Button ();
            this.SuspendLayout ();
            // 
            // mNewProjectButton
            // 
            this.mNewProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mNewProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNewProjectButton.Location = new System.Drawing.Point ( 13, 13 );
            this.mNewProjectButton.Margin = new System.Windows.Forms.Padding ( 4 );
            this.mNewProjectButton.Name = "mNewProjectButton";
            this.mNewProjectButton.Size = new System.Drawing.Size ( 230, 28 );
            this.mNewProjectButton.TabIndex = 1;
            this.mNewProjectButton.Text = "Create a &new project";
            this.mNewProjectButton.UseVisualStyleBackColor = true;
            this.mNewProjectButton.Click += new System.EventHandler ( this.mNewProjectButton_Click );
            // 
            // mOpenProjectButton
            // 
            this.mOpenProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOpenProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOpenProjectButton.Location = new System.Drawing.Point ( 13, 85 );
            this.mOpenProjectButton.Margin = new System.Windows.Forms.Padding ( 4 );
            this.mOpenProjectButton.Name = "mOpenProjectButton";
            this.mOpenProjectButton.Size = new System.Drawing.Size ( 230, 28 );
            this.mOpenProjectButton.TabIndex = 3;
            this.mOpenProjectButton.Text = "&Open an existing project";
            this.mOpenProjectButton.UseVisualStyleBackColor = true;
            this.mOpenProjectButton.Click += new System.EventHandler ( this.mOpenProjectButton_Click );
            // 
            // mOpenLastProjectButton
            // 
            this.mOpenLastProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOpenLastProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOpenLastProjectButton.Location = new System.Drawing.Point ( 13, 121 );
            this.mOpenLastProjectButton.Margin = new System.Windows.Forms.Padding ( 4 );
            this.mOpenLastProjectButton.Name = "mOpenLastProjectButton";
            this.mOpenLastProjectButton.Size = new System.Drawing.Size ( 230, 28 );
            this.mOpenLastProjectButton.TabIndex = 4;
            this.mOpenLastProjectButton.Text = "Open &last project";
            this.mOpenLastProjectButton.UseVisualStyleBackColor = true;
            this.mOpenLastProjectButton.Click += new System.EventHandler ( this.mOpenLastProjectButton_Click );
            // 
            // mOpenEmptyButton
            // 
            this.mOpenEmptyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mOpenEmptyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOpenEmptyButton.Location = new System.Drawing.Point ( 13, 157 );
            this.mOpenEmptyButton.Margin = new System.Windows.Forms.Padding ( 4 );
            this.mOpenEmptyButton.Name = "mOpenEmptyButton";
            this.mOpenEmptyButton.Size = new System.Drawing.Size ( 230, 28 );
            this.mOpenEmptyButton.TabIndex = 5;
            this.mOpenEmptyButton.Text = "Op&en Obi with no project";
            this.mOpenEmptyButton.UseVisualStyleBackColor = true;
            this.mOpenEmptyButton.Click += new System.EventHandler ( this.mOpenEmptyButton_Click );
            // 
            // mImportButton
            // 
            this.mImportButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mImportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mImportButton.Location = new System.Drawing.Point ( 13, 49 );
            this.mImportButton.Margin = new System.Windows.Forms.Padding ( 4 );
            this.mImportButton.Name = "mImportButton";
            this.mImportButton.Size = new System.Drawing.Size ( 230, 28 );
            this.mImportButton.TabIndex = 2;
            this.mImportButton.Text = "Create a new project from &import";
            this.mImportButton.UseVisualStyleBackColor = true;
            this.mImportButton.Click += new System.EventHandler ( this.mImportButton_Click );
            // 
            // mViewManualButton
            // 
            this.mViewManualButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mViewManualButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mViewManualButton.Location = new System.Drawing.Point ( 13, 193 );
            this.mViewManualButton.Margin = new System.Windows.Forms.Padding ( 4 );
            this.mViewManualButton.Name = "mViewManualButton";
            this.mViewManualButton.Size = new System.Drawing.Size ( 230, 28 );
            this.mViewManualButton.TabIndex = 6;
            this.mViewManualButton.Text = "View the &manual";
            this.mViewManualButton.UseVisualStyleBackColor = true;
            this.mViewManualButton.Click += new System.EventHandler ( this.mViewManualButton_Click );
            // 
            // WelcomeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF ( 8F, 16F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mOpenEmptyButton;
            this.ClientSize = new System.Drawing.Size ( 246, 211 );
            this.Controls.Add ( this.mViewManualButton );
            this.Controls.Add ( this.mImportButton );
            this.Controls.Add ( this.mOpenEmptyButton );
            this.Controls.Add ( this.mOpenLastProjectButton );
            this.Controls.Add ( this.mOpenProjectButton );
            this.Controls.Add ( this.mNewProjectButton );
            this.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
            this.Icon = ((System.Drawing.Icon)(resources.GetObject ( "$this.Icon" )));
            this.Margin = new System.Windows.Forms.Padding ( 4 );
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size ( 264, 261 );
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size ( 264, 261 );
            this.Name = "WelcomeDialog";
            this.Text = "Welcome to Obi";
            this.ResumeLayout ( false );

            }

        #endregion

            private System.Windows.Forms.Button mNewProjectButton;
        private System.Windows.Forms.Button mOpenProjectButton;
        private System.Windows.Forms.Button mOpenLastProjectButton;
        private System.Windows.Forms.Button mOpenEmptyButton;
        private System.Windows.Forms.Button mImportButton;
        private System.Windows.Forms.Button mViewManualButton;
        }
    }