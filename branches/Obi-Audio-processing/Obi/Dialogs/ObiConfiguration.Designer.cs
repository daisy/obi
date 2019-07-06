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
            // ObiConfiguration
            // 
            this.AcceptButton = this.m_Ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
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
    }
}