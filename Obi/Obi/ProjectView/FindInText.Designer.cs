namespace Obi.ProjectView
{
    partial class FindInText
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mLabel = new System.Windows.Forms.Label();
            this.mString = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel.Location = new System.Drawing.Point(3, 6);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(51, 13);
            this.mLabel.TabIndex = 0;
            this.mLabel.Text = "Search:";
            // 
            // mString
            // 
            this.mString.Location = new System.Drawing.Point(60, 3);
            this.mString.Name = "mString";
            this.mString.Size = new System.Drawing.Size(146, 20);
            this.mString.TabIndex = 1;
            this.mString.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mSearchString_KeyPress);
            // 
            // FindInText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.Controls.Add(this.mString);
            this.Controls.Add(this.mLabel);
            this.Name = "FindInText";
            this.Size = new System.Drawing.Size(212, 28);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FindInText_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.TextBox mString;
    }
}
