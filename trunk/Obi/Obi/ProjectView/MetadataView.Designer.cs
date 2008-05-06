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
            this.mMetadataContentTextbox = new System.Windows.Forms.TextBox();
            this.mCommitButton = new System.Windows.Forms.Button();
            this.mAddNewButton = new System.Windows.Forms.Button();
            this.mMetadataEntryTextbox = new System.Windows.Forms.TextBox();
            this.mNameColumn = new System.Windows.Forms.ColumnHeader();
            this.mContentColumn = new System.Windows.Forms.ColumnHeader();
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
            this.mMetadataListView.Location = new System.Drawing.Point(3, 3);
            this.mMetadataListView.MultiSelect = false;
            this.mMetadataListView.Name = "mMetadataListView";
            this.mMetadataListView.Size = new System.Drawing.Size(278, 282);
            this.mMetadataListView.TabIndex = 1;
            this.mMetadataListView.UseCompatibleStateImageBehavior = false;
            this.mMetadataListView.View = System.Windows.Forms.View.Details;
            this.mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.mMetadataListView_ItemChecked);
            this.mMetadataListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.mMetadataListView_ItemSelectionChanged);
            // 
            // mMetadataContentTextbox
            // 
            this.mMetadataContentTextbox.AccessibleName = "Metadata Content:";
            this.mMetadataContentTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mMetadataContentTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mMetadataContentTextbox.Location = new System.Drawing.Point(109, 291);
            this.mMetadataContentTextbox.Name = "mMetadataContentTextbox";
            this.mMetadataContentTextbox.Size = new System.Drawing.Size(172, 20);
            this.mMetadataContentTextbox.TabIndex = 4;
            // 
            // mCommitButton
            // 
            this.mCommitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mCommitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCommitButton.Location = new System.Drawing.Point(3, 317);
            this.mCommitButton.Name = "mCommitButton";
            this.mCommitButton.Size = new System.Drawing.Size(75, 25);
            this.mCommitButton.TabIndex = 5;
            this.mCommitButton.Text = "Commit";
            this.mCommitButton.UseVisualStyleBackColor = true;
            this.mCommitButton.Click += new System.EventHandler(this.mCommitButton_Click);
            // 
            // mAddNewButton
            // 
            this.mAddNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mAddNewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mAddNewButton.Location = new System.Drawing.Point(84, 317);
            this.mAddNewButton.Name = "mAddNewButton";
            this.mAddNewButton.Size = new System.Drawing.Size(75, 25);
            this.mAddNewButton.TabIndex = 6;
            this.mAddNewButton.Text = "Edit Name";
            this.mAddNewButton.UseVisualStyleBackColor = true;
            this.mAddNewButton.Click += new System.EventHandler(this.mAddNewButton_Click);
            // 
            // mMetadataEntryTextbox
            // 
            this.mMetadataEntryTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mMetadataEntryTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mMetadataEntryTextbox.Location = new System.Drawing.Point(3, 291);
            this.mMetadataEntryTextbox.Name = "mMetadataEntryTextbox";
            this.mMetadataEntryTextbox.Size = new System.Drawing.Size(100, 20);
            this.mMetadataEntryTextbox.TabIndex = 3;
            // 
            // ContentColumn
            // 
            this.mContentColumn.Text = "Content";
            // 
            // MetadataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mMetadataListView);
            this.Controls.Add(this.mMetadataEntryTextbox);
            this.Controls.Add(this.mCommitButton);
            this.Controls.Add(this.mAddNewButton);
            this.Controls.Add(this.mMetadataContentTextbox);
            this.Name = "MetadataView";
            this.Size = new System.Drawing.Size(284, 346);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView mMetadataListView;
        private System.Windows.Forms.TextBox mMetadataContentTextbox;
        private System.Windows.Forms.Button mCommitButton;
        private System.Windows.Forms.Button mAddNewButton;
        private System.Windows.Forms.TextBox mMetadataEntryTextbox;
        private System.Windows.Forms.ColumnHeader mNameColumn;
        private System.Windows.Forms.ColumnHeader mContentColumn;

    }
}
