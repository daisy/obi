namespace Obi.UserControls
{
    partial class EmptyStrip
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
            this.mSelectHandle = new System.Windows.Forms.Panel();
            this.mSizeHandle = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // mSelectHandle
            // 
            this.mSelectHandle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.mSelectHandle.Dock = System.Windows.Forms.DockStyle.Left;
            this.mSelectHandle.Location = new System.Drawing.Point(0, 0);
            this.mSelectHandle.Margin = new System.Windows.Forms.Padding(0);
            this.mSelectHandle.Name = "mSelectHandle";
            this.mSelectHandle.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mSelectHandle.Size = new System.Drawing.Size(8, 150);
            this.mSelectHandle.TabIndex = 0;
            // 
            // mSizeHandle
            // 
            this.mSizeHandle.BackColor = System.Drawing.Color.LightSteelBlue;
            this.mSizeHandle.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.mSizeHandle.Dock = System.Windows.Forms.DockStyle.Right;
            this.mSizeHandle.Location = new System.Drawing.Point(142, 0);
            this.mSizeHandle.Margin = new System.Windows.Forms.Padding(0);
            this.mSizeHandle.Name = "mSizeHandle";
            this.mSizeHandle.Size = new System.Drawing.Size(8, 150);
            this.mSizeHandle.TabIndex = 2;
            this.mSizeHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mSizeHandle_MouseDown);
            this.mSizeHandle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mSizeHandle_MouseMove);
            this.mSizeHandle.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mSizeHandle_MouseDoubleClick);
            this.mSizeHandle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mSizeHandle_MouseUp);
            // 
            // EmptyStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mSizeHandle);
            this.Controls.Add(this.mSelectHandle);
            this.MinimumSize = new System.Drawing.Size(22, 0);
            this.Name = "EmptyStrip";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mSelectHandle;
        private System.Windows.Forms.Panel mSizeHandle;
    }
}
