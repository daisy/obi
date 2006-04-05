namespace Obi.UserControls
{
    partial class ContainerStrip
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
            this.mContainerPanel = new Obi.UserControls.ContainerPanel();
            this.SuspendLayout();
            // 
            // mContainerPanel
            // 
            this.mContainerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.mContainerPanel.Location = new System.Drawing.Point(11, 3);
            this.mContainerPanel.Name = "mContainerPanel";
            this.mContainerPanel.Size = new System.Drawing.Size(128, 144);
            this.mContainerPanel.TabIndex = 1;
            // 
            // ContainerStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Controls.Add(this.mContainerPanel);
            this.Name = "ContainerStrip";
            this.Controls.SetChildIndex(this.mContainerPanel, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ContainerPanel mContainerPanel;



    }
}
