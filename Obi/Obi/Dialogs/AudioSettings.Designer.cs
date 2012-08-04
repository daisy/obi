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
            this.mlbSampleRate = new System.Windows.Forms.Label();
            this.mlbAudioChannels = new System.Windows.Forms.Label();
            this.mbtnOK = new System.Windows.Forms.Button();
            this.mbtnCancel = new System.Windows.Forms.Button();
            this.mcbSampleRate = new System.Windows.Forms.ComboBox();
            this.mcbAudioChannel = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // mlbSampleRate
            // 
            this.mlbSampleRate.AutoSize = true;
            this.mlbSampleRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlbSampleRate.Location = new System.Drawing.Point(12, 20);
            this.mlbSampleRate.Name = "mlbSampleRate";
            this.mlbSampleRate.Size = new System.Drawing.Size(93, 16);
            this.mlbSampleRate.TabIndex = 0;
            this.mlbSampleRate.Text = "&Sample Rate :";
            // 
            // mlbAudioChannels
            // 
            this.mlbAudioChannels.AutoSize = true;
            this.mlbAudioChannels.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mlbAudioChannels.Location = new System.Drawing.Point(12, 46);
            this.mlbAudioChannels.Name = "mlbAudioChannels";
            this.mlbAudioChannels.Size = new System.Drawing.Size(108, 16);
            this.mlbAudioChannels.TabIndex = 2;
            this.mlbAudioChannels.Text = "Audio &Channels :";
            // 
            // mbtnOK
            // 
            this.mbtnOK.AccessibleDescription = "ok button";
            this.mbtnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mbtnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbtnOK.Location = new System.Drawing.Point(38, 77);
            this.mbtnOK.Name = "mbtnOK";
            this.mbtnOK.Size = new System.Drawing.Size(100, 30);
            this.mbtnOK.TabIndex = 4;
            this.mbtnOK.Text = "&OK";
            this.mbtnOK.UseVisualStyleBackColor = true;
            this.mbtnOK.Click += new System.EventHandler(this.mbtnOK_Click);
            // 
            // mbtnCancel
            // 
            this.mbtnCancel.AccessibleDescription = "Cancel button";
            this.mbtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mbtnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mbtnCancel.Location = new System.Drawing.Point(173, 77);
            this.mbtnCancel.Name = "mbtnCancel";
            this.mbtnCancel.Size = new System.Drawing.Size(100, 30);
            this.mbtnCancel.TabIndex = 5;
            this.mbtnCancel.Text = "&Cancel";
            this.mbtnCancel.UseVisualStyleBackColor = true;
            this.mbtnCancel.Click += new System.EventHandler(this.mbtnCancel_Click);
            // 
            // mcbSampleRate
            // 
            this.mcbSampleRate.AccessibleName = "Sample rate combo box";
            this.mcbSampleRate.AllowDrop = true;
            this.mcbSampleRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcbSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcbSampleRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mcbSampleRate.FormattingEnabled = true;
            this.mcbSampleRate.Items.AddRange(new object[] {
            "11025",
            "22050",
            "44100",
            "48000"});
            this.mcbSampleRate.Location = new System.Drawing.Point(126, 19);
            this.mcbSampleRate.Name = "mcbSampleRate";
            this.mcbSampleRate.Size = new System.Drawing.Size(147, 21);
            this.mcbSampleRate.TabIndex = 2;
            // 
            // mcbAudioChannel
            // 
            this.mcbAudioChannel.AccessibleName = "Audio channel combo box";
            this.mcbAudioChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mcbAudioChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mcbAudioChannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mcbAudioChannel.FormattingEnabled = true;
            this.mcbAudioChannel.Items.AddRange(new object[] {
            "mono",
            "stereo"});
            this.mcbAudioChannel.Location = new System.Drawing.Point(126, 46);
            this.mcbAudioChannel.Name = "mcbAudioChannel";
            this.mcbAudioChannel.Size = new System.Drawing.Size(147, 21);
            this.mcbAudioChannel.TabIndex = 4;
            // 
            // AudioSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 119);
            this.Controls.Add(this.mcbAudioChannel);
            this.Controls.Add(this.mcbSampleRate);
            this.Controls.Add(this.mbtnCancel);
            this.Controls.Add(this.mbtnOK);
            this.Controls.Add(this.mlbAudioChannels);
            this.Controls.Add(this.mlbSampleRate);
            this.MaximizeBox = false;
            this.Name = "AudioSettings";
            this.Text = "AudioSettings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mlbSampleRate;
        private System.Windows.Forms.Label mlbAudioChannels;
        private System.Windows.Forms.Button mbtnOK;
        private System.Windows.Forms.Button mbtnCancel;
        private System.Windows.Forms.ComboBox mcbSampleRate;
        private System.Windows.Forms.ComboBox mcbAudioChannel;
    }
}