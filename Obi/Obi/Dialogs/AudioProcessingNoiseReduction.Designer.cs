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
            this.m_btn_Ok = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_lblNoiseReduction = new System.Windows.Forms.Label();
            this.m_SetNoiseReduction = new System.Windows.Forms.NumericUpDown();
            this.m_SetNoiseLevelInPercent = new System.Windows.Forms.NumericUpDown();
            this.m_lblNoiseLevel = new System.Windows.Forms.Label();
            this.m_SetNoiseLevelTrackBar = new System.Windows.Forms.TrackBar();
            this.m_lblPercent = new System.Windows.Forms.Label();
            this.m_SetNoiseReductionTrackBar = new System.Windows.Forms.TrackBar();
            this.m_lbdB = new System.Windows.Forms.Label();
            this.m_SelectPresetComboBox = new System.Windows.Forms.ComboBox();
            this.m_lblPreset = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_ApplyOnWholeBook = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReduction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelInPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReductionTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btn_Ok
            // 
            this.m_btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btn_Ok, "m_btn_Ok");
            this.m_btn_Ok.Name = "m_btn_Ok";
            this.m_btn_Ok.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btn_Cancel, "m_btn_Cancel");
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // m_lblNoiseReduction
            // 
            resources.ApplyResources(this.m_lblNoiseReduction, "m_lblNoiseReduction");
            this.m_lblNoiseReduction.Name = "m_lblNoiseReduction";
            // 
            // m_SetNoiseReduction
            // 
            resources.ApplyResources(this.m_SetNoiseReduction, "m_SetNoiseReduction");
            this.m_SetNoiseReduction.DecimalPlaces = 2;
            this.m_SetNoiseReduction.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_SetNoiseReduction.Maximum = new decimal(new int[] {
            97,
            0,
            0,
            0});
            this.m_SetNoiseReduction.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.m_SetNoiseReduction.Name = "m_SetNoiseReduction";
            this.m_SetNoiseReduction.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.m_SetNoiseReduction.ValueChanged += new System.EventHandler(this.m_SetNoiseReduction_ValueChanged);
            // 
            // m_SetNoiseLevelInPercent
            // 
            resources.ApplyResources(this.m_SetNoiseLevelInPercent, "m_SetNoiseLevelInPercent");
            this.m_SetNoiseLevelInPercent.DecimalPlaces = 1;
            this.m_SetNoiseLevelInPercent.Name = "m_SetNoiseLevelInPercent";
            this.m_SetNoiseLevelInPercent.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            this.m_SetNoiseLevelInPercent.ValueChanged += new System.EventHandler(this.m_SetNoiseLevelInPercent_ValueChanged);
            // 
            // m_lblNoiseLevel
            // 
            resources.ApplyResources(this.m_lblNoiseLevel, "m_lblNoiseLevel");
            this.m_lblNoiseLevel.Name = "m_lblNoiseLevel";
            // 
            // m_SetNoiseLevelTrackBar
            // 
            resources.ApplyResources(this.m_SetNoiseLevelTrackBar, "m_SetNoiseLevelTrackBar");
            this.m_SetNoiseLevelTrackBar.Maximum = 100;
            this.m_SetNoiseLevelTrackBar.Name = "m_SetNoiseLevelTrackBar";
            this.m_SetNoiseLevelTrackBar.TickFrequency = 6;
            this.m_SetNoiseLevelTrackBar.Value = 75;
            this.m_SetNoiseLevelTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseLevelTrackBar_ValueChanged);
            // 
            // m_lblPercent
            // 
            resources.ApplyResources(this.m_lblPercent, "m_lblPercent");
            this.m_lblPercent.Name = "m_lblPercent";
            // 
            // m_SetNoiseReductionTrackBar
            // 
            resources.ApplyResources(this.m_SetNoiseReductionTrackBar, "m_SetNoiseReductionTrackBar");
            this.m_SetNoiseReductionTrackBar.Maximum = 97;
            this.m_SetNoiseReductionTrackBar.Name = "m_SetNoiseReductionTrackBar";
            this.m_SetNoiseReductionTrackBar.TickFrequency = 10;
            this.m_SetNoiseReductionTrackBar.Value = 50;
            this.m_SetNoiseReductionTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseReductionTrackBar_ValueChanged);
            // 
            // m_lbdB
            // 
            resources.ApplyResources(this.m_lbdB, "m_lbdB");
            this.m_lbdB.Name = "m_lbdB";
            // 
            // m_SelectPresetComboBox
            // 
            resources.ApplyResources(this.m_SelectPresetComboBox, "m_SelectPresetComboBox");
            this.m_SelectPresetComboBox.FormattingEnabled = true;
            this.m_SelectPresetComboBox.Items.AddRange(new object[] {
            resources.GetString("m_SelectPresetComboBox.Items"),
            resources.GetString("m_SelectPresetComboBox.Items1"),
            resources.GetString("m_SelectPresetComboBox.Items2")});
            this.m_SelectPresetComboBox.Name = "m_SelectPresetComboBox";
            this.m_SelectPresetComboBox.SelectedIndexChanged += new System.EventHandler(this.m_SelectPresetComboBox_SelectedIndexChanged);
            // 
            // m_lblPreset
            // 
            resources.ApplyResources(this.m_lblPreset, "m_lblPreset");
            this.m_lblPreset.Name = "m_lblPreset";
            // 
            // m_ApplyOnWholeBook
            // 
            resources.ApplyResources(this.m_ApplyOnWholeBook, "m_ApplyOnWholeBook");
            this.m_ApplyOnWholeBook.Name = "m_ApplyOnWholeBook";
            this.helpProvider1.SetShowHelp(this.m_ApplyOnWholeBook, ((bool)(resources.GetObject("m_ApplyOnWholeBook.ShowHelp"))));
            this.m_ApplyOnWholeBook.UseVisualStyleBackColor = true;
            // 
            // AudioProcessingNoiseReduction
            // 
            this.AcceptButton = this.m_btn_Ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.m_ApplyOnWholeBook);
            this.Controls.Add(this.m_lblPreset);
            this.Controls.Add(this.m_SelectPresetComboBox);
            this.Controls.Add(this.m_lbdB);
            this.Controls.Add(this.m_SetNoiseReductionTrackBar);
            this.Controls.Add(this.m_lblPercent);
            this.Controls.Add(this.m_SetNoiseLevelTrackBar);
            this.Controls.Add(this.m_lblNoiseLevel);
            this.Controls.Add(this.m_SetNoiseLevelInPercent);
            this.Controls.Add(this.m_SetNoiseReduction);
            this.Controls.Add(this.m_lblNoiseReduction);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_Ok);
            this.Name = "AudioProcessingNoiseReduction";
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReduction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelInPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReductionTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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