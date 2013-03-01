namespace Obi.ProjectView
{
    partial class ZoomWaveform
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClose = new System.Windows.Forms.Button();
            this.btnNextPhrase = new System.Windows.Forms.Button();
            this.btnPreviousPhrase = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.panelZooomWaveform = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.No;
            this.btnClose.Location = new System.Drawing.Point(24, 533);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(91, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNextPhrase
            // 
            this.btnNextPhrase.Location = new System.Drawing.Point(146, 533);
            this.btnNextPhrase.Name = "btnNextPhrase";
            this.btnNextPhrase.Size = new System.Drawing.Size(90, 23);
            this.btnNextPhrase.TabIndex = 2;
            this.btnNextPhrase.Text = "Next Phrase";
            this.btnNextPhrase.UseVisualStyleBackColor = true;
            this.btnNextPhrase.Click += new System.EventHandler(this.btnNextPhrase_Click);
            // 
            // btnPreviousPhrase
            // 
            this.btnPreviousPhrase.Location = new System.Drawing.Point(271, 533);
            this.btnPreviousPhrase.Name = "btnPreviousPhrase";
            this.btnPreviousPhrase.Size = new System.Drawing.Size(92, 23);
            this.btnPreviousPhrase.TabIndex = 3;
            this.btnPreviousPhrase.Text = "Previous Phrase";
            this.btnPreviousPhrase.UseVisualStyleBackColor = true;
            this.btnPreviousPhrase.Click += new System.EventHandler(this.btnPreviousPhrase_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(402, 533);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(110, 23);
            this.btnZoomIn.TabIndex = 5;
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(531, 533);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(91, 23);
            this.btnZoomOut.TabIndex = 6;
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(651, 533);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // panelZooomWaveform
            // 
            this.panelZooomWaveform.AutoScroll = true;
            this.panelZooomWaveform.Location = new System.Drawing.Point(3, 3);
            this.panelZooomWaveform.Name = "panelZooomWaveform";
            this.panelZooomWaveform.Size = new System.Drawing.Size(123, 58);
            this.panelZooomWaveform.TabIndex = 8;
            // 
            // ZoomWaveform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelZooomWaveform);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.btnPreviousPhrase);
            this.Controls.Add(this.btnNextPhrase);
            this.Controls.Add(this.btnClose);
            this.Name = "ZoomWaveform";
            this.Size = new System.Drawing.Size(963, 559);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnNextPhrase;
        private System.Windows.Forms.Button btnPreviousPhrase;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Panel panelZooomWaveform;

    }
}
