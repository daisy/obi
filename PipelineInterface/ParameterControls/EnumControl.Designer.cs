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
        this.mComboBox = new System.Windows.Forms.ComboBox ();
        this.mNiceNameLabel = new System.Windows.Forms.Label ();
        this.SuspendLayout ();
        // 
        // mComboBox
        // 
        this.mComboBox.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
        this.mComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.mComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.mComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.mComboBox.FormattingEnabled = true;
        this.mComboBox.Location = new System.Drawing.Point ( 51, 20 );
        this.mComboBox.Margin = new System.Windows.Forms.Padding ( 4 );
        this.mComboBox.Name = "mComboBox";
        this.mComboBox.Size = new System.Drawing.Size ( 160, 24 );
        this.mComboBox.TabIndex = 1;
        // 
        // mNiceNameLabel
        // 
        this.mNiceNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.mNiceNameLabel.AutoSize = true;
        this.mNiceNameLabel.Location = new System.Drawing.Point ( 3, 23 );
        this.mNiceNameLabel.Margin = new System.Windows.Forms.Padding ( 4, 0, 4, 0 );
        this.mNiceNameLabel.Name = "mNiceNameLabel";
        this.mNiceNameLabel.Size = new System.Drawing.Size ( 39, 16 );
        this.mNiceNameLabel.TabIndex = 1;
        this.mNiceNameLabel.Text = "Nice:";
        this.mNiceNameLabel.LocationChanged += new System.EventHandler ( this.mNiceNameLabel_LocationChanged );
        // 
        // EnumControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF ( 8F, 16F );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.Transparent;
        this.Controls.Add ( this.mNiceNameLabel );
        this.Controls.Add ( this.mComboBox );
        this.Margin = new System.Windows.Forms.Padding ( 5 );
        this.Name = "EnumControl";
        this.Size = new System.Drawing.Size ( 215, 48 );
        this.Load += new System.EventHandler ( this.ComboboxControl_Load );
        this.Controls.SetChildIndex ( this.mLabel, 0 );
        this.Controls.SetChildIndex ( this.mComboBox, 0 );
        this.Controls.SetChildIndex ( this.mNiceNameLabel, 0 );
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.ComboBox mComboBox;
        private System.Windows.Forms.Label mNiceNameLabel;
    }
}
