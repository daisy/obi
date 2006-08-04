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
            this.mFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mFlowLayoutPanel
            // 
            this.mFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFlowLayoutPanel.AutoSize = true;
            this.mFlowLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mFlowLayoutPanel.Location = new System.Drawing.Point(2, 25);
            this.mFlowLayoutPanel.Name = "mFlowLayoutPanel";
            this.mFlowLayoutPanel.Size = new System.Drawing.Size(237, 0);
            this.mFlowLayoutPanel.TabIndex = 2;
            this.mFlowLayoutPanel.WrapContents = false;
            this.mFlowLayoutPanel.Click += new System.EventHandler(this.mFlowLayoutPanel_Click);
            // 
            // mTextBox
            // 
            this.mTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTextBox.BackColor = System.Drawing.Color.LemonChiffon;
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mTextBox.Location = new System.Drawing.Point(3, 0);
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.ReadOnly = true;
            this.mTextBox.Size = new System.Drawing.Size(235, 19);
            this.mTextBox.TabIndex = 1;
            this.mTextBox.Click += new System.EventHandler(this.SectionStrip_Click);
            this.mTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mTextBox_MouseDown);
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
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Name = "SectionStrip";
            this.Size = new System.Drawing.Size(241, 28);
            this.Enter += new System.EventHandler(this.SectionStrip_enter);
            this.Click += new System.EventHandler(this.SectionStrip_Click);
            this.Leave += new System.EventHandler(this.SectionStrip_leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mFlowLayoutPanel;
        private System.Windows.Forms.TextBox mTextBox;
    }
}
