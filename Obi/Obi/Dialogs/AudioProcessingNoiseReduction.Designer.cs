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
            this.label9 = new System.Windows.Forms.Label();
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
            this.m_btn_Ok.Location = new System.Drawing.Point(107, 257);
            this.m_btn_Ok.Name = "m_btn_Ok";
            this.m_btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Ok.TabIndex = 11;
            this.m_btn_Ok.Text = "&OK";
            this.m_btn_Ok.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Cancel.Location = new System.Drawing.Point(239, 257);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 12;
            this.m_btn_Cancel.Text = "&Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Noise Reduction( in dB)";
            // 
            // m_SetNoiseReduction
            // 
            this.m_SetNoiseReduction.AccessibleName = "Set Noise reduction(in Db)";
            this.m_SetNoiseReduction.DecimalPlaces = 2;
            this.m_SetNoiseReduction.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_SetNoiseReduction.Location = new System.Drawing.Point(306, 169);
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
            this.m_SetNoiseReduction.Size = new System.Drawing.Size(61, 20);
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
            this.m_SetNoiseLevelInPercent.AccessibleName = "Set Noise Level (in percent)";
            this.m_SetNoiseLevelInPercent.DecimalPlaces = 1;
            this.m_SetNoiseLevelInPercent.Location = new System.Drawing.Point(306, 94);
            this.m_SetNoiseLevelInPercent.Name = "m_SetNoiseLevelInPercent";
            this.m_SetNoiseLevelInPercent.Size = new System.Drawing.Size(56, 20);
            this.m_SetNoiseLevelInPercent.TabIndex = 5;
            this.m_SetNoiseLevelInPercent.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.m_SetNoiseLevelInPercent.ValueChanged += new System.EventHandler(this.m_SetNoiseLevelInPercent_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Noise Level (in percent):";
            // 
            // m_SetNoiseLevelTrackBar
            // 
            this.m_SetNoiseLevelTrackBar.AccessibleName = "Slide to Increase/Decrease noise level (in percentage). Right sliding will increa" +
    "se noise level";
            this.m_SetNoiseLevelTrackBar.Location = new System.Drawing.Point(27, 94);
            this.m_SetNoiseLevelTrackBar.Maximum = 100;
            this.m_SetNoiseLevelTrackBar.Name = "m_SetNoiseLevelTrackBar";
            this.m_SetNoiseLevelTrackBar.Size = new System.Drawing.Size(247, 45);
            this.m_SetNoiseLevelTrackBar.TabIndex = 4;
            this.m_SetNoiseLevelTrackBar.TickFrequency = 6;
            this.m_SetNoiseLevelTrackBar.Value = 100;
            this.m_SetNoiseLevelTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseLevelTrackBar_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(368, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "%";
            // 
            // m_SetNoiseReductionTrackBar
            // 
            this.m_SetNoiseReductionTrackBar.AccessibleName = "Slide to Increase/Decrease noise reduction (in percentage). Right sliding will in" +
    "crease noise reduction";
            this.m_SetNoiseReductionTrackBar.Location = new System.Drawing.Point(27, 169);
            this.m_SetNoiseReductionTrackBar.Maximum = 97;
            this.m_SetNoiseReductionTrackBar.Name = "m_SetNoiseReductionTrackBar";
            this.m_SetNoiseReductionTrackBar.Size = new System.Drawing.Size(247, 45);
            this.m_SetNoiseReductionTrackBar.TabIndex = 8;
            this.m_SetNoiseReductionTrackBar.TickFrequency = 10;
            this.m_SetNoiseReductionTrackBar.Value = 50;
            this.m_SetNoiseReductionTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseReductionTrackBar_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(371, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "dB";
            // 
            // m_SelectPresetComboBox
            // 
            this.m_SelectPresetComboBox.AccessibleName = "Preset";
            this.m_SelectPresetComboBox.FormattingEnabled = true;
            this.m_SelectPresetComboBox.Items.AddRange(new object[] {
            "Remove strong noise",
            "Remove medium noise",
            "Remove weak noise "});
            this.m_SelectPresetComboBox.Location = new System.Drawing.Point(72, 33);
            this.m_SelectPresetComboBox.Name = "m_SelectPresetComboBox";
            this.m_SelectPresetComboBox.Size = new System.Drawing.Size(290, 21);
            this.m_SelectPresetComboBox.TabIndex = 2;
            this.m_SelectPresetComboBox.SelectedIndexChanged += new System.EventHandler(this.m_SelectPresetComboBox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Preset";
            // 
            // AudioProcessingNoiseReduction
            // 
            this.AcceptButton = this.m_btn_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 306);
            this.Controls.Add(this.label9);
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
        private System.Windows.Forms.Label label9;
    }
}