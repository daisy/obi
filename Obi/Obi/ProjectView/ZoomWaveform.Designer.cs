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
            this.txtZoomSelected = new System.Windows.Forms.TextBox();
            this.mbtnZoomSelection = new System.Windows.Forms.Button();
            this.btntxtZoomSelected = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnClose.Location = new System.Drawing.Point(12, 533);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(91, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnNextPhrase
            // 
            this.btnNextPhrase.Location = new System.Drawing.Point(131, 533);
            this.btnNextPhrase.Name = "btnNextPhrase";
            this.btnNextPhrase.Size = new System.Drawing.Size(90, 23);
            this.btnNextPhrase.TabIndex = 2;
            this.btnNextPhrase.Text = "Next Phrase";
            this.btnNextPhrase.UseVisualStyleBackColor = true;
            this.btnNextPhrase.Click += new System.EventHandler(this.btnNextPhrase_Click);
            // 
            // btnPreviousPhrase
            // 
            this.btnPreviousPhrase.Location = new System.Drawing.Point(245, 533);
            this.btnPreviousPhrase.Name = "btnPreviousPhrase";
            this.btnPreviousPhrase.Size = new System.Drawing.Size(109, 23);
            this.btnPreviousPhrase.TabIndex = 3;
            this.btnPreviousPhrase.Text = "Previous Phrase";
            this.btnPreviousPhrase.UseVisualStyleBackColor = true;
            this.btnPreviousPhrase.Click += new System.EventHandler(this.btnPreviousPhrase_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(383, 533);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(110, 23);
            this.btnZoomIn.TabIndex = 5;
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(530, 533);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(91, 23);
            this.btnZoomOut.TabIndex = 6;
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(663, 533);
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
            this.panelZooomWaveform.BackColor = System.Drawing.SystemColors.Control;
            this.panelZooomWaveform.Location = new System.Drawing.Point(3, 3);
            this.panelZooomWaveform.Name = "panelZooomWaveform";
            this.panelZooomWaveform.Size = new System.Drawing.Size(946, 488);
            this.panelZooomWaveform.TabIndex = 9;
            // 
            // txtZoomSelected
            // 
            this.txtZoomSelected.Location = new System.Drawing.Point(24, 507);
            this.txtZoomSelected.Name = "txtZoomSelected";
            this.txtZoomSelected.ReadOnly = true;
            this.txtZoomSelected.Size = new System.Drawing.Size(702, 20);
            this.txtZoomSelected.TabIndex = 18;
            this.txtZoomSelected.Visible = false;
            // 
            // mbtnZoomSelection
            // 
            this.mbtnZoomSelection.Location = new System.Drawing.Point(777, 533);
            this.mbtnZoomSelection.Name = "mbtnZoomSelection";
            this.mbtnZoomSelection.Size = new System.Drawing.Size(107, 23);
            this.mbtnZoomSelection.TabIndex = 8;
            this.mbtnZoomSelection.Text = "Zoom Selection";
            this.mbtnZoomSelection.UseVisualStyleBackColor = true;
            this.mbtnZoomSelection.Click += new System.EventHandler(this.mbtnZoomSelection_Click);
            // 
            // btntxtZoomSelected
            // 
            this.btntxtZoomSelected.BackColor = System.Drawing.SystemColors.Control;
            this.btntxtZoomSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btntxtZoomSelected.Location = new System.Drawing.Point(24, 504);
            this.btntxtZoomSelected.Name = "btntxtZoomSelected";
            this.btntxtZoomSelected.Size = new System.Drawing.Size(702, 23);
            this.btntxtZoomSelected.TabIndex = 10;
            this.btntxtZoomSelected.UseVisualStyleBackColor = false;
            // 
            // ZoomWaveform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.btntxtZoomSelected);
            this.Controls.Add(this.mbtnZoomSelection);
            this.Controls.Add(this.txtZoomSelected);
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
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnNextPhrase;
        private System.Windows.Forms.Button btnPreviousPhrase;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Panel panelZooomWaveform;
        private System.Windows.Forms.TextBox txtZoomSelected;
        private System.Windows.Forms.Button mbtnZoomSelection;
        private System.Windows.Forms.Button btntxtZoomSelected;

    }
}
