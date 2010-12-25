namespace Obi.Dialogs
{
    partial class SelectMergeSectionRange
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
            this.m_lb_listofSectionsToMerge = new System.Windows.Forms.ListBox();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_statusStripForMergeSection = new System.Windows.Forms.StatusStrip();
            this.m_toolStripStatusLabelForMergeSection = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_statusStripForMergeSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lb_listofSectionsToMerge
            // 
            this.m_lb_listofSectionsToMerge.FormattingEnabled = true;
            this.m_lb_listofSectionsToMerge.Location = new System.Drawing.Point(55, 12);
            this.m_lb_listofSectionsToMerge.Name = "m_lb_listofSectionsToMerge";
            this.m_lb_listofSectionsToMerge.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.m_lb_listofSectionsToMerge.Size = new System.Drawing.Size(120, 173);
            this.m_lb_listofSectionsToMerge.TabIndex = 0;
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.Location = new System.Drawing.Point(12, 205);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 1;
            this.m_btn_OK.Text = "OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.Location = new System.Drawing.Point(124, 205);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 2;
            this.m_btn_Cancel.Text = "Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // m_statusStripForMergeSection
            // 
            this.m_statusStripForMergeSection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_toolStripStatusLabelForMergeSection});
            this.m_statusStripForMergeSection.Location = new System.Drawing.Point(0, 242);
            this.m_statusStripForMergeSection.Name = "m_statusStripForMergeSection";
            this.m_statusStripForMergeSection.Size = new System.Drawing.Size(243, 22);
            this.m_statusStripForMergeSection.TabIndex = 3;
            this.m_statusStripForMergeSection.Text = "MergeSectionStatusStrip";
            // 
            // m_toolStripStatusLabelForMergeSection
            // 
            this.m_toolStripStatusLabelForMergeSection.Name = "m_toolStripStatusLabelForMergeSection";
            this.m_toolStripStatusLabelForMergeSection.Size = new System.Drawing.Size(0, 17);
            // 
            // SelectMergeSectionRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 264);
            this.Controls.Add(this.m_statusStripForMergeSection);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_lb_listofSectionsToMerge);
            this.Name = "SelectMergeSectionRange";
            this.Text = "SelectMergeSectionRange";
            this.m_statusStripForMergeSection.ResumeLayout(false);
            this.m_statusStripForMergeSection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox m_lb_listofSectionsToMerge;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.StatusStrip m_statusStripForMergeSection;
        private System.Windows.Forms.ToolStripStatusLabel m_toolStripStatusLabelForMergeSection;
    }
}