namespace Obi.Dialogs
{
    partial class AddAzureKey
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
            this.m_AddBtn = new System.Windows.Forms.Button();
            this.m_RegionLbl = new System.Windows.Forms.Label();
            this.m_KeyLbl = new System.Windows.Forms.Label();
            this.m_ResgionTB = new System.Windows.Forms.TextBox();
            this.m_KeyTB = new System.Windows.Forms.TextBox();
            this.m_CancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_AddBtn
            // 
            this.m_AddBtn.AccessibleName = "Add Key";
            this.m_AddBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_AddBtn.Location = new System.Drawing.Point(282, 270);
            this.m_AddBtn.Name = "m_AddBtn";
            this.m_AddBtn.Size = new System.Drawing.Size(135, 40);
            this.m_AddBtn.TabIndex = 9;
            this.m_AddBtn.Text = "&Add Key";
            this.m_AddBtn.UseVisualStyleBackColor = true;
            this.m_AddBtn.Click += new System.EventHandler(this.m_AddBtn_Click);
            // 
            // m_RegionLbl
            // 
            this.m_RegionLbl.AutoSize = true;
            this.m_RegionLbl.Location = new System.Drawing.Point(33, 175);
            this.m_RegionLbl.Name = "m_RegionLbl";
            this.m_RegionLbl.Size = new System.Drawing.Size(66, 20);
            this.m_RegionLbl.TabIndex = 8;
            this.m_RegionLbl.Text = "Region:";
            // 
            // m_KeyLbl
            // 
            this.m_KeyLbl.AutoSize = true;
            this.m_KeyLbl.Location = new System.Drawing.Point(37, 94);
            this.m_KeyLbl.Name = "m_KeyLbl";
            this.m_KeyLbl.Size = new System.Drawing.Size(91, 20);
            this.m_KeyLbl.TabIndex = 7;
            this.m_KeyLbl.Text = "Azure Key:";
            // 
            // m_ResgionTB
            // 
            this.m_ResgionTB.AccessibleName = "Region";
            this.m_ResgionTB.Location = new System.Drawing.Point(149, 172);
            this.m_ResgionTB.Name = "m_ResgionTB";
            this.m_ResgionTB.Size = new System.Drawing.Size(201, 26);
            this.m_ResgionTB.TabIndex = 6;
            // 
            // m_KeyTB
            // 
            this.m_KeyTB.AccessibleName = "Azure Key";
            this.m_KeyTB.Location = new System.Drawing.Point(149, 91);
            this.m_KeyTB.Name = "m_KeyTB";
            this.m_KeyTB.Size = new System.Drawing.Size(720, 26);
            this.m_KeyTB.TabIndex = 5;
            // 
            // m_CancelBtn
            // 
            this.m_CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_CancelBtn.Location = new System.Drawing.Point(466, 270);
            this.m_CancelBtn.Name = "m_CancelBtn";
            this.m_CancelBtn.Size = new System.Drawing.Size(125, 40);
            this.m_CancelBtn.TabIndex = 10;
            this.m_CancelBtn.Text = "&Cancel";
            this.m_CancelBtn.UseVisualStyleBackColor = true;
            // 
            // AddAzureKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 361);
            this.Controls.Add(this.m_CancelBtn);
            this.Controls.Add(this.m_AddBtn);
            this.Controls.Add(this.m_RegionLbl);
            this.Controls.Add(this.m_KeyLbl);
            this.Controls.Add(this.m_ResgionTB);
            this.Controls.Add(this.m_KeyTB);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddAzureKey";
            this.Text = "Add Azure Key";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_AddBtn;
        private System.Windows.Forms.Label m_RegionLbl;
        private System.Windows.Forms.Label m_KeyLbl;
        private System.Windows.Forms.TextBox m_ResgionTB;
        private System.Windows.Forms.TextBox m_KeyTB;
        private System.Windows.Forms.Button m_CancelBtn;
    }
}