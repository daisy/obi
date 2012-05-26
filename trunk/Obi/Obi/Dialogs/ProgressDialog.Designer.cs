namespace Obi.Dialogs
{
    partial class ProgressDialog
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
            this.mProgressBar = new System.Windows.Forms.ProgressBar();
            this.m_BtnCancel = new System.Windows.Forms.Button();
            this.m_lbWaitForCancellation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mProgressBar
            // 
            this.mProgressBar.Location = new System.Drawing.Point(4, 24);
            this.mProgressBar.Name = "mProgressBar";
            this.mProgressBar.Size = new System.Drawing.Size(373, 23);
            this.mProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.mProgressBar.TabIndex = 1;
            this.mProgressBar.UseWaitCursor = true;
            // 
            // m_BtnCancel
            // 
            this.m_BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_BtnCancel.Location = new System.Drawing.Point(302, 63);
            this.m_BtnCancel.Name = "m_BtnCancel";
            this.m_BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_BtnCancel.TabIndex = 2;
            this.m_BtnCancel.Text = "&Cancel";
            this.m_BtnCancel.UseVisualStyleBackColor = true;
            this.m_BtnCancel.UseWaitCursor = true;
            this.m_BtnCancel.Click += new System.EventHandler(this.m_BtnCancel_Click);
            // 
            // m_lbWaitForCancellation
            // 
            this.m_lbWaitForCancellation.AutoSize = true;
            this.m_lbWaitForCancellation.Location = new System.Drawing.Point(1, 9);
            this.m_lbWaitForCancellation.Name = "m_lbWaitForCancellation";
            this.m_lbWaitForCancellation.Size = new System.Drawing.Size(277, 16);
            this.m_lbWaitForCancellation.TabIndex = 3;
            this.m_lbWaitForCancellation.Text = "Operation cancelled.. Completing current task";
            this.m_lbWaitForCancellation.UseWaitCursor = true;
            this.m_lbWaitForCancellation.Visible = false;
            // 
            // ProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 87);
            this.ControlBox = false;
            this.Controls.Add(this.m_lbWaitForCancellation);
            this.Controls.Add(this.m_BtnCancel);
            this.Controls.Add(this.mProgressBar);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(400, 125);
            this.MinimumSize = new System.Drawing.Size(400, 74);
            this.Name = "ProgressDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Operation in progress";
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar mProgressBar;
        private System.Windows.Forms.Button m_BtnCancel;
        private System.Windows.Forms.Label m_lbWaitForCancellation;
    }
}