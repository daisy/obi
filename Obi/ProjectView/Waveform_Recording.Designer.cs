namespace Obi.ProjectView
{
    partial class Waveform_Recording
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.markPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phraseIsTODOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectPhraseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markPageToolStripMenuItem,
            this.deleteSelectedToolStripMenuItem,
            this.addSectionToolStripMenuItem,
            this.phraseIsTODOToolStripMenuItem,
            this.deselectPhraseToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(166, 136);
            this.contextMenuStrip1.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip1_Closed);
            // 
            // markPageToolStripMenuItem
            // 
            this.markPageToolStripMenuItem.Name = "markPageToolStripMenuItem";
            this.markPageToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.markPageToolStripMenuItem.Text = "Mark page";
            this.markPageToolStripMenuItem.Click += new System.EventHandler(this.markPageToolStripMenuItem_Click);
            // 
            // deleteSelectedToolStripMenuItem
            // 
            this.deleteSelectedToolStripMenuItem.Name = "deleteSelectedToolStripMenuItem";
            this.deleteSelectedToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.deleteSelectedToolStripMenuItem.Text = "Delete selected";
            this.deleteSelectedToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedToolStripMenuItem_Click);
            // 
            // addSectionToolStripMenuItem
            // 
            this.addSectionToolStripMenuItem.Name = "addSectionToolStripMenuItem";
            this.addSectionToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.addSectionToolStripMenuItem.Text = "Add section";
            this.addSectionToolStripMenuItem.Click += new System.EventHandler(this.addSectionToolStripMenuItem_Click);
            // 
            // phraseIsTODOToolStripMenuItem
            // 
            this.phraseIsTODOToolStripMenuItem.Name = "phraseIsTODOToolStripMenuItem";
            this.phraseIsTODOToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.phraseIsTODOToolStripMenuItem.Text = "Phrase is TODO";
            this.phraseIsTODOToolStripMenuItem.Visible = false;
            this.phraseIsTODOToolStripMenuItem.Click += new System.EventHandler(this.phraseIsTODOToolStripMenuItem_Click);
            // 
            // deselectPhraseToolStripMenuItem
            // 
            this.deselectPhraseToolStripMenuItem.Name = "deselectPhraseToolStripMenuItem";
            this.deselectPhraseToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.deselectPhraseToolStripMenuItem.Text = "Deselect Selected";
            this.deselectPhraseToolStripMenuItem.Click += new System.EventHandler(this.deselectSelectedToolStripMenuItem_Click);
            // 
            // Waveform_Recording
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.MaximumSize = new System.Drawing.Size(10000, 124);
            this.MinimumSize = new System.Drawing.Size(10000, 124);
            this.Name = "Waveform_Recording";
            this.Size = new System.Drawing.Size(10000, 124);
            this.VisibleChanged += new System.EventHandler(this.Waveform_Recording_VisibleChanged);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Waveform_Recording_MouseMove);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Waveform_Recording_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Waveform_Recording_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Waveform_Recording_MouseUp);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem markPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem phraseIsTODOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselectPhraseToolStripMenuItem;
    }
}
