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
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.toolStripContainer1.RightToolStripPanel.Controls.Add(this.toolStripTop);
            this.toolStripContainer1.RightToolStripPanel.Controls.Add(this.toolStripBottom);
            // 
            // toolStripTop
            // 
            resources.ApplyResources(this.toolStripTop, "toolStripTop");
            this.toolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTop.ImageScalingSize = new System.Drawing.Size(24, 40);
            this.toolStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_BtnGoToBegining,
            this.m_BtnLargeIncrementUp,
            this.m_BtnSmallIncrementUp});
            this.toolStripTop.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripTop.Name = "toolStripTop";
            // 
            // m_BtnGoToBegining
            // 
            resources.ApplyResources(this.m_BtnGoToBegining, "m_BtnGoToBegining");
            this.m_BtnGoToBegining.AutoToolTip = false;
            this.m_BtnGoToBegining.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnGoToBegining.Margin = new System.Windows.Forms.Padding(4, 1, 0, 10);
            this.m_BtnGoToBegining.Name = "m_BtnGoToBegining";
            this.m_BtnGoToBegining.Click += new System.EventHandler(this.m_BtnGoToBegining_Click);
            // 
            // m_BtnLargeIncrementUp
            // 
            resources.ApplyResources(this.m_BtnLargeIncrementUp, "m_BtnLargeIncrementUp");
            this.m_BtnLargeIncrementUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnLargeIncrementUp.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnLargeIncrementUp.Name = "m_BtnLargeIncrementUp";
            this.m_BtnLargeIncrementUp.Click += new System.EventHandler(this.m_BtnLargeIncrementUp_Click);
            // 
            // m_BtnSmallIncrementUp
            // 
            resources.ApplyResources(this.m_BtnSmallIncrementUp, "m_BtnSmallIncrementUp");
            this.m_BtnSmallIncrementUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnSmallIncrementUp.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnSmallIncrementUp.Name = "m_BtnSmallIncrementUp";
            this.m_BtnSmallIncrementUp.Click += new System.EventHandler(this.m_BtnSmallIncrementUp_Click);
            // 
            // toolStripBottom
            // 
            resources.ApplyResources(this.toolStripBottom, "toolStripBottom");
            this.toolStripBottom.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripBottom.ImageScalingSize = new System.Drawing.Size(24, 40);
            this.toolStripBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_BtnSmallIncrementDown,
            this.m_BtnLargeIncrementDown,
            this.m_BtnGoToEnd});
            this.toolStripBottom.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripBottom.Name = "toolStripBottom";
            // 
            // m_BtnSmallIncrementDown
            // 
            resources.ApplyResources(this.m_BtnSmallIncrementDown, "m_BtnSmallIncrementDown");
            this.m_BtnSmallIncrementDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnSmallIncrementDown.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnSmallIncrementDown.Name = "m_BtnSmallIncrementDown";
            this.m_BtnSmallIncrementDown.Click += new System.EventHandler(this.m_BtnSmallIncrementDown_Click);
            // 
            // m_BtnLargeIncrementDown
            // 
            resources.ApplyResources(this.m_BtnLargeIncrementDown, "m_BtnLargeIncrementDown");
            this.m_BtnLargeIncrementDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnLargeIncrementDown.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnLargeIncrementDown.Name = "m_BtnLargeIncrementDown";
            this.m_BtnLargeIncrementDown.Click += new System.EventHandler(this.m_BtnLargeIncrementDown_Click);
            // 
            // m_BtnGoToEnd
            // 
            resources.ApplyResources(this.m_BtnGoToEnd, "m_BtnGoToEnd");
            this.m_BtnGoToEnd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.m_BtnGoToEnd.Margin = new System.Windows.Forms.Padding(7, 1, 0, 10);
            this.m_BtnGoToEnd.Name = "m_BtnGoToEnd";
            this.m_BtnGoToEnd.Click += new System.EventHandler(this.m_BtnGoToEnd_Click);
            // 
            // trackBar1
            // 
            resources.ApplyResources(this.trackBar1, "trackBar1");
            this.trackBar1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            // 
            // VerticalScrollToolStripContainer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "VerticalScrollToolStripContainer";
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
