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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoosePageAudio));
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_txtSelectRecordedAudio = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnBrowse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnBrowse.Location = new System.Drawing.Point(563, 76);
            this.m_btnBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(80, 28);
            this.m_btnBrowse.TabIndex = 4;
            this.m_btnBrowse.Text = "&Browse";
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.m_btnBrowse_Click);
            // 
            // m_txtSelectRecordedAudio
            // 
            this.m_txtSelectRecordedAudio.AccessibleName = "Select file with recorded audio";
            this.m_txtSelectRecordedAudio.Location = new System.Drawing.Point(218, 80);
            this.m_txtSelectRecordedAudio.Margin = new System.Windows.Forms.Padding(2);
            this.m_txtSelectRecordedAudio.Name = "m_txtSelectRecordedAudio";
            this.m_txtSelectRecordedAudio.ReadOnly = true;
            this.m_txtSelectRecordedAudio.Size = new System.Drawing.Size(341, 24);
            this.m_txtSelectRecordedAudio.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(10, 83);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(204, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Select file with recorded audio";
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.m_btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnCancel.Location = new System.Drawing.Point(322, 123);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(83, 33);
            this.m_btnCancel.TabIndex = 6;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.m_btnOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_btnOk.Location = new System.Drawing.Point(233, 123);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(83, 33);
            this.m_btnOk.TabIndex = 5;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(13, 13);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.MaxLength = 0;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(630, 44);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // ChoosePageAudio
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(653, 171);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnBrowse);
            this.Controls.Add(this.m_txtSelectRecordedAudio);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChoosePageAudio";
            this.Text = "Choose Page Audio";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChoosePageAudio_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnBrowse;
        private System.Windows.Forms.TextBox m_txtSelectRecordedAudio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}