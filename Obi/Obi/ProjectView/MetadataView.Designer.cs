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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetadataView));
            mMetadataListView = new System.Windows.Forms.ListView();
            mNameColumn = new System.Windows.Forms.ColumnHeader();
            mContentColumn = new System.Windows.Forms.ColumnHeader();
            mContentTextbox = new System.Windows.Forms.TextBox();
            mUpdateButton = new System.Windows.Forms.Button();
            mNameTextbox = new System.Windows.Forms.TextBox();
            mNameLabel = new System.Windows.Forms.Label();
            mContentLabel = new System.Windows.Forms.Label();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            m_BtnContextMenu = new System.Windows.Forms.Button();
            mMetadataContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(components);
            SetDefaultMetadataStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            SetDefaultMetadataOverwriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            SaveAsDefaultMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            mMetadataContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mMetadataListView
            // 
            resources.ApplyResources(mMetadataListView, "mMetadataListView");
            mMetadataListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mMetadataListView.CheckBoxes = true;
            mMetadataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { mNameColumn, mContentColumn });
            mMetadataListView.FullRowSelect = true;
            mMetadataListView.MultiSelect = false;
            mMetadataListView.Name = "mMetadataListView";
            mMetadataListView.ShowItemToolTips = true;
            mMetadataListView.UseCompatibleStateImageBehavior = false;
            mMetadataListView.View = System.Windows.Forms.View.Details;
            mMetadataListView.ItemCheck += mMetadataListView_ItemCheck;
            mMetadataListView.ItemChecked += mMetadataListView_ItemChecked;
            mMetadataListView.ItemMouseHover += mMetadataListView_ItemMouseHover;
            mMetadataListView.SelectedIndexChanged += mMetadataListView_SelectedIndexChanged;
            // 
            // mNameColumn
            // 
            resources.ApplyResources(mNameColumn, "mNameColumn");
            // 
            // mContentColumn
            // 
            resources.ApplyResources(mContentColumn, "mContentColumn");
            // 
            // mContentTextbox
            // 
            resources.ApplyResources(mContentTextbox, "mContentTextbox");
            mContentTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mContentTextbox.Name = "mContentTextbox";
            mContentTextbox.KeyDown += mContentTextbox_KeyDown;
            mContentTextbox.Leave += mContentTextbox_Leave;
            // 
            // mUpdateButton
            // 
            resources.ApplyResources(mUpdateButton, "mUpdateButton");
            mUpdateButton.Name = "mUpdateButton";
            mUpdateButton.UseVisualStyleBackColor = true;
            mUpdateButton.Click += mCommitButton_Click;
            // 
            // mNameTextbox
            // 
            resources.ApplyResources(mNameTextbox, "mNameTextbox");
            mNameTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mNameTextbox.Name = "mNameTextbox";
            mNameTextbox.Leave += mNameTextbox_Leave;
            // 
            // mNameLabel
            // 
            resources.ApplyResources(mNameLabel, "mNameLabel");
            mNameLabel.Name = "mNameLabel";
            // 
            // mContentLabel
            // 
            resources.ApplyResources(mContentLabel, "mContentLabel");
            mContentLabel.Name = "mContentLabel";
            // 
            // m_BtnContextMenu
            // 
            resources.ApplyResources(m_BtnContextMenu, "m_BtnContextMenu");
            m_BtnContextMenu.Name = "m_BtnContextMenu";
            m_BtnContextMenu.UseVisualStyleBackColor = true;
            m_BtnContextMenu.Click += m_BtnContextMenu_Click;
            // 
            // mMetadataContextMenuStrip
            // 
            mMetadataContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            mMetadataContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { SetDefaultMetadataStripMenuItem, SetDefaultMetadataOverwriteToolStripMenuItem, SaveAsDefaultMetadataToolStripMenuItem });
            mMetadataContextMenuStrip.Name = "mMetadataContextMenuStrip";
            resources.ApplyResources(mMetadataContextMenuStrip, "mMetadataContextMenuStrip");
            // 
            // SetDefaultMetadataStripMenuItem
            // 
            SetDefaultMetadataStripMenuItem.Name = "SetDefaultMetadataStripMenuItem";
            resources.ApplyResources(SetDefaultMetadataStripMenuItem, "SetDefaultMetadataStripMenuItem");
            SetDefaultMetadataStripMenuItem.Click += SetDefaultMetadataStripMenuItem_Click;
            // 
            // SetDefaultMetadataOverwriteToolStripMenuItem
            // 
            SetDefaultMetadataOverwriteToolStripMenuItem.Name = "SetDefaultMetadataOverwriteToolStripMenuItem";
            resources.ApplyResources(SetDefaultMetadataOverwriteToolStripMenuItem, "SetDefaultMetadataOverwriteToolStripMenuItem");
            SetDefaultMetadataOverwriteToolStripMenuItem.Click += SetDefaultMetadataOverwriteToolStripMenuItem_Click;
            // 
            // SaveAsDefaultMetadataToolStripMenuItem
            // 
            SaveAsDefaultMetadataToolStripMenuItem.Name = "SaveAsDefaultMetadataToolStripMenuItem";
            resources.ApplyResources(SaveAsDefaultMetadataToolStripMenuItem, "SaveAsDefaultMetadataToolStripMenuItem");
            SaveAsDefaultMetadataToolStripMenuItem.Click += SaveAsDefaultMetadataToolStripMenuItem_Click;
            // 
            // MetadataView
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(m_BtnContextMenu);
            Controls.Add(mContentLabel);
            Controls.Add(mNameLabel);
            Controls.Add(mMetadataListView);
            Controls.Add(mNameTextbox);
            Controls.Add(mUpdateButton);
            Controls.Add(mContentTextbox);
            resources.ApplyResources(this, "$this");
            Name = "MetadataView";
            VisibleChanged += MetadataView_VisibleChanged;
            mMetadataContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button m_BtnContextMenu;
        private System.Windows.Forms.ContextMenuStrip mMetadataContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem SetDefaultMetadataStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsDefaultMetadataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetDefaultMetadataOverwriteToolStripMenuItem;

    }
}
