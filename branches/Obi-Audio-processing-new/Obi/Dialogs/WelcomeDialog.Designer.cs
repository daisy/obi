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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeDialog));
                this.mNewProjectButton = new System.Windows.Forms.Button();
                this.mOpenProjectButton = new System.Windows.Forms.Button();
                this.mOpenLastProjectButton = new System.Windows.Forms.Button();
                this.mOpenEmptyButton = new System.Windows.Forms.Button();
                this.mImportButton = new System.Windows.Forms.Button();
                this.mViewManualButton = new System.Windows.Forms.Button();
                this.helpProvider1 = new System.Windows.Forms.HelpProvider();
                this.SuspendLayout();
                // 
                // mNewProjectButton
                // 
                this.mNewProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
                resources.ApplyResources(this.mNewProjectButton, "mNewProjectButton");
                this.mNewProjectButton.Name = "mNewProjectButton";
                this.mNewProjectButton.UseVisualStyleBackColor = true;
                this.mNewProjectButton.Click += new System.EventHandler(this.mNewProjectButton_Click);
                // 
                // mOpenProjectButton
                // 
                this.mOpenProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
                resources.ApplyResources(this.mOpenProjectButton, "mOpenProjectButton");
                this.mOpenProjectButton.Name = "mOpenProjectButton";
                this.mOpenProjectButton.UseVisualStyleBackColor = true;
                this.mOpenProjectButton.Click += new System.EventHandler(this.mOpenProjectButton_Click);
                // 
                // mOpenLastProjectButton
                // 
                this.mOpenLastProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
                resources.ApplyResources(this.mOpenLastProjectButton, "mOpenLastProjectButton");
                this.mOpenLastProjectButton.Name = "mOpenLastProjectButton";
                this.mOpenLastProjectButton.UseVisualStyleBackColor = true;
                this.mOpenLastProjectButton.Click += new System.EventHandler(this.mOpenLastProjectButton_Click);
                // 
                // mOpenEmptyButton
                // 
                this.mOpenEmptyButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                resources.ApplyResources(this.mOpenEmptyButton, "mOpenEmptyButton");
                this.mOpenEmptyButton.Name = "mOpenEmptyButton";
                this.mOpenEmptyButton.UseVisualStyleBackColor = true;
                this.mOpenEmptyButton.Click += new System.EventHandler(this.mOpenEmptyButton_Click);
                // 
                // mImportButton
                // 
                this.mImportButton.DialogResult = System.Windows.Forms.DialogResult.OK;
                resources.ApplyResources(this.mImportButton, "mImportButton");
                this.mImportButton.Name = "mImportButton";
                this.mImportButton.UseVisualStyleBackColor = true;
                this.mImportButton.Click += new System.EventHandler(this.mImportButton_Click);
                // 
                // mViewManualButton
                // 
                this.mViewManualButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                resources.ApplyResources(this.mViewManualButton, "mViewManualButton");
                this.mViewManualButton.Name = "mViewManualButton";
                this.mViewManualButton.UseVisualStyleBackColor = true;
                this.mViewManualButton.Click += new System.EventHandler(this.mViewManualButton_Click);
                // 
                // helpProvider1
                // 
                resources.ApplyResources(this.helpProvider1, "helpProvider1");
                // 
                // WelcomeDialog
                // 
                resources.ApplyResources(this, "$this");
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.CancelButton = this.mOpenEmptyButton;
                this.Controls.Add(this.mViewManualButton);
                this.Controls.Add(this.mImportButton);
                this.Controls.Add(this.mOpenEmptyButton);
                this.Controls.Add(this.mOpenLastProjectButton);
                this.Controls.Add(this.mOpenProjectButton);
                this.Controls.Add(this.mNewProjectButton);
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "WelcomeDialog";
                this.ResumeLayout(false);

            }

        #endregion

            private System.Windows.Forms.Button mNewProjectButton;
        private System.Windows.Forms.Button mOpenProjectButton;
        private System.Windows.Forms.Button mOpenLastProjectButton;
        private System.Windows.Forms.Button mOpenEmptyButton;
        private System.Windows.Forms.Button mImportButton;
        private System.Windows.Forms.Button mViewManualButton;
        private System.Windows.Forms.HelpProvider helpProvider1;
        }
    }