namespace Obi.Dialogs
{
    partial class TimeElapsed
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
            this.m_lblTimeElapsed = new System.Windows.Forms.Label();
            this.m_txtBoxTotalTimeElapsed = new System.Windows.Forms.TextBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lblTimeElapsed
            // 
            this.m_lblTimeElapsed.AutoSize = true;
            this.m_lblTimeElapsed.Location = new System.Drawing.Point(12, 53);
            this.m_lblTimeElapsed.Name = "m_lblTimeElapsed";
            this.m_lblTimeElapsed.Size = new System.Drawing.Size(225, 13);
            this.m_lblTimeElapsed.TabIndex = 0;
            this.m_lblTimeElapsed.Text = "Time Elapsed between Begin and End marks :";
            // 
            // m_txtBoxTotalTimeElapsed
            // 
            this.m_txtBoxTotalTimeElapsed.Location = new System.Drawing.Point(243, 50);
            this.m_txtBoxTotalTimeElapsed.Name = "m_txtBoxTotalTimeElapsed";
            this.m_txtBoxTotalTimeElapsed.ReadOnly = true;
            this.m_txtBoxTotalTimeElapsed.Size = new System.Drawing.Size(129, 20);
            this.m_txtBoxTotalTimeElapsed.TabIndex = 1;
            // 
            // m_btnClose
            // 
            this.m_btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnClose.Location = new System.Drawing.Point(150, 108);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 2;
            this.m_btnClose.Text = "&Close";
            this.m_btnClose.UseVisualStyleBackColor = true;
            this.m_btnClose.Click += new System.EventHandler(this.m_btnClose_Click);
            // 
            // TimeElapsed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 155);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_txtBoxTotalTimeElapsed);
            this.Controls.Add(this.m_lblTimeElapsed);
            this.Name = "TimeElapsed";
            this.Text = "Time Elapsed";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblTimeElapsed;
        private System.Windows.Forms.TextBox m_txtBoxTotalTimeElapsed;
        private System.Windows.Forms.Button m_btnClose;
    }
}