namespace PipelineInterface
{
    partial class PipelineInterfaceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PipelineInterfaceForm));
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // mOKButton
            // 
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mLayoutPanel
            // 
            resources.ApplyResources(this.mLayoutPanel, "mLayoutPanel");
            this.mLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.mLayoutPanel.Name = "mLayoutPanel";
            // 
            // PipelineInterfaceForm
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.mLayoutPanel);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PipelineInterfaceForm";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.PipelineInterfaceForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.FlowLayoutPanel mLayoutPanel;
    }
}