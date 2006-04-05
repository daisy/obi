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
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // selectHandle
            // 
            this.selectHandle.BackColor = System.Drawing.Color.Transparent;
            this.selectHandle.Dock = System.Windows.Forms.DockStyle.Left;
            this.selectHandle.Location = new System.Drawing.Point(0, 0);
            this.selectHandle.Margin = new System.Windows.Forms.Padding(0);
            this.selectHandle.Name = "selectHandle";
            this.selectHandle.Size = new System.Drawing.Size(15, 96);
            this.selectHandle.TabIndex = 0;
            this.selectHandle.MouseClick += new System.Windows.Forms.MouseEventHandler(this.selectHandle_MouseClick);
            this.selectHandle.Paint += new System.Windows.Forms.PaintEventHandler(this.selectHandle_Paint);
            // 
            // sizeHandle
            // 
            this.sizeHandle.BackColor = System.Drawing.Color.Transparent;
            this.sizeHandle.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.sizeHandle.Dock = System.Windows.Forms.DockStyle.Right;
            this.sizeHandle.Location = new System.Drawing.Point(225, 0);
            this.sizeHandle.Margin = new System.Windows.Forms.Padding(0);
            this.sizeHandle.Name = "sizeHandle";
            this.sizeHandle.Size = new System.Drawing.Size(15, 96);
            this.sizeHandle.TabIndex = 1;
            this.sizeHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sizeHandle_MouseDown);
            this.sizeHandle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.sizeHandle_MouseMove);
            this.sizeHandle.Paint += new System.Windows.Forms.PaintEventHandler(this.sizeHandle_Paint);
            this.sizeHandle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sizeHandle_MouseUp);
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(19, 4);
            this.label.Margin = new System.Windows.Forms.Padding(4);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(79, 12);
            this.label.TabIndex = 0;
            this.label.Text = "(No controller)";
            // 
            // StripUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label);
            this.Controls.Add(this.sizeHandle);
            this.Controls.Add(this.selectHandle);
            this.Margin = new System.Windows.Forms.Padding(8, 8, 8, 0);
            this.Name = "StripUserControl";
            this.Size = new System.Drawing.Size(240, 96);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.StripUserControl_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Panel selectHandle;
        protected System.Windows.Forms.Panel sizeHandle;
        protected System.Windows.Forms.Label label;

    }
}
