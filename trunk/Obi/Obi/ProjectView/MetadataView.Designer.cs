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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            this.mMetadataListView.Location = new System.Drawing.Point(4, 4);
            this.mMetadataListView.Margin = new System.Windows.Forms.Padding(4);
            this.mMetadataListView.MultiSelect = false;
            this.mMetadataListView.Name = "mMetadataListView";
            this.mMetadataListView.Size = new System.Drawing.Size(296, 237);
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
            this.mContentTextbox.Location = new System.Drawing.Point(75, 280);
            this.mContentTextbox.Margin = new System.Windows.Forms.Padding(4);
            this.mContentTextbox.Name = "mContentTextbox";
            this.mContentTextbox.Size = new System.Drawing.Size(225, 22);
            this.mContentTextbox.TabIndex = 4;
            this.mContentTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mContentTextbox_KeyDown);
            // 
            // mUpdateButton
            // 
            this.mUpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mUpdateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mUpdateButton.Location = new System.Drawing.Point(201, 312);
            this.mUpdateButton.Margin = new System.Windows.Forms.Padding(4);
            this.mUpdateButton.Name = "mUpdateButton";
            this.mUpdateButton.Size = new System.Drawing.Size(100, 31);
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
            this.mNameTextbox.Location = new System.Drawing.Point(75, 248);
            this.mNameTextbox.Margin = new System.Windows.Forms.Padding(4);
            this.mNameTextbox.Name = "mNameTextbox";
            this.mNameTextbox.Size = new System.Drawing.Size(225, 22);
            this.mNameTextbox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 250);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 282);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Content:";
            // 
            // MetadataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mMetadataListView);
            this.Controls.Add(this.mNameTextbox);
            this.Controls.Add(this.mUpdateButton);
            this.Controls.Add(this.mContentTextbox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MetadataView";
            this.Size = new System.Drawing.Size(305, 347);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

    }
}
