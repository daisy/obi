namespace Obi.Dialogs
{
    partial class MergeProject
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
            this.mbtnDesendingOrder = new System.Windows.Forms.Button();
            this.lstManualArrange = new System.Windows.Forms.ListBox();
            this.m_grpArrangeAudioFiles = new System.Windows.Forms.GroupBox();
            this.mbtnAscendingOrder = new System.Windows.Forms.Button();
            this.m_grpAddFiles = new System.Windows.Forms.GroupBox();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.m_btnMoveUp = new System.Windows.Forms.Button();
            this.m_btnMoveDown = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.m_grpArrangeAudioFiles.SuspendLayout();
            this.m_grpAddFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // mbtnDesendingOrder
            // 
            this.mbtnDesendingOrder.AccessibleName = "Descending Order";
            this.mbtnDesendingOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mbtnDesendingOrder.Location = new System.Drawing.Point(168, 34);
            this.mbtnDesendingOrder.Name = "mbtnDesendingOrder";
            this.mbtnDesendingOrder.Size = new System.Drawing.Size(132, 26);
            this.mbtnDesendingOrder.TabIndex = 8;
            this.mbtnDesendingOrder.Text = "D&escending Order";
            this.mbtnDesendingOrder.UseVisualStyleBackColor = true;
            this.mbtnDesendingOrder.Click += new System.EventHandler(this.mbtnDesendingOrder_Click);
            // 
            // lstManualArrange
            // 
            this.lstManualArrange.AccessibleName = "Audio files list box.";
            this.lstManualArrange.FormattingEnabled = true;
            this.lstManualArrange.HorizontalScrollbar = true;
            this.lstManualArrange.Location = new System.Drawing.Point(10, 11);
            this.lstManualArrange.Name = "lstManualArrange";
            this.lstManualArrange.Size = new System.Drawing.Size(433, 108);
            this.lstManualArrange.TabIndex = 2;
            this.lstManualArrange.SelectedIndexChanged += new System.EventHandler(this.lstManualArrange_SelectedIndexChanged);
            // 
            // m_grpArrangeAudioFiles
            // 
            this.m_grpArrangeAudioFiles.AccessibleName = "Arrange audio files group box";
            this.m_grpArrangeAudioFiles.Controls.Add(this.mbtnDesendingOrder);
            this.m_grpArrangeAudioFiles.Controls.Add(this.mbtnAscendingOrder);
            this.m_grpArrangeAudioFiles.Location = new System.Drawing.Point(65, 138);
            this.m_grpArrangeAudioFiles.Name = "m_grpArrangeAudioFiles";
            this.m_grpArrangeAudioFiles.Size = new System.Drawing.Size(315, 65);
            this.m_grpArrangeAudioFiles.TabIndex = 7;
            this.m_grpArrangeAudioFiles.TabStop = false;
            this.m_grpArrangeAudioFiles.Text = "Arrange audio files:";
            // 
            // mbtnAscendingOrder
            // 
            this.mbtnAscendingOrder.AccessibleName = "Ascending Order";
            this.mbtnAscendingOrder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mbtnAscendingOrder.Location = new System.Drawing.Point(10, 34);
            this.mbtnAscendingOrder.Name = "mbtnAscendingOrder";
            this.mbtnAscendingOrder.Size = new System.Drawing.Size(118, 26);
            this.mbtnAscendingOrder.TabIndex = 7;
            this.mbtnAscendingOrder.Text = "&A&scending Order";
            this.mbtnAscendingOrder.UseVisualStyleBackColor = true;
            this.mbtnAscendingOrder.Click += new System.EventHandler(this.mbtnAscendingOrder_Click);
            // 
            // m_grpAddFiles
            // 
            this.m_grpAddFiles.AccessibleName = "GroupBox to Add and arrange files.";
            this.m_grpAddFiles.Controls.Add(this.m_btnRemove);
            this.m_grpAddFiles.Controls.Add(this.m_grpArrangeAudioFiles);
            this.m_grpAddFiles.Controls.Add(this.lstManualArrange);
            this.m_grpAddFiles.Controls.Add(this.m_btnAdd);
            this.m_grpAddFiles.Controls.Add(this.m_btnMoveUp);
            this.m_grpAddFiles.Controls.Add(this.m_btnMoveDown);
            this.m_grpAddFiles.Location = new System.Drawing.Point(12, 12);
            this.m_grpAddFiles.Name = "m_grpAddFiles";
            this.m_grpAddFiles.Size = new System.Drawing.Size(570, 209);
            this.m_grpAddFiles.TabIndex = 2;
            this.m_grpAddFiles.TabStop = false;
            // 
            // m_btnRemove
            // 
            this.m_btnRemove.AccessibleName = "Remove";
            this.m_btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnRemove.Location = new System.Drawing.Point(464, 109);
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.Size = new System.Drawing.Size(88, 23);
            this.m_btnRemove.TabIndex = 6;
            this.m_btnRemove.Text = "&Remove";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            this.m_btnRemove.Click += new System.EventHandler(this.m_btnRemove_Click);
            // 
            // m_btnAdd
            // 
            this.m_btnAdd.AccessibleName = "Add";
            this.m_btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnAdd.Location = new System.Drawing.Point(462, 80);
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.Size = new System.Drawing.Size(91, 23);
            this.m_btnAdd.TabIndex = 5;
            this.m_btnAdd.Text = "&Add";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
            // 
            // m_btnMoveUp
            // 
            this.m_btnMoveUp.AccessibleName = "Move Up";
            this.m_btnMoveUp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnMoveUp.Location = new System.Drawing.Point(462, 19);
            this.m_btnMoveUp.Name = "m_btnMoveUp";
            this.m_btnMoveUp.Size = new System.Drawing.Size(91, 23);
            this.m_btnMoveUp.TabIndex = 3;
            this.m_btnMoveUp.Text = "Move &UP";
            this.m_btnMoveUp.UseVisualStyleBackColor = true;
            this.m_btnMoveUp.Click += new System.EventHandler(this.m_btnMoveUp_Click);
            // 
            // m_btnMoveDown
            // 
            this.m_btnMoveDown.AccessibleName = "Move Down";
            this.m_btnMoveDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnMoveDown.Location = new System.Drawing.Point(462, 51);
            this.m_btnMoveDown.Name = "m_btnMoveDown";
            this.m_btnMoveDown.Size = new System.Drawing.Size(91, 23);
            this.m_btnMoveDown.TabIndex = 4;
            this.m_btnMoveDown.Text = "Move &Down";
            this.m_btnMoveDown.UseVisualStyleBackColor = true;
            this.m_btnMoveDown.Click += new System.EventHandler(this.m_btnMoveDown_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.AccessibleName = "Cancel";
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mCancelButton.Location = new System.Drawing.Point(292, 238);
            this.mCancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(100, 31);
            this.mCancelButton.TabIndex = 25;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mOKButton
            // 
            this.mOKButton.AccessibleName = "Ok";
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOKButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mOKButton.Location = new System.Drawing.Point(174, 238);
            this.mOKButton.Margin = new System.Windows.Forms.Padding(4);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(100, 31);
            this.mOKButton.TabIndex = 24;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // MergeProject
            // 
            this.AccessibleName = "Merge Project";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 282);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.m_grpAddFiles);
            this.Name = "MergeProject";
            this.Text = "Merge Project";
            this.m_grpArrangeAudioFiles.ResumeLayout(false);
            this.m_grpAddFiles.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mbtnDesendingOrder;
        private System.Windows.Forms.ListBox lstManualArrange;
        private System.Windows.Forms.GroupBox m_grpArrangeAudioFiles;
        private System.Windows.Forms.Button mbtnAscendingOrder;
        private System.Windows.Forms.GroupBox m_grpAddFiles;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.Button m_btnMoveUp;
        private System.Windows.Forms.Button m_btnMoveDown;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Button mOKButton;


    }
}