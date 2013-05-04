namespace Obi.Dialogs
{
    partial class AssociateSpecialNode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssociateSpecialNode));
            this.m_lb_ListOfSpecialNodes = new System.Windows.Forms.ListBox();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_txtBox_SectionName = new System.Windows.Forms.TextBox();
            this.m_btn_Associate = new System.Windows.Forms.Button();
            this.m_btn_Deassociate = new System.Windows.Forms.Button();
            this.m_lb_listOfAllAnchorNodes = new System.Windows.Forms.ListBox();
            this.m_btn_ShowAll = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lb_ListOfSpecialNodes
            // 
            this.m_lb_ListOfSpecialNodes.FormattingEnabled = true;
            resources.ApplyResources(this.m_lb_ListOfSpecialNodes, "m_lb_ListOfSpecialNodes");
            this.m_lb_ListOfSpecialNodes.Name = "m_lb_ListOfSpecialNodes";
            this.m_lb_ListOfSpecialNodes.SelectedIndexChanged += new System.EventHandler(this.m_lb_ListOfSpecialNodes_SelectedIndexChanged);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
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
            // 
            // m_txtBox_SectionName
            // 
            resources.ApplyResources(this.m_txtBox_SectionName, "m_txtBox_SectionName");
            this.m_txtBox_SectionName.Name = "m_txtBox_SectionName";
            this.m_txtBox_SectionName.ReadOnly = true;
            // 
            // m_btn_Associate
            // 
            resources.ApplyResources(this.m_btn_Associate, "m_btn_Associate");
            this.m_btn_Associate.Name = "m_btn_Associate";
            this.m_btn_Associate.UseVisualStyleBackColor = true;
            this.m_btn_Associate.Click += new System.EventHandler(this.m_btn_Associate_Click);
            // 
            // m_btn_Deassociate
            // 
            resources.ApplyResources(this.m_btn_Deassociate, "m_btn_Deassociate");
            this.m_btn_Deassociate.Name = "m_btn_Deassociate";
            this.m_btn_Deassociate.UseVisualStyleBackColor = true;
            this.m_btn_Deassociate.Click += new System.EventHandler(this.m_btn_Deassociate_Click);
            // 
            // m_lb_listOfAllAnchorNodes
            // 
            this.m_lb_listOfAllAnchorNodes.FormattingEnabled = true;
            resources.ApplyResources(this.m_lb_listOfAllAnchorNodes, "m_lb_listOfAllAnchorNodes");
            this.m_lb_listOfAllAnchorNodes.Name = "m_lb_listOfAllAnchorNodes";
            this.m_lb_listOfAllAnchorNodes.SelectedIndexChanged += new System.EventHandler(this.m_lb_listOfAllAnchorNodes_SelectedIndexChanged);
            // 
            // m_btn_ShowAll
            // 
            resources.ApplyResources(this.m_btn_ShowAll, "m_btn_ShowAll");
            this.m_btn_ShowAll.Name = "m_btn_ShowAll";
            this.m_btn_ShowAll.UseVisualStyleBackColor = true;
            this.m_btn_ShowAll.Click += new System.EventHandler(this.m_btn_ShowAll_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_lb_listOfAllAnchorNodes);
            this.groupBox1.Controls.Add(this.m_txtBox_SectionName);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.m_lb_ListOfSpecialNodes);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.m_btn_Associate);
            this.groupBox3.Controls.Add(this.m_btn_Deassociate);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // AssociateSpecialNode
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_btn_ShowAll);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AssociateSpecialNode";
            this.Load += new System.EventHandler(this.AssociateSpecialNode_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox m_lb_ListOfSpecialNodes;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.TextBox m_txtBox_SectionName;
        private System.Windows.Forms.Button m_btn_Associate;
        private System.Windows.Forms.Button m_btn_Deassociate;
        private System.Windows.Forms.ListBox m_lb_listOfAllAnchorNodes;
        private System.Windows.Forms.Button m_btn_ShowAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}