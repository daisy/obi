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
            this.m_lb_ListOfSpecialNodes = new System.Windows.Forms.ListBox();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_txtBox_SectionName = new System.Windows.Forms.TextBox();
            this.m_btn_Associate = new System.Windows.Forms.Button();
            this.m_btn_Deassociate = new System.Windows.Forms.Button();
            this.m_lb_listOfAllAnchorNodes = new System.Windows.Forms.ListBox();
            this.m_btn_ShowAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lb_ListOfSpecialNodes
            // 
            this.m_lb_ListOfSpecialNodes.FormattingEnabled = true;
            this.m_lb_ListOfSpecialNodes.Location = new System.Drawing.Point(146, 38);
            this.m_lb_ListOfSpecialNodes.Name = "m_lb_ListOfSpecialNodes";
            this.m_lb_ListOfSpecialNodes.Size = new System.Drawing.Size(256, 147);
            this.m_lb_ListOfSpecialNodes.TabIndex = 0;
            this.m_lb_ListOfSpecialNodes.SelectedIndexChanged += new System.EventHandler(this.m_lb_ListOfSpecialNodes_SelectedIndexChanged);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_OK.Location = new System.Drawing.Point(222, 238);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 1;
            this.m_btn_OK.Text = "OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.Location = new System.Drawing.Point(314, 238);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(88, 23);
            this.m_btn_Cancel.TabIndex = 2;
            this.m_btn_Cancel.Text = "Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // m_txtBox_SectionName
            // 
            this.m_txtBox_SectionName.Location = new System.Drawing.Point(6, 38);
            this.m_txtBox_SectionName.Name = "m_txtBox_SectionName";
            this.m_txtBox_SectionName.Size = new System.Drawing.Size(120, 20);
            this.m_txtBox_SectionName.TabIndex = 3;
            // 
            // m_btn_Associate
            // 
            this.m_btn_Associate.Location = new System.Drawing.Point(11, 238);
            this.m_btn_Associate.Name = "m_btn_Associate";
            this.m_btn_Associate.Size = new System.Drawing.Size(95, 23);
            this.m_btn_Associate.TabIndex = 4;
            this.m_btn_Associate.Text = "Associate";
            this.m_btn_Associate.UseVisualStyleBackColor = true;
            this.m_btn_Associate.Click += new System.EventHandler(this.m_btn_Associate_Click);
            // 
            // m_btn_Deassociate
            // 
            this.m_btn_Deassociate.Location = new System.Drawing.Point(122, 238);
            this.m_btn_Deassociate.Name = "m_btn_Deassociate";
            this.m_btn_Deassociate.Size = new System.Drawing.Size(88, 23);
            this.m_btn_Deassociate.TabIndex = 5;
            this.m_btn_Deassociate.Text = "Deasscociate";
            this.m_btn_Deassociate.UseVisualStyleBackColor = true;
            this.m_btn_Deassociate.Click += new System.EventHandler(this.m_btn_Deassociate_Click);
            // 
            // m_lb_listOfAllAnchorNodes
            // 
            this.m_lb_listOfAllAnchorNodes.FormattingEnabled = true;
            this.m_lb_listOfAllAnchorNodes.Location = new System.Drawing.Point(6, 38);
            this.m_lb_listOfAllAnchorNodes.Name = "m_lb_listOfAllAnchorNodes";
            this.m_lb_listOfAllAnchorNodes.Size = new System.Drawing.Size(120, 147);
            this.m_lb_listOfAllAnchorNodes.TabIndex = 6;
            this.m_lb_listOfAllAnchorNodes.Visible = false;
            this.m_lb_listOfAllAnchorNodes.SelectedIndexChanged += new System.EventHandler(this.m_lb_listOfAllAnchorNodes_SelectedIndexChanged);
            // 
            // m_btn_ShowAll
            // 
            this.m_btn_ShowAll.Location = new System.Drawing.Point(12, 204);
            this.m_btn_ShowAll.Name = "m_btn_ShowAll";
            this.m_btn_ShowAll.Size = new System.Drawing.Size(75, 23);
            this.m_btn_ShowAll.TabIndex = 7;
            this.m_btn_ShowAll.Text = "Show all";
            this.m_btn_ShowAll.UseVisualStyleBackColor = true;
            this.m_btn_ShowAll.Click += new System.EventHandler(this.m_btn_ShowAll_Click);
            // 
            // AssociateSpecialNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 273);
            this.Controls.Add(this.m_btn_ShowAll);
            this.Controls.Add(this.m_lb_listOfAllAnchorNodes);
            this.Controls.Add(this.m_btn_Deassociate);
            this.Controls.Add(this.m_btn_Associate);
            this.Controls.Add(this.m_txtBox_SectionName);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_lb_ListOfSpecialNodes);
            this.Name = "AssociateSpecialNode";
            this.Text = "AssociateSpecialNode";
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}