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
            this.m_btn_Ok = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.m_SetNoiseReduction = new System.Windows.Forms.NumericUpDown();
            this.m_SetNoiseLevelInPercent = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.m_SetNoiseLevelTrackBar = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.m_SetNoiseReductionTrackBar = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.m_SelectPresetComboBox = new System.Windows.Forms.ComboBox();
            this.m_lblPreset = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReduction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelInPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReductionTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btn_Ok
            // 
            this.m_btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_Ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Ok.Location = new System.Drawing.Point(160, 395);
            this.m_btn_Ok.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.m_btn_Ok.Name = "m_btn_Ok";
            this.m_btn_Ok.Size = new System.Drawing.Size(112, 35);
            this.m_btn_Ok.TabIndex = 11;
            this.m_btn_Ok.Text = "&OK";
            this.m_btn_Ok.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Cancel.Location = new System.Drawing.Point(358, 395);
            this.m_btn_Cancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(112, 35);
            this.m_btn_Cancel.TabIndex = 12;
            this.m_btn_Cancel.Text = "&Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 218);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Noise Reduction( in dB)";
            // 
            // m_SetNoiseReduction
            // 
            this.m_SetNoiseReduction.AccessibleName = "Noise reduction: Set in Db";
            this.m_SetNoiseReduction.DecimalPlaces = 2;
            this.m_SetNoiseReduction.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_SetNoiseReduction.Location = new System.Drawing.Point(459, 260);
            this.m_SetNoiseReduction.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.m_SetNoiseReduction.Size = new System.Drawing.Size(92, 26);
            this.m_SetNoiseReduction.TabIndex = 9;
            this.m_SetNoiseReduction.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.m_SetNoiseReduction.ValueChanged += new System.EventHandler(this.m_SetNoiseReduction_ValueChanged);
            // 
            // m_SetNoiseLevelInPercent
            // 
            this.m_SetNoiseLevelInPercent.AccessibleName = "Noise Level: set in percent";
            this.m_SetNoiseLevelInPercent.DecimalPlaces = 1;
            this.m_SetNoiseLevelInPercent.Location = new System.Drawing.Point(459, 145);
            this.m_SetNoiseLevelInPercent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.m_SetNoiseLevelInPercent.Name = "m_SetNoiseLevelInPercent";
            this.m_SetNoiseLevelInPercent.Size = new System.Drawing.Size(84, 26);
            this.m_SetNoiseLevelInPercent.TabIndex = 5;
            this.m_SetNoiseLevelInPercent.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            this.m_SetNoiseLevelInPercent.ValueChanged += new System.EventHandler(this.m_SetNoiseLevelInPercent_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 111);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(178, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "Noise Level (in percent):";
            // 
            // m_SetNoiseLevelTrackBar
            // 
            this.m_SetNoiseLevelTrackBar.AccessibleName = "noise level: track bar to increase/decrease in percentage. Right sliding will inc" +
    "rease the level.";
            this.m_SetNoiseLevelTrackBar.Location = new System.Drawing.Point(40, 145);
            this.m_SetNoiseLevelTrackBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.m_SetNoiseLevelTrackBar.Maximum = 100;
            this.m_SetNoiseLevelTrackBar.Name = "m_SetNoiseLevelTrackBar";
            this.m_SetNoiseLevelTrackBar.Size = new System.Drawing.Size(370, 69);
            this.m_SetNoiseLevelTrackBar.TabIndex = 4;
            this.m_SetNoiseLevelTrackBar.TickFrequency = 6;
            this.m_SetNoiseLevelTrackBar.Value = 75;
            this.m_SetNoiseLevelTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseLevelTrackBar_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(552, 151);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "%";
            // 
            // m_SetNoiseReductionTrackBar
            // 
            this.m_SetNoiseReductionTrackBar.AccessibleName = "noise reduction: Track bar to increase/decrease in percentage. Right sliding will" +
    " reduce noise.";
            this.m_SetNoiseReductionTrackBar.Location = new System.Drawing.Point(40, 260);
            this.m_SetNoiseReductionTrackBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.m_SetNoiseReductionTrackBar.Maximum = 97;
            this.m_SetNoiseReductionTrackBar.Name = "m_SetNoiseReductionTrackBar";
            this.m_SetNoiseReductionTrackBar.Size = new System.Drawing.Size(370, 69);
            this.m_SetNoiseReductionTrackBar.TabIndex = 8;
            this.m_SetNoiseReductionTrackBar.TickFrequency = 10;
            this.m_SetNoiseReductionTrackBar.Value = 50;
            this.m_SetNoiseReductionTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseReductionTrackBar_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(556, 266);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 20);
            this.label8.TabIndex = 10;
            this.label8.Text = "dB";
            // 
            // m_SelectPresetComboBox
            // 
            this.m_SelectPresetComboBox.AccessibleName = "Background noise levvel";
            this.m_SelectPresetComboBox.FormattingEnabled = true;
            this.m_SelectPresetComboBox.Items.AddRange(new object[] {
            "Remove strong noise",
            "Remove medium noise",
            "Remove weak noise "});
            this.m_SelectPresetComboBox.Location = new System.Drawing.Point(108, 51);
            this.m_SelectPresetComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.m_SelectPresetComboBox.Name = "m_SelectPresetComboBox";
            this.m_SelectPresetComboBox.Size = new System.Drawing.Size(433, 28);
            this.m_SelectPresetComboBox.TabIndex = 2;
            this.m_SelectPresetComboBox.SelectedIndexChanged += new System.EventHandler(this.m_SelectPresetComboBox_SelectedIndexChanged);
            // 
            // m_lblPreset
            // 
            this.m_lblPreset.AutoSize = true;
            this.m_lblPreset.Location = new System.Drawing.Point(36, 55);
            this.m_lblPreset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_lblPreset.Name = "m_lblPreset";
            this.m_lblPreset.Size = new System.Drawing.Size(172, 20);
            this.m_lblPreset.TabIndex = 1;
            this.m_lblPreset.Text = "Background noise level";
            // 
            // AudioProcessingNoiseReduction
            // 
            this.AcceptButton = this.m_btn_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 471);
            this.Controls.Add(this.m_lblPreset);
            this.Controls.Add(this.m_SelectPresetComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.m_SetNoiseReductionTrackBar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.m_SetNoiseLevelTrackBar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.m_SetNoiseLevelInPercent);
            this.Controls.Add(this.m_SetNoiseReduction);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_Ok);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "AudioProcessingNoiseReduction";
            this.Text = "Noise Reduction";
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown m_SetNoiseReduction;
        private System.Windows.Forms.NumericUpDown m_SetNoiseLevelInPercent;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar m_SetNoiseLevelTrackBar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar m_SetNoiseReductionTrackBar;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox m_SelectPresetComboBox;
        private System.Windows.Forms.Label m_lblPreset;
    }
}