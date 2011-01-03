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
            this.m_StatusLabelForMergeSection = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_tb_SelectedSection = new System.Windows.Forms.TextBox();
            this.m_btn_SelectAll = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.m_statusStripForMergeSection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lb_listofSectionsToMerge
            // 
            this.m_lb_listofSectionsToMerge.FormattingEnabled = true;
            this.m_lb_listofSectionsToMerge.Location = new System.Drawing.Point(15, 19);
            this.m_lb_listofSectionsToMerge.Name = "m_lb_listofSectionsToMerge";
            this.m_lb_listofSectionsToMerge.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.m_lb_listofSectionsToMerge.Size = new System.Drawing.Size(174, 199);
            this.m_lb_listofSectionsToMerge.TabIndex = 0;
            this.m_lb_listofSectionsToMerge.SelectedIndexChanged += new System.EventHandler(this.m_lb_listofSectionsToMerge_SelectedIndexChanged);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_OK.Location = new System.Drawing.Point(239, 207);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(92, 23);
            this.m_btn_OK.TabIndex = 1;
            this.m_btn_OK.Text = "OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.Location = new System.Drawing.Point(346, 207);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(100, 23);
            this.m_btn_Cancel.TabIndex = 2;
            this.m_btn_Cancel.Text = "Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // m_statusStripForMergeSection
            // 
            this.m_statusStripForMergeSection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusLabelForMergeSection});
            this.m_statusStripForMergeSection.Location = new System.Drawing.Point(0, 252);
            this.m_statusStripForMergeSection.Name = "m_statusStripForMergeSection";
            this.m_statusStripForMergeSection.Size = new System.Drawing.Size(473, 22);
            this.m_statusStripForMergeSection.TabIndex = 3;
            this.m_statusStripForMergeSection.Text = "MergeSectionStatusStrip";
            // 
            // m_StatusLabelForMergeSection
            // 
            this.m_StatusLabelForMergeSection.Name = "m_StatusLabelForMergeSection";
            this.m_StatusLabelForMergeSection.Size = new System.Drawing.Size(0, 17);
            // 
            // m_tb_SelectedSection
            // 
            this.m_tb_SelectedSection.Location = new System.Drawing.Point(6, 30);
            this.m_tb_SelectedSection.Multiline = true;
            this.m_tb_SelectedSection.Name = "m_tb_SelectedSection";
            this.m_tb_SelectedSection.ReadOnly = true;
            this.m_tb_SelectedSection.Size = new System.Drawing.Size(174, 20);
            this.m_tb_SelectedSection.TabIndex = 4;
            // 
            // m_btn_SelectAll
            // 
            this.m_btn_SelectAll.Location = new System.Drawing.Point(12, 74);
            this.m_btn_SelectAll.Name = "m_btn_SelectAll";
            this.m_btn_SelectAll.Size = new System.Drawing.Size(91, 23);
            this.m_btn_SelectAll.TabIndex = 5;
            this.m_btn_SelectAll.Text = "Select All";
            this.m_btn_SelectAll.UseVisualStyleBackColor = true;
            this.m_btn_SelectAll.Click += new System.EventHandler(this.m_btn_SelectAll_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_lb_listofSectionsToMerge);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 230);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "List of sections to merge";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.m_tb_SelectedSection);
            this.groupBox2.Controls.Add(this.m_btn_SelectAll);
            this.groupBox2.Location = new System.Drawing.Point(226, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 116);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // SelectMergeSectionRange
            // 
            this.AcceptButton = this.m_btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 274);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_statusStripForMergeSection);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Name = "SelectMergeSectionRange";
            this.Text = "SelectMergeSectionRange";
            this.m_statusStripForMergeSection.ResumeLayout(false);
            this.m_statusStripForMergeSection.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox m_lb_listofSectionsToMerge;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.StatusStrip m_statusStripForMergeSection;
        private System.Windows.Forms.ToolStripStatusLabel m_StatusLabelForMergeSection;
        private System.Windows.Forms.TextBox m_tb_SelectedSection;
        private System.Windows.Forms.Button m_btn_SelectAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}