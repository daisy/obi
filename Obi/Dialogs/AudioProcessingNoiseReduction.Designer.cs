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
            this.m_rbNAudioNoiseReduction = new System.Windows.Forms.RadioButton();
            this.m_rbFfmpegNoiseReduction = new System.Windows.Forms.RadioButton();
            this.m_btn_Ok = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.m_tb_HighPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_tb_LowPass = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.m_rbFfmpegAfftdnNoiseReduction = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.m_SetNoiseReduction = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.m_SetNoiseFloor = new System.Windows.Forms.NumericUpDown();
            this.m_rbFfmpegAnlmdnNoiseReduction = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.m_SetDenoisingStrength = new System.Windows.Forms.NumericUpDown();
            this.m_SetNoiseLevelInPercent = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.m_SetNoiseLevelTrackBar = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.m_SetNoiseReductionTrackBar = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.m_SelectPresetComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReduction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseFloor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetDenoisingStrength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelInPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReductionTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // m_rbNAudioNoiseReduction
            // 
            this.m_rbNAudioNoiseReduction.AutoSize = true;
            this.m_rbNAudioNoiseReduction.Location = new System.Drawing.Point(233, 56);
            this.m_rbNAudioNoiseReduction.Name = "m_rbNAudioNoiseReduction";
            this.m_rbNAudioNoiseReduction.Size = new System.Drawing.Size(60, 17);
            this.m_rbNAudioNoiseReduction.TabIndex = 3;
            this.m_rbNAudioNoiseReduction.Text = "NAudio";
            this.m_rbNAudioNoiseReduction.UseVisualStyleBackColor = true;
            this.m_rbNAudioNoiseReduction.CheckedChanged += new System.EventHandler(this.m_rbNAudioNoiseReduction_CheckedChanged);
            // 
            // m_rbFfmpegNoiseReduction
            // 
            this.m_rbFfmpegNoiseReduction.AutoSize = true;
            this.m_rbFfmpegNoiseReduction.Location = new System.Drawing.Point(327, 56);
            this.m_rbFfmpegNoiseReduction.Name = "m_rbFfmpegNoiseReduction";
            this.m_rbFfmpegNoiseReduction.Size = new System.Drawing.Size(60, 17);
            this.m_rbFfmpegNoiseReduction.TabIndex = 4;
            this.m_rbFfmpegNoiseReduction.Text = "Ffmpeg";
            this.m_rbFfmpegNoiseReduction.UseVisualStyleBackColor = true;
            this.m_rbFfmpegNoiseReduction.CheckedChanged += new System.EventHandler(this.m_rbFfmpegNoiseReduction_CheckedChanged);
            // 
            // m_btn_Ok
            // 
            this.m_btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_Ok.Location = new System.Drawing.Point(42, 443);
            this.m_btn_Ok.Name = "m_btn_Ok";
            this.m_btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Ok.TabIndex = 19;
            this.m_btn_Ok.Text = "&OK";
            this.m_btn_Ok.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.Location = new System.Drawing.Point(174, 443);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 20;
            this.m_btn_Cancel.Text = "&Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 353);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "High Pass Freqency";
            // 
            // m_tb_HighPass
            // 
            this.m_tb_HighPass.AccessibleName = "High Pass Freqency";
            this.m_tb_HighPass.Enabled = false;
            this.m_tb_HighPass.Location = new System.Drawing.Point(160, 346);
            this.m_tb_HighPass.Name = "m_tb_HighPass";
            this.m_tb_HighPass.Size = new System.Drawing.Size(100, 20);
            this.m_tb_HighPass.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 397);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Low Pass Frequency";
            // 
            // m_tb_LowPass
            // 
            this.m_tb_LowPass.AccessibleName = "Low Pass Freqency";
            this.m_tb_LowPass.Enabled = false;
            this.m_tb_LowPass.Location = new System.Drawing.Point(160, 390);
            this.m_tb_LowPass.Name = "m_tb_LowPass";
            this.m_tb_LowPass.Size = new System.Drawing.Size(100, 20);
            this.m_tb_LowPass.TabIndex = 18;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(70, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(186, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Select library for noise reduction";
            // 
            // m_rbFfmpegAfftdnNoiseReduction
            // 
            this.m_rbFfmpegAfftdnNoiseReduction.AutoSize = true;
            this.m_rbFfmpegAfftdnNoiseReduction.Checked = true;
            this.m_rbFfmpegAfftdnNoiseReduction.Location = new System.Drawing.Point(12, 56);
            this.m_rbFfmpegAfftdnNoiseReduction.Name = "m_rbFfmpegAfftdnNoiseReduction";
            this.m_rbFfmpegAfftdnNoiseReduction.Size = new System.Drawing.Size(91, 17);
            this.m_rbFfmpegAfftdnNoiseReduction.TabIndex = 1;
            this.m_rbFfmpegAfftdnNoiseReduction.TabStop = true;
            this.m_rbFfmpegAfftdnNoiseReduction.Text = "Ffmpeg Afftdn";
            this.m_rbFfmpegAfftdnNoiseReduction.UseVisualStyleBackColor = true;
            this.m_rbFfmpegAfftdnNoiseReduction.CheckedChanged += new System.EventHandler(this.m_rbFfmpegAfftdnNoiseReduction_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 10;
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
            this.m_SetNoiseReduction.Location = new System.Drawing.Point(306, 227);
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
            this.m_SetNoiseReduction.TabIndex = 12;
            this.m_SetNoiseReduction.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.m_SetNoiseReduction.ValueChanged += new System.EventHandler(this.m_SetNoiseReduction_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 282);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Set noise floor(in Db)";
            this.label4.Visible = false;
            // 
            // m_SetNoiseFloor
            // 
            this.m_SetNoiseFloor.AccessibleName = "Set noise floor(in Db)";
            this.m_SetNoiseFloor.DecimalPlaces = 2;
            this.m_SetNoiseFloor.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_SetNoiseFloor.Location = new System.Drawing.Point(188, 281);
            this.m_SetNoiseFloor.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.m_SetNoiseFloor.Minimum = new decimal(new int[] {
            80,
            0,
            0,
            -2147483648});
            this.m_SetNoiseFloor.Name = "m_SetNoiseFloor";
            this.m_SetNoiseFloor.Size = new System.Drawing.Size(61, 20);
            this.m_SetNoiseFloor.TabIndex = 12;
            this.m_SetNoiseFloor.Value = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.m_SetNoiseFloor.Visible = false;
            this.m_SetNoiseFloor.ValueChanged += new System.EventHandler(this.m_SetNoiseFloor_ValueChanged);
            // 
            // m_rbFfmpegAnlmdnNoiseReduction
            // 
            this.m_rbFfmpegAnlmdnNoiseReduction.AutoSize = true;
            this.m_rbFfmpegAnlmdnNoiseReduction.Location = new System.Drawing.Point(119, 56);
            this.m_rbFfmpegAnlmdnNoiseReduction.Name = "m_rbFfmpegAnlmdnNoiseReduction";
            this.m_rbFfmpegAnlmdnNoiseReduction.Size = new System.Drawing.Size(98, 17);
            this.m_rbFfmpegAnlmdnNoiseReduction.TabIndex = 2;
            this.m_rbFfmpegAnlmdnNoiseReduction.TabStop = true;
            this.m_rbFfmpegAnlmdnNoiseReduction.Text = "Ffmpeg Anlmdn";
            this.m_rbFfmpegAnlmdnNoiseReduction.UseVisualStyleBackColor = true;
            this.m_rbFfmpegAnlmdnNoiseReduction.CheckedChanged += new System.EventHandler(this.m_rbFfmpegAnlmdnNoiseReduction_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 295);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Set Denoising Strength";
            // 
            // m_SetDenoisingStrength
            // 
            this.m_SetDenoisingStrength.Enabled = false;
            this.m_SetDenoisingStrength.Location = new System.Drawing.Point(188, 293);
            this.m_SetDenoisingStrength.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.m_SetDenoisingStrength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            327680});
            this.m_SetDenoisingStrength.Name = "m_SetDenoisingStrength";
            this.m_SetDenoisingStrength.Size = new System.Drawing.Size(61, 20);
            this.m_SetDenoisingStrength.TabIndex = 14;
            this.m_SetDenoisingStrength.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // m_SetNoiseLevelInPercent
            // 
            this.m_SetNoiseLevelInPercent.DecimalPlaces = 1;
            this.m_SetNoiseLevelInPercent.Location = new System.Drawing.Point(306, 152);
            this.m_SetNoiseLevelInPercent.Name = "m_SetNoiseLevelInPercent";
            this.m_SetNoiseLevelInPercent.Size = new System.Drawing.Size(56, 20);
            this.m_SetNoiseLevelInPercent.TabIndex = 9;
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
            this.label6.Location = new System.Drawing.Point(24, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Noise Level (in percent):";
            // 
            // m_SetNoiseLevelTrackBar
            // 
            this.m_SetNoiseLevelTrackBar.Location = new System.Drawing.Point(27, 152);
            this.m_SetNoiseLevelTrackBar.Maximum = 100;
            this.m_SetNoiseLevelTrackBar.Name = "m_SetNoiseLevelTrackBar";
            this.m_SetNoiseLevelTrackBar.Size = new System.Drawing.Size(247, 45);
            this.m_SetNoiseLevelTrackBar.TabIndex = 8;
            this.m_SetNoiseLevelTrackBar.TickFrequency = 6;
            this.m_SetNoiseLevelTrackBar.Value = 100;
            this.m_SetNoiseLevelTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseLevelTrackBar_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(368, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "%";
            // 
            // m_SetNoiseReductionTrackBar
            // 
            this.m_SetNoiseReductionTrackBar.Location = new System.Drawing.Point(27, 227);
            this.m_SetNoiseReductionTrackBar.Maximum = 97;
            this.m_SetNoiseReductionTrackBar.Name = "m_SetNoiseReductionTrackBar";
            this.m_SetNoiseReductionTrackBar.Size = new System.Drawing.Size(247, 45);
            this.m_SetNoiseReductionTrackBar.TabIndex = 11;
            this.m_SetNoiseReductionTrackBar.TickFrequency = 10;
            this.m_SetNoiseReductionTrackBar.Value = 50;
            this.m_SetNoiseReductionTrackBar.ValueChanged += new System.EventHandler(this.m_SetNoiseReductionTrackBar_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(373, 231);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "dB";
            // 
            // m_SelectPresetComboBox
            // 
            this.m_SelectPresetComboBox.FormattingEnabled = true;
            this.m_SelectPresetComboBox.Items.AddRange(new object[] {
            "Remove strong noise",
            "Remove medium noise",
            "Remove weak noise "});
            this.m_SelectPresetComboBox.Location = new System.Drawing.Point(77, 91);
            this.m_SelectPresetComboBox.Name = "m_SelectPresetComboBox";
            this.m_SelectPresetComboBox.Size = new System.Drawing.Size(290, 21);
            this.m_SelectPresetComboBox.TabIndex = 6;
            this.m_SelectPresetComboBox.SelectedIndexChanged += new System.EventHandler(this.m_SelectPresetComboBox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Preset";
            // 
            // AudioProcessingNoiseReduction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 470);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.m_SelectPresetComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.m_SetNoiseReductionTrackBar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.m_SetNoiseLevelTrackBar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.m_SetNoiseLevelInPercent);
            this.Controls.Add(this.m_SetDenoisingStrength);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.m_rbFfmpegAnlmdnNoiseReduction);
            this.Controls.Add(this.m_SetNoiseFloor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_SetNoiseReduction);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.m_rbFfmpegAfftdnNoiseReduction);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_tb_LowPass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_tb_HighPass);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_Ok);
            this.Controls.Add(this.m_rbFfmpegNoiseReduction);
            this.Controls.Add(this.m_rbNAudioNoiseReduction);
            this.Name = "AudioProcessingNoiseReduction";
            this.Text = "Noise Reduction";
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReduction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseFloor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetDenoisingStrength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelInPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseLevelTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_SetNoiseReductionTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton m_rbNAudioNoiseReduction;
        private System.Windows.Forms.RadioButton m_rbFfmpegNoiseReduction;
        private System.Windows.Forms.Button m_btn_Ok;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_tb_HighPass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox m_tb_LowPass;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton m_rbFfmpegAfftdnNoiseReduction;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown m_SetNoiseReduction;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown m_SetNoiseFloor;
        private System.Windows.Forms.RadioButton m_rbFfmpegAnlmdnNoiseReduction;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown m_SetDenoisingStrength;
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