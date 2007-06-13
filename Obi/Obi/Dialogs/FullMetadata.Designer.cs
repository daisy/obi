namespace Obi.Dialogs
{
    partial class FullMetadata
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
            this.mOKButton = new System.Windows.Forms.Button();
            this.mAddButton = new System.Windows.Forms.Button();
            this.mMetadataPanels = new System.Windows.Forms.FlowLayoutPanel();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(261, 238);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 1;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mAddButton
            // 
            this.mAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mAddButton.Location = new System.Drawing.Point(12, 238);
            this.mAddButton.Name = "mAddButton";
            this.mAddButton.Size = new System.Drawing.Size(75, 23);
            this.mAddButton.TabIndex = 2;
            this.mAddButton.Text = "&Add";
            this.mAddButton.UseVisualStyleBackColor = true;
            this.mAddButton.Click += new System.EventHandler(this.mAddButton_Click);
            // 
            // mMetadataPanels
            // 
            this.mMetadataPanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mMetadataPanels.AutoScroll = true;
            this.mMetadataPanels.BackColor = System.Drawing.SystemColors.Control;
            this.mMetadataPanels.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mMetadataPanels.Location = new System.Drawing.Point(12, 12);
            this.mMetadataPanels.Name = "mMetadataPanels";
            this.mMetadataPanels.Size = new System.Drawing.Size(405, 220);
            this.mMetadataPanels.TabIndex = 0;
            this.mMetadataPanels.WrapContents = false;
            this.mMetadataPanels.SizeChanged += new System.EventHandler(this.mMetadataPanels_SizeChanged);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(342, 238);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 3;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // FullMetadata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(429, 273);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mAddButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mMetadataPanels);
            this.Name = "FullMetadata";
            this.Text = "FullMetadata";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mAddButton;
        private System.Windows.Forms.FlowLayoutPanel mMetadataPanels;
        private System.Windows.Forms.Button mCancelButton;
    }
}