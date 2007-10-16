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
            this.mSearchLabel = new System.Windows.Forms.Label();
            this.mSearchString = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mSearchLabel
            // 
            this.mSearchLabel.AutoSize = true;
            this.mSearchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSearchLabel.Location = new System.Drawing.Point(3, 6);
            this.mSearchLabel.Name = "mSearchLabel";
            this.mSearchLabel.Size = new System.Drawing.Size(51, 13);
            this.mSearchLabel.TabIndex = 0;
            this.mSearchLabel.Text = "Search:";
            // 
            // mSearchString
            // 
            this.mSearchString.Location = new System.Drawing.Point(60, 3);
            this.mSearchString.Name = "mSearchString";
            this.mSearchString.Size = new System.Drawing.Size(146, 20);
            this.mSearchString.TabIndex = 1;
            this.mSearchString.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mSearchString_KeyPress);
            // 
            // SearchText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Honeydew;
            this.Controls.Add(this.mSearchString);
            this.Controls.Add(this.mSearchLabel);
            this.Name = "SearchText";
            this.Size = new System.Drawing.Size(209, 25);
            this.Load += new System.EventHandler(this.SearchText_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mSearchLabel;
        private System.Windows.Forms.TextBox mSearchString;
    }
}
