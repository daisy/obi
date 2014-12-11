namespace Obi.Dialogs
{
    partial class ChooseMergeOrChangeLevel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseMergeOrChangeLevel));
            this.m_rbMerge = new System.Windows.Forms.RadioButton();
            this.m_rbChangeLevel = new System.Windows.Forms.RadioButton();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_rbMerge
            // 
            resources.ApplyResources(this.m_rbMerge, "m_rbMerge");
            this.m_rbMerge.Name = "m_rbMerge";
            this.m_rbMerge.TabStop = true;
            this.m_rbMerge.UseVisualStyleBackColor = true;
            // 
            // m_rbChangeLevel
            // 
            resources.ApplyResources(this.m_rbChangeLevel, "m_rbChangeLevel");
            this.m_rbChangeLevel.Name = "m_rbChangeLevel";
            this.m_rbChangeLevel.TabStop = true;
            this.m_rbChangeLevel.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // ChooseMergeOrChangeLevel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_rbChangeLevel);
            this.Controls.Add(this.m_rbMerge);
            this.Name = "ChooseMergeOrChangeLevel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton m_rbMerge;
        private System.Windows.Forms.RadioButton m_rbChangeLevel;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
    }
}