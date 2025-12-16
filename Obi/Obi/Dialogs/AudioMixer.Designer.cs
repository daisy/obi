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
            m_lblSelectAudioForMixing = new System.Windows.Forms.Label();
            m_btnBrowse = new System.Windows.Forms.Button();
            m_txtSelectAudioForMixing = new System.Windows.Forms.TextBox();
            m_lblWeightOfSound = new System.Windows.Forms.Label();
            m_WeightOfSoundNumericUpDown = new System.Windows.Forms.NumericUpDown();
            m_btnOK = new System.Windows.Forms.Button();
            m_btnCancel = new System.Windows.Forms.Button();
            m_lblDropoutTransition = new System.Windows.Forms.Label();
            m_DropoutTransitionNumericUpDown = new System.Windows.Forms.NumericUpDown();
            m_cbStreamDuration = new System.Windows.Forms.CheckBox();
            m_DurationOfMixingAudioNumericUpDown = new System.Windows.Forms.NumericUpDown();
            m_cbDuationOfMixingAudio = new System.Windows.Forms.CheckBox();
            m_cblSelectSecondAudioForMixing = new System.Windows.Forms.CheckBox();
            m_txtSelectSecondAudioForMixing = new System.Windows.Forms.TextBox();
            m_btnBrowseSecondAudio = new System.Windows.Forms.Button();
            m_lblWeightOfSecondAudioSound = new System.Windows.Forms.Label();
            m_WeightOfSecondAudioSoundNumericUpDown = new System.Windows.Forms.NumericUpDown();
            m_txtNote = new System.Windows.Forms.TextBox();
            helpProvider1 = new System.Windows.Forms.HelpProvider();
            ((System.ComponentModel.ISupportInitialize)m_WeightOfSoundNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)m_DropoutTransitionNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)m_DurationOfMixingAudioNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)m_WeightOfSecondAudioSoundNumericUpDown).BeginInit();
            SuspendLayout();
            // 
            // m_lblSelectAudioForMixing
            // 
            resources.ApplyResources(m_lblSelectAudioForMixing, "m_lblSelectAudioForMixing");
            m_lblSelectAudioForMixing.Name = "m_lblSelectAudioForMixing";
            helpProvider1.SetShowHelp(m_lblSelectAudioForMixing, (bool)resources.GetObject("m_lblSelectAudioForMixing.ShowHelp"));
            // 
            // m_btnBrowse
            // 
            resources.ApplyResources(m_btnBrowse, "m_btnBrowse");
            m_btnBrowse.Name = "m_btnBrowse";
            helpProvider1.SetShowHelp(m_btnBrowse, (bool)resources.GetObject("m_btnBrowse.ShowHelp"));
            m_btnBrowse.UseVisualStyleBackColor = true;
            m_btnBrowse.Click += m_btnBrowse_Click;
            // 
            // m_txtSelectAudioForMixing
            // 
            resources.ApplyResources(m_txtSelectAudioForMixing, "m_txtSelectAudioForMixing");
            m_txtSelectAudioForMixing.Name = "m_txtSelectAudioForMixing";
            helpProvider1.SetShowHelp(m_txtSelectAudioForMixing, (bool)resources.GetObject("m_txtSelectAudioForMixing.ShowHelp"));
            // 
            // m_lblWeightOfSound
            // 
            resources.ApplyResources(m_lblWeightOfSound, "m_lblWeightOfSound");
            m_lblWeightOfSound.Name = "m_lblWeightOfSound";
            helpProvider1.SetShowHelp(m_lblWeightOfSound, (bool)resources.GetObject("m_lblWeightOfSound.ShowHelp"));
            // 
            // m_WeightOfSoundNumericUpDown
            // 
            resources.ApplyResources(m_WeightOfSoundNumericUpDown, "m_WeightOfSoundNumericUpDown");
            m_WeightOfSoundNumericUpDown.DecimalPlaces = 2;
            m_WeightOfSoundNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            m_WeightOfSoundNumericUpDown.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            m_WeightOfSoundNumericUpDown.Name = "m_WeightOfSoundNumericUpDown";
            helpProvider1.SetShowHelp(m_WeightOfSoundNumericUpDown, (bool)resources.GetObject("m_WeightOfSoundNumericUpDown.ShowHelp"));
            m_WeightOfSoundNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // m_btnOK
            // 
            m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(m_btnOK, "m_btnOK");
            m_btnOK.Name = "m_btnOK";
            helpProvider1.SetShowHelp(m_btnOK, (bool)resources.GetObject("m_btnOK.ShowHelp"));
            m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(m_btnCancel, "m_btnCancel");
            m_btnCancel.Name = "m_btnCancel";
            helpProvider1.SetShowHelp(m_btnCancel, (bool)resources.GetObject("m_btnCancel.ShowHelp"));
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // m_lblDropoutTransition
            // 
            resources.ApplyResources(m_lblDropoutTransition, "m_lblDropoutTransition");
            m_lblDropoutTransition.Name = "m_lblDropoutTransition";
            helpProvider1.SetShowHelp(m_lblDropoutTransition, (bool)resources.GetObject("m_lblDropoutTransition.ShowHelp"));
            // 
            // m_DropoutTransitionNumericUpDown
            // 
            resources.ApplyResources(m_DropoutTransitionNumericUpDown, "m_DropoutTransitionNumericUpDown");
            m_DropoutTransitionNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            m_DropoutTransitionNumericUpDown.Name = "m_DropoutTransitionNumericUpDown";
            helpProvider1.SetShowHelp(m_DropoutTransitionNumericUpDown, (bool)resources.GetObject("m_DropoutTransitionNumericUpDown.ShowHelp"));
            m_DropoutTransitionNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // m_cbStreamDuration
            // 
            resources.ApplyResources(m_cbStreamDuration, "m_cbStreamDuration");
            m_cbStreamDuration.Name = "m_cbStreamDuration";
            helpProvider1.SetShowHelp(m_cbStreamDuration, (bool)resources.GetObject("m_cbStreamDuration.ShowHelp"));
            m_cbStreamDuration.UseVisualStyleBackColor = true;
            m_cbStreamDuration.CheckedChanged += m_cbStreamDuration_CheckedChanged;
            // 
            // m_DurationOfMixingAudioNumericUpDown
            // 
            resources.ApplyResources(m_DurationOfMixingAudioNumericUpDown, "m_DurationOfMixingAudioNumericUpDown");
            m_DurationOfMixingAudioNumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            m_DurationOfMixingAudioNumericUpDown.Name = "m_DurationOfMixingAudioNumericUpDown";
            helpProvider1.SetShowHelp(m_DurationOfMixingAudioNumericUpDown, (bool)resources.GetObject("m_DurationOfMixingAudioNumericUpDown.ShowHelp"));
            m_DurationOfMixingAudioNumericUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // m_cbDuationOfMixingAudio
            // 
            resources.ApplyResources(m_cbDuationOfMixingAudio, "m_cbDuationOfMixingAudio");
            m_cbDuationOfMixingAudio.Name = "m_cbDuationOfMixingAudio";
            helpProvider1.SetShowHelp(m_cbDuationOfMixingAudio, (bool)resources.GetObject("m_cbDuationOfMixingAudio.ShowHelp"));
            m_cbDuationOfMixingAudio.UseVisualStyleBackColor = true;
            m_cbDuationOfMixingAudio.CheckedChanged += m_cbDuationOfMixingAudio_CheckedChanged;
            // 
            // m_cblSelectSecondAudioForMixing
            // 
            resources.ApplyResources(m_cblSelectSecondAudioForMixing, "m_cblSelectSecondAudioForMixing");
            m_cblSelectSecondAudioForMixing.Name = "m_cblSelectSecondAudioForMixing";
            helpProvider1.SetShowHelp(m_cblSelectSecondAudioForMixing, (bool)resources.GetObject("m_cblSelectSecondAudioForMixing.ShowHelp"));
            m_cblSelectSecondAudioForMixing.UseVisualStyleBackColor = true;
            m_cblSelectSecondAudioForMixing.CheckedChanged += m_cblSelectSecondAudioForMixing_CheckedChanged;
            // 
            // m_txtSelectSecondAudioForMixing
            // 
            resources.ApplyResources(m_txtSelectSecondAudioForMixing, "m_txtSelectSecondAudioForMixing");
            m_txtSelectSecondAudioForMixing.Name = "m_txtSelectSecondAudioForMixing";
            helpProvider1.SetShowHelp(m_txtSelectSecondAudioForMixing, (bool)resources.GetObject("m_txtSelectSecondAudioForMixing.ShowHelp"));
            // 
            // m_btnBrowseSecondAudio
            // 
            resources.ApplyResources(m_btnBrowseSecondAudio, "m_btnBrowseSecondAudio");
            m_btnBrowseSecondAudio.Name = "m_btnBrowseSecondAudio";
            helpProvider1.SetShowHelp(m_btnBrowseSecondAudio, (bool)resources.GetObject("m_btnBrowseSecondAudio.ShowHelp"));
            m_btnBrowseSecondAudio.UseVisualStyleBackColor = true;
            m_btnBrowseSecondAudio.Click += m_btnBrowseSecondAudio_Click;
            // 
            // m_lblWeightOfSecondAudioSound
            // 
            resources.ApplyResources(m_lblWeightOfSecondAudioSound, "m_lblWeightOfSecondAudioSound");
            m_lblWeightOfSecondAudioSound.Name = "m_lblWeightOfSecondAudioSound";
            helpProvider1.SetShowHelp(m_lblWeightOfSecondAudioSound, (bool)resources.GetObject("m_lblWeightOfSecondAudioSound.ShowHelp"));
            // 
            // m_WeightOfSecondAudioSoundNumericUpDown
            // 
            resources.ApplyResources(m_WeightOfSecondAudioSoundNumericUpDown, "m_WeightOfSecondAudioSoundNumericUpDown");
            m_WeightOfSecondAudioSoundNumericUpDown.DecimalPlaces = 2;
            m_WeightOfSecondAudioSoundNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            m_WeightOfSecondAudioSoundNumericUpDown.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            m_WeightOfSecondAudioSoundNumericUpDown.Name = "m_WeightOfSecondAudioSoundNumericUpDown";
            helpProvider1.SetShowHelp(m_WeightOfSecondAudioSoundNumericUpDown, (bool)resources.GetObject("m_WeightOfSecondAudioSoundNumericUpDown.ShowHelp"));
            m_WeightOfSecondAudioSoundNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // m_txtNote
            // 
            resources.ApplyResources(m_txtNote, "m_txtNote");
            m_txtNote.Name = "m_txtNote";
            m_txtNote.ReadOnly = true;
            helpProvider1.SetShowHelp(m_txtNote, (bool)resources.GetObject("m_txtNote.ShowHelp"));
            // 
            // AudioMixer
            // 
            AcceptButton = m_btnOK;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = m_btnCancel;
            Controls.Add(m_txtNote);
            Controls.Add(m_WeightOfSecondAudioSoundNumericUpDown);
            Controls.Add(m_lblWeightOfSecondAudioSound);
            Controls.Add(m_btnBrowseSecondAudio);
            Controls.Add(m_txtSelectSecondAudioForMixing);
            Controls.Add(m_cblSelectSecondAudioForMixing);
            Controls.Add(m_cbDuationOfMixingAudio);
            Controls.Add(m_DurationOfMixingAudioNumericUpDown);
            Controls.Add(m_cbStreamDuration);
            Controls.Add(m_DropoutTransitionNumericUpDown);
            Controls.Add(m_lblDropoutTransition);
            Controls.Add(m_btnCancel);
            Controls.Add(m_btnOK);
            Controls.Add(m_WeightOfSoundNumericUpDown);
            Controls.Add(m_lblWeightOfSound);
            Controls.Add(m_txtSelectAudioForMixing);
            Controls.Add(m_btnBrowse);
            Controls.Add(m_lblSelectAudioForMixing);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Name = "AudioMixer";
            helpProvider1.SetShowHelp(this, (bool)resources.GetObject("$this.ShowHelp"));
            FormClosing += AudioMixer_FormClosing;
            ((System.ComponentModel.ISupportInitialize)m_WeightOfSoundNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)m_DropoutTransitionNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)m_DurationOfMixingAudioNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)m_WeightOfSecondAudioSoundNumericUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();

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