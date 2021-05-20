namespace Obi.Dialogs
{
    partial class ChoosePageAudio
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
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_txtSelectRecordedAudio = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_rbPagesWithRecordedAudio = new System.Windows.Forms.RadioButton();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(25, 44);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(207, 21);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Pages with synthetic &speech";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.Enabled = false;
            this.m_btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnBrowse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnBrowse.Location = new System.Drawing.Point(454, 117);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(97, 37);
            this.m_btnBrowse.TabIndex = 15;
            this.m_btnBrowse.Text = "Browse";
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.m_btnBrowse_Click);
            // 
            // m_txtSelectRecordedAudio
            // 
            this.m_txtSelectRecordedAudio.AccessibleName = "Select file with recorded audio";
            this.m_txtSelectRecordedAudio.Enabled = false;
            this.m_txtSelectRecordedAudio.Location = new System.Drawing.Point(241, 124);
            this.m_txtSelectRecordedAudio.Name = "m_txtSelectRecordedAudio";
            this.m_txtSelectRecordedAudio.ReadOnly = true;
            this.m_txtSelectRecordedAudio.Size = new System.Drawing.Size(182, 22);
            this.m_txtSelectRecordedAudio.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(24, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Select file with recorded audio";
            // 
            // m_rbPagesWithRecordedAudio
            // 
            this.m_rbPagesWithRecordedAudio.AutoSize = true;
            this.m_rbPagesWithRecordedAudio.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_rbPagesWithRecordedAudio.Location = new System.Drawing.Point(25, 94);
            this.m_rbPagesWithRecordedAudio.Name = "m_rbPagesWithRecordedAudio";
            this.m_rbPagesWithRecordedAudio.Size = new System.Drawing.Size(197, 21);
            this.m_rbPagesWithRecordedAudio.TabIndex = 12;
            this.m_rbPagesWithRecordedAudio.Text = "Pages with recorded audio";
            this.m_rbPagesWithRecordedAudio.UseVisualStyleBackColor = true;
            this.m_rbPagesWithRecordedAudio.CheckedChanged += new System.EventHandler(this.m_rbPagesWithRecordedAudio_CheckedChanged);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.m_btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnCancel.Location = new System.Drawing.Point(306, 210);
            this.m_btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(117, 36);
            this.m_btnCancel.TabIndex = 17;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.m_btnOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnOk.Location = new System.Drawing.Point(178, 210);
            this.m_btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(117, 36);
            this.m_btnOk.TabIndex = 16;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // ChoosePageAudio
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(601, 282);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_txtSelectRecordedAudio);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_rbPagesWithRecordedAudio);
            this.Controls.Add(this.radioButton1);
            this.Name = "ChoosePageAudio";
            this.Text = "Choose Page Audio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button m_btnBrowse;
        private System.Windows.Forms.TextBox m_txtSelectRecordedAudio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton m_rbPagesWithRecordedAudio;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOk;
    }
}