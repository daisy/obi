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
            this.label1 = new System.Windows.Forms.Label();
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
            this.mAudioLayoutPanel.Location = new System.Drawing.Point(3, 49);
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
            this.mTextBox.Location = new System.Drawing.Point(0, 0);
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.ReadOnly = true;
            this.mTextBox.ShortcutsEnabled = false;
            this.mTextBox.Size = new System.Drawing.Size(235, 19);
            this.mTextBox.TabIndex = 1;
            this.mTextBox.Visible = false;
            this.mTextBox.Click += new System.EventHandler(this.SectionStrip_Click);
            this.mTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mTextBox_MouseDown);
            this.mTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mTextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 20);
            this.label1.TabIndex = 3;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mStructureLayoutPanel
            // 
            this.mStructureLayoutPanel.AutoSize = true;
            this.mStructureLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mStructureLayoutPanel.Location = new System.Drawing.Point(3, 25);
            this.mStructureLayoutPanel.Name = "mStructureLayoutPanel";
            this.mStructureLayoutPanel.Size = new System.Drawing.Size(0, 18);
            this.mStructureLayoutPanel.TabIndex = 3;
            this.mStructureLayoutPanel.Click += new System.EventHandler(this.mStructureLayoutPanel_Click);
            // 
            // SectionStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Gold;
            this.Controls.Add(this.mStructureLayoutPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mTextBox);
            this.Controls.Add(this.mAudioLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Name = "SectionStrip";
            this.Size = new System.Drawing.Size(241, 52);
            this.Enter += new System.EventHandler(this.SectionStrip_enter);
            this.Click += new System.EventHandler(this.SectionStrip_Click);
            this.Leave += new System.EventHandler(this.SectionStrip_leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mAudioLayoutPanel;
        private System.Windows.Forms.TextBox mTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel mStructureLayoutPanel;
    }
}
