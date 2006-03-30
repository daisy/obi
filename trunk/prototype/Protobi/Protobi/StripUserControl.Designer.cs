namespace Protobi
{
    partial class StripUserControl
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
            this.selectHandle = new System.Windows.Forms.Panel();
            this.sizeHandle = new System.Windows.Forms.Panel();
            this.layoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // selectHandle
            // 
            this.selectHandle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.selectHandle.Dock = System.Windows.Forms.DockStyle.Left;
            this.selectHandle.Location = new System.Drawing.Point(0, 0);
            this.selectHandle.Margin = new System.Windows.Forms.Padding(0);
            this.selectHandle.Name = "selectHandle";
            this.selectHandle.Size = new System.Drawing.Size(15, 94);
            this.selectHandle.TabIndex = 0;
            this.selectHandle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.selectHandle_MouseClick);
            // 
            // sizeHandle
            // 
            this.sizeHandle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.sizeHandle.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.sizeHandle.Dock = System.Windows.Forms.DockStyle.Right;
            this.sizeHandle.Location = new System.Drawing.Point(223, 0);
            this.sizeHandle.Margin = new System.Windows.Forms.Padding(0);
            this.sizeHandle.Name = "sizeHandle";
            this.sizeHandle.Size = new System.Drawing.Size(15, 94);
            this.sizeHandle.TabIndex = 1;
            this.sizeHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sizeHandle_MouseDown);
            this.sizeHandle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.sizeHandle_MouseMove);
            this.sizeHandle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sizeHandle_MouseUp);
            // 
            // layoutPanel
            // 
            this.layoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.layoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.layoutPanel.Location = new System.Drawing.Point(18, 0);
            this.layoutPanel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Size = new System.Drawing.Size(202, 94);
            this.layoutPanel.TabIndex = 3;
            this.layoutPanel.WrapContents = false;
            // 
            // StripUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.layoutPanel);
            this.Controls.Add(this.sizeHandle);
            this.Controls.Add(this.selectHandle);
            this.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.Name = "StripUserControl";
            this.Size = new System.Drawing.Size(238, 94);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel selectHandle;
        private System.Windows.Forms.Panel sizeHandle;
        protected System.Windows.Forms.FlowLayoutPanel layoutPanel;
    }
}
