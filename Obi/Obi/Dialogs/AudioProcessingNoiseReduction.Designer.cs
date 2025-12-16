namespace Obi.Dialogs
{
    partial class AudioProcessingNoiseReduction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioProcessingNoiseReduction));
            m_btn_Ok = new System.Windows.Forms.Button();
            m_btn_Cancel = new System.Windows.Forms.Button();
            m_lblNoiseReduction = new System.Windows.Forms.Label();
            m_SetNoiseReduction = new System.Windows.Forms.NumericUpDown();
            m_SetNoiseLevelInPercent = new System.Windows.Forms.NumericUpDown();
            m_lblNoiseLevel = new System.Windows.Forms.Label();
            m_SetNoiseLevelTrackBar = new System.Windows.Forms.TrackBar();
            m_lblPercent = new System.Windows.Forms.Label();
            m_SetNoiseReductionTrackBar = new System.Windows.Forms.TrackBar();
            m_lbdB = new System.Windows.Forms.Label();
            m_SelectPresetComboBox = new System.Windows.Forms.ComboBox();
            m_lblPreset = new System.Windows.Forms.Label();
            helpProvider1 = new System.Windows.Forms.HelpProvider();
            m_ApplyOnWholeBook = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseReduction).BeginInit();
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseLevelInPercent).BeginInit();
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseLevelTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseReductionTrackBar).BeginInit();
            SuspendLayout();
            // 
            // m_btn_Ok
            // 
            m_btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(m_btn_Ok, "m_btn_Ok");
            m_btn_Ok.Name = "m_btn_Ok";
            m_btn_Ok.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(m_btn_Cancel, "m_btn_Cancel");
            m_btn_Cancel.Name = "m_btn_Cancel";
            m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // m_lblNoiseReduction
            // 
            resources.ApplyResources(m_lblNoiseReduction, "m_lblNoiseReduction");
            m_lblNoiseReduction.Name = "m_lblNoiseReduction";
            // 
            // m_SetNoiseReduction
            // 
            resources.ApplyResources(m_SetNoiseReduction, "m_SetNoiseReduction");
            m_SetNoiseReduction.DecimalPlaces = 2;
            m_SetNoiseReduction.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            m_SetNoiseReduction.Maximum = new decimal(new int[] { 97, 0, 0, 0 });
            m_SetNoiseReduction.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            m_SetNoiseReduction.Name = "m_SetNoiseReduction";
            m_SetNoiseReduction.Value = new decimal(new int[] { 50, 0, 0, 0 });
            m_SetNoiseReduction.ValueChanged += m_SetNoiseReduction_ValueChanged;
            // 
            // m_SetNoiseLevelInPercent
            // 
            resources.ApplyResources(m_SetNoiseLevelInPercent, "m_SetNoiseLevelInPercent");
            m_SetNoiseLevelInPercent.DecimalPlaces = 1;
            m_SetNoiseLevelInPercent.Name = "m_SetNoiseLevelInPercent";
            m_SetNoiseLevelInPercent.Value = new decimal(new int[] { 75, 0, 0, 0 });
            m_SetNoiseLevelInPercent.ValueChanged += m_SetNoiseLevelInPercent_ValueChanged;
            // 
            // m_lblNoiseLevel
            // 
            resources.ApplyResources(m_lblNoiseLevel, "m_lblNoiseLevel");
            m_lblNoiseLevel.Name = "m_lblNoiseLevel";
            // 
            // m_SetNoiseLevelTrackBar
            // 
            resources.ApplyResources(m_SetNoiseLevelTrackBar, "m_SetNoiseLevelTrackBar");
            m_SetNoiseLevelTrackBar.Maximum = 100;
            m_SetNoiseLevelTrackBar.Name = "m_SetNoiseLevelTrackBar";
            m_SetNoiseLevelTrackBar.TickFrequency = 6;
            m_SetNoiseLevelTrackBar.Value = 75;
            m_SetNoiseLevelTrackBar.ValueChanged += m_SetNoiseLevelTrackBar_ValueChanged;
            // 
            // m_lblPercent
            // 
            resources.ApplyResources(m_lblPercent, "m_lblPercent");
            m_lblPercent.Name = "m_lblPercent";
            // 
            // m_SetNoiseReductionTrackBar
            // 
            resources.ApplyResources(m_SetNoiseReductionTrackBar, "m_SetNoiseReductionTrackBar");
            m_SetNoiseReductionTrackBar.Maximum = 97;
            m_SetNoiseReductionTrackBar.Name = "m_SetNoiseReductionTrackBar";
            m_SetNoiseReductionTrackBar.TickFrequency = 10;
            m_SetNoiseReductionTrackBar.Value = 50;
            m_SetNoiseReductionTrackBar.ValueChanged += m_SetNoiseReductionTrackBar_ValueChanged;
            // 
            // m_lbdB
            // 
            resources.ApplyResources(m_lbdB, "m_lbdB");
            m_lbdB.Name = "m_lbdB";
            // 
            // m_SelectPresetComboBox
            // 
            resources.ApplyResources(m_SelectPresetComboBox, "m_SelectPresetComboBox");
            m_SelectPresetComboBox.FormattingEnabled = true;
            m_SelectPresetComboBox.Items.AddRange(new object[] { resources.GetString("m_SelectPresetComboBox.Items"), resources.GetString("m_SelectPresetComboBox.Items1"), resources.GetString("m_SelectPresetComboBox.Items2") });
            m_SelectPresetComboBox.Name = "m_SelectPresetComboBox";
            m_SelectPresetComboBox.SelectedIndexChanged += m_SelectPresetComboBox_SelectedIndexChanged;
            // 
            // m_lblPreset
            // 
            resources.ApplyResources(m_lblPreset, "m_lblPreset");
            m_lblPreset.Name = "m_lblPreset";
            // 
            // m_ApplyOnWholeBook
            // 
            resources.ApplyResources(m_ApplyOnWholeBook, "m_ApplyOnWholeBook");
            m_ApplyOnWholeBook.Name = "m_ApplyOnWholeBook";
            helpProvider1.SetShowHelp(m_ApplyOnWholeBook, (bool)resources.GetObject("m_ApplyOnWholeBook.ShowHelp"));
            m_ApplyOnWholeBook.UseVisualStyleBackColor = true;
            // 
            // AudioProcessingNoiseReduction
            // 
            AcceptButton = m_btn_Ok;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = m_btn_Cancel;
            Controls.Add(m_ApplyOnWholeBook);
            Controls.Add(m_lblPreset);
            Controls.Add(m_SelectPresetComboBox);
            Controls.Add(m_lbdB);
            Controls.Add(m_SetNoiseReductionTrackBar);
            Controls.Add(m_lblPercent);
            Controls.Add(m_SetNoiseLevelTrackBar);
            Controls.Add(m_lblNoiseLevel);
            Controls.Add(m_SetNoiseLevelInPercent);
            Controls.Add(m_SetNoiseReduction);
            Controls.Add(m_lblNoiseReduction);
            Controls.Add(m_btn_Cancel);
            Controls.Add(m_btn_Ok);
            Name = "AudioProcessingNoiseReduction";
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseReduction).EndInit();
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseLevelInPercent).EndInit();
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseLevelTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)m_SetNoiseReductionTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btn_Ok;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.Label m_lblNoiseReduction;
        private System.Windows.Forms.NumericUpDown m_SetNoiseReduction;
        private System.Windows.Forms.NumericUpDown m_SetNoiseLevelInPercent;
        private System.Windows.Forms.Label m_lblNoiseLevel;
        private System.Windows.Forms.TrackBar m_SetNoiseLevelTrackBar;
        private System.Windows.Forms.Label m_lblPercent;
        private System.Windows.Forms.TrackBar m_SetNoiseReductionTrackBar;
        private System.Windows.Forms.Label m_lbdB;
        private System.Windows.Forms.ComboBox m_SelectPresetComboBox;
        private System.Windows.Forms.Label m_lblPreset;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.CheckBox m_ApplyOnWholeBook;
    }
}