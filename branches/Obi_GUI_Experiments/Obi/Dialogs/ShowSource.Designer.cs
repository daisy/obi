namespace Obi.Dialogs
{
    partial class ShowSource
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowSource));
            this.sourceBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sourceBox
            // 
            this.sourceBox.AccessibleDescription = null;
            this.sourceBox.AccessibleName = null;
            resources.ApplyResources(this.sourceBox, "sourceBox");
            this.sourceBox.BackColor = System.Drawing.Color.White;
            this.sourceBox.BackgroundImage = null;
            this.sourceBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sourceBox.Name = "sourceBox";
            this.sourceBox.ReadOnly = true;
            // 
            // ShowSource
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.sourceBox);
            this.Font = null;
            this.Name = "ShowSource";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SourceView_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox sourceBox;

    }
}