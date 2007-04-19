namespace Obi.Audio
{
    partial class VuMeterForm
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
            this.graphicalVuMeterPanel = new Obi.UserControls.GraphicalVuMeter();
            this.SuspendLayout();
            // 
            // graphicalVuMeterPanel
            // 
            this.graphicalVuMeterPanel.BackColor = System.Drawing.Color.White;
            this.graphicalVuMeterPanel.Location = new System.Drawing.Point(0, 0);
            this.graphicalVuMeterPanel.Name = "graphicalVuMeterPanel";
            this.graphicalVuMeterPanel.ResizeParent = false;
            this.graphicalVuMeterPanel.Size = new System.Drawing.Size(110, 357);
            this.graphicalVuMeterPanel.TabIndex = 0;
            this.graphicalVuMeterPanel.VuMeter = null;
            // 
            // VuMeterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(142, 423);
            this.Controls.Add(this.graphicalVuMeterPanel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.Name = "VuMeterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VuMeter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VuMeterForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private Obi.UserControls.GraphicalVuMeter graphicalVuMeterPanel;

        
    }
}