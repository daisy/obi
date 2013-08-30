namespace Obi.Dialogs
{
    partial class ExportAdvance
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
            this.lblReplayGain = new System.Windows.Forms.Label();
            this.lblStereoToMono = new System.Windows.Forms.Label();
            this.txtReplayGain = new System.Windows.Forms.TextBox();
            this.txtStereoToMono = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblReplayGain
            // 
            this.lblReplayGain.AccessibleName = "";
            this.lblReplayGain.AutoSize = true;
            this.lblReplayGain.Location = new System.Drawing.Point(35, 28);
            this.lblReplayGain.Name = "lblReplayGain";
            this.lblReplayGain.Size = new System.Drawing.Size(65, 13);
            this.lblReplayGain.TabIndex = 0;
            this.lblReplayGain.Text = "Replay Gain";
            // 
            // lblStereoToMono
            // 
            this.lblStereoToMono.AutoSize = true;
            this.lblStereoToMono.Location = new System.Drawing.Point(35, 73);
            this.lblStereoToMono.Name = "lblStereoToMono";
            this.lblStereoToMono.Size = new System.Drawing.Size(139, 13);
            this.lblStereoToMono.TabIndex = 1;
            this.lblStereoToMono.Text = "Downmix stereo file to mono";
            // 
            // txtReplayGain
            // 
            this.txtReplayGain.AccessibleName = "Replay Gain ";
            this.txtReplayGain.Location = new System.Drawing.Point(204, 21);
            this.txtReplayGain.Name = "txtReplayGain";
            this.txtReplayGain.Size = new System.Drawing.Size(100, 20);
            this.txtReplayGain.TabIndex = 3;
            // 
            // txtStereoToMono
            // 
            this.txtStereoToMono.AccessibleName = "Downmix stereo file to mono";
            this.txtStereoToMono.Location = new System.Drawing.Point(204, 70);
            this.txtStereoToMono.Name = "txtStereoToMono";
            this.txtStereoToMono.Size = new System.Drawing.Size(100, 20);
            this.txtStereoToMono.TabIndex = 4;
            // 
            // btnOk
            // 
            this.btnOk.AccessibleName = "Ok";
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(82, 180);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleName = "Cancel";
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(204, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ExportAdvance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 214);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtStereoToMono);
            this.Controls.Add(this.txtReplayGain);
            this.Controls.Add(this.lblStereoToMono);
            this.Controls.Add(this.lblReplayGain);
            this.Name = "ExportAdvance";
            this.Text = "Export Advance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblReplayGain;
        private System.Windows.Forms.Label lblStereoToMono;
        private System.Windows.Forms.TextBox txtReplayGain;
        private System.Windows.Forms.TextBox txtStereoToMono;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}