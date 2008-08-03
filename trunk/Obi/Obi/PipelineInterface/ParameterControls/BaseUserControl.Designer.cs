namespace Obi.PipelineInterface.ParameterControls
{
    partial class BaseUserControl
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
            this.BaseTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BaseTextBox
            // 
            this.BaseTextBox.Location = new System.Drawing.Point(10, 10);
            this.BaseTextBox.Name = "BaseTextBox";
            this.BaseTextBox.ReadOnly = true;
            this.BaseTextBox.Size = new System.Drawing.Size(150, 20);
            this.BaseTextBox.TabIndex = 0;
            // 
            // BaseUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.BaseTextBox);
            this.Name = "BaseUserControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BaseTextBox;
    }
}
