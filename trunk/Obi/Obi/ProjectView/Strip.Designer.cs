namespace Obi.ProjectView
{
    partial class Strip
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
            this.mLabel = new Obi.ProjectView.EditableLabel();
            this.SuspendLayout();
            // 
            // mBlocksPanel
            // 
            this.mBlocksPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mBlocksPanel.BackColor = System.Drawing.Color.CornflowerBlue;
            this.mBlocksPanel.Location = new System.Drawing.Point(3, 25);
            this.mBlocksPanel.Name = "mBlocksPanel";
            this.mBlocksPanel.Size = new System.Drawing.Size(284, 104);
            this.mBlocksPanel.TabIndex = 0;
            this.mBlocksPanel.WrapContents = false;
            this.mBlocksPanel.Click += new System.EventHandler(this.Strip_Click);
            // 
            // mLabel
            // 
            this.mLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mLabel.BackColor = System.Drawing.Color.Thistle;
            this.mLabel.Editable = false;
            this.mLabel.FontSize = 8.25F;
            this.mLabel.Label = "*** NOT YET INITIALIZED ***";
            this.mLabel.Location = new System.Drawing.Point(3, 3);
            this.mLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.mLabel.MinimumSize = new System.Drawing.Size(150, 0);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(284, 21);
            this.mLabel.TabIndex = 0;
            this.mLabel.LabelEditedByUser += new System.EventHandler(this.Label_LabelEditedByUser);
            this.mLabel.EditableChanged += new System.EventHandler(this.Label_EditableChanged);
            this.mLabel.SizeChanged += new System.EventHandler(this.Label_SizeChanged);
            // 
            // Strip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.Controls.Add(this.mLabel);
            this.Controls.Add(this.mBlocksPanel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Name = "Strip";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(290, 132);
            this.Enter += new System.EventHandler(this.Strip_Enter);
            this.Click += new System.EventHandler(this.Strip_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mBlocksPanel;
        private EditableLabel mLabel;



    }
}
