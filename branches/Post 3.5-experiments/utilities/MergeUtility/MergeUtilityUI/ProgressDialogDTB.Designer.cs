namespace MergeUtilityUI
{
    partial class ProgressDialogDTB
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
            this.m_pgBarDTB = new System.Windows.Forms.ProgressBar();
            this.m_BtnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_pgBarDTB
            // 
            this.m_pgBarDTB.Location = new System.Drawing.Point(12, 23);
            this.m_pgBarDTB.Name = "m_pgBarDTB";
            this.m_pgBarDTB.Size = new System.Drawing.Size(597, 32);
            this.m_pgBarDTB.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.m_pgBarDTB.TabIndex = 0;
            // 
            // m_BtnCancel
            // 
            this.m_BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_BtnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_BtnCancel.Location = new System.Drawing.Point(462, 72);
            this.m_BtnCancel.Name = "m_BtnCancel";
            this.m_BtnCancel.Size = new System.Drawing.Size(147, 35);
            this.m_BtnCancel.TabIndex = 1;
            this.m_BtnCancel.Text = "&Cancel";
            this.m_BtnCancel.UseVisualStyleBackColor = true;
            // 
            // ProgressDialogDTB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_BtnCancel;
            this.ClientSize = new System.Drawing.Size(630, 119);
            this.Controls.Add(this.m_BtnCancel);
            this.Controls.Add(this.m_pgBarDTB);
            this.KeyPreview = true;
            this.Name = "ProgressDialogDTB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Merging Files... Please Wait";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar m_pgBarDTB;
        private System.Windows.Forms.Button m_BtnCancel;
    }
}