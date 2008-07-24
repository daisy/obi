namespace Obi.ProjectView
{
    partial class MetadataView
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
            this.mMetadataListView = new System.Windows.Forms.ListView();
            this.mNameColumn = new System.Windows.Forms.ColumnHeader();
            this.mContentColumn = new System.Windows.Forms.ColumnHeader();
            this.mContentTextbox = new System.Windows.Forms.TextBox();
            this.mUpdateButton = new System.Windows.Forms.Button();
            this.mNameTextbox = new System.Windows.Forms.TextBox();
            this.mNameLabel = new System.Windows.Forms.Label();
            this.mContentLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mMetadataListView
            // 
            this.mMetadataListView.AccessibleName = "Metadata List";
            this.mMetadataListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mMetadataListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mMetadataListView.CheckBoxes = true;
            this.mMetadataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mNameColumn,
            this.mContentColumn});
            this.mMetadataListView.FullRowSelect = true;
            this.mMetadataListView.Location = new System.Drawing.Point(0, 0);
            this.mMetadataListView.Margin = new System.Windows.Forms.Padding(4);
            this.mMetadataListView.MultiSelect = false;
            this.mMetadataListView.Name = "mMetadataListView";
            this.mMetadataListView.Size = new System.Drawing.Size(311, 321);
            this.mMetadataListView.TabIndex = 1;
            this.mMetadataListView.UseCompatibleStateImageBehavior = false;
            this.mMetadataListView.View = System.Windows.Forms.View.Details;
            this.mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.mMetadataListView_ItemChecked);
            this.mMetadataListView.SelectedIndexChanged += new System.EventHandler(this.mMetadataListView_SelectedIndexChanged);
            // 
            // mNameColumn
            // 
            this.mNameColumn.Text = "Name";
            this.mNameColumn.Width = 143;
            // 
            // mContentColumn
            // 
            this.mContentColumn.Text = "Content";
            this.mContentColumn.Width = 178;
            // 
            // mContentTextbox
            // 
            this.mContentTextbox.AccessibleName = "Metadata Content:";
            this.mContentTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mContentTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mContentTextbox.Location = new System.Drawing.Point(64, 359);
            this.mContentTextbox.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.mContentTextbox.Name = "mContentTextbox";
            this.mContentTextbox.Size = new System.Drawing.Size(247, 22);
            this.mContentTextbox.TabIndex = 4;
            this.mContentTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mContentTextbox_KeyDown);
            // 
            // mUpdateButton
            // 
            this.mUpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mUpdateButton.AutoSize = true;
            this.mUpdateButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mUpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mUpdateButton.Location = new System.Drawing.Point(246, 389);
            this.mUpdateButton.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.mUpdateButton.Name = "mUpdateButton";
            this.mUpdateButton.Size = new System.Drawing.Size(65, 28);
            this.mUpdateButton.TabIndex = 5;
            this.mUpdateButton.Text = "&Update";
            this.mUpdateButton.UseVisualStyleBackColor = true;
            this.mUpdateButton.Click += new System.EventHandler(this.mCommitButton_Click);
            // 
            // mNameTextbox
            // 
            this.mNameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mNameTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mNameTextbox.Location = new System.Drawing.Point(64, 329);
            this.mNameTextbox.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.mNameTextbox.Name = "mNameTextbox";
            this.mNameTextbox.Size = new System.Drawing.Size(247, 22);
            this.mNameTextbox.TabIndex = 3;
            // 
            // mNameLabel
            // 
            this.mNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mNameLabel.AutoSize = true;
            this.mNameLabel.Location = new System.Drawing.Point(8, 331);
            this.mNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.mNameLabel.Name = "mNameLabel";
            this.mNameLabel.Size = new System.Drawing.Size(48, 16);
            this.mNameLabel.TabIndex = 7;
            this.mNameLabel.Text = "&Name:";
            // 
            // mContentLabel
            // 
            this.mContentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mContentLabel.AutoSize = true;
            this.mContentLabel.Location = new System.Drawing.Point(0, 361);
            this.mContentLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.mContentLabel.Name = "mContentLabel";
            this.mContentLabel.Size = new System.Drawing.Size(56, 16);
            this.mContentLabel.TabIndex = 8;
            this.mContentLabel.Text = "&Content:";
            // 
            // MetadataView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.mContentLabel);
            this.Controls.Add(this.mNameLabel);
            this.Controls.Add(this.mMetadataListView);
            this.Controls.Add(this.mNameTextbox);
            this.Controls.Add(this.mUpdateButton);
            this.Controls.Add(this.mContentTextbox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MetadataView";
            this.Size = new System.Drawing.Size(311, 417);
            this.VisibleChanged += new System.EventHandler(this.MetadataView_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView mMetadataListView;
        private System.Windows.Forms.TextBox mContentTextbox;
        private System.Windows.Forms.Button mUpdateButton;
        private System.Windows.Forms.TextBox mNameTextbox;
        private System.Windows.Forms.ColumnHeader mNameColumn;
        private System.Windows.Forms.ColumnHeader mContentColumn;
        private System.Windows.Forms.Label mNameLabel;
        private System.Windows.Forms.Label mContentLabel;

    }
}
