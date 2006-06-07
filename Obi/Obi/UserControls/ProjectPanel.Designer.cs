namespace Obi.UserControls
{
    partial class ProjectPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.mSplitContainer = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mSplitContainer.Panel1.SuspendLayout();
            this.mSplitContainer.Panel2.SuspendLayout();
            this.mSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(285, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "(No project)";
            // 
            // mSplitContainer
            // 
            this.mSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mSplitContainer.Name = "mSplitContainer";
            // 
            // mSplitContainer.Panel1
            // 
            this.mSplitContainer.Panel1.BackColor = System.Drawing.Color.PowderBlue;
            this.mSplitContainer.Panel1.Controls.Add(this.label2);
            // 
            // mSplitContainer.Panel2
            // 
            this.mSplitContainer.Panel2.BackColor = System.Drawing.Color.SpringGreen;
            this.mSplitContainer.Panel2.Controls.Add(this.label4);
            this.mSplitContainer.Size = new System.Drawing.Size(631, 330);
            this.mSplitContainer.SplitterDistance = 210;
            this.mSplitContainer.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(91, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "NCX";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.SpringGreen;
            this.label4.Location = new System.Drawing.Point(191, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Strips";
            // 
            // ProjectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mSplitContainer);
            this.Controls.Add(this.label1);
            this.Name = "ProjectPanel";
            this.Size = new System.Drawing.Size(631, 330);
            this.mSplitContainer.Panel1.ResumeLayout(false);
            this.mSplitContainer.Panel1.PerformLayout();
            this.mSplitContainer.Panel2.ResumeLayout(false);
            this.mSplitContainer.Panel2.PerformLayout();
            this.mSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer mSplitContainer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;

    }
}
