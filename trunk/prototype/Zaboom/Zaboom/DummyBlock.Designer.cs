namespace Zaboom
{
    partial class DummyBlock
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
            // DummyBlock
            // 
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.Size = new System.Drawing.Size(30, 150);
            this.Click += new System.EventHandler(this.DummyBlock_Click);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
