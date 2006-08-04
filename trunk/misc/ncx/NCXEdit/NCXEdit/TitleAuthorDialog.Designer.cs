namespace NCXEdit
{
    partial class TitleAuthorDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.titleText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.titleAudio = new System.Windows.Forms.TextBox();
            this.titleChooseButton = new System.Windows.Forms.Button();
            this.titlePlayButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.titlePlayButton);
            this.groupBox1.Controls.Add(this.titleChooseButton);
            this.groupBox1.Controls.Add(this.titleAudio);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.titleText);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 97);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Title";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Text:";
            // 
            // titleText
            // 
            this.titleText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.titleText.Location = new System.Drawing.Point(68, 18);
            this.titleText.Name = "titleText";
            this.titleText.Size = new System.Drawing.Size(220, 19);
            this.titleText.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Audio file:";
            // 
            // titleAudio
            // 
            this.titleAudio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.titleAudio.Location = new System.Drawing.Point(68, 43);
            this.titleAudio.Name = "titleAudio";
            this.titleAudio.Size = new System.Drawing.Size(220, 19);
            this.titleAudio.TabIndex = 2;
            // 
            // titleChooseButton
            // 
            this.titleChooseButton.Location = new System.Drawing.Point(132, 68);
            this.titleChooseButton.Name = "titleChooseButton";
            this.titleChooseButton.Size = new System.Drawing.Size(75, 23);
            this.titleChooseButton.TabIndex = 1;
            this.titleChooseButton.Text = "Choose &file";
            this.titleChooseButton.UseVisualStyleBackColor = true;
            this.titleChooseButton.Click += new System.EventHandler(this.titleChooseButton_Click);
            // 
            // titlePlayButton
            // 
            this.titlePlayButton.Location = new System.Drawing.Point(213, 68);
            this.titlePlayButton.Name = "titlePlayButton";
            this.titlePlayButton.Size = new System.Drawing.Size(75, 23);
            this.titlePlayButton.TabIndex = 1;
            this.titlePlayButton.Text = "&Play audio";
            this.titlePlayButton.UseVisualStyleBackColor = true;
            this.titlePlayButton.Click += new System.EventHandler(this.titlePlayButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(150, 131);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button4.Location = new System.Drawing.Point(231, 131);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 2;
            this.button4.Text = "&Cancel";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // TitleAuthorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 166);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "TitleAuthorDialog";
            this.Text = "Edit title and author(s)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox titleText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button titleChooseButton;
        private System.Windows.Forms.TextBox titleAudio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button titlePlayButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button button4;
    }
}