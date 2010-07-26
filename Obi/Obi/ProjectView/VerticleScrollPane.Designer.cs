namespace Obi.ProjectView
{
    partial class VerticleScrollPane
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
        this.components = new System.ComponentModel.Container ();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( VerticleScrollPane ) );
        this.m_BtnGoToBegining = new System.Windows.Forms.Button ();
        this.m_BtnSmallIncrementUp = new System.Windows.Forms.Button ();
        this.m_BtnLargeIncrementUp = new System.Windows.Forms.Button ();
        this.m_BtnLargeIncrementDown = new System.Windows.Forms.Button ();
        this.m_BtnSmallIncrementDown = new System.Windows.Forms.Button ();
        this.m_BtnGoToEnd = new System.Windows.Forms.Button ();
        this.trackBar1 = new System.Windows.Forms.TrackBar ();
        this.m_VerticalScrollTooltip = new System.Windows.Forms.ToolTip ( this.components );
        ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit ();
        this.SuspendLayout ();
        // 
        // m_BtnGoToBegining
        // 
        this.m_BtnGoToBegining.AccessibleDescription = "Button";
        this.m_BtnGoToBegining.AccessibleName = "Go To Begining ";
        this.m_BtnGoToBegining.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.m_BtnGoToBegining.Image = ((System.Drawing.Image)(resources.GetObject ( "m_BtnGoToBegining.Image" )));
        this.m_BtnGoToBegining.Location = new System.Drawing.Point ( 3, 31 );
        this.m_BtnGoToBegining.Name = "m_BtnGoToBegining";
        this.m_BtnGoToBegining.Size = new System.Drawing.Size ( 25, 50 );
        this.m_BtnGoToBegining.TabIndex = 0;
        this.m_VerticalScrollTooltip.SetToolTip ( this.m_BtnGoToBegining, "Go To Begining\r\n" );
        this.m_BtnGoToBegining.UseVisualStyleBackColor = false;
        this.m_BtnGoToBegining.Click += new System.EventHandler ( this.m_BtnGoToBegining_Click );
        // 
        // m_BtnSmallIncrementUp
        // 
        this.m_BtnSmallIncrementUp.AccessibleDescription = "Button";
        this.m_BtnSmallIncrementUp.AccessibleName = "Move One line up ";
        this.m_BtnSmallIncrementUp.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.m_BtnSmallIncrementUp.Image = ((System.Drawing.Image)(resources.GetObject ( "m_BtnSmallIncrementUp.Image" )));
        this.m_BtnSmallIncrementUp.Location = new System.Drawing.Point ( 2, 111 );
        this.m_BtnSmallIncrementUp.Name = "m_BtnSmallIncrementUp";
        this.m_BtnSmallIncrementUp.Size = new System.Drawing.Size ( 25, 50 );
        this.m_BtnSmallIncrementUp.TabIndex = 1;
        this.m_VerticalScrollTooltip.SetToolTip ( this.m_BtnSmallIncrementUp, "One line Up" );
        this.m_BtnSmallIncrementUp.UseVisualStyleBackColor = false;
        this.m_BtnSmallIncrementUp.Click += new System.EventHandler ( this.m_BtnSmallIncrementUp_Click );
        // 
        // m_BtnLargeIncrementUp
        // 
        this.m_BtnLargeIncrementUp.AccessibleDescription = "Button";
        this.m_BtnLargeIncrementUp.AccessibleName = "Move up to Large Increment";
        this.m_BtnLargeIncrementUp.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.m_BtnLargeIncrementUp.Image = ((System.Drawing.Image)(resources.GetObject ( "m_BtnLargeIncrementUp.Image" )));
        this.m_BtnLargeIncrementUp.Location = new System.Drawing.Point ( 3, 191 );
        this.m_BtnLargeIncrementUp.Name = "m_BtnLargeIncrementUp";
        this.m_BtnLargeIncrementUp.Size = new System.Drawing.Size ( 22, 50 );
        this.m_BtnLargeIncrementUp.TabIndex = 2;
        this.m_VerticalScrollTooltip.SetToolTip ( this.m_BtnLargeIncrementUp, "Large Increment Up\r\n\r\n" );
        this.m_BtnLargeIncrementUp.UseVisualStyleBackColor = false;
        this.m_BtnLargeIncrementUp.Click += new System.EventHandler ( this.m_BtnLargeIncrementUp_Click );
        // 
        // m_BtnLargeIncrementDown
        // 
        this.m_BtnLargeIncrementDown.AccessibleDescription = "Button";
        this.m_BtnLargeIncrementDown.AccessibleName = "Move Down to Large Increment";
        this.m_BtnLargeIncrementDown.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.m_BtnLargeIncrementDown.Image = ((System.Drawing.Image)(resources.GetObject ( "m_BtnLargeIncrementDown.Image" )));
        this.m_BtnLargeIncrementDown.Location = new System.Drawing.Point ( 3, 341 );
        this.m_BtnLargeIncrementDown.Name = "m_BtnLargeIncrementDown";
        this.m_BtnLargeIncrementDown.Size = new System.Drawing.Size ( 22, 50 );
        this.m_BtnLargeIncrementDown.TabIndex = 3;
        this.m_VerticalScrollTooltip.SetToolTip ( this.m_BtnLargeIncrementDown, "Large Increment Down\r\n" );
        this.m_BtnLargeIncrementDown.UseVisualStyleBackColor = false;
        this.m_BtnLargeIncrementDown.Click += new System.EventHandler ( this.m_BtnLargeIncrementDown_Click );
        // 
        // m_BtnSmallIncrementDown
        // 
        this.m_BtnSmallIncrementDown.AccessibleDescription = "Button";
        this.m_BtnSmallIncrementDown.AccessibleName = "Move one line Down ";
        this.m_BtnSmallIncrementDown.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.m_BtnSmallIncrementDown.Image = ((System.Drawing.Image)(resources.GetObject ( "m_BtnSmallIncrementDown.Image" )));
        this.m_BtnSmallIncrementDown.Location = new System.Drawing.Point ( 2, 423 );
        this.m_BtnSmallIncrementDown.Name = "m_BtnSmallIncrementDown";
        this.m_BtnSmallIncrementDown.Size = new System.Drawing.Size ( 25, 50 );
        this.m_BtnSmallIncrementDown.TabIndex = 4;
        this.m_VerticalScrollTooltip.SetToolTip ( this.m_BtnSmallIncrementDown, "One Line Down" );
        this.m_BtnSmallIncrementDown.UseVisualStyleBackColor = false;
        this.m_BtnSmallIncrementDown.Click += new System.EventHandler ( this.m_BtnSmallIncrementDown_Click );
        // 
        // m_BtnGoToEnd
        // 
        this.m_BtnGoToEnd.AccessibleDescription = "Button";
        this.m_BtnGoToEnd.AccessibleName = "Go to end ";
        this.m_BtnGoToEnd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
        this.m_BtnGoToEnd.Image = ((System.Drawing.Image)(resources.GetObject ( "m_BtnGoToEnd.Image" )));
        this.m_BtnGoToEnd.Location = new System.Drawing.Point ( 2, 507 );
        this.m_BtnGoToEnd.Name = "m_BtnGoToEnd";
        this.m_BtnGoToEnd.Size = new System.Drawing.Size ( 25, 52 );
        this.m_BtnGoToEnd.TabIndex = 5;
        this.m_VerticalScrollTooltip.SetToolTip ( this.m_BtnGoToEnd, "Go to End" );
        this.m_BtnGoToEnd.UseVisualStyleBackColor = false;
        this.m_BtnGoToEnd.Click += new System.EventHandler ( this.m_BtnGoToEnd_Click );
        // 
        // trackBar1
        // 
        this.trackBar1.Enabled = false;
        this.trackBar1.Location = new System.Drawing.Point ( 0, 258 );
        this.trackBar1.Maximum = 100;
        this.trackBar1.Name = "trackBar1";
        this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
        this.trackBar1.Size = new System.Drawing.Size ( 42, 77 );
        this.trackBar1.TabIndex = 6;
        this.m_VerticalScrollTooltip.SetToolTip ( this.trackBar1, "Median of Phrases" );
        this.trackBar1.Scroll += new System.EventHandler ( this.trackBar1_Scroll );
        // 
        // m_VerticalScrollTooltip
        // 
        this.m_VerticalScrollTooltip.Tag = "Go to beginning";
        this.m_VerticalScrollTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
        // 
        // VerticleScrollPane
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add ( this.trackBar1 );
        this.Controls.Add ( this.m_BtnGoToEnd );
        this.Controls.Add ( this.m_BtnSmallIncrementDown );
        this.Controls.Add ( this.m_BtnLargeIncrementDown );
        this.Controls.Add ( this.m_BtnLargeIncrementUp );
        this.Controls.Add ( this.m_BtnSmallIncrementUp );
        this.Controls.Add ( this.m_BtnGoToBegining );
        this.Name = "VerticleScrollPane";
        this.Size = new System.Drawing.Size ( 30, 563 );
        ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit ();
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.Button m_BtnGoToBegining;
        private System.Windows.Forms.Button m_BtnSmallIncrementUp;
        private System.Windows.Forms.Button m_BtnLargeIncrementUp;
        private System.Windows.Forms.Button m_BtnLargeIncrementDown;
        private System.Windows.Forms.Button m_BtnSmallIncrementDown;
        private System.Windows.Forms.Button m_BtnGoToEnd;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ToolTip m_VerticalScrollTooltip;
    }
}
