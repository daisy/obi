namespace Obi.Dialogs
{
    partial class ImportFileSplitSize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFileSplitSize));
            this.label1 = new System.Windows.Forms.Label();
            this.mPhraseSizeTextBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mSplitCheckBox = new System.Windows.Forms.CheckBox();
            this.mCreateAudioFilePerSectionCheckBox = new System.Windows.Forms.CheckBox();
            this.lstManualArrange = new System.Windows.Forms.ListBox();
            this.m_btnMoveUp = new System.Windows.Forms.Button();
            this.m_btnMoveDown = new System.Windows.Forms.Button();
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_radioBtnNo = new System.Windows.Forms.RadioButton();
            this.m_radiobtnYes = new System.Windows.Forms.RadioButton();
            this.m_lblAscendingOrder = new System.Windows.Forms.Label();
            this.m_txtCharToReplaceWithSpace = new System.Windows.Forms.TextBox();
            this.m_txtPageIdentificationString = new System.Windows.Forms.TextBox();
            this.lblToReplaceWithSpace = new System.Windows.Forms.Label();
            this.lblCountToTruncateFromStart = new System.Windows.Forms.Label();
            this.lbltPageIdentificationString = new System.Windows.Forms.Label();
            this.m_numCharCountToTruncateFromStart = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numCharCountToTruncateFromStart)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // mPhraseSizeTextBox
            // 
            resources.ApplyResources(this.mPhraseSizeTextBox, "mPhraseSizeTextBox");
            this.mPhraseSizeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPhraseSizeTextBox.Name = "mPhraseSizeTextBox";
            // 
            // mOKButton
            // 
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mSplitCheckBox
            // 
            resources.ApplyResources(this.mSplitCheckBox, "mSplitCheckBox");
            this.mSplitCheckBox.Name = "mSplitCheckBox";
            this.mSplitCheckBox.UseVisualStyleBackColor = true;
            this.mSplitCheckBox.CheckedChanged += new System.EventHandler(this.mSplitCheckBox_CheckedChanged);
            // 
            // mCreateAudioFilePerSectionCheckBox
            // 
            resources.ApplyResources(this.mCreateAudioFilePerSectionCheckBox, "mCreateAudioFilePerSectionCheckBox");
            this.mCreateAudioFilePerSectionCheckBox.Name = "mCreateAudioFilePerSectionCheckBox";
            this.mCreateAudioFilePerSectionCheckBox.UseVisualStyleBackColor = true;
            // 
            // lstManualArrange
            // 
            this.lstManualArrange.FormattingEnabled = true;
            resources.ApplyResources(this.lstManualArrange, "lstManualArrange");
            this.lstManualArrange.Name = "lstManualArrange";
            // 
            // m_btnMoveUp
            // 
            resources.ApplyResources(this.m_btnMoveUp, "m_btnMoveUp");
            this.m_btnMoveUp.Name = "m_btnMoveUp";
            this.m_btnMoveUp.UseVisualStyleBackColor = true;
            this.m_btnMoveUp.Click += new System.EventHandler(this.m_btnMoveUp_Click);
            // 
            // m_btnMoveDown
            // 
            resources.ApplyResources(this.m_btnMoveDown, "m_btnMoveDown");
            this.m_btnMoveDown.Name = "m_btnMoveDown";
            this.m_btnMoveDown.UseVisualStyleBackColor = true;
            this.m_btnMoveDown.Click += new System.EventHandler(this.m_btnMoveDown_Click);
            // 
            // m_btnAdd
            // 
            resources.ApplyResources(this.m_btnAdd, "m_btnAdd");
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_radioBtnNo);
            this.groupBox1.Controls.Add(this.m_radiobtnYes);
            this.groupBox1.Controls.Add(this.m_lblAscendingOrder);
            this.groupBox1.Controls.Add(this.lstManualArrange);
            this.groupBox1.Controls.Add(this.m_btnAdd);
            this.groupBox1.Controls.Add(this.m_btnMoveUp);
            this.groupBox1.Controls.Add(this.m_btnMoveDown);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // m_radioBtnNo
            // 
            resources.ApplyResources(this.m_radioBtnNo, "m_radioBtnNo");
            this.m_radioBtnNo.Name = "m_radioBtnNo";
            this.m_radioBtnNo.TabStop = true;
            this.m_radioBtnNo.UseVisualStyleBackColor = true;
            // 
            // m_radiobtnYes
            // 
            resources.ApplyResources(this.m_radiobtnYes, "m_radiobtnYes");
            this.m_radiobtnYes.Name = "m_radiobtnYes";
            this.m_radiobtnYes.TabStop = true;
            this.m_radiobtnYes.UseVisualStyleBackColor = true;
            // 
            // m_lblAscendingOrder
            // 
            resources.ApplyResources(this.m_lblAscendingOrder, "m_lblAscendingOrder");
            this.m_lblAscendingOrder.Name = "m_lblAscendingOrder";
            // 
            // m_txtCharToReplaceWithSpace
            // 
            resources.ApplyResources(this.m_txtCharToReplaceWithSpace, "m_txtCharToReplaceWithSpace");
            this.m_txtCharToReplaceWithSpace.Name = "m_txtCharToReplaceWithSpace";
            // 
            // m_txtPageIdentificationString
            // 
            resources.ApplyResources(this.m_txtPageIdentificationString, "m_txtPageIdentificationString");
            this.m_txtPageIdentificationString.Name = "m_txtPageIdentificationString";
            // 
            // lblToReplaceWithSpace
            // 
            resources.ApplyResources(this.lblToReplaceWithSpace, "lblToReplaceWithSpace");
            this.lblToReplaceWithSpace.Name = "lblToReplaceWithSpace";
            // 
            // lblCountToTruncateFromStart
            // 
            resources.ApplyResources(this.lblCountToTruncateFromStart, "lblCountToTruncateFromStart");
            this.lblCountToTruncateFromStart.Name = "lblCountToTruncateFromStart";
            // 
            // lbltPageIdentificationString
            // 
            resources.ApplyResources(this.lbltPageIdentificationString, "lbltPageIdentificationString");
            this.lbltPageIdentificationString.Name = "lbltPageIdentificationString";
            // 
            // m_numCharCountToTruncateFromStart
            // 
            resources.ApplyResources(this.m_numCharCountToTruncateFromStart, "m_numCharCountToTruncateFromStart");
            this.m_numCharCountToTruncateFromStart.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.m_numCharCountToTruncateFromStart.Name = "m_numCharCountToTruncateFromStart";
            // 
            // ImportFileSplitSize
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.m_numCharCountToTruncateFromStart);
            this.Controls.Add(this.lbltPageIdentificationString);
            this.Controls.Add(this.lblCountToTruncateFromStart);
            this.Controls.Add(this.lblToReplaceWithSpace);
            this.Controls.Add(this.m_txtPageIdentificationString);
            this.Controls.Add(this.m_txtCharToReplaceWithSpace);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mCreateAudioFilePerSectionCheckBox);
            this.Controls.Add(this.mSplitCheckBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mPhraseSizeTextBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportFileSplitSize";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportFileSplitSize_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numCharCountToTruncateFromStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mPhraseSizeTextBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mSplitCheckBox;
        private System.Windows.Forms.CheckBox mCreateAudioFilePerSectionCheckBox;
        private System.Windows.Forms.ListBox lstManualArrange;
        private System.Windows.Forms.Button m_btnMoveUp;
        private System.Windows.Forms.Button m_btnMoveDown;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton m_radioBtnNo;
        private System.Windows.Forms.RadioButton m_radiobtnYes;
        private System.Windows.Forms.Label m_lblAscendingOrder;
        private System.Windows.Forms.TextBox m_txtCharToReplaceWithSpace;
        private System.Windows.Forms.TextBox m_txtPageIdentificationString;
        private System.Windows.Forms.Label lblToReplaceWithSpace;
        private System.Windows.Forms.Label lblCountToTruncateFromStart;
        private System.Windows.Forms.Label lbltPageIdentificationString;
        private System.Windows.Forms.NumericUpDown m_numCharCountToTruncateFromStart;
    }
}