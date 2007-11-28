namespace Obi.ProjectView
{
    partial class ContainerBlock : Block
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mBlocksPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // mBlocksPanel
            // 
            this.mBlocksPanel.BackColor = System.Drawing.Color.Violet;
            this.mBlocksPanel.Location = new System.Drawing.Point(3, 18);
            this.mBlocksPanel.Name = "mBlocksPanel";
            this.mBlocksPanel.Size = new System.Drawing.Size(98, 83);
            this.mBlocksPanel.TabIndex = 4;
            // 
            // ContainerBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.mBlocksPanel);
            this.Name = "ContainerBlock";
            this.Size = new System.Drawing.Size(104, 107);
            this.Controls.SetChildIndex(this.mBlocksPanel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mBlocksPanel;

    }
}
