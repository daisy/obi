namespace Obi.UserControls
{
    partial class SectionStrip
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
            this.mLabel = new System.Windows.Forms.Label();
            this.mFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mLabel.Location = new System.Drawing.Point(3, 0);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(32, 16);
            this.mLabel.TabIndex = 0;
            this.mLabel.Text = "xxx";
            this.mLabel.Click += new System.EventHandler(this.SectionStrip_Click);
            // 
            // mFlowLayoutPanel
            // 
            this.mFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFlowLayoutPanel.AutoSize = true;
            this.mFlowLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mFlowLayoutPanel.Location = new System.Drawing.Point(2, 19);
            this.mFlowLayoutPanel.Name = "mFlowLayoutPanel";
            this.mFlowLayoutPanel.Size = new System.Drawing.Size(237, 10);
            this.mFlowLayoutPanel.TabIndex = 1;
            this.mFlowLayoutPanel.WrapContents = false;
            // 
            // mTextBox
            // 
            this.mTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTextBox.BackColor = System.Drawing.Color.LemonChiffon;
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTextBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mTextBox.Location = new System.Drawing.Point(3, 0);
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.Size = new System.Drawing.Size(235, 16);
            this.mTextBox.TabIndex = 2;
            this.mTextBox.Visible = false;
            this.mTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mTextBox_KeyDown);
            // 
            // SectionStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.Controls.Add(this.mTextBox);
            this.Controls.Add(this.mFlowLayoutPanel);
            this.Controls.Add(this.mLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Name = "SectionStrip";
            this.Size = new System.Drawing.Size(241, 32);
            this.Click += new System.EventHandler(this.SectionStrip_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.FlowLayoutPanel mFlowLayoutPanel;
        private System.Windows.Forms.TextBox mTextBox;
    }
}
