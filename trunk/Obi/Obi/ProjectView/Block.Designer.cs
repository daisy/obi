namespace Obi.ProjectView
{
    partial class Block
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Block));
            this.mLabel = new System.Windows.Forms.Label();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_NotePictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_NotePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.BackColor = System.Drawing.Color.Transparent;
            this.mLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel.Location = new System.Drawing.Point(0, 3);
            this.mLabel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(42, 16);
            this.mLabel.TabIndex = 2;
            this.mLabel.Text = "Label";
            this.mLabel.Click += new System.EventHandler(this.Block_Click);
            // 
            // m_NotePictureBox
            // 
            this.m_NotePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("m_NotePictureBox.Image")));
            this.m_NotePictureBox.Location = new System.Drawing.Point(0, 0);
            this.m_NotePictureBox.Name = "m_NotePictureBox";
            this.m_NotePictureBox.Size = new System.Drawing.Size(12, 19);
            this.m_NotePictureBox.TabIndex = 3;
            this.m_NotePictureBox.TabStop = false;
            this.m_NotePictureBox.Visible = false;
            // 
            // Block
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.HotPink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.m_NotePictureBox);
            this.Controls.Add(this.mLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.Name = "Block";
            this.Size = new System.Drawing.Size(104, 104);
            this.Click += new System.EventHandler(this.Block_Click);
            this.Enter += new System.EventHandler(this.Block_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.m_NotePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label mLabel;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.PictureBox m_NotePictureBox;

    }
}
