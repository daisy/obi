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
            this.label1 = new System.Windows.Forms.Label();
            this.mTemplateBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mDirectoryBox = new System.Windows.Forms.TextBox();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.labelInputDeviceName = new System.Windows.Forms.Label();
            this.comboInputDevice = new System.Windows.Forms.ComboBox();
            this.labelOutputDeviceName = new System.Windows.Forms.Label();
            this.comboOutputDevice = new System.Windows.Forms.ComboBox();
            this.mTab = new System.Windows.Forms.TabControl();
            this.mProjectTab = new System.Windows.Forms.TabPage();
            this.mAudioTab = new System.Windows.Forms.TabPage();
            this.comboChannels = new System.Windows.Forms.ComboBox();
            this.labelChannels = new System.Windows.Forms.Label();
            this.comboSampleRate = new System.Windows.Forms.ComboBox();
            this.labelSampleRate = new System.Windows.Forms.Label();
            this.mTab.SuspendLayout();
            this.mProjectTab.SuspendLayout();
            this.mAudioTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project identifier &template:";
            // 
            // mTemplateBox
            // 
            this.mTemplateBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTemplateBox.Location = new System.Drawing.Point(152, 6);
            this.mTemplateBox.Name = "mTemplateBox";
            this.mTemplateBox.Size = new System.Drawing.Size(453, 19);
            this.mTemplateBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Default projects &directory:";
            // 
            // mDirectoryBox
            // 
            this.mDirectoryBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mDirectoryBox.Location = new System.Drawing.Point(152, 31);
            this.mDirectoryBox.Name = "mDirectoryBox";
            this.mDirectoryBox.Size = new System.Drawing.Size(372, 19);
            this.mDirectoryBox.TabIndex = 3;
            // 
            // mBrowseButton
            // 
            this.mBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseButton.Location = new System.Drawing.Point(530, 31);
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.mBrowseButton.TabIndex = 4;
            this.mBrowseButton.Text = "&Browse";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(243, 430);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 5;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(324, 430);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 6;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // labelInputDeviceName
            // 
            this.labelInputDeviceName.AutoSize = true;
            this.labelInputDeviceName.Location = new System.Drawing.Point(15, 9);
            this.labelInputDeviceName.Name = "labelInputDeviceName";
            this.labelInputDeviceName.Size = new System.Drawing.Size(100, 12);
            this.labelInputDeviceName.TabIndex = 7;
            this.labelInputDeviceName.Text = "&Input device name:";
            // 
            // comboInputDevice
            // 
            this.comboInputDevice.AllowDrop = true;
            this.comboInputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInputDevice.FormattingEnabled = true;
            this.comboInputDevice.Location = new System.Drawing.Point(121, 6);
            this.comboInputDevice.Name = "comboInputDevice";
            this.comboInputDevice.Size = new System.Drawing.Size(484, 20);
            this.comboInputDevice.TabIndex = 8;
            // 
            // labelOutputDeviceName
            // 
            this.labelOutputDeviceName.AutoSize = true;
            this.labelOutputDeviceName.Location = new System.Drawing.Point(6, 35);
            this.labelOutputDeviceName.Name = "labelOutputDeviceName";
            this.labelOutputDeviceName.Size = new System.Drawing.Size(109, 12);
            this.labelOutputDeviceName.TabIndex = 9;
            this.labelOutputDeviceName.Text = "O&utput device name:";
            // 
            // comboOutputDevice
            // 
            this.comboOutputDevice.AllowDrop = true;
            this.comboOutputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOutputDevice.FormattingEnabled = true;
            this.comboOutputDevice.Location = new System.Drawing.Point(121, 32);
            this.comboOutputDevice.Name = "comboOutputDevice";
            this.comboOutputDevice.Size = new System.Drawing.Size(484, 20);
            this.comboOutputDevice.TabIndex = 10;
            // 
            // mTab
            // 
            this.mTab.Controls.Add(this.mProjectTab);
            this.mTab.Controls.Add(this.mAudioTab);
            this.mTab.Location = new System.Drawing.Point(12, 12);
            this.mTab.Name = "mTab";
            this.mTab.SelectedIndex = 0;
            this.mTab.Size = new System.Drawing.Size(619, 412);
            this.mTab.TabIndex = 11;
            // 
            // mProjectTab
            // 
            this.mProjectTab.Controls.Add(this.label1);
            this.mProjectTab.Controls.Add(this.mTemplateBox);
            this.mProjectTab.Controls.Add(this.label2);
            this.mProjectTab.Controls.Add(this.mBrowseButton);
            this.mProjectTab.Controls.Add(this.mDirectoryBox);
            this.mProjectTab.Location = new System.Drawing.Point(4, 21);
            this.mProjectTab.Name = "mProjectTab";
            this.mProjectTab.Padding = new System.Windows.Forms.Padding(3);
            this.mProjectTab.Size = new System.Drawing.Size(611, 387);
            this.mProjectTab.TabIndex = 0;
            this.mProjectTab.Text = "Project";
            this.mProjectTab.UseVisualStyleBackColor = true;
            // 
            // mAudioTab
            // 
            this.mAudioTab.Controls.Add(this.comboChannels);
            this.mAudioTab.Controls.Add(this.labelChannels);
            this.mAudioTab.Controls.Add(this.comboSampleRate);
            this.mAudioTab.Controls.Add(this.labelSampleRate);
            this.mAudioTab.Controls.Add(this.comboOutputDevice);
            this.mAudioTab.Controls.Add(this.comboInputDevice);
            this.mAudioTab.Controls.Add(this.labelOutputDeviceName);
            this.mAudioTab.Controls.Add(this.labelInputDeviceName);
            this.mAudioTab.Location = new System.Drawing.Point(4, 21);
            this.mAudioTab.Name = "mAudioTab";
            this.mAudioTab.Padding = new System.Windows.Forms.Padding(3);
            this.mAudioTab.Size = new System.Drawing.Size(611, 387);
            this.mAudioTab.TabIndex = 1;
            this.mAudioTab.Text = "Audio";
            this.mAudioTab.UseVisualStyleBackColor = true;
            // 
            // comboChannels
            // 
            this.comboChannels.AllowDrop = true;
            this.comboChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboChannels.FormattingEnabled = true;
            this.comboChannels.Location = new System.Drawing.Point(121, 84);
            this.comboChannels.Name = "comboChannels";
            this.comboChannels.Size = new System.Drawing.Size(484, 20);
            this.comboChannels.TabIndex = 16;
            // 
            // labelChannels
            // 
            this.labelChannels.AutoSize = true;
            this.labelChannels.Location = new System.Drawing.Point(22, 87);
            this.labelChannels.Name = "labelChannels";
            this.labelChannels.Size = new System.Drawing.Size(93, 12);
            this.labelChannels.TabIndex = 15;
            this.labelChannels.Text = "Default &channels:";
            // 
            // comboSampleRate
            // 
            this.comboSampleRate.AllowDrop = true;
            this.comboSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSampleRate.FormattingEnabled = true;
            this.comboSampleRate.Location = new System.Drawing.Point(121, 58);
            this.comboSampleRate.Name = "comboSampleRate";
            this.comboSampleRate.Size = new System.Drawing.Size(484, 20);
            this.comboSampleRate.TabIndex = 14;
            // 
            // labelSampleRate
            // 
            this.labelSampleRate.AutoSize = true;
            this.labelSampleRate.Location = new System.Drawing.Point(7, 61);
            this.labelSampleRate.Name = "labelSampleRate";
            this.labelSampleRate.Size = new System.Drawing.Size(108, 12);
            this.labelSampleRate.TabIndex = 13;
            this.labelSampleRate.Text = "Default &sample rate:";
            // 
            // Preferences
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(643, 465);
            this.Controls.Add(this.mTab);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.MinimumSize = new System.Drawing.Size(8, 162);
            this.Name = "Preferences";
            this.Text = "Edit preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.mTab.ResumeLayout(false);
            this.mProjectTab.ResumeLayout(false);
            this.mProjectTab.PerformLayout();
            this.mAudioTab.ResumeLayout(false);
            this.mAudioTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mTemplateBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mDirectoryBox;
        private System.Windows.Forms.Button mBrowseButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label labelInputDeviceName;
        private System.Windows.Forms.ComboBox comboInputDevice;
        private System.Windows.Forms.Label labelOutputDeviceName;
        private System.Windows.Forms.ComboBox comboOutputDevice;
        private System.Windows.Forms.TabControl mTab;
        private System.Windows.Forms.TabPage mProjectTab;
        private System.Windows.Forms.TabPage mAudioTab;
        private System.Windows.Forms.Label labelSampleRate;
        private System.Windows.Forms.ComboBox comboSampleRate;
        private System.Windows.Forms.ComboBox comboChannels;
        private System.Windows.Forms.Label labelChannels;
    }
}