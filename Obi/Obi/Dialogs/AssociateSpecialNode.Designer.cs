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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lb_ListOfSpecialNodes
            // 
            this.m_lb_ListOfSpecialNodes.FormattingEnabled = true;
            this.m_lb_ListOfSpecialNodes.HorizontalScrollbar = true;
            this.m_lb_ListOfSpecialNodes.Location = new System.Drawing.Point(7, 20);
            this.m_lb_ListOfSpecialNodes.Name = "m_lb_ListOfSpecialNodes";
            this.m_lb_ListOfSpecialNodes.Size = new System.Drawing.Size(216, 212);
            this.m_lb_ListOfSpecialNodes.TabIndex = 4;
            this.m_lb_ListOfSpecialNodes.SelectedIndexChanged += new System.EventHandler(this.m_lb_ListOfSpecialNodes_SelectedIndexChanged);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_OK.Location = new System.Drawing.Point(478, 219);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 9;
            this.m_btn_OK.Text = "O&K";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Cancel.Location = new System.Drawing.Point(562, 219);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(72, 23);
            this.m_btn_Cancel.TabIndex = 10;
            this.m_btn_Cancel.Text = "Canc&el";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // m_txtBox_SectionName
            // 
            this.m_txtBox_SectionName.Location = new System.Drawing.Point(8, 22);
            this.m_txtBox_SectionName.Name = "m_txtBox_SectionName";
            this.m_txtBox_SectionName.ReadOnly = true;
            this.m_txtBox_SectionName.Size = new System.Drawing.Size(222, 20);
            this.m_txtBox_SectionName.TabIndex = 1;
            // 
            // m_btn_Associate
            // 
            this.m_btn_Associate.Enabled = false;
            this.m_btn_Associate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Associate.Location = new System.Drawing.Point(19, 21);
            this.m_btn_Associate.Name = "m_btn_Associate";
            this.m_btn_Associate.Size = new System.Drawing.Size(95, 23);
            this.m_btn_Associate.TabIndex = 7;
            this.m_btn_Associate.Text = "Associa&te";
            this.m_btn_Associate.UseVisualStyleBackColor = true;
            this.m_btn_Associate.Click += new System.EventHandler(this.m_btn_Associate_Click);
            // 
            // m_btn_Deassociate
            // 
            this.m_btn_Deassociate.Enabled = false;
            this.m_btn_Deassociate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_Deassociate.Location = new System.Drawing.Point(19, 54);
            this.m_btn_Deassociate.Name = "m_btn_Deassociate";
            this.m_btn_Deassociate.Size = new System.Drawing.Size(95, 23);
            this.m_btn_Deassociate.TabIndex = 8;
            this.m_btn_Deassociate.Text = "Deass&ociate";
            this.m_btn_Deassociate.UseVisualStyleBackColor = true;
            this.m_btn_Deassociate.Click += new System.EventHandler(this.m_btn_Deassociate_Click);
            // 
            // m_lb_listOfAllAnchorNodes
            // 
            this.m_lb_listOfAllAnchorNodes.FormattingEnabled = true;
            this.m_lb_listOfAllAnchorNodes.HorizontalScrollbar = true;
            this.m_lb_listOfAllAnchorNodes.Location = new System.Drawing.Point(6, 20);
            this.m_lb_listOfAllAnchorNodes.Name = "m_lb_listOfAllAnchorNodes";
            this.m_lb_listOfAllAnchorNodes.Size = new System.Drawing.Size(222, 212);
            this.m_lb_listOfAllAnchorNodes.TabIndex = 2;
            this.m_lb_listOfAllAnchorNodes.Visible = false;
            this.m_lb_listOfAllAnchorNodes.SelectedIndexChanged += new System.EventHandler(this.m_lb_listOfAllAnchorNodes_SelectedIndexChanged);
            // 
            // m_btn_ShowAll
            // 
            this.m_btn_ShowAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btn_ShowAll.Location = new System.Drawing.Point(478, 31);
            this.m_btn_ShowAll.Name = "m_btn_ShowAll";
            this.m_btn_ShowAll.Size = new System.Drawing.Size(131, 23);
            this.m_btn_ShowAll.TabIndex = 5;
            this.m_btn_ShowAll.Text = "Sho&w all";
            this.m_btn_ShowAll.UseVisualStyleBackColor = true;
            this.m_btn_ShowAll.Click += new System.EventHandler(this.m_btn_ShowAll_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_lb_listOfAllAnchorNodes);
            this.groupBox1.Controls.Add(this.m_txtBox_SectionName);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 239);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.m_lb_ListOfSpecialNodes);
            this.groupBox2.Location = new System.Drawing.Point(243, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(229, 240);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "List of ski&ppable notes";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.m_btn_Associate);
            this.groupBox3.Controls.Add(this.m_btn_Deassociate);
            this.groupBox3.Location = new System.Drawing.Point(478, 89);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(131, 89);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            // 
            // AssociateSpecialNode
            // 
            this.AcceptButton = this.m_btn_OK;
            this.AccessibleName = "Associate Special Node";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.ClientSize = new System.Drawing.Size(642, 252);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_btn_ShowAll);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AssociateSpecialNode";
            this.Text = "Add/Remove reference (Associate/Deassociate)";
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
    }
}