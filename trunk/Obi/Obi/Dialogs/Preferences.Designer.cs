namespace Obi.Dialogs
{
    partial class Preferences
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.label2 = new System.Windows.Forms.Label();
            this.mDirectoryTextbox = new System.Windows.Forms.TextBox();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.labelInputDeviceName = new System.Windows.Forms.Label();
            this.mInputDeviceCombo = new System.Windows.Forms.ComboBox();
            this.labelOutputDeviceName = new System.Windows.Forms.Label();
            this.mOutputDeviceCombo = new System.Windows.Forms.ComboBox();
            this.mTab = new System.Windows.Forms.TabControl();
            this.mProjectTab = new System.Windows.Forms.TabPage();
            this.mLastOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.mAudioTab = new System.Windows.Forms.TabPage();
            this.mChannelsTextbox = new System.Windows.Forms.TextBox();
            this.mSampleRateTextbox = new System.Windows.Forms.TextBox();
            this.mChannelsCombo = new System.Windows.Forms.ComboBox();
            this.labelChannels = new System.Windows.Forms.Label();
            this.mSampleRateCombo = new System.Windows.Forms.ComboBox();
            this.labelSampleRate = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.mCultureBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mOrganizationTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mFullNameTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mPreviewDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.mAudioCluesCheckBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mNoiseLevelComboBox = new System.Windows.Forms.ComboBox();
            this.mTab.SuspendLayout();
            this.mProjectTab.SuspendLayout();
            this.mAudioTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewDurationUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default projects &directory:";
            // 
            // mDirectoryTextbox
            // 
            this.mDirectoryTextbox.AccessibleName = "Default projects directory:";
            this.mDirectoryTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mDirectoryTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mDirectoryTextbox.Location = new System.Drawing.Point(152, 9);
            this.mDirectoryTextbox.Name = "mDirectoryTextbox";
            this.mDirectoryTextbox.Size = new System.Drawing.Size(361, 20);
            this.mDirectoryTextbox.TabIndex = 3;
            // 
            // mBrowseButton
            // 
            this.mBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mBrowseButton.Location = new System.Drawing.Point(519, 6);
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.Size = new System.Drawing.Size(75, 25);
            this.mBrowseButton.TabIndex = 4;
            this.mBrowseButton.Text = "&Browse";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.mBrowseButton_Click);
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOKButton.Location = new System.Drawing.Point(228, 240);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 25);
            this.mOKButton.TabIndex = 5;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancelButton.Location = new System.Drawing.Point(309, 240);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 25);
            this.mCancelButton.TabIndex = 6;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // labelInputDeviceName
            // 
            this.labelInputDeviceName.AutoSize = true;
            this.labelInputDeviceName.Location = new System.Drawing.Point(15, 10);
            this.labelInputDeviceName.Name = "labelInputDeviceName";
            this.labelInputDeviceName.Size = new System.Drawing.Size(98, 13);
            this.labelInputDeviceName.TabIndex = 7;
            this.labelInputDeviceName.Text = "&Input device name:";
            // 
            // mInputDeviceCombo
            // 
            this.mInputDeviceCombo.AccessibleName = "Input device name:";
            this.mInputDeviceCombo.AllowDrop = true;
            this.mInputDeviceCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mInputDeviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mInputDeviceCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mInputDeviceCombo.FormattingEnabled = true;
            this.mInputDeviceCombo.Location = new System.Drawing.Point(121, 7);
            this.mInputDeviceCombo.Name = "mInputDeviceCombo";
            this.mInputDeviceCombo.Size = new System.Drawing.Size(453, 21);
            this.mInputDeviceCombo.TabIndex = 8;
            // 
            // labelOutputDeviceName
            // 
            this.labelOutputDeviceName.AutoSize = true;
            this.labelOutputDeviceName.Location = new System.Drawing.Point(6, 38);
            this.labelOutputDeviceName.Name = "labelOutputDeviceName";
            this.labelOutputDeviceName.Size = new System.Drawing.Size(106, 13);
            this.labelOutputDeviceName.TabIndex = 9;
            this.labelOutputDeviceName.Text = "O&utput device name:";
            // 
            // mOutputDeviceCombo
            // 
            this.mOutputDeviceCombo.AccessibleName = "Output device name:";
            this.mOutputDeviceCombo.AllowDrop = true;
            this.mOutputDeviceCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mOutputDeviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mOutputDeviceCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOutputDeviceCombo.FormattingEnabled = true;
            this.mOutputDeviceCombo.Location = new System.Drawing.Point(121, 35);
            this.mOutputDeviceCombo.Name = "mOutputDeviceCombo";
            this.mOutputDeviceCombo.Size = new System.Drawing.Size(453, 21);
            this.mOutputDeviceCombo.TabIndex = 10;
            // 
            // mTab
            // 
            this.mTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTab.Controls.Add(this.mProjectTab);
            this.mTab.Controls.Add(this.mAudioTab);
            this.mTab.Controls.Add(this.tabPage1);
            this.mTab.Location = new System.Drawing.Point(12, 13);
            this.mTab.Name = "mTab";
            this.mTab.SelectedIndex = 0;
            this.mTab.Size = new System.Drawing.Size(588, 220);
            this.mTab.TabIndex = 11;
            // 
            // mProjectTab
            // 
            this.mProjectTab.Controls.Add(this.mLastOpenCheckBox);
            this.mProjectTab.Controls.Add(this.label2);
            this.mProjectTab.Controls.Add(this.mBrowseButton);
            this.mProjectTab.Controls.Add(this.mDirectoryTextbox);
            this.mProjectTab.Location = new System.Drawing.Point(4, 22);
            this.mProjectTab.Name = "mProjectTab";
            this.mProjectTab.Padding = new System.Windows.Forms.Padding(3);
            this.mProjectTab.Size = new System.Drawing.Size(600, 194);
            this.mProjectTab.TabIndex = 0;
            this.mProjectTab.Text = "Project";
            this.mProjectTab.UseVisualStyleBackColor = true;
            // 
            // mLastOpenCheckBox
            // 
            this.mLastOpenCheckBox.AutoSize = true;
            this.mLastOpenCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mLastOpenCheckBox.Location = new System.Drawing.Point(6, 35);
            this.mLastOpenCheckBox.Name = "mLastOpenCheckBox";
            this.mLastOpenCheckBox.Size = new System.Drawing.Size(188, 17);
            this.mLastOpenCheckBox.TabIndex = 8;
            this.mLastOpenCheckBox.Text = "Open &last project when starting Obi";
            this.mLastOpenCheckBox.UseVisualStyleBackColor = true;
            // 
            // mAudioTab
            // 
            this.mAudioTab.Controls.Add(this.mNoiseLevelComboBox);
            this.mAudioTab.Controls.Add(this.label8);
            this.mAudioTab.Controls.Add(this.mAudioCluesCheckBox);
            this.mAudioTab.Controls.Add(this.label7);
            this.mAudioTab.Controls.Add(this.mPreviewDurationUpDown);
            this.mAudioTab.Controls.Add(this.label3);
            this.mAudioTab.Controls.Add(this.mChannelsTextbox);
            this.mAudioTab.Controls.Add(this.mSampleRateTextbox);
            this.mAudioTab.Controls.Add(this.mChannelsCombo);
            this.mAudioTab.Controls.Add(this.labelChannels);
            this.mAudioTab.Controls.Add(this.mSampleRateCombo);
            this.mAudioTab.Controls.Add(this.labelSampleRate);
            this.mAudioTab.Controls.Add(this.mOutputDeviceCombo);
            this.mAudioTab.Controls.Add(this.mInputDeviceCombo);
            this.mAudioTab.Controls.Add(this.labelOutputDeviceName);
            this.mAudioTab.Controls.Add(this.labelInputDeviceName);
            this.mAudioTab.Location = new System.Drawing.Point(4, 22);
            this.mAudioTab.Name = "mAudioTab";
            this.mAudioTab.Padding = new System.Windows.Forms.Padding(3);
            this.mAudioTab.Size = new System.Drawing.Size(580, 194);
            this.mAudioTab.TabIndex = 1;
            this.mAudioTab.Text = "Audio";
            this.mAudioTab.UseVisualStyleBackColor = true;
            // 
            // mChannelsTextbox
            // 
            this.mChannelsTextbox.AccessibleName = "Project channels:";
            this.mChannelsTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mChannelsTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mChannelsTextbox.Location = new System.Drawing.Point(121, 91);
            this.mChannelsTextbox.Name = "mChannelsTextbox";
            this.mChannelsTextbox.ReadOnly = true;
            this.mChannelsTextbox.Size = new System.Drawing.Size(453, 20);
            this.mChannelsTextbox.TabIndex = 18;
            this.mChannelsTextbox.Visible = false;
            // 
            // mSampleRateTextbox
            // 
            this.mSampleRateTextbox.AccessibleName = "Project sample rate:";
            this.mSampleRateTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mSampleRateTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mSampleRateTextbox.Location = new System.Drawing.Point(121, 63);
            this.mSampleRateTextbox.Name = "mSampleRateTextbox";
            this.mSampleRateTextbox.ReadOnly = true;
            this.mSampleRateTextbox.Size = new System.Drawing.Size(453, 20);
            this.mSampleRateTextbox.TabIndex = 17;
            this.mSampleRateTextbox.Visible = false;
            // 
            // mChannelsCombo
            // 
            this.mChannelsCombo.AccessibleName = "Default channels:";
            this.mChannelsCombo.AllowDrop = true;
            this.mChannelsCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mChannelsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mChannelsCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mChannelsCombo.FormattingEnabled = true;
            this.mChannelsCombo.Location = new System.Drawing.Point(121, 91);
            this.mChannelsCombo.Name = "mChannelsCombo";
            this.mChannelsCombo.Size = new System.Drawing.Size(453, 21);
            this.mChannelsCombo.TabIndex = 16;
            // 
            // labelChannels
            // 
            this.labelChannels.AutoSize = true;
            this.labelChannels.Location = new System.Drawing.Point(30, 94);
            this.labelChannels.Name = "labelChannels";
            this.labelChannels.Size = new System.Drawing.Size(83, 13);
            this.labelChannels.TabIndex = 15;
            this.labelChannels.Text = "Audio &channels:";
            // 
            // mSampleRateCombo
            // 
            this.mSampleRateCombo.AccessibleName = "Default sample rate:";
            this.mSampleRateCombo.AllowDrop = true;
            this.mSampleRateCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mSampleRateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mSampleRateCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mSampleRateCombo.FormattingEnabled = true;
            this.mSampleRateCombo.Location = new System.Drawing.Point(121, 63);
            this.mSampleRateCombo.Name = "mSampleRateCombo";
            this.mSampleRateCombo.Size = new System.Drawing.Size(453, 21);
            this.mSampleRateCombo.TabIndex = 14;
            // 
            // labelSampleRate
            // 
            this.labelSampleRate.AutoSize = true;
            this.labelSampleRate.Location = new System.Drawing.Point(47, 66);
            this.labelSampleRate.Name = "labelSampleRate";
            this.labelSampleRate.Size = new System.Drawing.Size(66, 13);
            this.labelSampleRate.TabIndex = 13;
            this.labelSampleRate.Text = "&Sample rate:";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.mCultureBox);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.mOrganizationTextbox);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.mFullNameTextbox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(600, 194);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "User profile";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 13);
            this.label6.TabIndex = 7;
            // 
            // mCultureBox
            // 
            this.mCultureBox.AccessibleName = "Language:";
            this.mCultureBox.AllowDrop = true;
            this.mCultureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mCultureBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCultureBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCultureBox.FormattingEnabled = true;
            this.mCultureBox.Location = new System.Drawing.Point(81, 57);
            this.mCultureBox.Name = "mCultureBox";
            this.mCultureBox.Size = new System.Drawing.Size(513, 21);
            this.mCultureBox.Sorted = true;
            this.mCultureBox.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "&Language:";
            // 
            // mOrganizationTextbox
            // 
            this.mOrganizationTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mOrganizationTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mOrganizationTextbox.Location = new System.Drawing.Point(81, 32);
            this.mOrganizationTextbox.Name = "mOrganizationTextbox";
            this.mOrganizationTextbox.Size = new System.Drawing.Size(513, 20);
            this.mOrganizationTextbox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "&Organization:";
            // 
            // mFullNameTextbox
            // 
            this.mFullNameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFullNameTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mFullNameTextbox.Location = new System.Drawing.Point(81, 6);
            this.mFullNameTextbox.Name = "mFullNameTextbox";
            this.mFullNameTextbox.Size = new System.Drawing.Size(513, 20);
            this.mFullNameTextbox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Full &name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "&Preview duration:";
            // 
            // mPreviewDurationUpDown
            // 
            this.mPreviewDurationUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewDurationUpDown.DecimalPlaces = 1;
            this.mPreviewDurationUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.mPreviewDurationUpDown.Location = new System.Drawing.Point(121, 118);
            this.mPreviewDurationUpDown.Name = "mPreviewDurationUpDown";
            this.mPreviewDurationUpDown.Size = new System.Drawing.Size(120, 20);
            this.mPreviewDurationUpDown.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(247, 120);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "second(s)";
            // 
            // mAudioCluesCheckBox
            // 
            this.mAudioCluesCheckBox.AutoSize = true;
            this.mAudioCluesCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mAudioCluesCheckBox.Location = new System.Drawing.Point(6, 171);
            this.mAudioCluesCheckBox.Name = "mAudioCluesCheckBox";
            this.mAudioCluesCheckBox.Size = new System.Drawing.Size(78, 17);
            this.mAudioCluesCheckBox.TabIndex = 25;
            this.mAudioCluesCheckBox.Text = "&Audio clues";
            this.mAudioCluesCheckBox.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(50, 147);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "&Noise level:";
            // 
            // mNoiseLevelComboBox
            // 
            this.mNoiseLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mNoiseLevelComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNoiseLevelComboBox.FormattingEnabled = true;
            this.mNoiseLevelComboBox.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.mNoiseLevelComboBox.Location = new System.Drawing.Point(120, 144);
            this.mNoiseLevelComboBox.Name = "mNoiseLevelComboBox";
            this.mNoiseLevelComboBox.Size = new System.Drawing.Size(121, 21);
            this.mNoiseLevelComboBox.TabIndex = 24;
            // 
            // Preferences
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(612, 278);
            this.Controls.Add(this.mTab);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 305);
            this.Name = "Preferences";
            this.Text = "Edit preferences";
            this.mTab.ResumeLayout(false);
            this.mProjectTab.ResumeLayout(false);
            this.mProjectTab.PerformLayout();
            this.mAudioTab.ResumeLayout(false);
            this.mAudioTab.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewDurationUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mDirectoryTextbox;
        private System.Windows.Forms.Button mBrowseButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label labelInputDeviceName;
        private System.Windows.Forms.ComboBox mInputDeviceCombo;
        private System.Windows.Forms.Label labelOutputDeviceName;
        private System.Windows.Forms.ComboBox mOutputDeviceCombo;
        private System.Windows.Forms.TabControl mTab;
        private System.Windows.Forms.TabPage mProjectTab;
        private System.Windows.Forms.TabPage mAudioTab;
        private System.Windows.Forms.Label labelSampleRate;
        private System.Windows.Forms.ComboBox mSampleRateCombo;
        private System.Windows.Forms.ComboBox mChannelsCombo;
        private System.Windows.Forms.Label labelChannels;
        private System.Windows.Forms.CheckBox mLastOpenCheckBox;
        private System.Windows.Forms.TextBox mChannelsTextbox;
        private System.Windows.Forms.TextBox mSampleRateTextbox;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mFullNameTextbox;
        private System.Windows.Forms.TextBox mOrganizationTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox mCultureBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox mAudioCluesCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown mPreviewDurationUpDown;
        private System.Windows.Forms.ComboBox mNoiseLevelComboBox;
        private System.Windows.Forms.Label label8;
    }
}