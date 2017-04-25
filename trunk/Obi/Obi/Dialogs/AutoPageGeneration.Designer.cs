namespace Obi.Dialogs
{
    partial class AutoPageGeneration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoPageGeneration));
            this.m_lbGapsInPages = new System.Windows.Forms.Label();
            this.m_lbStartingSectionIndex = new System.Windows.Forms.Label();
            this.m_cbStartingSectionIndex = new System.Windows.Forms.ComboBox();
            this.m_rbGenerateTTS = new System.Windows.Forms.RadioButton();
            this.m_rbKeepEmptyPages = new System.Windows.Forms.RadioButton();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_gpPages = new System.Windows.Forms.GroupBox();
            this.m_nudGapsInPages = new System.Windows.Forms.NumericUpDown();
            this.m_gpPages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudGapsInPages)).BeginInit();
            this.SuspendLayout();
            // 
            // m_lbGapsInPages
            // 
            resources.ApplyResources(this.m_lbGapsInPages, "m_lbGapsInPages");
            this.m_lbGapsInPages.Name = "m_lbGapsInPages";
            // 
            // m_lbStartingSectionIndex
            // 
            resources.ApplyResources(this.m_lbStartingSectionIndex, "m_lbStartingSectionIndex");
            this.m_lbStartingSectionIndex.Name = "m_lbStartingSectionIndex";
            // 
            // m_cbStartingSectionIndex
            // 
            this.m_cbStartingSectionIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cbStartingSectionIndex.FormattingEnabled = true;
            resources.ApplyResources(this.m_cbStartingSectionIndex, "m_cbStartingSectionIndex");
            this.m_cbStartingSectionIndex.Name = "m_cbStartingSectionIndex";
            // 
            // m_rbGenerateTTS
            // 
            resources.ApplyResources(this.m_rbGenerateTTS, "m_rbGenerateTTS");
            this.m_rbGenerateTTS.Name = "m_rbGenerateTTS";
            this.m_rbGenerateTTS.UseVisualStyleBackColor = true;
            // 
            // m_rbKeepEmptyPages
            // 
            resources.ApplyResources(this.m_rbKeepEmptyPages, "m_rbKeepEmptyPages");
            this.m_rbKeepEmptyPages.Checked = true;
            this.m_rbKeepEmptyPages.Name = "m_rbKeepEmptyPages";
            this.m_rbKeepEmptyPages.TabStop = true;
            this.m_rbKeepEmptyPages.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_gpPages
            // 
            this.m_gpPages.Controls.Add(this.m_rbGenerateTTS);
            this.m_gpPages.Controls.Add(this.m_rbKeepEmptyPages);
            resources.ApplyResources(this.m_gpPages, "m_gpPages");
            this.m_gpPages.Name = "m_gpPages";
            this.m_gpPages.TabStop = false;
            // 
            // m_nudGapsInPages
            // 
            resources.ApplyResources(this.m_nudGapsInPages, "m_nudGapsInPages");
            this.m_nudGapsInPages.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.m_nudGapsInPages.Name = "m_nudGapsInPages";
            this.m_nudGapsInPages.ReadOnly = true;
            this.m_nudGapsInPages.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // AutoPageGeneration
            // 
            this.AcceptButton = this.m_btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.m_nudGapsInPages);
            this.Controls.Add(this.m_gpPages);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_cbStartingSectionIndex);
            this.Controls.Add(this.m_lbStartingSectionIndex);
            this.Controls.Add(this.m_lbGapsInPages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AutoPageGeneration";
            this.m_gpPages.ResumeLayout(false);
            this.m_gpPages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_nudGapsInPages)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lbGapsInPages;
        private System.Windows.Forms.Label m_lbStartingSectionIndex;
        private System.Windows.Forms.ComboBox m_cbStartingSectionIndex;
        private System.Windows.Forms.RadioButton m_rbGenerateTTS;
        private System.Windows.Forms.RadioButton m_rbKeepEmptyPages;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.GroupBox m_gpPages;
        private System.Windows.Forms.NumericUpDown m_nudGapsInPages;
    }
}