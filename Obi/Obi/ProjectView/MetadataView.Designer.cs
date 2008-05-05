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
            this.mLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.mMetadataListView = new System.Windows.Forms.ListView();
            this.mMetadataTextbox = new System.Windows.Forms.TextBox();
            this.mCommitButton = new System.Windows.Forms.Button();
            this.mAddNewButton = new System.Windows.Forms.Button();
            this.mMetadataEntryTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mLayout
            // 
            this.mLayout.AutoScroll = true;
            this.mLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mLayout.Location = new System.Drawing.Point(0, 0);
            this.mLayout.Margin = new System.Windows.Forms.Padding(0);
            this.mLayout.Name = "mLayout";
            this.mLayout.Size = new System.Drawing.Size(150, 150);
            this.mLayout.TabIndex = 0;
            this.mLayout.WrapContents = false;
            this.mLayout.SizeChanged += new System.EventHandler(this.mLayout_SizeChanged);
            // 
            // mMetadataListView
            // 
            this.mMetadataListView.AccessibleName = "Metadata List";
            this.mMetadataListView.CheckBoxes = true;
            this.mMetadataListView.FullRowSelect = true;
            this.mMetadataListView.Location = new System.Drawing.Point(5, 5);
            this.mMetadataListView.MultiSelect = false;
            this.mMetadataListView.Name = "mMetadataListView";
            this.mMetadataListView.Size = new System.Drawing.Size(200, 100);
            this.mMetadataListView.TabIndex = 1;
            this.mMetadataListView.UseCompatibleStateImageBehavior = false;
            this.mMetadataListView.View = System.Windows.Forms.View.Details;
            this.mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.mMetadataListView_ItemChecked);
            this.mMetadataListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.mMetadataListView_ItemSelectionChanged);
            // 
            // mMetadataTextbox
            // 
            this.mMetadataTextbox.AccessibleName = "Metadata Content:";
            this.mMetadataTextbox.Location = new System.Drawing.Point(110, 115);
            this.mMetadataTextbox.Name = "mMetadataTextbox";
            this.mMetadataTextbox.Size = new System.Drawing.Size(100, 20);
            this.mMetadataTextbox.TabIndex = 4;
            // 
            // mCommitButton
            // 
            this.mCommitButton.Location = new System.Drawing.Point(0, 150);
            this.mCommitButton.Name = "mCommitButton";
            this.mCommitButton.Size = new System.Drawing.Size(75, 16);
            this.mCommitButton.TabIndex = 5;
            this.mCommitButton.Text = "Commit";
            this.mCommitButton.UseVisualStyleBackColor = true;
            this.mCommitButton.Click += new System.EventHandler(this.mCommitButton_Click);
            // 
            // mAddNewButton
            // 
            this.mAddNewButton.Location = new System.Drawing.Point(80, 150);
            this.mAddNewButton.Name = "mAddNewButton";
            this.mAddNewButton.Size = new System.Drawing.Size(75, 16);
            this.mAddNewButton.TabIndex = 6;
            this.mAddNewButton.Text = "Edit Name";
            this.mAddNewButton.UseVisualStyleBackColor = true;
            this.mAddNewButton.Click += new System.EventHandler(this.mAddNewButton_Click);
            // 
            // mMetadataEntryTextbox
            // 
            this.mMetadataEntryTextbox.Location = new System.Drawing.Point(0, 115);
            this.mMetadataEntryTextbox.Name = "mMetadataEntryTextbox";
            this.mMetadataEntryTextbox.Size = new System.Drawing.Size(100, 20);
            this.mMetadataEntryTextbox.TabIndex = 3;
            // 
            // MetadataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mMetadataEntryTextbox);
            this.Controls.Add(this.mAddNewButton);
            this.Controls.Add(this.mCommitButton);
            this.Controls.Add(this.mMetadataTextbox);
            this.Controls.Add(this.mMetadataListView);
            this.Controls.Add(this.mLayout);
            this.Name = "MetadataView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel mLayout;
        private System.Windows.Forms.ListView mMetadataListView;
        private System.Windows.Forms.TextBox mMetadataTextbox;
        private System.Windows.Forms.Button mCommitButton;
        private System.Windows.Forms.Button mAddNewButton;
        private System.Windows.Forms.TextBox mMetadataEntryTextbox;

    }
}
