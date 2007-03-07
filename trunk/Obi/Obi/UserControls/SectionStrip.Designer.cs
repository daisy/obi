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
            this.components = new System.ComponentModel.Container();
            this.mAudioLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mRenameBox = new System.Windows.Forms.TextBox();
            this.mLabel = new System.Windows.Forms.Label();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mAnnotationLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // mAudioLayoutPanel
            // 
            this.mAudioLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mAudioLayoutPanel.AutoSize = true;
            this.mAudioLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mAudioLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.mAudioLayoutPanel.Location = new System.Drawing.Point(7, 33);
            this.mAudioLayoutPanel.Name = "mAudioLayoutPanel";
            this.mAudioLayoutPanel.Size = new System.Drawing.Size(0, 0);
            this.mAudioLayoutPanel.TabIndex = 3;
            this.mAudioLayoutPanel.WrapContents = false;
            this.mAudioLayoutPanel.Click += new System.EventHandler(this.mAudioLayoutPanel_Click);
            // 
            // mRenameBox
            // 
            this.mRenameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mRenameBox.BackColor = System.Drawing.Color.Gold;
            this.mRenameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mRenameBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.mRenameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mRenameBox.Location = new System.Drawing.Point(7, 4);
            this.mRenameBox.Name = "mRenameBox";
            this.mRenameBox.ShortcutsEnabled = false;
            this.mRenameBox.Size = new System.Drawing.Size(0, 19);
            this.mRenameBox.TabIndex = 1;
            this.mRenameBox.Visible = false;
            this.mRenameBox.Enter += new System.EventHandler(this.mRenameBox_Enter);
            this.mRenameBox.Click += new System.EventHandler(this.SectionStrip_Click);
            this.mRenameBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mTextBox_MouseDown);
            this.mRenameBox.Leave += new System.EventHandler(this.mRenameBox_Leave);
            this.mRenameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mTextBox_KeyDown);
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel.Location = new System.Drawing.Point(7, 4);
            this.mLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(131, 20);
            this.mLabel.TabIndex = 0;
            this.mLabel.Text = "(Section name)";
            this.mLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mLabel.Click += new System.EventHandler(this.SectionStrip_Click);
            // 
            // mToolTip
            // 
            this.mToolTip.AutomaticDelay = 3000;
            this.mToolTip.AutoPopDelay = 4000;
            this.mToolTip.InitialDelay = 3000;
            this.mToolTip.IsBalloon = true;
            this.mToolTip.ReshowDelay = 600;
            this.mToolTip.ToolTipTitle = "Section Strip";
            // 
            // mAnnotationLayoutPanel
            // 
            this.mAnnotationLayoutPanel.AutoSize = true;
            this.mAnnotationLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mAnnotationLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.mAnnotationLayoutPanel.Location = new System.Drawing.Point(7, 36);
            this.mAnnotationLayoutPanel.Name = "mAnnotationLayoutPanel";
            this.mAnnotationLayoutPanel.Size = new System.Drawing.Size(0, 0);
            this.mAnnotationLayoutPanel.TabIndex = 2;
            this.mAnnotationLayoutPanel.WrapContents = false;
            this.mAnnotationLayoutPanel.Click += new System.EventHandler(this.mAnnotationLayoutPanel_Click);
            // 
            // SectionStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.mAnnotationLayoutPanel);
            this.Controls.Add(this.mLabel);
            this.Controls.Add(this.mRenameBox);
            this.Controls.Add(this.mAudioLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Name = "SectionStrip";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(298, 43);
            this.Enter += new System.EventHandler(this.SectionStrip_Enter);
            this.DoubleClick += new System.EventHandler(this.SectionStrip_DoubleClick);
            this.Click += new System.EventHandler(this.SectionStrip_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mAudioLayoutPanel;
        private System.Windows.Forms.TextBox mRenameBox;
        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.FlowLayoutPanel mAnnotationLayoutPanel;
    }
}
