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
            this.mNameBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mContentBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mDescriptionBox = new System.Windows.Forms.TextBox();
            this.mMetadataGrid = new System.Windows.Forms.DataGridView();
            this.DisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisplayContent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mAddItemButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mMetadataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mNameBox
            // 
            this.mNameBox.AllowDrop = true;
            this.mNameBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mNameBox.FormattingEnabled = true;
            this.mNameBox.Location = new System.Drawing.Point(54, 6);
            this.mNameBox.Name = "mNameBox";
            this.mNameBox.Size = new System.Drawing.Size(542, 20);
            this.mNameBox.TabIndex = 0;
            this.mNameBox.SelectedValueChanged += new System.EventHandler(this.mNameBox_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Value:";
            // 
            // mContentBox
            // 
            this.mContentBox.Location = new System.Drawing.Point(54, 32);
            this.mContentBox.Name = "mContentBox";
            this.mContentBox.Size = new System.Drawing.Size(542, 19);
            this.mContentBox.TabIndex = 3;
            // 
            // mOKButton
            // 
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(521, 402);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 4;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(521, 373);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 5;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mDescriptionBox
            // 
            this.mDescriptionBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mDescriptionBox.Location = new System.Drawing.Point(14, 62);
            this.mDescriptionBox.Name = "mDescriptionBox";
            this.mDescriptionBox.ReadOnly = true;
            this.mDescriptionBox.Size = new System.Drawing.Size(501, 12);
            this.mDescriptionBox.TabIndex = 6;
            this.mDescriptionBox.Text = "(please chose a metadata item above)";
            // 
            // mMetadataGrid
            // 
            this.mMetadataGrid.AllowUserToAddRows = false;
            this.mMetadataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mMetadataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DisplayName,
            this.DisplayContent});
            this.mMetadataGrid.Location = new System.Drawing.Point(12, 122);
            this.mMetadataGrid.Name = "mMetadataGrid";
            this.mMetadataGrid.RowTemplate.Height = 21;
            this.mMetadataGrid.Size = new System.Drawing.Size(584, 245);
            this.mMetadataGrid.TabIndex = 7;
            // 
            // DisplayName
            // 
            this.DisplayName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DisplayName.HeaderText = "Name";
            this.DisplayName.Name = "DisplayName";
            this.DisplayName.ReadOnly = true;
            // 
            // DisplayContent
            // 
            this.DisplayContent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DisplayContent.HeaderText = "Content";
            this.DisplayContent.Name = "DisplayContent";
            // 
            // mAddItemButton
            // 
            this.mAddItemButton.Enabled = false;
            this.mAddItemButton.Location = new System.Drawing.Point(521, 57);
            this.mAddItemButton.Name = "mAddItemButton";
            this.mAddItemButton.Size = new System.Drawing.Size(75, 23);
            this.mAddItemButton.TabIndex = 8;
            this.mAddItemButton.Text = "&Add item";
            this.mAddItemButton.UseVisualStyleBackColor = true;
            this.mAddItemButton.Click += new System.EventHandler(this.mAddItemButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "Current book metadata:";
            // 
            // FullMetadata
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(608, 437);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mAddItemButton);
            this.Controls.Add(this.mMetadataGrid);
            this.Controls.Add(this.mDescriptionBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mContentBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mNameBox);
            this.Name = "FullMetadata";
            this.Text = "Edit full metadata";
            ((System.ComponentModel.ISupportInitialize)(this.mMetadataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox mNameBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mContentBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.TextBox mDescriptionBox;
        private System.Windows.Forms.DataGridView mMetadataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisplayName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisplayContent;
        private System.Windows.Forms.Button mAddItemButton;
        private System.Windows.Forms.Label label3;
    }
}