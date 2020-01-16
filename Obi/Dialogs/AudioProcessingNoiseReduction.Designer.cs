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
            this.SuspendLayout();
            // 
            // m_rbNAudioNoiseReduction
            // 
            this.m_rbNAudioNoiseReduction.AutoSize = true;
            this.m_rbNAudioNoiseReduction.Location = new System.Drawing.Point(67, 66);
            this.m_rbNAudioNoiseReduction.Name = "m_rbNAudioNoiseReduction";
            this.m_rbNAudioNoiseReduction.Size = new System.Drawing.Size(170, 17);
            this.m_rbNAudioNoiseReduction.TabIndex = 0;
            this.m_rbNAudioNoiseReduction.TabStop = true;
            this.m_rbNAudioNoiseReduction.Text = "Noise Reduction using NAudio";
            this.m_rbNAudioNoiseReduction.UseVisualStyleBackColor = true;
            // 
            // m_rbFfmpegNoiseReduction
            // 
            this.m_rbFfmpegNoiseReduction.AutoSize = true;
            this.m_rbFfmpegNoiseReduction.Location = new System.Drawing.Point(67, 142);
            this.m_rbFfmpegNoiseReduction.Name = "m_rbFfmpegNoiseReduction";
            this.m_rbFfmpegNoiseReduction.Size = new System.Drawing.Size(167, 17);
            this.m_rbFfmpegNoiseReduction.TabIndex = 1;
            this.m_rbFfmpegNoiseReduction.TabStop = true;
            this.m_rbFfmpegNoiseReduction.Text = "Noise Reduction using ffmpeg";
            this.m_rbFfmpegNoiseReduction.UseVisualStyleBackColor = true;
            // 
            // m_btn_Ok
            // 
            this.m_btn_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_Ok.Location = new System.Drawing.Point(92, 239);
            this.m_btn_Ok.Name = "m_btn_Ok";
            this.m_btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Ok.TabIndex = 2;
            this.m_btn_Ok.Text = "&OK";
            this.m_btn_Ok.UseVisualStyleBackColor = true;
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.Location = new System.Drawing.Point(252, 239);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 3;
            this.m_btn_Cancel.Text = "&Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // AudioProcessingNoiseReduction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 315);
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
    }
}