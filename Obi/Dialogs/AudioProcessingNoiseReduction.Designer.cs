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
            this.SuspendLayout();
            // 
            // m_rbNAudioNoiseReduction
            // 
            this.m_rbNAudioNoiseReduction.AutoSize = true;
            this.m_rbNAudioNoiseReduction.Checked = true;
            this.m_rbNAudioNoiseReduction.Location = new System.Drawing.Point(59, 56);
            this.m_rbNAudioNoiseReduction.Name = "m_rbNAudioNoiseReduction";
            this.m_rbNAudioNoiseReduction.Size = new System.Drawing.Size(60, 17);
            this.m_rbNAudioNoiseReduction.TabIndex = 1;
            this.m_rbNAudioNoiseReduction.TabStop = true;
            this.m_rbNAudioNoiseReduction.Text = "NAudio";
            this.m_rbNAudioNoiseReduction.UseVisualStyleBackColor = true;
            // 
            // m_rbFfmpegNoiseReduction
            // 
            this.m_rbFfmpegNoiseReduction.AutoSize = true;
            this.m_rbFfmpegNoiseReduction.Location = new System.Drawing.Point(188, 56);
            this.m_rbFfmpegNoiseReduction.Name = "m_rbFfmpegNoiseReduction";
            this.m_rbFfmpegNoiseReduction.Size = new System.Drawing.Size(60, 17);
            this.m_rbFfmpegNoiseReduction.TabIndex = 2;
            this.m_rbFfmpegNoiseReduction.TabStop = true;
            this.m_rbFfmpegNoiseReduction.Text = "Ffmpeg";
            this.m_rbFfmpegNoiseReduction.UseVisualStyleBackColor = true;
            // 
            // m_btn_Ok
            // 
            this.m_btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_Ok.Location = new System.Drawing.Point(42, 239);
            this.m_btn_Ok.Name = "m_btn_Ok";
            this.m_btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Ok.TabIndex = 7;
            this.m_btn_Ok.Text = "&OK";
            this.m_btn_Ok.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.Location = new System.Drawing.Point(174, 239);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 8;
            this.m_btn_Cancel.Text = "&Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "High Pass Freqency";
            // 
            // m_tb_HighPass
            // 
            this.m_tb_HighPass.AccessibleName = "High Pass Freqency";
            this.m_tb_HighPass.Location = new System.Drawing.Point(160, 96);
            this.m_tb_HighPass.Name = "m_tb_HighPass";
            this.m_tb_HighPass.Size = new System.Drawing.Size(100, 20);
            this.m_tb_HighPass.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Low Pass Frequency";
            // 
            // m_tb_LowPass
            // 
            this.m_tb_LowPass.AccessibleName = "Low Pass Freqency";
            this.m_tb_LowPass.Location = new System.Drawing.Point(160, 140);
            this.m_tb_LowPass.Name = "m_tb_LowPass";
            this.m_tb_LowPass.Size = new System.Drawing.Size(100, 20);
            this.m_tb_LowPass.TabIndex = 6;
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
            // AudioProcessingNoiseReduction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 315);
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
    }
}