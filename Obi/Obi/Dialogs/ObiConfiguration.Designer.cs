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
            this.labelOutputDeviceName.AutoSize = true;
            this.labelOutputDeviceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOutputDeviceName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelOutputDeviceName.Location = new System.Drawing.Point(34, 132);
            this.labelOutputDeviceName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelOutputDeviceName.Name = "labelOutputDeviceName";
            this.labelOutputDeviceName.Size = new System.Drawing.Size(122, 15);
            this.labelOutputDeviceName.TabIndex = 3;
            this.labelOutputDeviceName.Text = "O&utput device name :";
            // 
            // labelInputDeviceName
            // 
            this.labelInputDeviceName.AutoSize = true;
            this.labelInputDeviceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInputDeviceName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelInputDeviceName.Location = new System.Drawing.Point(43, 100);
            this.labelInputDeviceName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInputDeviceName.Name = "labelInputDeviceName";
            this.labelInputDeviceName.Size = new System.Drawing.Size(113, 15);
            this.labelInputDeviceName.TabIndex = 1;
            this.labelInputDeviceName.Text = "&Input device name :";
            // 
            // m_lblShortcutsProfile
            // 
            this.m_lblShortcutsProfile.AutoSize = true;
            this.m_lblShortcutsProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblShortcutsProfile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblShortcutsProfile.Location = new System.Drawing.Point(19, 202);
            this.m_lblShortcutsProfile.Name = "m_lblShortcutsProfile";
            this.m_lblShortcutsProfile.Size = new System.Drawing.Size(139, 15);
            this.m_lblShortcutsProfile.TabIndex = 7;
            this.m_lblShortcutsProfile.Text = "Select Shortcuts Profile :";
            // 
            // m_lblSelectProfile
            // 
            this.m_lblSelectProfile.AutoSize = true;
            this.m_lblSelectProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblSelectProfile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_lblSelectProfile.Location = new System.Drawing.Point(71, 171);
            this.m_lblSelectProfile.Name = "m_lblSelectProfile";
            this.m_lblSelectProfile.Size = new System.Drawing.Size(85, 15);
            this.m_lblSelectProfile.TabIndex = 5;
            this.m_lblSelectProfile.Text = "Select Profile :";
            // 
            // m_Ok
            // 
            this.m_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_Ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_Ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_Ok.Location = new System.Drawing.Point(262, 252);
            this.m_Ok.Name = "m_Ok";
            this.m_Ok.Size = new System.Drawing.Size(93, 25);
            this.m_Ok.TabIndex = 9;
            this.m_Ok.Text = "&OK";
            this.m_Ok.UseVisualStyleBackColor = true;
            this.m_Ok.Click += new System.EventHandler(this.m_Ok_Click);
            // 
            // m_cb_InputDevice
            // 
            this.m_cb_InputDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_cb_InputDevice.FormattingEnabled = true;
            this.m_cb_InputDevice.Location = new System.Drawing.Point(192, 91);
            this.m_cb_InputDevice.Name = "m_cb_InputDevice";
            this.m_cb_InputDevice.Size = new System.Drawing.Size(418, 21);
            this.m_cb_InputDevice.TabIndex = 2;
            // 
            // m_cb_OutPutDevice
            // 
            this.m_cb_OutPutDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_cb_OutPutDevice.FormattingEnabled = true;
            this.m_cb_OutPutDevice.Location = new System.Drawing.Point(192, 126);
            this.m_cb_OutPutDevice.Name = "m_cb_OutPutDevice";
            this.m_cb_OutPutDevice.Size = new System.Drawing.Size(418, 21);
            this.m_cb_OutPutDevice.TabIndex = 4;
            // 
            // m_cb_SelectProfile
            // 
            this.m_cb_SelectProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_cb_SelectProfile.FormattingEnabled = true;
            this.m_cb_SelectProfile.Location = new System.Drawing.Point(192, 164);
            this.m_cb_SelectProfile.Name = "m_cb_SelectProfile";
            this.m_cb_SelectProfile.Size = new System.Drawing.Size(418, 21);
            this.m_cb_SelectProfile.TabIndex = 6;
            // 
            // m_cb_SelectShortcutsProfile
            // 
            this.m_cb_SelectShortcutsProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_cb_SelectShortcutsProfile.FormattingEnabled = true;
            this.m_cb_SelectShortcutsProfile.Location = new System.Drawing.Point(192, 199);
            this.m_cb_SelectShortcutsProfile.Name = "m_cb_SelectShortcutsProfile";
            this.m_cb_SelectShortcutsProfile.Size = new System.Drawing.Size(418, 21);
            this.m_cb_SelectShortcutsProfile.TabIndex = 8;
            // 
            // m_tb_ObiConfigInstructions
            // 
            this.m_tb_ObiConfigInstructions.Location = new System.Drawing.Point(92, 12);
            this.m_tb_ObiConfigInstructions.Multiline = true;
            this.m_tb_ObiConfigInstructions.Name = "m_tb_ObiConfigInstructions";
            this.m_tb_ObiConfigInstructions.ReadOnly = true;
            this.m_tb_ObiConfigInstructions.Size = new System.Drawing.Size(449, 62);
            this.m_tb_ObiConfigInstructions.TabIndex = 10;
            // 
            // ObiConfiguration
            // 
            this.AcceptButton = this.m_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 292);
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
            this.Text = "Configure OBI";
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