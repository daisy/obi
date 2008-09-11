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
            this.SuspendLayout();
            // 
            // InheritedTOCView
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FullRowSelect = true;
            this.LabelEdit = true;
            this.LineColor = System.Drawing.Color.Black;
            this.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterCollapse);
            this.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TOCTree_AfterLabelEdit);
            this.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterSelect);
            this.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TOCTree_BeforeLabelEdit);
            this.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.TOCTree_BeforeSelect);
            this.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.TOCTree_AfterExpand);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
