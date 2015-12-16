namespace Obi.Dialogs
{
    partial class AudioSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioSettings));
            this.mlbSampleRate = new System.Windows.Forms.Label();
            this.mlbAudioChannels = new System.Windows.Forms.Label();
            this.mbtnOK = new System.Windows.Forms.Button();
            this.mcbSampleRate = new System.Windows.Forms.ComboBox();
            this.mcbAudioChannel = new System.Windows.Forms.ComboBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // mlbSampleRate
            // 
            resources.ApplyResources(this.mlbSampleRate, "mlbSampleRate");
            this.mlbSampleRate.Name = "mlbSampleRate";
            // 
            // mlbAudioChannels
            // 
            resources.ApplyResources(this.mlbAudioChannels, "mlbAudioChannels");
            this.mlbAudioChannels.Name = "mlbAudioChannels";
            // 
            // mbtnOK
            // 
            resources.ApplyResources(this.mbtnOK, "mbtnOK");
            this.mbtnOK.Name = "mbtnOK";
            this.mbtnOK.UseVisualStyleBackColor = true;
            this.mbtnOK.Click += new System.EventHandler(this.mbtnOK_Click);
            // 
            // mcbSampleRate
            // 
            resources.ApplyResources(this.mcbSampleRate, "mcbSampleRate");
            this.mcbSampleRate.AllowDrop = true;
            this.mcbSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcbSampleRate.FormattingEnabled = true;
            this.mcbSampleRate.Items.AddRange(new object[] {
            resources.GetString("mcbSampleRate.Items"),
            resources.GetString("mcbSampleRate.Items1"),
            resources.GetString("mcbSampleRate.Items2")});
            this.mcbSampleRate.Name = "mcbSampleRate";
            // 
            // mcbAudioChannel
            // 
            resources.ApplyResources(this.mcbAudioChannel, "mcbAudioChannel");
            this.mcbAudioChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcbAudioChannel.FormattingEnabled = true;
            this.mcbAudioChannel.Items.AddRange(new object[] {
            resources.GetString("mcbAudioChannel.Items"),
            resources.GetString("mcbAudioChannel.Items1")});
            this.mcbAudioChannel.Name = "mcbAudioChannel";
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // AudioSettings
            // 
            this.AcceptButton = this.mbtnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mcbAudioChannel);
            this.Controls.Add(this.mcbSampleRate);
            this.Controls.Add(this.mbtnOK);
            this.Controls.Add(this.mlbAudioChannels);
            this.Controls.Add(this.mlbSampleRate);
            this.MaximizeBox = false;
            this.Name = "AudioSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mlbSampleRate;
        private System.Windows.Forms.Label mlbAudioChannels;
        private System.Windows.Forms.Button mbtnOK;
        private System.Windows.Forms.ComboBox mcbSampleRate;
        private System.Windows.Forms.ComboBox mcbAudioChannel;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}