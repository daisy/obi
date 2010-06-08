namespace Obi.ProjectView
    {
    partial class ContentViewLabel
        {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose ();
                }
            base.Dispose ( disposing );
            }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
            {
                this.m_lblStaticLabel = new System.Windows.Forms.Label();
                this.m_lblSectionName = new System.Windows.Forms.Label();
                this.SuspendLayout();
                // 
                // m_lblStaticLabel
                // 
                this.m_lblStaticLabel.AutoSize = true;
                this.m_lblStaticLabel.Location = new System.Drawing.Point(10, 4);
                this.m_lblStaticLabel.Name = "m_lblStaticLabel";
                this.m_lblStaticLabel.Size = new System.Drawing.Size(144, 13);
                this.m_lblStaticLabel.TabIndex = 0;
                this.m_lblStaticLabel.Text = "Showing contents of section:";
                // 
                // m_lblSectionName
                // 
                this.m_lblSectionName.AutoSize = true;
                this.m_lblSectionName.BackColor = System.Drawing.SystemColors.Control;
                this.m_lblSectionName.Location = new System.Drawing.Point(151, 4);
                this.m_lblSectionName.Name = "m_lblSectionName";
                this.m_lblSectionName.Size = new System.Drawing.Size(104, 13);
                this.m_lblSectionName.TabIndex = 1;
                this.m_lblSectionName.Text = "No section selected.";
                // 
                // ContentViewLabel
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Controls.Add(this.m_lblSectionName);
                this.Controls.Add(this.m_lblStaticLabel);
                this.Name = "ContentViewLabel";
                this.Size = new System.Drawing.Size(297, 22);
                this.ResumeLayout(false);
                this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Label m_lblStaticLabel;
        private System.Windows.Forms.Label m_lblSectionName;
        }
    }
