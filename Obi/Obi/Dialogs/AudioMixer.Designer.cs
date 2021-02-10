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
            this.m_cbStreamDuration = new System.Windows.Forms.CheckBox();
            this.m_DurationOfMixingAudioNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.m_cbDuationOfMixingAudio = new System.Windows.Forms.CheckBox();
            this.m_cblSelectSecondAudioForMixing = new System.Windows.Forms.CheckBox();
            this.m_txtSelectSecondAudioForMixing = new System.Windows.Forms.TextBox();
            this.m_btnBrowseSecondAudio = new System.Windows.Forms.Button();
            this.m_lblWeightOfSecondAudioSound = new System.Windows.Forms.Label();
            this.m_WeightOfSecondAudioSoundNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.m_txtNote = new System.Windows.Forms.TextBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            ((System.ComponentModel.ISupportInitialize)(this.m_WeightOfSoundNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_DropoutTransitionNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_DurationOfMixingAudioNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_WeightOfSecondAudioSoundNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lblSelectAudioForMixing
            // 
            resources.ApplyResources(this.m_lblSelectAudioForMixing, "m_lblSelectAudioForMixing");
            this.m_lblSelectAudioForMixing.Name = "m_lblSelectAudioForMixing";
            this.helpProvider1.SetShowHelp(this.m_lblSelectAudioForMixing, ((bool)(resources.GetObject("m_lblSelectAudioForMixing.ShowHelp"))));
            // 
            // m_btnBrowse
            // 
            resources.ApplyResources(this.m_btnBrowse, "m_btnBrowse");
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.helpProvider1.SetShowHelp(this.m_btnBrowse, ((bool)(resources.GetObject("m_btnBrowse.ShowHelp"))));
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.m_btnBrowse_Click);
            // 
            // m_txtSelectAudioForMixing
            // 
            resources.ApplyResources(this.m_txtSelectAudioForMixing, "m_txtSelectAudioForMixing");
            this.m_txtSelectAudioForMixing.Name = "m_txtSelectAudioForMixing";
            this.helpProvider1.SetShowHelp(this.m_txtSelectAudioForMixing, ((bool)(resources.GetObject("m_txtSelectAudioForMixing.ShowHelp"))));
            // 
            // m_lblWeightOfSound
            // 
            resources.ApplyResources(this.m_lblWeightOfSound, "m_lblWeightOfSound");
            this.m_lblWeightOfSound.Name = "m_lblWeightOfSound";
            this.helpProvider1.SetShowHelp(this.m_lblWeightOfSound, ((bool)(resources.GetObject("m_lblWeightOfSound.ShowHelp"))));
            // 
            // m_WeightOfSoundNumericUpDown
            // 
            resources.ApplyResources(this.m_WeightOfSoundNumericUpDown, "m_WeightOfSoundNumericUpDown");
            this.m_WeightOfSoundNumericUpDown.DecimalPlaces = 2;
            this.m_WeightOfSoundNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.m_WeightOfSoundNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_WeightOfSoundNumericUpDown.Name = "m_WeightOfSoundNumericUpDown";
            this.helpProvider1.SetShowHelp(this.m_WeightOfSoundNumericUpDown, ((bool)(resources.GetObject("m_WeightOfSoundNumericUpDown.ShowHelp"))));
            this.m_WeightOfSoundNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // m_btnOK
            // 
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btnOK, "m_btnOK");
            this.m_btnOK.Name = "m_btnOK";
            this.helpProvider1.SetShowHelp(this.m_btnOK, ((bool)(resources.GetObject("m_btnOK.ShowHelp"))));
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.Name = "m_btnCancel";
            this.helpProvider1.SetShowHelp(this.m_btnCancel, ((bool)(resources.GetObject("m_btnCancel.ShowHelp"))));
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_lblDropoutTransition
            // 
            resources.ApplyResources(this.m_lblDropoutTransition, "m_lblDropoutTransition");
            this.m_lblDropoutTransition.Name = "m_lblDropoutTransition";
            this.helpProvider1.SetShowHelp(this.m_lblDropoutTransition, ((bool)(resources.GetObject("m_lblDropoutTransition.ShowHelp"))));
            // 
            // m_DropoutTransitionNumericUpDown
            // 
            resources.ApplyResources(this.m_DropoutTransitionNumericUpDown, "m_DropoutTransitionNumericUpDown");
            this.m_DropoutTransitionNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_DropoutTransitionNumericUpDown.Name = "m_DropoutTransitionNumericUpDown";
            this.helpProvider1.SetShowHelp(this.m_DropoutTransitionNumericUpDown, ((bool)(resources.GetObject("m_DropoutTransitionNumericUpDown.ShowHelp"))));
            this.m_DropoutTransitionNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // m_cbStreamDuration
            // 
            resources.ApplyResources(this.m_cbStreamDuration, "m_cbStreamDuration");
            this.m_cbStreamDuration.Name = "m_cbStreamDuration";
            this.helpProvider1.SetShowHelp(this.m_cbStreamDuration, ((bool)(resources.GetObject("m_cbStreamDuration.ShowHelp"))));
            this.m_cbStreamDuration.UseVisualStyleBackColor = true;
            this.m_cbStreamDuration.CheckedChanged += new System.EventHandler(this.m_cbStreamDuration_CheckedChanged);
            // 
            // m_DurationOfMixingAudioNumericUpDown
            // 
            resources.ApplyResources(this.m_DurationOfMixingAudioNumericUpDown, "m_DurationOfMixingAudioNumericUpDown");
            this.m_DurationOfMixingAudioNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.m_DurationOfMixingAudioNumericUpDown.Name = "m_DurationOfMixingAudioNumericUpDown";
            this.helpProvider1.SetShowHelp(this.m_DurationOfMixingAudioNumericUpDown, ((bool)(resources.GetObject("m_DurationOfMixingAudioNumericUpDown.ShowHelp"))));
            this.m_DurationOfMixingAudioNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // m_cbDuationOfMixingAudio
            // 
            resources.ApplyResources(this.m_cbDuationOfMixingAudio, "m_cbDuationOfMixingAudio");
            this.m_cbDuationOfMixingAudio.Name = "m_cbDuationOfMixingAudio";
            this.helpProvider1.SetShowHelp(this.m_cbDuationOfMixingAudio, ((bool)(resources.GetObject("m_cbDuationOfMixingAudio.ShowHelp"))));
            this.m_cbDuationOfMixingAudio.UseVisualStyleBackColor = true;
            this.m_cbDuationOfMixingAudio.CheckedChanged += new System.EventHandler(this.m_cbDuationOfMixingAudio_CheckedChanged);
            // 
            // m_cblSelectSecondAudioForMixing
            // 
            resources.ApplyResources(this.m_cblSelectSecondAudioForMixing, "m_cblSelectSecondAudioForMixing");
            this.m_cblSelectSecondAudioForMixing.Name = "m_cblSelectSecondAudioForMixing";
            this.helpProvider1.SetShowHelp(this.m_cblSelectSecondAudioForMixing, ((bool)(resources.GetObject("m_cblSelectSecondAudioForMixing.ShowHelp"))));
            this.m_cblSelectSecondAudioForMixing.UseVisualStyleBackColor = true;
            this.m_cblSelectSecondAudioForMixing.CheckedChanged += new System.EventHandler(this.m_cblSelectSecondAudioForMixing_CheckedChanged);
            // 
            // m_txtSelectSecondAudioForMixing
            // 
            resources.ApplyResources(this.m_txtSelectSecondAudioForMixing, "m_txtSelectSecondAudioForMixing");
            this.m_txtSelectSecondAudioForMixing.Name = "m_txtSelectSecondAudioForMixing";
            this.helpProvider1.SetShowHelp(this.m_txtSelectSecondAudioForMixing, ((bool)(resources.GetObject("m_txtSelectSecondAudioForMixing.ShowHelp"))));
            // 
            // m_btnBrowseSecondAudio
            // 
            resources.ApplyResources(this.m_btnBrowseSecondAudio, "m_btnBrowseSecondAudio");
            this.m_btnBrowseSecondAudio.Name = "m_btnBrowseSecondAudio";
            this.helpProvider1.SetShowHelp(this.m_btnBrowseSecondAudio, ((bool)(resources.GetObject("m_btnBrowseSecondAudio.ShowHelp"))));
            this.m_btnBrowseSecondAudio.UseVisualStyleBackColor = true;
            this.m_btnBrowseSecondAudio.Click += new System.EventHandler(this.m_btnBrowseSecondAudio_Click);
            // 
            // m_lblWeightOfSecondAudioSound
            // 
            resources.ApplyResources(this.m_lblWeightOfSecondAudioSound, "m_lblWeightOfSecondAudioSound");
            this.m_lblWeightOfSecondAudioSound.Name = "m_lblWeightOfSecondAudioSound";
            this.helpProvider1.SetShowHelp(this.m_lblWeightOfSecondAudioSound, ((bool)(resources.GetObject("m_lblWeightOfSecondAudioSound.ShowHelp"))));
            // 
            // m_WeightOfSecondAudioSoundNumericUpDown
            // 
            resources.ApplyResources(this.m_WeightOfSecondAudioSoundNumericUpDown, "m_WeightOfSecondAudioSoundNumericUpDown");
            this.m_WeightOfSecondAudioSoundNumericUpDown.DecimalPlaces = 2;
            this.m_WeightOfSecondAudioSoundNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.m_WeightOfSecondAudioSoundNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_WeightOfSecondAudioSoundNumericUpDown.Name = "m_WeightOfSecondAudioSoundNumericUpDown";
            this.helpProvider1.SetShowHelp(this.m_WeightOfSecondAudioSoundNumericUpDown, ((bool)(resources.GetObject("m_WeightOfSecondAudioSoundNumericUpDown.ShowHelp"))));
            this.m_WeightOfSecondAudioSoundNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // m_txtNote
            // 
            resources.ApplyResources(this.m_txtNote, "m_txtNote");
            this.m_txtNote.Name = "m_txtNote";
            this.m_txtNote.ReadOnly = true;
            this.helpProvider1.SetShowHelp(this.m_txtNote, ((bool)(resources.GetObject("m_txtNote.ShowHelp"))));
            // 
            // AudioMixer
            // 
            this.AcceptButton = this.m_btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.m_txtNote);
            this.Controls.Add(this.m_WeightOfSecondAudioSoundNumericUpDown);
            this.Controls.Add(this.m_lblWeightOfSecondAudioSound);
            this.Controls.Add(this.m_btnBrowseSecondAudio);
            this.Controls.Add(this.m_txtSelectSecondAudioForMixing);
            this.Controls.Add(this.m_cblSelectSecondAudioForMixing);
            this.Controls.Add(this.m_cbDuationOfMixingAudio);
            this.Controls.Add(this.m_DurationOfMixingAudioNumericUpDown);
            this.Controls.Add(this.m_cbStreamDuration);
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
            this.helpProvider1.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AudioMixer_FormClosing);
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
        private System.Windows.Forms.CheckBox m_cbStreamDuration;
        private System.Windows.Forms.NumericUpDown m_DurationOfMixingAudioNumericUpDown;
        private System.Windows.Forms.CheckBox m_cbDuationOfMixingAudio;
        private System.Windows.Forms.CheckBox m_cblSelectSecondAudioForMixing;
        private System.Windows.Forms.TextBox m_txtSelectSecondAudioForMixing;
        private System.Windows.Forms.Button m_btnBrowseSecondAudio;
        private System.Windows.Forms.Label m_lblWeightOfSecondAudioSound;
        private System.Windows.Forms.NumericUpDown m_WeightOfSecondAudioSoundNumericUpDown;
        private System.Windows.Forms.TextBox m_txtNote;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}