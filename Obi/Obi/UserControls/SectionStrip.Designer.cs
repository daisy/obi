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
            this.mAudioLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.mLabel = new System.Windows.Forms.Label();
            this.mStructureLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // mAudioLayoutPanel
            // 
            this.mAudioLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mAudioLayoutPanel.AutoSize = true;
            this.mAudioLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mAudioLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mAudioLayoutPanel.Location = new System.Drawing.Point(3, 45);
            this.mAudioLayoutPanel.Name = "mAudioLayoutPanel";
            this.mAudioLayoutPanel.Size = new System.Drawing.Size(0, 0);
            this.mAudioLayoutPanel.TabIndex = 2;
            this.mAudioLayoutPanel.WrapContents = false;
            this.mAudioLayoutPanel.Click += new System.EventHandler(this.mAudioLayoutPanel_Click);
            // 
            // mTextBox
            // 
            this.mTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTextBox.BackColor = System.Drawing.Color.Gold;
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.mTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mTextBox.Location = new System.Drawing.Point(3, 0);
            this.mTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.ReadOnly = true;
            this.mTextBox.ShortcutsEnabled = false;
            this.mTextBox.Size = new System.Drawing.Size(243, 19);
            this.mTextBox.TabIndex = 1;
            this.mTextBox.Visible = false;
            this.mTextBox.Click += new System.EventHandler(this.SectionStrip_Click);
            this.mTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mTextBox_MouseDown);
            this.mTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mTextBox_KeyDown);
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel.Location = new System.Drawing.Point(3, 0);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(0, 20);
            this.mLabel.TabIndex = 3;
            this.mLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mLabel.Click += new System.EventHandler(this.SectionStrip_Click);
            // 
            // mStructureLayoutPanel
            // 
            this.mStructureLayoutPanel.AutoSize = true;
            this.mStructureLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mStructureLayoutPanel.Location = new System.Drawing.Point(3, 23);
            this.mStructureLayoutPanel.Name = "mStructureLayoutPanel";
            this.mStructureLayoutPanel.Size = new System.Drawing.Size(0, 17);
            this.mStructureLayoutPanel.TabIndex = 3;
            this.mStructureLayoutPanel.Click += new System.EventHandler(this.mStructureLayoutPanel_Click);
            // 
            // SectionStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Gold;
            this.Controls.Add(this.mStructureLayoutPanel);
            this.Controls.Add(this.mLabel);
            this.Controls.Add(this.mTextBox);
            this.Controls.Add(this.mAudioLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Name = "SectionStrip";
            this.Size = new System.Drawing.Size(249, 48);
            this.Enter += new System.EventHandler(this.SectionStrip_enter);
            this.Click += new System.EventHandler(this.SectionStrip_Click);
            this.Leave += new System.EventHandler(this.SectionStrip_leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mAudioLayoutPanel;
        private System.Windows.Forms.TextBox mTextBox;
        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.FlowLayoutPanel mStructureLayoutPanel;
    }
}
