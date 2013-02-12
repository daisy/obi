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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectMergeSectionRange));
            this.m_lb_listofSectionsToMerge = new System.Windows.Forms.ListBox();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_statusStripForMergeSection = new System.Windows.Forms.StatusStrip();
            this.m_StatusLabelForMergeSection = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_tb_SelectedSection = new System.Windows.Forms.TextBox();
            this.m_btn_SelectAll = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_statusStripForMergeSection.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lb_listofSectionsToMerge
            // 
            this.m_lb_listofSectionsToMerge.FormattingEnabled = true;
            resources.ApplyResources(this.m_lb_listofSectionsToMerge, "m_lb_listofSectionsToMerge");
            this.m_lb_listofSectionsToMerge.Name = "m_lb_listofSectionsToMerge";
            this.m_lb_listofSectionsToMerge.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.m_lb_listofSectionsToMerge.SelectedIndexChanged += new System.EventHandler(this.m_lb_listofSectionsToMerge_SelectedIndexChanged);
            // 
            // m_btn_OK
            // 
            resources.ApplyResources(this.m_btn_OK, "m_btn_OK");
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btn_Cancel, "m_btn_Cancel");
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // m_statusStripForMergeSection
            // 
            this.m_statusStripForMergeSection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusLabelForMergeSection});
            resources.ApplyResources(this.m_statusStripForMergeSection, "m_statusStripForMergeSection");
            this.m_statusStripForMergeSection.Name = "m_statusStripForMergeSection";
            // 
            // m_StatusLabelForMergeSection
            // 
            this.m_StatusLabelForMergeSection.Name = "m_StatusLabelForMergeSection";
            resources.ApplyResources(this.m_StatusLabelForMergeSection, "m_StatusLabelForMergeSection");
            // 
            // m_tb_SelectedSection
            // 
            resources.ApplyResources(this.m_tb_SelectedSection, "m_tb_SelectedSection");
            this.m_tb_SelectedSection.Name = "m_tb_SelectedSection";
            this.m_tb_SelectedSection.ReadOnly = true;
            // 
            // m_btn_SelectAll
            // 
            resources.ApplyResources(this.m_btn_SelectAll, "m_btn_SelectAll");
            this.m_btn_SelectAll.Name = "m_btn_SelectAll";
            this.m_btn_SelectAll.UseVisualStyleBackColor = true;
            this.m_btn_SelectAll.Click += new System.EventHandler(this.m_btn_SelectAll_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_lb_listofSectionsToMerge);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.m_tb_SelectedSection);
            this.groupBox2.Controls.Add(this.m_btn_SelectAll);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // SelectMergeSectionRange
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_statusStripForMergeSection);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectMergeSectionRange";
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}