namespace Obi.Dialogs
{
    partial class ObiConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObiConfiguration));
            this.labelOutputDeviceName = new System.Windows.Forms.Label();
            this.labelInputDeviceName = new System.Windows.Forms.Label();
            this.m_lblShortcutsProfile = new System.Windows.Forms.Label();
            this.m_lblSelectProfile = new System.Windows.Forms.Label();
            this.m_Ok = new System.Windows.Forms.Button();
            this.m_cb_InputDevice = new System.Windows.Forms.ComboBox();
            this.m_cb_OutPutDevice = new System.Windows.Forms.ComboBox();
            this.m_cb_SelectProfile = new System.Windows.Forms.ComboBox();
            this.m_cb_SelectShortcutsProfile = new System.Windows.Forms.ComboBox();
            this.m_tb_ObiConfigInstructions = new System.Windows.Forms.TextBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_DirectoryTextbox = new System.Windows.Forms.TextBox();
            this.m_BrowseButton = new System.Windows.Forms.Button();
            this.m_lblDefaultProjectDirectoryTemp = new System.Windows.Forms.Label();
            this.m_lblDefaultProjectDirectory = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelOutputDeviceName
            // 
            resources.ApplyResources(this.labelOutputDeviceName, "labelOutputDeviceName");
            this.labelOutputDeviceName.Name = "labelOutputDeviceName";
            // 
            // labelInputDeviceName
            // 
            resources.ApplyResources(this.labelInputDeviceName, "labelInputDeviceName");
            this.labelInputDeviceName.Name = "labelInputDeviceName";
            // 
            // m_lblShortcutsProfile
            // 
            resources.ApplyResources(this.m_lblShortcutsProfile, "m_lblShortcutsProfile");
            this.m_lblShortcutsProfile.Name = "m_lblShortcutsProfile";
            // 
            // m_lblSelectProfile
            // 
            resources.ApplyResources(this.m_lblSelectProfile, "m_lblSelectProfile");
            this.m_lblSelectProfile.Name = "m_lblSelectProfile";
            // 
            // m_Ok
            // 
            this.m_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_Ok, "m_Ok");
            this.m_Ok.Name = "m_Ok";
            this.m_Ok.UseVisualStyleBackColor = true;
            this.m_Ok.Click += new System.EventHandler(this.m_Ok_Click);
            // 
            // m_cb_InputDevice
            // 
            this.m_cb_InputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_cb_InputDevice, "m_cb_InputDevice");
            this.m_cb_InputDevice.FormattingEnabled = true;
            this.m_cb_InputDevice.Name = "m_cb_InputDevice";
            // 
            // m_cb_OutPutDevice
            // 
            this.m_cb_OutPutDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_cb_OutPutDevice, "m_cb_OutPutDevice");
            this.m_cb_OutPutDevice.FormattingEnabled = true;
            this.m_cb_OutPutDevice.Name = "m_cb_OutPutDevice";
            // 
            // m_cb_SelectProfile
            // 
            this.m_cb_SelectProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_cb_SelectProfile, "m_cb_SelectProfile");
            this.m_cb_SelectProfile.FormattingEnabled = true;
            this.m_cb_SelectProfile.Name = "m_cb_SelectProfile";
            // 
            // m_cb_SelectShortcutsProfile
            // 
            this.m_cb_SelectShortcutsProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_cb_SelectShortcutsProfile, "m_cb_SelectShortcutsProfile");
            this.m_cb_SelectShortcutsProfile.FormattingEnabled = true;
            this.m_cb_SelectShortcutsProfile.Name = "m_cb_SelectShortcutsProfile";
            // 
            // m_tb_ObiConfigInstructions
            // 
            resources.ApplyResources(this.m_tb_ObiConfigInstructions, "m_tb_ObiConfigInstructions");
            this.m_tb_ObiConfigInstructions.Name = "m_tb_ObiConfigInstructions";
            this.m_tb_ObiConfigInstructions.ReadOnly = true;
            // 
            // m_DirectoryTextbox
            // 
            resources.ApplyResources(this.m_DirectoryTextbox, "m_DirectoryTextbox");
            this.m_DirectoryTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_DirectoryTextbox.Name = "m_DirectoryTextbox";
            this.m_DirectoryTextbox.ReadOnly = true;
            this.helpProvider1.SetShowHelp(this.m_DirectoryTextbox, ((bool)(resources.GetObject("m_DirectoryTextbox.ShowHelp"))));
            // 
            // m_BrowseButton
            // 
            resources.ApplyResources(this.m_BrowseButton, "m_BrowseButton");
            this.m_BrowseButton.Name = "m_BrowseButton";
            this.helpProvider1.SetShowHelp(this.m_BrowseButton, ((bool)(resources.GetObject("m_BrowseButton.ShowHelp"))));
            this.m_BrowseButton.UseVisualStyleBackColor = true;
            this.m_BrowseButton.Click += new System.EventHandler(this.m_BrowseButton_Click);
            // 
            // m_lblDefaultProjectDirectoryTemp
            // 
            resources.ApplyResources(this.m_lblDefaultProjectDirectoryTemp, "m_lblDefaultProjectDirectoryTemp");
            this.m_lblDefaultProjectDirectoryTemp.Name = "m_lblDefaultProjectDirectoryTemp";
            // 
            // m_lblDefaultProjectDirectory
            // 
            resources.ApplyResources(this.m_lblDefaultProjectDirectory, "m_lblDefaultProjectDirectory");
            this.m_lblDefaultProjectDirectory.Name = "m_lblDefaultProjectDirectory";
            // 
            // ObiConfiguration
            // 
            this.AcceptButton = this.m_Ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.m_lblDefaultProjectDirectory);
            this.Controls.Add(this.m_BrowseButton);
            this.Controls.Add(this.m_DirectoryTextbox);
            this.Controls.Add(this.m_lblDefaultProjectDirectoryTemp);
            this.Controls.Add(this.m_tb_ObiConfigInstructions);
            this.Controls.Add(this.m_cb_SelectShortcutsProfile);
            this.Controls.Add(this.m_cb_SelectProfile);
            this.Controls.Add(this.m_cb_OutPutDevice);
            this.Controls.Add(this.m_cb_InputDevice);
            this.Controls.Add(this.m_Ok);
            this.Controls.Add(this.m_lblShortcutsProfile);
            this.Controls.Add(this.m_lblSelectProfile);
            this.Controls.Add(this.labelOutputDeviceName);
            this.Controls.Add(this.labelInputDeviceName);
            this.MaximizeBox = false;
            this.Name = "ObiConfiguration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelOutputDeviceName;
        private System.Windows.Forms.Label labelInputDeviceName;
        private System.Windows.Forms.Label m_lblShortcutsProfile;
        private System.Windows.Forms.Label m_lblSelectProfile;
        private System.Windows.Forms.Button m_Ok;
        private System.Windows.Forms.ComboBox m_cb_InputDevice;
        private System.Windows.Forms.ComboBox m_cb_OutPutDevice;
        private System.Windows.Forms.ComboBox m_cb_SelectProfile;
        private System.Windows.Forms.ComboBox m_cb_SelectShortcutsProfile;
        private System.Windows.Forms.TextBox m_tb_ObiConfigInstructions;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Label m_lblDefaultProjectDirectoryTemp;
        private System.Windows.Forms.TextBox m_DirectoryTextbox;
        private System.Windows.Forms.Button m_BrowseButton;
        private System.Windows.Forms.Label m_lblDefaultProjectDirectory;
    }
}