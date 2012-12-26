namespace Obi.ProjectView
{
    partial class AudioBlock : Block
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioBlock));
            this.mWaveform = new Obi.ProjectView.Waveform();
            this.mRecordingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mWaveform
            // 
            this.mWaveform.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mWaveform.BackColor = System.Drawing.Color.OliveDrab;
            this.mWaveform.CancelRendering = false;
            this.mWaveform.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mWaveform.FinalSelectionPosition = 0;
            resources.ApplyResources(this.mWaveform, "mWaveform");
            this.mWaveform.Name = "mWaveform";
            this.mWaveform.Selection = null;
            this.mWaveform.SelectionPointPosition = -1;
            this.mWaveform.DoubleClick += new System.EventHandler(this.mWaveform_DoubleClick);
            this.mWaveform.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseMove);
            this.mWaveform.Click += new System.EventHandler(this.mWaveform_Click);
            this.mWaveform.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mWaveform_KeyUp);
            this.mWaveform.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseDown);
            this.mWaveform.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseUp);
            this.mWaveform.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mWaveform_KeyDown);
            // 
            // mRecordingLabel
            // 
            resources.ApplyResources(this.mRecordingLabel, "mRecordingLabel");
            this.mRecordingLabel.Name = "mRecordingLabel";
            // 
            // AudioBlock
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.mWaveform);
            this.Controls.Add(this.mRecordingLabel);
            this.Name = "AudioBlock";
            this.Controls.SetChildIndex(this.mRecordingLabel, 0);
            this.Controls.SetChildIndex(this.mWaveform, 0);
            this.Controls.SetChildIndex(this.mLabel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Waveform mWaveform;
        private System.Windows.Forms.Label mRecordingLabel;
    }
}
