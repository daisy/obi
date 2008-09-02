namespace Obi.Dialogs
{
    partial class SetPageNumber
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( SetPageNumber ) );
        this.label1 = new System.Windows.Forms.Label ();
        this.mNumberBox = new System.Windows.Forms.TextBox ();
        this.mOKButton = new System.Windows.Forms.Button ();
        this.mCancelButton = new System.Windows.Forms.Button ();
        this.mRenumber = new System.Windows.Forms.CheckBox ();
        this.mNumberOfPagesBox = new System.Windows.Forms.TextBox ();
        this.label2 = new System.Windows.Forms.Label ();
        this.mPageKindComboBox = new System.Windows.Forms.ComboBox ();
        this.label3 = new System.Windows.Forms.Label ();
        this.SuspendLayout ();
        // 
        // label1
        // 
        resources.ApplyResources ( this.label1, "label1" );
        this.label1.Name = "label1";
        // 
        // mNumberBox
        // 
        resources.ApplyResources ( this.mNumberBox, "mNumberBox" );
        this.mNumberBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mNumberBox.Name = "mNumberBox";
        // 
        // mOKButton
        // 
        resources.ApplyResources ( this.mOKButton, "mOKButton" );
        this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.mOKButton.Name = "mOKButton";
        this.mOKButton.UseVisualStyleBackColor = true;
        // 
        // mCancelButton
        // 
        resources.ApplyResources ( this.mCancelButton, "mCancelButton" );
        this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.mCancelButton.Name = "mCancelButton";
        this.mCancelButton.UseVisualStyleBackColor = true;
        // 
        // mRenumber
        // 
        resources.ApplyResources ( this.mRenumber, "mRenumber" );
        this.mRenumber.Name = "mRenumber";
        this.mRenumber.UseVisualStyleBackColor = true;
        // 
        // mNumberOfPagesBox
        // 
        resources.ApplyResources ( this.mNumberOfPagesBox, "mNumberOfPagesBox" );
        this.mNumberOfPagesBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mNumberOfPagesBox.Name = "mNumberOfPagesBox";
        // 
        // label2
        // 
        resources.ApplyResources ( this.label2, "label2" );
        this.label2.Name = "label2";
        // 
        // mPageKindComboBox
        // 
        this.mPageKindComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        resources.ApplyResources ( this.mPageKindComboBox, "mPageKindComboBox" );
        this.mPageKindComboBox.FormattingEnabled = true;
        this.mPageKindComboBox.Items.AddRange ( new object[] {
            resources.GetString("mPageKindComboBox.Items"),
            resources.GetString("mPageKindComboBox.Items1"),
            resources.GetString("mPageKindComboBox.Items2")} );
        this.mPageKindComboBox.Name = "mPageKindComboBox";
        this.mPageKindComboBox.SelectedIndexChanged += new System.EventHandler ( this.mPageKindComboBox_SelectedIndexChanged );
        // 
        // label3
        // 
        resources.ApplyResources ( this.label3, "label3" );
        this.label3.Name = "label3";
        // 
        // SetPageNumber
        // 
        this.AcceptButton = this.mOKButton;
        resources.ApplyResources ( this, "$this" );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.mCancelButton;
        this.Controls.Add ( this.label3 );
        this.Controls.Add ( this.mPageKindComboBox );
        this.Controls.Add ( this.label2 );
        this.Controls.Add ( this.mNumberOfPagesBox );
        this.Controls.Add ( this.mRenumber );
        this.Controls.Add ( this.mCancelButton );
        this.Controls.Add ( this.mOKButton );
        this.Controls.Add ( this.mNumberBox );
        this.Controls.Add ( this.label1 );
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SetPageNumber";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mNumberBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mRenumber;
        private System.Windows.Forms.TextBox mNumberOfPagesBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox mPageKindComboBox;
        private System.Windows.Forms.Label label3;
    }
}