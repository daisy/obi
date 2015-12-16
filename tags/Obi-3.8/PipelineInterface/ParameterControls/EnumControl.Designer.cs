namespace PipelineInterface.ParameterControls
{
    partial class EnumControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnumControl));
            this.mComboBox = new System.Windows.Forms.ComboBox();
            this.mNiceNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mComboBox
            // 
            this.mComboBox.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
            resources.ApplyResources(this.mComboBox, "mComboBox");
            this.mComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mComboBox.FormattingEnabled = true;
            this.mComboBox.Name = "mComboBox";
            // 
            // mNiceNameLabel
            // 
            resources.ApplyResources(this.mNiceNameLabel, "mNiceNameLabel");
            this.mNiceNameLabel.Name = "mNiceNameLabel";
            this.mNiceNameLabel.LocationChanged += new System.EventHandler(this.mNiceNameLabel_LocationChanged);
            // 
            // EnumControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mNiceNameLabel);
            this.Controls.Add(this.mComboBox);
            this.Name = "EnumControl";
            this.Load += new System.EventHandler(this.ComboboxControl_Load);
            this.Controls.SetChildIndex(this.mLabel, 0);
            this.Controls.SetChildIndex(this.mComboBox, 0);
            this.Controls.SetChildIndex(this.mNiceNameLabel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox mComboBox;
        private System.Windows.Forms.Label mNiceNameLabel;
    }
}
