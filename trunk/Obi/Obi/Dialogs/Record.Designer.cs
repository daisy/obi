namespace Obi.Dialogs
{
    partial class Record
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
            this.components = new System.ComponentModel.Container();
            this.btnRecordAndPause = new System.Windows.Forms.Button();
            this.labelDisplayTime = new System.Windows.Forms.Label();
            this.txtDisplayTime = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPhraseMarker = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRecordAndPause
            // 
            this.btnRecordAndPause.Location = new System.Drawing.Point(30, 65);
            this.btnRecordAndPause.Name = "btnRecordAndPause";
            this.btnRecordAndPause.Size = new System.Drawing.Size(75, 25);
            this.btnRecordAndPause.TabIndex = 0;
            this.btnRecordAndPause.Text = "&Record";
            this.btnRecordAndPause.UseVisualStyleBackColor = true;
            this.btnRecordAndPause.Click += new System.EventHandler(this.btnRecordAndPause_Click);
            // 
            // labelDisplayTime
            // 
            this.labelDisplayTime.AutoSize = true;
            this.labelDisplayTime.Location = new System.Drawing.Point(37, 9);
            this.labelDisplayTime.Name = "labelDisplayTime";
            this.labelDisplayTime.Size = new System.Drawing.Size(73, 13);
            this.labelDisplayTime.TabIndex = 1;
            this.labelDisplayTime.Text = "Running &Time";
            // 
            // txtDisplayTime
            // 
            this.txtDisplayTime.Location = new System.Drawing.Point(39, 30);
            this.txtDisplayTime.Name = "txtDisplayTime";
            this.txtDisplayTime.ReadOnly = true;
            this.txtDisplayTime.Size = new System.Drawing.Size(100, 20);
            this.txtDisplayTime.TabIndex = 2;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(123, 67);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "&Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPhraseMarker
            // 
            this.btnPhraseMarker.Location = new System.Drawing.Point(221, 67);
            this.btnPhraseMarker.Name = "btnPhraseMarker";
            this.btnPhraseMarker.Size = new System.Drawing.Size(75, 23);
            this.btnPhraseMarker.TabIndex = 4;
            this.btnPhraseMarker.Text = "Phrase &Marker";
            this.btnPhraseMarker.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(4, 121);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(290, 42);
            this.trackBar1.TabIndex = 5;
            // 
            // Record
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 183);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.btnPhraseMarker);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.txtDisplayTime);
            this.Controls.Add(this.labelDisplayTime);
            this.Controls.Add(this.btnRecordAndPause);
            this.Name = "Record";
            this.Text = "Record";
            this.Load += new System.EventHandler(this.Record_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRecordAndPause;
        private System.Windows.Forms.Label labelDisplayTime;
        private System.Windows.Forms.TextBox txtDisplayTime;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPhraseMarker;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}