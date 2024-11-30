namespace Obi.Dialogs
{
    partial class AddAzureVoice
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
            this.m_AddAzureVoiceListBox = new System.Windows.Forms.ListBox();
            this.m_AddVoiceBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_AddAzureVoiceListBox
            // 
            this.m_AddAzureVoiceListBox.AccessibleName = "Voices available in Azure";
            this.m_AddAzureVoiceListBox.FormattingEnabled = true;
            this.m_AddAzureVoiceListBox.ItemHeight = 20;
            this.m_AddAzureVoiceListBox.Location = new System.Drawing.Point(133, 49);
            this.m_AddAzureVoiceListBox.Name = "m_AddAzureVoiceListBox";
            this.m_AddAzureVoiceListBox.Size = new System.Drawing.Size(457, 504);
            this.m_AddAzureVoiceListBox.TabIndex = 0;
            // 
            // m_AddVoiceBtn
            // 
            this.m_AddVoiceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_AddVoiceBtn.Location = new System.Drawing.Point(302, 599);
            this.m_AddVoiceBtn.Name = "m_AddVoiceBtn";
            this.m_AddVoiceBtn.Size = new System.Drawing.Size(94, 29);
            this.m_AddVoiceBtn.TabIndex = 1;
            this.m_AddVoiceBtn.Text = "&Add Voice";
            this.m_AddVoiceBtn.UseVisualStyleBackColor = true;
            this.m_AddVoiceBtn.Click += new System.EventHandler(this.m_AddVoiceBtn_Click);
            // 
            // AddAzureVoice
            // 
            this.AccessibleName = "Add Azure voice";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 659);
            this.Controls.Add(this.m_AddVoiceBtn);
            this.Controls.Add(this.m_AddAzureVoiceListBox);
            this.Name = "AddAzureVoice";
            this.Text = "Add Azure Voice";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox m_AddAzureVoiceListBox;
        private System.Windows.Forms.Button m_AddVoiceBtn;
    }
}