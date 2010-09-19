namespace Obi.ProjectView
{
    partial class VerticalScrollToolStripContainer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerticalScrollToolStripContainer));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStripTop = new System.Windows.Forms.ToolStrip();
            this.m_BtnGoToBegining = new System.Windows.Forms.ToolStripButton();
            this.m_BtnLargeIncrementUp = new System.Windows.Forms.ToolStripButton();
            this.m_BtnSmallIncrementUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripBottom = new System.Windows.Forms.ToolStrip();
            this.m_BtnSmallIncrementDown = new System.Windows.Forms.ToolStripButton();
            this.m_BtnLargeIncrementDown = new System.Windows.Forms.ToolStripButton();
            this.m_BtnGoToEnd = new System.Windows.Forms.ToolStripButton();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.toolStripContainer1.RightToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStripTop.SuspendLayout();
            this.toolStripBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(0, 508);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 10);
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.toolStripContainer1.RightToolStripPanel.Controls.Add(this.toolStripTop);
            this.toolStripContainer1.RightToolStripPanel.Controls.Add(this.toolStripBottom);
            this.toolStripContainer1.Size = new System.Drawing.Size(30, 533);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripTop
            // 
            this.toolStripTop.AutoSize = false;
            this.toolStripTop.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTop.ImageScalingSize = new System.Drawing.Size(24, 40);
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_BtnGoToBegining,
            this.m_BtnLargeIncrementUp,
            this.m_BtnSmallIncrementUp});
            this.toolStripTop.Location = new System.Drawing.Point(0, 3);
            this.toolStripTop.Name = "toolStripTop";
            this.toolStripTop.Size = new System.Drawing.Size(35, 195);
            this.toolStripTop.TabIndex = 1;
            // 
            // m_BtnGoToBegining
            // 
            this.m_BtnGoToBegining.AutoSize = false;
            this.m_BtnGoToBegining.AutoToolTip = false;
            this.m_BtnGoToBegining.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnGoToBegining.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnGoToBegining.Image")));
            this.m_BtnGoToBegining.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_BtnGoToBegining.Margin = new System.Windows.Forms.Padding(4, 1, 0, 10);
            this.m_BtnGoToBegining.Name = "m_BtnGoToBegining";
            this.m_BtnGoToBegining.Size = new System.Drawing.Size(29, 44);
            this.m_BtnGoToBegining.Text = "Go to beginning";
            this.m_BtnGoToBegining.ToolTipText = "Go to beginning";
            this.m_BtnGoToBegining.Click += new System.EventHandler(this.m_BtnGoToBegining_Click);
            // 
            // m_BtnLargeIncrementUp
            // 
            this.m_BtnLargeIncrementUp.AutoSize = false;
            this.m_BtnLargeIncrementUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnLargeIncrementUp.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnLargeIncrementUp.Image")));
            this.m_BtnLargeIncrementUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_BtnLargeIncrementUp.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnLargeIncrementUp.Name = "m_BtnLargeIncrementUp";
            this.m_BtnLargeIncrementUp.Size = new System.Drawing.Size(27, 44);
            this.m_BtnLargeIncrementUp.Text = "Large increment up";
            this.m_BtnLargeIncrementUp.ToolTipText = "Large increment up";
            this.m_BtnLargeIncrementUp.Click += new System.EventHandler(this.m_BtnLargeIncrementUp_Click);
            // 
            // m_BtnSmallIncrementUp
            // 
            this.m_BtnSmallIncrementUp.AutoSize = false;
            this.m_BtnSmallIncrementUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnSmallIncrementUp.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnSmallIncrementUp.Image")));
            this.m_BtnSmallIncrementUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_BtnSmallIncrementUp.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnSmallIncrementUp.Name = "m_BtnSmallIncrementUp";
            this.m_BtnSmallIncrementUp.Size = new System.Drawing.Size(27, 44);
            this.m_BtnSmallIncrementUp.Text = "Go one line up";
            this.m_BtnSmallIncrementUp.ToolTipText = "Go one line up";
            this.m_BtnSmallIncrementUp.Click += new System.EventHandler(this.m_BtnSmallIncrementUp_Click);
            // 
            // toolStripBottom
            // 
            this.toolStripBottom.AutoSize = false;
            this.toolStripBottom.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripBottom.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripBottom.ImageScalingSize = new System.Drawing.Size(24, 40);
            this.toolStripBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_BtnSmallIncrementDown,
            this.m_BtnLargeIncrementDown,
            this.m_BtnGoToEnd});
            this.toolStripBottom.Location = new System.Drawing.Point(0, 293);
            this.toolStripBottom.Name = "toolStripBottom";
            this.toolStripBottom.Size = new System.Drawing.Size(35, 195);
            this.toolStripBottom.TabIndex = 0;
            // 
            // m_BtnSmallIncrementDown
            // 
            this.m_BtnSmallIncrementDown.AutoSize = false;
            this.m_BtnSmallIncrementDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnSmallIncrementDown.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnSmallIncrementDown.Image")));
            this.m_BtnSmallIncrementDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_BtnSmallIncrementDown.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnSmallIncrementDown.Name = "m_BtnSmallIncrementDown";
            this.m_BtnSmallIncrementDown.Size = new System.Drawing.Size(27, 44);
            this.m_BtnSmallIncrementDown.Text = "Go one line down";
            this.m_BtnSmallIncrementDown.ToolTipText = "Go one line down";
            this.m_BtnSmallIncrementDown.Click += new System.EventHandler(this.m_BtnSmallIncrementDown_Click);
            // 
            // m_BtnLargeIncrementDown
            // 
            this.m_BtnLargeIncrementDown.AutoSize = false;
            this.m_BtnLargeIncrementDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnLargeIncrementDown.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnLargeIncrementDown.Image")));
            this.m_BtnLargeIncrementDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_BtnLargeIncrementDown.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnLargeIncrementDown.Name = "m_BtnLargeIncrementDown";
            this.m_BtnLargeIncrementDown.Size = new System.Drawing.Size(27, 44);
            this.m_BtnLargeIncrementDown.Text = "Large increment down";
            this.m_BtnLargeIncrementDown.ToolTipText = "Large increment down";
            this.m_BtnLargeIncrementDown.Click += new System.EventHandler(this.m_BtnLargeIncrementDown_Click);
            // 
            // m_BtnGoToEnd
            // 
            this.m_BtnGoToEnd.AutoSize = false;
            this.m_BtnGoToEnd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnGoToEnd.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnGoToEnd.Image")));
            this.m_BtnGoToEnd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.m_BtnGoToEnd.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnGoToEnd.Name = "m_BtnGoToEnd";
            this.m_BtnGoToEnd.Size = new System.Drawing.Size(27, 44);
            this.m_BtnGoToEnd.Text = "Go to end";
            this.m_BtnGoToEnd.ToolTipText = "Go to end";
            this.m_BtnGoToEnd.Click += new System.EventHandler(this.m_BtnGoToEnd_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.AutoSize = false;
            this.trackBar1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.trackBar1.Enabled = false;
            this.trackBar1.Location = new System.Drawing.Point(-2, 245);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(32, 104);
            this.trackBar1.TabIndex = 1;
            // 
            // VerticalScrollToolStripContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "VerticalScrollToolStripContainer";
            this.Size = new System.Drawing.Size(33, 539);
            this.Resize += new System.EventHandler(this.VerticalScrollToolStripContainer_Resize);
            this.toolStripContainer1.RightToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStripTop.ResumeLayout(false);
            this.toolStripTop.PerformLayout();
            this.toolStripBottom.ResumeLayout(false);
            this.toolStripBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ToolStrip toolStripBottom;
        private System.Windows.Forms.ToolStrip toolStripTop;
        private System.Windows.Forms.ToolStripButton m_BtnSmallIncrementDown;
        private System.Windows.Forms.ToolStripButton m_BtnLargeIncrementDown;
        private System.Windows.Forms.ToolStripButton m_BtnGoToEnd;
        private System.Windows.Forms.ToolStripButton m_BtnGoToBegining;
        private System.Windows.Forms.ToolStripButton m_BtnLargeIncrementUp;
        private System.Windows.Forms.ToolStripButton m_BtnSmallIncrementUp;
    }
}
