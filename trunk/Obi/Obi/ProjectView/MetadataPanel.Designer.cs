namespace Obi.ProjectView
{
    partial class MetadataPanel
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
            this.mNameComboBox = new System.Windows.Forms.ComboBox();
            this.mContentBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mNameComboBox
            // 
            this.mNameComboBox.AllowDrop = true;
            this.mNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mNameComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNameComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mNameComboBox.FormattingEnabled = true;
            this.mNameComboBox.Location = new System.Drawing.Point(9, 6);
            this.mNameComboBox.Name = "mNameComboBox";
            this.mNameComboBox.Size = new System.Drawing.Size(354, 23);
            this.mNameComboBox.TabIndex = 0;
            this.mNameComboBox.SelectionChangeCommitted += new System.EventHandler(this.mNameComboBox_SelectionChangeCommitted);
            this.mNameComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mNameComboBox_KeyDown);
            this.mNameComboBox.DropDown += new System.EventHandler(this.mNameComboBox_DropDown);
            // 
            // mContentBox
            // 
            this.mContentBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mContentBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mContentBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mContentBox.Location = new System.Drawing.Point(9, 35);
            this.mContentBox.Name = "mContentBox";
            this.mContentBox.Size = new System.Drawing.Size(354, 21);
            this.mContentBox.TabIndex = 1;
            this.mContentBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mContentBox_KeyDown);
            // 
            // MetadataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mContentBox);
            this.Controls.Add(this.mNameComboBox);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MetadataPanel";
            this.Padding = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.Size = new System.Drawing.Size(369, 62);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox mNameComboBox;
        private System.Windows.Forms.TextBox mContentBox;
    }
}
