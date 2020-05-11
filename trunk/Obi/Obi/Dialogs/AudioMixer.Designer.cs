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
            ((System.ComponentModel.ISupportInitialize)(this.m_WeightOfSoundNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_DropoutTransitionNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblSelectAudioForMixing
            // 
            this.m_lblSelectAudioForMixing.AutoSize = true;
            this.m_lblSelectAudioForMixing.Location = new System.Drawing.Point(32, 35);
            this.m_lblSelectAudioForMixing.Name = "m_lblSelectAudioForMixing";
            this.m_lblSelectAudioForMixing.Size = new System.Drawing.Size(116, 13);
            this.m_lblSelectAudioForMixing.TabIndex = 0;
            this.m_lblSelectAudioForMixing.Text = "Select audio for mixing ";
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnBrowse.Location = new System.Drawing.Point(357, 30);
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
            this.m_txtSelectAudioForMixing.Location = new System.Drawing.Point(154, 30);
            this.m_txtSelectAudioForMixing.Name = "m_txtSelectAudioForMixing";
            this.m_txtSelectAudioForMixing.Size = new System.Drawing.Size(197, 20);
            this.m_txtSelectAudioForMixing.TabIndex = 1;
            // 
            // m_lblWeightOfSound
            // 
            this.m_lblWeightOfSound.AutoSize = true;
            this.m_lblWeightOfSound.Location = new System.Drawing.Point(32, 90);
            this.m_lblWeightOfSound.Name = "m_lblWeightOfSound";
            this.m_lblWeightOfSound.Size = new System.Drawing.Size(82, 13);
            this.m_lblWeightOfSound.TabIndex = 3;
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
            this.m_WeightOfSoundNumericUpDown.Location = new System.Drawing.Point(154, 83);
            this.m_WeightOfSoundNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_WeightOfSoundNumericUpDown.Name = "m_WeightOfSoundNumericUpDown";
            this.m_WeightOfSoundNumericUpDown.Size = new System.Drawing.Size(58, 20);
            this.m_WeightOfSoundNumericUpDown.TabIndex = 4;
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
            this.m_btnOK.Location = new System.Drawing.Point(134, 204);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 9;
            this.m_btnOK.Text = "&OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Location = new System.Drawing.Point(265, 204);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 10;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_lblDropoutTransition
            // 
            this.m_lblDropoutTransition.AutoSize = true;
            this.m_lblDropoutTransition.Location = new System.Drawing.Point(32, 132);
            this.m_lblDropoutTransition.Name = "m_lblDropoutTransition";
            this.m_lblDropoutTransition.Size = new System.Drawing.Size(94, 13);
            this.m_lblDropoutTransition.TabIndex = 5;
            this.m_lblDropoutTransition.Text = "Dropout Transition";
            // 
            // m_DropoutTransitionNumericUpDown
            // 
            this.m_DropoutTransitionNumericUpDown.Location = new System.Drawing.Point(154, 127);
            this.m_DropoutTransitionNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_DropoutTransitionNumericUpDown.Name = "m_DropoutTransitionNumericUpDown";
            this.m_DropoutTransitionNumericUpDown.Size = new System.Drawing.Size(58, 20);
            this.m_DropoutTransitionNumericUpDown.TabIndex = 6;
            this.m_DropoutTransitionNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // m_lblSeconds
            // 
            this.m_lblSeconds.AutoSize = true;
            this.m_lblSeconds.Location = new System.Drawing.Point(218, 129);
            this.m_lblSeconds.Name = "m_lblSeconds";
            this.m_lblSeconds.Size = new System.Drawing.Size(49, 13);
            this.m_lblSeconds.TabIndex = 7;
            this.m_lblSeconds.Text = "Seconds";
            // 
            // m_cbStreamDuration
            // 
            this.m_cbStreamDuration.AutoSize = true;
            this.m_cbStreamDuration.Location = new System.Drawing.Point(35, 165);
            this.m_cbStreamDuration.Name = "m_cbStreamDuration";
            this.m_cbStreamDuration.Size = new System.Drawing.Size(238, 17);
            this.m_cbStreamDuration.TabIndex = 8;
            this.m_cbStreamDuration.Text = "Set end of stream duration to phrase duration";
            this.m_cbStreamDuration.UseVisualStyleBackColor = true;
            // 
            // AudioMixer
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 261);
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
    }
}