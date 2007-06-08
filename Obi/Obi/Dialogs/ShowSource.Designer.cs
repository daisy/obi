namespace Obi.Dialogs
{
    partial class ShowSource
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
            this.mSourceTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mSourceTextBox
            // 
            this.mSourceTextBox.BackColor = System.Drawing.Color.White;
            this.mSourceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mSourceTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSourceTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSourceTextBox.Location = new System.Drawing.Point(0, 0);
            this.mSourceTextBox.Multiline = true;
            this.mSourceTextBox.Name = "mSourceTextBox";
            this.mSourceTextBox.ReadOnly = true;
            this.mSourceTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mSourceTextBox.Size = new System.Drawing.Size(732, 629);
            this.mSourceTextBox.TabIndex = 0;
            this.mSourceTextBox.WordWrap = false;
            // 
            // ShowSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 629);
            this.Controls.Add(this.mSourceTextBox);
            this.Name = "ShowSource";
            this.Text = "Show source";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mSourceTextBox;
    }
}