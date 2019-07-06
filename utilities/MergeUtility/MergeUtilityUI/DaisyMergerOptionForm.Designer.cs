namespace MergeUtilityUI
{
    partial class DaisyMergerOptionForm
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
        this.groupBox1 = new System.Windows.Forms.GroupBox ();
        this.m_rdbDaisy202 = new System.Windows.Forms.RadioButton ();
        this.m_rdbDaisy3 = new System.Windows.Forms.RadioButton ();
        this.m_btnOK = new System.Windows.Forms.Button ();
        this.m_btnCancel = new System.Windows.Forms.Button ();
        this.groupBox1.SuspendLayout ();
        this.SuspendLayout ();
        // 
        // groupBox1
        // 
        this.groupBox1.Controls.Add ( this.m_rdbDaisy202 );
        this.groupBox1.Controls.Add ( this.m_rdbDaisy3 );
        this.groupBox1.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
        this.groupBox1.Location = new System.Drawing.Point ( 45, 25 );
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Size = new System.Drawing.Size ( 446, 76 );
        this.groupBox1.TabIndex = 0;
        this.groupBox1.TabStop = false;
        this.groupBox1.Text = " Type of files to be Merged ";
        // 
        // m_rdbDaisy202
        // 
        this.m_rdbDaisy202.AutoSize = true;
        this.m_rdbDaisy202.Location = new System.Drawing.Point ( 294, 34 );
        this.m_rdbDaisy202.Name = "m_rdbDaisy202";
        this.m_rdbDaisy202.Size = new System.Drawing.Size ( 103, 20 );
        this.m_rdbDaisy202.TabIndex = 1;
        this.m_rdbDaisy202.Text = "D&AISY 2.02";
        this.m_rdbDaisy202.UseVisualStyleBackColor = true;
        // 
        // m_rdbDaisy3
        // 
        this.m_rdbDaisy3.AutoSize = true;
        this.m_rdbDaisy3.Checked = true;
        this.m_rdbDaisy3.Location = new System.Drawing.Point ( 31, 34 );
        this.m_rdbDaisy3.Name = "m_rdbDaisy3";
        this.m_rdbDaisy3.Size = new System.Drawing.Size ( 83, 20 );
        this.m_rdbDaisy3.TabIndex = 0;
        this.m_rdbDaisy3.TabStop = true;
        this.m_rdbDaisy3.Text = "&DAISY 3";
        this.m_rdbDaisy3.UseVisualStyleBackColor = true;
        // 
        // m_btnOK
        // 
        this.m_btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.m_btnOK.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
        this.m_btnOK.Location = new System.Drawing.Point ( 111, 126 );
        this.m_btnOK.Name = "m_btnOK";
        this.m_btnOK.Size = new System.Drawing.Size ( 96, 26 );
        this.m_btnOK.TabIndex = 2;
        this.m_btnOK.Text = "&OK";
        this.m_btnOK.UseVisualStyleBackColor = true;
        this.m_btnOK.Click += new System.EventHandler ( this.m_btnOK_Click );
        // 
        // m_btnCancel
        // 
        this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.m_btnCancel.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
        this.m_btnCancel.Location = new System.Drawing.Point ( 316, 126 );
        this.m_btnCancel.Name = "m_btnCancel";
        this.m_btnCancel.Size = new System.Drawing.Size ( 96, 26 );
        this.m_btnCancel.TabIndex = 3;
        this.m_btnCancel.Text = "&Cancel";
        this.m_btnCancel.UseVisualStyleBackColor = true;
        this.m_btnCancel.Click += new System.EventHandler ( this.m_btnCancel_Click );
        // 
        // DaisyMergerOptionForm
        // 
        this.AcceptButton = this.m_btnOK;
        this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.m_btnCancel;
        this.ClientSize = new System.Drawing.Size ( 524, 185 );
        this.Controls.Add ( this.m_btnOK );
        this.Controls.Add ( this.m_btnCancel );
        this.Controls.Add ( this.groupBox1 );
        this.HelpButton = true;
        this.MaximizeBox = false;
        this.MaximumSize = new System.Drawing.Size ( 532, 212 );
        this.MinimizeBox = false;
        this.MinimumSize = new System.Drawing.Size ( 532, 212 );
        this.Name = "DaisyMergerOptionForm";
        this.Text = "DAISY Standard Selection ";
        this.HelpRequested += new System.Windows.Forms.HelpEventHandler ( this.DaisyMergerOptionForm_HelpRequested );
        this.groupBox1.ResumeLayout ( false );
        this.groupBox1.PerformLayout ();
        this.ResumeLayout ( false );

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton m_rdbDaisy202;
        private System.Windows.Forms.RadioButton m_rdbDaisy3;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
    }
}