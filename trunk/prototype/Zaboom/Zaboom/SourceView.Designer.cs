namespace Zaboom
{
    partial class SourceView
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
            this.sourceBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sourceBox
            // 
            this.sourceBox.BackColor = System.Drawing.Color.White;
            this.sourceBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sourceBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceBox.Location = new System.Drawing.Point(0, 0);
            this.sourceBox.Multiline = true;
            this.sourceBox.Name = "sourceBox";
            this.sourceBox.ReadOnly = true;
            this.sourceBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sourceBox.Size = new System.Drawing.Size(676, 609);
            this.sourceBox.TabIndex = 0;
            this.sourceBox.WordWrap = false;
            // 
            // SourceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 609);
            this.Controls.Add(this.sourceBox);
            this.Name = "SourceView";
            this.Text = "Source view";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox sourceBox;

    }
}