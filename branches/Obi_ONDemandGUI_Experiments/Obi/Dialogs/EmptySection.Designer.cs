namespace Obi.Dialogs
{
    partial class EmptySection
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
            this.mContinueButton = new System.Windows.Forms.Button();
            this.mMessageLabel = new System.Windows.Forms.Label();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mKeepWarningCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // mContinueButton
            // 
            this.mContinueButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mContinueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mContinueButton.Location = new System.Drawing.Point(91, 157);
            this.mContinueButton.Margin = new System.Windows.Forms.Padding(4);
            this.mContinueButton.Name = "mContinueButton";
            this.mContinueButton.Size = new System.Drawing.Size(100, 28);
            this.mContinueButton.TabIndex = 2;
            this.mContinueButton.Text = "C&ontinue";
            this.mContinueButton.UseVisualStyleBackColor = true;
            // 
            // mMessageLabel
            // 
            this.mMessageLabel.Location = new System.Drawing.Point(12, 11);
            this.mMessageLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mMessageLabel.Name = "mMessageLabel";
            this.mMessageLabel.Size = new System.Drawing.Size(364, 80);
            this.mMessageLabel.TabIndex = 0;
            this.mMessageLabel.Text = "Section \"{0}\" has no audio content and will not be exported, nor will any of its " +
                "subsections. Do you want to continue?";
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancelButton.Location = new System.Drawing.Point(199, 157);
            this.mCancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(100, 28);
            this.mCancelButton.TabIndex = 3;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mKeepWarningCheckbox
            // 
            this.mKeepWarningCheckbox.AutoSize = true;
            this.mKeepWarningCheckbox.Checked = true;
            this.mKeepWarningCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mKeepWarningCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mKeepWarningCheckbox.Location = new System.Drawing.Point(12, 94);
            this.mKeepWarningCheckbox.Name = "mKeepWarningCheckbox";
            this.mKeepWarningCheckbox.Size = new System.Drawing.Size(337, 20);
            this.mKeepWarningCheckbox.TabIndex = 1;
            this.mKeepWarningCheckbox.Text = "Keep warning me about empty sections in this project";
            this.mKeepWarningCheckbox.UseVisualStyleBackColor = true;
            // 
            // EmptySection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 198);
            this.ControlBox = false;
            this.Controls.Add(this.mKeepWarningCheckbox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mMessageLabel);
            this.Controls.Add(this.mContinueButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmptySection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Section will not be exported";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mContinueButton;
        private System.Windows.Forms.Label mMessageLabel;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mKeepWarningCheckbox;
    }
}