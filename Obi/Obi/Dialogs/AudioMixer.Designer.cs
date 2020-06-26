namespace Obi.Dialogs
{
    partial class AudioMixer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioMixer));
            this.m_lblSelectAudioForMixing = new System.Windows.Forms.Label();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_txtSelectAudioForMixing = new System.Windows.Forms.TextBox();
            this.m_lblWeightOfSound = new System.Windows.Forms.Label();
            this.m_WeightOfSoundNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblDropoutTransition = new System.Windows.Forms.Label();
            this.m_DropoutTransitionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.m_lblSeconds = new System.Windows.Forms.Label();
            this.m_cbStreamDuration = new System.Windows.Forms.CheckBox();
            this.m_lblSecondsDurationOfAudioMixing = new System.Windows.Forms.Label();
            this.m_DurationOfMixingAudioNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.m_cbDuationOfMixingAudio = new System.Windows.Forms.CheckBox();
            this.m_cblSelectSecondAudioForMixing = new System.Windows.Forms.CheckBox();
            this.m_txtSelectSecondAudioForMixing = new System.Windows.Forms.TextBox();
            this.m_btnBrowseSecondAudio = new System.Windows.Forms.Button();
            this.m_lblWeightOfSecondAudioSound = new System.Windows.Forms.Label();
            this.m_WeightOfSecondAudioSoundNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.m_txtNote = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_WeightOfSoundNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_DropoutTransitionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_DurationOfMixingAudioNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_WeightOfSecondAudioSoundNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblSelectAudioForMixing
            // 
            this.m_lblSelectAudioForMixing.AutoSize = true;
            this.m_lblSelectAudioForMixing.Location = new System.Drawing.Point(31, 33);
            this.m_lblSelectAudioForMixing.Name = "m_lblSelectAudioForMixing";
            this.m_lblSelectAudioForMixing.Size = new System.Drawing.Size(116, 13);
            this.m_lblSelectAudioForMixing.TabIndex = 0;
            this.m_lblSelectAudioForMixing.Text = "Select audio for mixing ";
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnBrowse.Location = new System.Drawing.Point(408, 30);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowse.TabIndex = 2;
            this.m_btnBrowse.Text = "Browse";
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.m_btnBrowse_Click);
            // 
            // m_txtSelectAudioForMixing
            // 
            this.m_txtSelectAudioForMixing.AccessibleName = "Select audio for mixing";
            this.m_txtSelectAudioForMixing.Location = new System.Drawing.Point(205, 30);
            this.m_txtSelectAudioForMixing.Name = "m_txtSelectAudioForMixing";
            this.m_txtSelectAudioForMixing.Size = new System.Drawing.Size(197, 20);
            this.m_txtSelectAudioForMixing.TabIndex = 1;
            // 
            // m_lblWeightOfSound
            // 
            this.m_lblWeightOfSound.AutoSize = true;
            this.m_lblWeightOfSound.Location = new System.Drawing.Point(32, 121);
            this.m_lblWeightOfSound.Name = "m_lblWeightOfSound";
            this.m_lblWeightOfSound.Size = new System.Drawing.Size(82, 13);
            this.m_lblWeightOfSound.TabIndex = 6;
            this.m_lblWeightOfSound.Text = "Weight of audio";
            // 
            // m_WeightOfSoundNumericUpDown
            // 
            this.m_WeightOfSoundNumericUpDown.AccessibleName = "Weight of audio";
            this.m_WeightOfSoundNumericUpDown.DecimalPlaces = 2;
            this.m_WeightOfSoundNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.m_WeightOfSoundNumericUpDown.Location = new System.Drawing.Point(134, 114);
            this.m_WeightOfSoundNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_WeightOfSoundNumericUpDown.Name = "m_WeightOfSoundNumericUpDown";
            this.m_WeightOfSoundNumericUpDown.Size = new System.Drawing.Size(58, 20);
            this.m_WeightOfSoundNumericUpDown.TabIndex = 7;
            this.m_WeightOfSoundNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOK.Location = new System.Drawing.Point(136, 434);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 18;
            this.m_btnOK.Text = "&OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Location = new System.Drawing.Point(267, 434);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 19;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_lblDropoutTransition
            // 
            this.m_lblDropoutTransition.AutoSize = true;
            this.m_lblDropoutTransition.Location = new System.Drawing.Point(32, 163);
            this.m_lblDropoutTransition.Name = "m_lblDropoutTransition";
            this.m_lblDropoutTransition.Size = new System.Drawing.Size(156, 13);
            this.m_lblDropoutTransition.TabIndex = 10;
            this.m_lblDropoutTransition.Text = "Dropout Transition (in Seconds)";
            // 
            // m_DropoutTransitionNumericUpDown
            // 
            this.m_DropoutTransitionNumericUpDown.AccessibleName = "";
            this.m_DropoutTransitionNumericUpDown.Location = new System.Drawing.Point(204, 161);
            this.m_DropoutTransitionNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_DropoutTransitionNumericUpDown.Name = "m_DropoutTransitionNumericUpDown";
            this.m_DropoutTransitionNumericUpDown.Size = new System.Drawing.Size(58, 20);
            this.m_DropoutTransitionNumericUpDown.TabIndex = 11;
            this.m_DropoutTransitionNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // m_lblSeconds
            // 
            this.m_lblSeconds.AutoSize = true;
            this.m_lblSeconds.Location = new System.Drawing.Point(268, 163);
            this.m_lblSeconds.Name = "m_lblSeconds";
            this.m_lblSeconds.Size = new System.Drawing.Size(49, 13);
            this.m_lblSeconds.TabIndex = 12;
            this.m_lblSeconds.Text = "Seconds";
            // 
            // m_cbStreamDuration
            // 
            this.m_cbStreamDuration.AutoSize = true;
            this.m_cbStreamDuration.Location = new System.Drawing.Point(35, 196);
            this.m_cbStreamDuration.Name = "m_cbStreamDuration";
            this.m_cbStreamDuration.Size = new System.Drawing.Size(238, 17);
            this.m_cbStreamDuration.TabIndex = 13;
            this.m_cbStreamDuration.Text = "Set end of stream duration to phrase duration";
            this.m_cbStreamDuration.UseVisualStyleBackColor = true;
            this.m_cbStreamDuration.CheckedChanged += new System.EventHandler(this.m_cbStreamDuration_CheckedChanged);
            // 
            // m_lblSecondsDurationOfAudioMixing
            // 
            this.m_lblSecondsDurationOfAudioMixing.AutoSize = true;
            this.m_lblSecondsDurationOfAudioMixing.Location = new System.Drawing.Point(346, 230);
            this.m_lblSecondsDurationOfAudioMixing.Name = "m_lblSecondsDurationOfAudioMixing";
            this.m_lblSecondsDurationOfAudioMixing.Size = new System.Drawing.Size(49, 13);
            this.m_lblSecondsDurationOfAudioMixing.TabIndex = 16;
            this.m_lblSecondsDurationOfAudioMixing.Text = "Seconds";
            // 
            // m_DurationOfMixingAudioNumericUpDown
            // 
            this.m_DurationOfMixingAudioNumericUpDown.AccessibleName = "Set duration of mixing audio after phrase ends (in seconds)";
            this.m_DurationOfMixingAudioNumericUpDown.Enabled = false;
            this.m_DurationOfMixingAudioNumericUpDown.Location = new System.Drawing.Point(282, 228);
            this.m_DurationOfMixingAudioNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.m_DurationOfMixingAudioNumericUpDown.Name = "m_DurationOfMixingAudioNumericUpDown";
            this.m_DurationOfMixingAudioNumericUpDown.Size = new System.Drawing.Size(58, 20);
            this.m_DurationOfMixingAudioNumericUpDown.TabIndex = 15;
            this.m_DurationOfMixingAudioNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // m_cbDuationOfMixingAudio
            // 
            this.m_cbDuationOfMixingAudio.AutoSize = true;
            this.m_cbDuationOfMixingAudio.Location = new System.Drawing.Point(35, 231);
            this.m_cbDuationOfMixingAudio.Name = "m_cbDuationOfMixingAudio";
            this.m_cbDuationOfMixingAudio.Size = new System.Drawing.Size(241, 17);
            this.m_cbDuationOfMixingAudio.TabIndex = 14;
            this.m_cbDuationOfMixingAudio.Text = "Set duration of mixing audio after phrase ends";
            this.m_cbDuationOfMixingAudio.UseVisualStyleBackColor = true;
            this.m_cbDuationOfMixingAudio.CheckedChanged += new System.EventHandler(this.m_cbDuationOfMixingAudio_CheckedChanged);
            // 
            // m_cblSelectSecondAudioForMixing
            // 
            this.m_cblSelectSecondAudioForMixing.AutoSize = true;
            this.m_cblSelectSecondAudioForMixing.Location = new System.Drawing.Point(12, 65);
            this.m_cblSelectSecondAudioForMixing.Name = "m_cblSelectSecondAudioForMixing";
            this.m_cblSelectSecondAudioForMixing.Size = new System.Drawing.Size(170, 17);
            this.m_cblSelectSecondAudioForMixing.TabIndex = 3;
            this.m_cblSelectSecondAudioForMixing.Text = "Select second audio for mixing";
            this.m_cblSelectSecondAudioForMixing.UseVisualStyleBackColor = true;
            this.m_cblSelectSecondAudioForMixing.CheckedChanged += new System.EventHandler(this.m_cblSelectSecondAudioForMixing_CheckedChanged);
            // 
            // m_txtSelectSecondAudioForMixing
            // 
            this.m_txtSelectSecondAudioForMixing.Enabled = false;
            this.m_txtSelectSecondAudioForMixing.Location = new System.Drawing.Point(205, 62);
            this.m_txtSelectSecondAudioForMixing.Name = "m_txtSelectSecondAudioForMixing";
            this.m_txtSelectSecondAudioForMixing.Size = new System.Drawing.Size(197, 20);
            this.m_txtSelectSecondAudioForMixing.TabIndex = 4;
            // 
            // m_btnBrowseSecondAudio
            // 
            this.m_btnBrowseSecondAudio.Enabled = false;
            this.m_btnBrowseSecondAudio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnBrowseSecondAudio.Location = new System.Drawing.Point(408, 60);
            this.m_btnBrowseSecondAudio.Name = "m_btnBrowseSecondAudio";
            this.m_btnBrowseSecondAudio.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseSecondAudio.TabIndex = 5;
            this.m_btnBrowseSecondAudio.Text = "Browse";
            this.m_btnBrowseSecondAudio.UseVisualStyleBackColor = true;
            this.m_btnBrowseSecondAudio.Click += new System.EventHandler(this.m_btnBrowseSecondAudio_Click);
            // 
            // m_lblWeightOfSecondAudioSound
            // 
            this.m_lblWeightOfSecondAudioSound.AutoSize = true;
            this.m_lblWeightOfSecondAudioSound.Location = new System.Drawing.Point(222, 121);
            this.m_lblWeightOfSecondAudioSound.Name = "m_lblWeightOfSecondAudioSound";
            this.m_lblWeightOfSecondAudioSound.Size = new System.Drawing.Size(120, 13);
            this.m_lblWeightOfSecondAudioSound.TabIndex = 8;
            this.m_lblWeightOfSecondAudioSound.Text = "Weight of second audio";
            this.m_lblWeightOfSecondAudioSound.Visible = false;
            // 
            // m_WeightOfSecondAudioSoundNumericUpDown
            // 
            this.m_WeightOfSecondAudioSoundNumericUpDown.AccessibleName = "Weight of second audio";
            this.m_WeightOfSecondAudioSoundNumericUpDown.DecimalPlaces = 2;
            this.m_WeightOfSecondAudioSoundNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.m_WeightOfSecondAudioSoundNumericUpDown.Location = new System.Drawing.Point(361, 114);
            this.m_WeightOfSecondAudioSoundNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_WeightOfSecondAudioSoundNumericUpDown.Name = "m_WeightOfSecondAudioSoundNumericUpDown";
            this.m_WeightOfSecondAudioSoundNumericUpDown.Size = new System.Drawing.Size(58, 20);
            this.m_WeightOfSecondAudioSoundNumericUpDown.TabIndex = 9;
            this.m_WeightOfSecondAudioSoundNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_WeightOfSecondAudioSoundNumericUpDown.Visible = false;
            // 
            // m_txtNote
            // 
            this.m_txtNote.Location = new System.Drawing.Point(12, 267);
            this.m_txtNote.Multiline = true;
            this.m_txtNote.Name = "m_txtNote";
            this.m_txtNote.ReadOnly = true;
            this.m_txtNote.Size = new System.Drawing.Size(471, 161);
            this.m_txtNote.TabIndex = 17;
            this.m_txtNote.Text = resources.GetString("m_txtNote.Text");
            // 
            // AudioMixer
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 469);
            this.Controls.Add(this.m_txtNote);
            this.Controls.Add(this.m_WeightOfSecondAudioSoundNumericUpDown);
            this.Controls.Add(this.m_lblWeightOfSecondAudioSound);
            this.Controls.Add(this.m_btnBrowseSecondAudio);
            this.Controls.Add(this.m_txtSelectSecondAudioForMixing);
            this.Controls.Add(this.m_cblSelectSecondAudioForMixing);
            this.Controls.Add(this.m_cbDuationOfMixingAudio);
            this.Controls.Add(this.m_lblSecondsDurationOfAudioMixing);
            this.Controls.Add(this.m_DurationOfMixingAudioNumericUpDown);
            this.Controls.Add(this.m_cbStreamDuration);
            this.Controls.Add(this.m_lblSeconds);
            this.Controls.Add(this.m_DropoutTransitionNumericUpDown);
            this.Controls.Add(this.m_lblDropoutTransition);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_WeightOfSoundNumericUpDown);
            this.Controls.Add(this.m_lblWeightOfSound);
            this.Controls.Add(this.m_txtSelectAudioForMixing);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_lblSelectAudioForMixing);
            this.Name = "AudioMixer";
            this.Text = "Audio Mixer";
            ((System.ComponentModel.ISupportInitialize)(this.m_WeightOfSoundNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_DropoutTransitionNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_DurationOfMixingAudioNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_WeightOfSecondAudioSoundNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblSelectAudioForMixing;
        private System.Windows.Forms.Button m_btnBrowse;
        private System.Windows.Forms.TextBox m_txtSelectAudioForMixing;
        private System.Windows.Forms.Label m_lblWeightOfSound;
        private System.Windows.Forms.NumericUpDown m_WeightOfSoundNumericUpDown;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblDropoutTransition;
        private System.Windows.Forms.NumericUpDown m_DropoutTransitionNumericUpDown;
        private System.Windows.Forms.Label m_lblSeconds;
        private System.Windows.Forms.CheckBox m_cbStreamDuration;
        private System.Windows.Forms.Label m_lblSecondsDurationOfAudioMixing;
        private System.Windows.Forms.NumericUpDown m_DurationOfMixingAudioNumericUpDown;
        private System.Windows.Forms.CheckBox m_cbDuationOfMixingAudio;
        private System.Windows.Forms.CheckBox m_cblSelectSecondAudioForMixing;
        private System.Windows.Forms.TextBox m_txtSelectSecondAudioForMixing;
        private System.Windows.Forms.Button m_btnBrowseSecondAudio;
        private System.Windows.Forms.Label m_lblWeightOfSecondAudioSound;
        private System.Windows.Forms.NumericUpDown m_WeightOfSecondAudioSoundNumericUpDown;
        private System.Windows.Forms.TextBox m_txtNote;
    }
}