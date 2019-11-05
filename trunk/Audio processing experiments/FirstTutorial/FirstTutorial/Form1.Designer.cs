namespace FirstTutorial
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.m_IncreseAmplitudeComboBox = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.FadeOutDurationTextBox = new System.Windows.Forms.TextBox();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.FadeInDurationTextBox = new System.Windows.Forms.TextBox();
            this.FadeOutDurationButton = new System.Windows.Forms.Button();
            this.FadeInDurationButton = new System.Windows.Forms.Button();
            this.FadeOutStartingPointTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.m_IncreaseAmplitude = new System.Windows.Forms.Button();
            this.m_DecreaseAmplitudeComboBox = new System.Windows.Forms.ComboBox();
            this.m_DecreaseAmplitudeButton = new System.Windows.Forms.Button();
            this.m_NoiseReductionButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.m_BandPassFrequencyTextBox = new System.Windows.Forms.TextBox();
            this.m_ffmpegNoiseReduction = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(434, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open WAV File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(13, 56);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(434, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Normalize";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(13, 176);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(434, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "Fade Out Whole Audio";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // m_IncreseAmplitudeComboBox
            // 
            this.m_IncreseAmplitudeComboBox.AccessibleName = "Increase Amplitude Combo Box";
            this.m_IncreseAmplitudeComboBox.FormattingEnabled = true;
            this.m_IncreseAmplitudeComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.m_IncreseAmplitudeComboBox.Location = new System.Drawing.Point(13, 93);
            this.m_IncreseAmplitudeComboBox.Name = "m_IncreseAmplitudeComboBox";
            this.m_IncreseAmplitudeComboBox.Size = new System.Drawing.Size(121, 21);
            this.m_IncreseAmplitudeComboBox.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(13, 216);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(434, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "Fade In Whole Audio";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // FadeOutDurationTextBox
            // 
            this.FadeOutDurationTextBox.AccessibleName = "Enter Fade Out Duration";
            this.FadeOutDurationTextBox.Location = new System.Drawing.Point(347, 263);
            this.FadeOutDurationTextBox.Name = "FadeOutDurationTextBox";
            this.FadeOutDurationTextBox.Size = new System.Drawing.Size(100, 20);
            this.FadeOutDurationTextBox.TabIndex = 12;
            this.FadeOutDurationTextBox.Text = "0";
            // 
            // FadeInDurationTextBox
            // 
            this.FadeInDurationTextBox.AccessibleName = "Fade In Duration";
            this.FadeInDurationTextBox.Location = new System.Drawing.Point(178, 324);
            this.FadeInDurationTextBox.Name = "FadeInDurationTextBox";
            this.FadeInDurationTextBox.Size = new System.Drawing.Size(100, 20);
            this.FadeInDurationTextBox.TabIndex = 15;
            this.FadeInDurationTextBox.Text = "0";
            // 
            // FadeOutDurationButton
            // 
            this.FadeOutDurationButton.Enabled = false;
            this.FadeOutDurationButton.Location = new System.Drawing.Point(474, 260);
            this.FadeOutDurationButton.Name = "FadeOutDurationButton";
            this.FadeOutDurationButton.Size = new System.Drawing.Size(134, 23);
            this.FadeOutDurationButton.TabIndex = 13;
            this.FadeOutDurationButton.Text = "Fade Out Duration";
            this.FadeOutDurationButton.UseVisualStyleBackColor = true;
            this.FadeOutDurationButton.Click += new System.EventHandler(this.FadeOutDurationButton_Click);
            // 
            // FadeInDurationButton
            // 
            this.FadeInDurationButton.Enabled = false;
            this.FadeInDurationButton.Location = new System.Drawing.Point(347, 324);
            this.FadeInDurationButton.Name = "FadeInDurationButton";
            this.FadeInDurationButton.Size = new System.Drawing.Size(135, 23);
            this.FadeInDurationButton.TabIndex = 16;
            this.FadeInDurationButton.Text = "Fade In Duration";
            this.FadeInDurationButton.UseVisualStyleBackColor = true;
            this.FadeInDurationButton.Click += new System.EventHandler(this.FadeInDurationButton_Click);
            // 
            // FadeOutStartingPointTextBox
            // 
            this.FadeOutStartingPointTextBox.AccessibleName = "Enter Fade Out Starting Point";
            this.FadeOutStartingPointTextBox.Location = new System.Drawing.Point(141, 263);
            this.FadeOutStartingPointTextBox.Name = "FadeOutStartingPointTextBox";
            this.FadeOutStartingPointTextBox.Size = new System.Drawing.Size(100, 20);
            this.FadeOutStartingPointTextBox.TabIndex = 10;
            this.FadeOutStartingPointTextBox.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Fade Out Duration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 266);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Fade Out Starting Point";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 326);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Fade In Duration";
            // 
            // m_IncreaseAmplitude
            // 
            this.m_IncreaseAmplitude.Enabled = false;
            this.m_IncreaseAmplitude.Location = new System.Drawing.Point(166, 91);
            this.m_IncreaseAmplitude.Name = "m_IncreaseAmplitude";
            this.m_IncreaseAmplitude.Size = new System.Drawing.Size(281, 23);
            this.m_IncreaseAmplitude.TabIndex = 4;
            this.m_IncreaseAmplitude.Text = "Increase Amplitude";
            this.m_IncreaseAmplitude.UseVisualStyleBackColor = true;
            this.m_IncreaseAmplitude.Click += new System.EventHandler(this.m_IncreaseAmplitude_Click);
            // 
            // m_DecreaseAmplitudeComboBox
            // 
            this.m_DecreaseAmplitudeComboBox.AccessibleName = "Decrease Amplitude Combo Box";
            this.m_DecreaseAmplitudeComboBox.FormattingEnabled = true;
            this.m_DecreaseAmplitudeComboBox.Items.AddRange(new object[] {
            "0.25",
            "0.5",
            "0.75"});
            this.m_DecreaseAmplitudeComboBox.Location = new System.Drawing.Point(13, 137);
            this.m_DecreaseAmplitudeComboBox.Name = "m_DecreaseAmplitudeComboBox";
            this.m_DecreaseAmplitudeComboBox.Size = new System.Drawing.Size(121, 21);
            this.m_DecreaseAmplitudeComboBox.TabIndex = 5;
            // 
            // m_DecreaseAmplitudeButton
            // 
            this.m_DecreaseAmplitudeButton.Enabled = false;
            this.m_DecreaseAmplitudeButton.Location = new System.Drawing.Point(166, 135);
            this.m_DecreaseAmplitudeButton.Name = "m_DecreaseAmplitudeButton";
            this.m_DecreaseAmplitudeButton.Size = new System.Drawing.Size(281, 23);
            this.m_DecreaseAmplitudeButton.TabIndex = 6;
            this.m_DecreaseAmplitudeButton.Text = "Decrease Amplitude";
            this.m_DecreaseAmplitudeButton.UseVisualStyleBackColor = true;
            this.m_DecreaseAmplitudeButton.Click += new System.EventHandler(this.m_DecreaseAmplitudeButton_Click);
            // 
            // m_NoiseReductionButton
            // 
            this.m_NoiseReductionButton.Enabled = false;
            this.m_NoiseReductionButton.Location = new System.Drawing.Point(276, 452);
            this.m_NoiseReductionButton.Name = "m_NoiseReductionButton";
            this.m_NoiseReductionButton.Size = new System.Drawing.Size(157, 23);
            this.m_NoiseReductionButton.TabIndex = 19;
            this.m_NoiseReductionButton.Text = "Noise Reduction";
            this.m_NoiseReductionButton.UseVisualStyleBackColor = true;
            this.m_NoiseReductionButton.Visible = false;
            this.m_NoiseReductionButton.Click += new System.EventHandler(this.m_NoiseReductionButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-2, 457);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Band Pass Frequency";
            this.label4.Visible = false;
            // 
            // m_BandPassFrequencyTextBox
            // 
            this.m_BandPassFrequencyTextBox.AccessibleName = "Band Pass Filter Frequency";
            this.m_BandPassFrequencyTextBox.Location = new System.Drawing.Point(131, 454);
            this.m_BandPassFrequencyTextBox.Name = "m_BandPassFrequencyTextBox";
            this.m_BandPassFrequencyTextBox.Size = new System.Drawing.Size(100, 20);
            this.m_BandPassFrequencyTextBox.TabIndex = 18;
            this.m_BandPassFrequencyTextBox.Text = "3000";
            this.m_BandPassFrequencyTextBox.Visible = false;
            // 
            // m_ffmpegNoiseReduction
            // 
            this.m_ffmpegNoiseReduction.Enabled = false;
            this.m_ffmpegNoiseReduction.Location = new System.Drawing.Point(101, 388);
            this.m_ffmpegNoiseReduction.Name = "m_ffmpegNoiseReduction";
            this.m_ffmpegNoiseReduction.Size = new System.Drawing.Size(391, 23);
            this.m_ffmpegNoiseReduction.TabIndex = 20;
            this.m_ffmpegNoiseReduction.Text = "ffmpeg Noise Reduction";
            this.m_ffmpegNoiseReduction.UseVisualStyleBackColor = true;
            this.m_ffmpegNoiseReduction.Click += new System.EventHandler(this.m_ffmpegNoiseReduction_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 498);
            this.Controls.Add(this.m_ffmpegNoiseReduction);
            this.Controls.Add(this.m_BandPassFrequencyTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_NoiseReductionButton);
            this.Controls.Add(this.m_DecreaseAmplitudeButton);
            this.Controls.Add(this.m_DecreaseAmplitudeComboBox);
            this.Controls.Add(this.m_IncreaseAmplitude);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FadeOutStartingPointTextBox);
            this.Controls.Add(this.FadeInDurationButton);
            this.Controls.Add(this.FadeOutDurationButton);
            this.Controls.Add(this.FadeInDurationTextBox);
            this.Controls.Add(this.FadeOutDurationTextBox);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.m_IncreseAmplitudeComboBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "First Tutorial";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox m_IncreseAmplitudeComboBox;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox FadeOutDurationTextBox;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.TextBox FadeInDurationTextBox;
        private System.Windows.Forms.Button FadeOutDurationButton;
        private System.Windows.Forms.Button FadeInDurationButton;
        private System.Windows.Forms.TextBox FadeOutStartingPointTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button m_IncreaseAmplitude;
        private System.Windows.Forms.ComboBox m_DecreaseAmplitudeComboBox;
        private System.Windows.Forms.Button m_DecreaseAmplitudeButton;
        private System.Windows.Forms.Button m_NoiseReductionButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox m_BandPassFrequencyTextBox;
        private System.Windows.Forms.Button m_ffmpegNoiseReduction;
    }
}

