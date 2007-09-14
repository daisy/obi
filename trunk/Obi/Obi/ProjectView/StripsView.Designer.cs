namespace Obi.ProjectView
{
    partial class StripsView
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
            this.mLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.editableLabel1 = new Obi.ProjectView.EditableLabel();
            this.SuspendLayout();
            // 
            // mLayoutPanel
            // 
            this.mLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mLayoutPanel.Name = "mLayoutPanel";
            this.mLayoutPanel.Size = new System.Drawing.Size(398, 297);
            this.mLayoutPanel.TabIndex = 0;
            this.mLayoutPanel.WrapContents = false;
            // 
            // editableLabel1
            // 
            this.editableLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.editableLabel1.BackColor = System.Drawing.Color.Thistle;
            this.editableLabel1.Editable = false;
            this.editableLabel1.Label = "label1";
            this.editableLabel1.Location = new System.Drawing.Point(0, 0);
            this.editableLabel1.Margin = new System.Windows.Forms.Padding(0);
            this.editableLabel1.Name = "editableLabel1";
            this.editableLabel1.Size = new System.Drawing.Size(398, 55);
            this.editableLabel1.TabIndex = 1;
            // 
            // StripsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.editableLabel1);
            this.Controls.Add(this.mLayoutPanel);
            this.Name = "StripsView";
            this.Size = new System.Drawing.Size(398, 297);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mLayoutPanel;
        private EditableLabel editableLabel1;
    }
}
