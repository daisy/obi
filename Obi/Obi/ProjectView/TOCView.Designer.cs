namespace Obi.ProjectView
{
    partial class TOCView
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
            this.mTOCTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // mTOCTree
            // 
            this.mTOCTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTOCTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCTree.LabelEdit = true;
            this.mTOCTree.Location = new System.Drawing.Point(0, 0);
            this.mTOCTree.Name = "mTOCTree";
            this.mTOCTree.Size = new System.Drawing.Size(150, 150);
            this.mTOCTree.TabIndex = 0;
            this.mTOCTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.mTOCTree_AfterLabelEdit);
            // 
            // TOCView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.mTOCTree);
            this.Name = "TOCView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView mTOCTree;

    }
}
