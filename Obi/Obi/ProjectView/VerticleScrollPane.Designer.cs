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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerticleScrollPane));
            this.m_BtnGoToBegining = new System.Windows.Forms.Button();
            this.m_BtnOneLineUp = new System.Windows.Forms.Button();
            this.m_BtnLargeIncrementUp = new System.Windows.Forms.Button();
            this.m_BtnLargeIncrementDown = new System.Windows.Forms.Button();
            this.m_BtnOneLineDown = new System.Windows.Forms.Button();
            this.m_BtnGoToEnd = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // m_BtnGoToBegining
            // 
            this.m_BtnGoToBegining.AccessibleDescription = "";
            this.m_BtnGoToBegining.AccessibleName = "Go To Begining of the Section";
            this.m_BtnGoToBegining.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.m_BtnGoToBegining.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnGoToBegining.Image")));
            this.m_BtnGoToBegining.Location = new System.Drawing.Point(3, 31);
            this.m_BtnGoToBegining.Name = "m_BtnGoToBegining";
            this.m_BtnGoToBegining.Size = new System.Drawing.Size(25, 50);
            this.m_BtnGoToBegining.TabIndex = 0;
            this.toolTip1.SetToolTip(this.m_BtnGoToBegining, "Go To Begining\r\n");
            this.m_BtnGoToBegining.UseVisualStyleBackColor = false;
            this.m_BtnGoToBegining.Click += new System.EventHandler(this.m_BtnGoToBegining_Click);
            // 
            // m_BtnOneLineUp
            // 
            this.m_BtnOneLineUp.AccessibleDescription = "";
            this.m_BtnOneLineUp.AccessibleName = "Move One line up of the current Phrases.";
            this.m_BtnOneLineUp.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.m_BtnOneLineUp.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnOneLineUp.Image")));
            this.m_BtnOneLineUp.Location = new System.Drawing.Point(2, 111);
            this.m_BtnOneLineUp.Name = "m_BtnOneLineUp";
            this.m_BtnOneLineUp.Size = new System.Drawing.Size(25, 50);
            this.m_BtnOneLineUp.TabIndex = 1;
            this.toolTip1.SetToolTip(this.m_BtnOneLineUp, "One line Up");
            this.m_BtnOneLineUp.UseVisualStyleBackColor = false;
            this.m_BtnOneLineUp.Click += new System.EventHandler(this.m_BtnOneLineUp_Click);
            // 
            // m_BtnLargeIncrementUp
            // 
            this.m_BtnLargeIncrementUp.AccessibleDescription = "";
            this.m_BtnLargeIncrementUp.AccessibleName = "Move up to Large Increment";
            this.m_BtnLargeIncrementUp.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.m_BtnLargeIncrementUp.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnLargeIncrementUp.Image")));
            this.m_BtnLargeIncrementUp.Location = new System.Drawing.Point(3, 191);
            this.m_BtnLargeIncrementUp.Name = "m_BtnLargeIncrementUp";
            this.m_BtnLargeIncrementUp.Size = new System.Drawing.Size(22, 50);
            this.m_BtnLargeIncrementUp.TabIndex = 2;
            this.toolTip1.SetToolTip(this.m_BtnLargeIncrementUp, "Large Increment Up\r\n\r\n");
            this.m_BtnLargeIncrementUp.UseVisualStyleBackColor = false;
            this.m_BtnLargeIncrementUp.Click += new System.EventHandler(this.m_BtnLargeIncrementUp_Click);
            // 
            // m_BtnLargeIncrementDown
            // 
            this.m_BtnLargeIncrementDown.AccessibleDescription = "";
            this.m_BtnLargeIncrementDown.AccessibleName = "Move Down to Large Increment";
            this.m_BtnLargeIncrementDown.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.m_BtnLargeIncrementDown.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnLargeIncrementDown.Image")));
            this.m_BtnLargeIncrementDown.Location = new System.Drawing.Point(3, 341);
            this.m_BtnLargeIncrementDown.Name = "m_BtnLargeIncrementDown";
            this.m_BtnLargeIncrementDown.Size = new System.Drawing.Size(22, 50);
            this.m_BtnLargeIncrementDown.TabIndex = 3;
            this.toolTip1.SetToolTip(this.m_BtnLargeIncrementDown, "Large Increment Down\r\n");
            this.m_BtnLargeIncrementDown.UseVisualStyleBackColor = false;
            this.m_BtnLargeIncrementDown.Click += new System.EventHandler(this.m_BtnLargeIncrementDown_Click);
            // 
            // m_BtnOneLineDown
            // 
            this.m_BtnOneLineDown.AccessibleDescription = "";
            this.m_BtnOneLineDown.AccessibleName = "Move one line Down to current phrases.";
            this.m_BtnOneLineDown.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.m_BtnOneLineDown.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnOneLineDown.Image")));
            this.m_BtnOneLineDown.Location = new System.Drawing.Point(2, 423);
            this.m_BtnOneLineDown.Name = "m_BtnOneLineDown";
            this.m_BtnOneLineDown.Size = new System.Drawing.Size(25, 50);
            this.m_BtnOneLineDown.TabIndex = 4;
            this.toolTip1.SetToolTip(this.m_BtnOneLineDown, "One Line Down");
            this.m_BtnOneLineDown.UseVisualStyleBackColor = false;
            this.m_BtnOneLineDown.Click += new System.EventHandler(this.m_BtnOneLineDown_Click);
            // 
            // m_BtnGoToEnd
            // 
            this.m_BtnGoToEnd.AccessibleDescription = "";
            this.m_BtnGoToEnd.AccessibleName = "Go to end of the section";
            this.m_BtnGoToEnd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.m_BtnGoToEnd.Image = ((System.Drawing.Image)(resources.GetObject("m_BtnGoToEnd.Image")));
            this.m_BtnGoToEnd.Location = new System.Drawing.Point(2, 507);
            this.m_BtnGoToEnd.Name = "m_BtnGoToEnd";
            this.m_BtnGoToEnd.Size = new System.Drawing.Size(25, 52);
            this.m_BtnGoToEnd.TabIndex = 5;
            this.toolTip1.SetToolTip(this.m_BtnGoToEnd, "Go to End");
            this.m_BtnGoToEnd.UseVisualStyleBackColor = false;
            this.m_BtnGoToEnd.Click += new System.EventHandler(this.m_BtnGoToEnd_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(0, 258);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 77);
            this.trackBar1.TabIndex = 6;
            this.toolTip1.SetToolTip(this.trackBar1, "Median of Phrases");
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // toolTip1
            // 
            this.toolTip1.Tag = "Go to beginning";
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // VerticleScrollPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.m_BtnGoToEnd);
            this.Controls.Add(this.m_BtnOneLineDown);
            this.Controls.Add(this.m_BtnLargeIncrementDown);
            this.Controls.Add(this.m_BtnLargeIncrementUp);
            this.Controls.Add(this.m_BtnOneLineUp);
            this.Controls.Add(this.m_BtnGoToBegining);
            this.Name = "VerticleScrollPane";
            this.Size = new System.Drawing.Size(30, 563);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_BtnGoToBegining;
        private System.Windows.Forms.Button m_BtnOneLineUp;
        private System.Windows.Forms.Button m_BtnLargeIncrementUp;
        private System.Windows.Forms.Button m_BtnLargeIncrementDown;
        private System.Windows.Forms.Button m_BtnOneLineDown;
        private System.Windows.Forms.Button m_BtnGoToEnd;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
