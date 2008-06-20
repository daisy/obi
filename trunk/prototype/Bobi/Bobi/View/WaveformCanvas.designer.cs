namespace Bobi.View
{
    partial class WaveformCanvas
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
            this.SuspendLayout();
            // 
            // WaveformCanvas
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WaveformCanvas_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WaveformCanvas_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WaveformCanvas_MouseUp);
            this.SizeChanged += new System.EventHandler(this.Waveform_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
