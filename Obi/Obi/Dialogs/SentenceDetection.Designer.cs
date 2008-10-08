namespace Obi.Dialogs
{
    partial class SentenceDetection
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( SentenceDetection ) );
        this.mOKButton = new System.Windows.Forms.Button ();
        this.mCancelButton = new System.Windows.Forms.Button ();
        this.label1 = new System.Windows.Forms.Label ();
        this.mThresholdBox = new System.Windows.Forms.TextBox ();
        this.label2 = new System.Windows.Forms.Label ();
        this.mGapBox = new System.Windows.Forms.TextBox ();
        this.label3 = new System.Windows.Forms.Label ();
        this.mLeadingSilenceBox = new System.Windows.Forms.TextBox ();
        this.SuspendLayout ();
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
        // label1
        // 
        resources.ApplyResources ( this.label1, "label1" );
        this.label1.Name = "label1";
        // 
        // mThresholdBox
        // 
        resources.ApplyResources ( this.mThresholdBox, "mThresholdBox" );
        this.mThresholdBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mThresholdBox.Name = "mThresholdBox";
        // 
        // label2
        // 
        resources.ApplyResources ( this.label2, "label2" );
        this.label2.Name = "label2";
        // 
        // mGapBox
        // 
        resources.ApplyResources ( this.mGapBox, "mGapBox" );
        this.mGapBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mGapBox.Name = "mGapBox";
        this.mGapBox.TextChanged += new System.EventHandler ( this.mGapBox_TextChanged );
        // 
        // label3
        // 
        resources.ApplyResources ( this.label3, "label3" );
        this.label3.Name = "label3";
        // 
        // mLeadingSilenceBox
        // 
        resources.ApplyResources ( this.mLeadingSilenceBox, "mLeadingSilenceBox" );
        this.mLeadingSilenceBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mLeadingSilenceBox.Name = "mLeadingSilenceBox";
        this.mLeadingSilenceBox.TextChanged += new System.EventHandler ( this.mLeadingSilenceBox_TextChanged );
        // 
        // SentenceDetection
        // 
        this.AcceptButton = this.mOKButton;
        resources.ApplyResources ( this, "$this" );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.mCancelButton;
        this.Controls.Add ( this.mLeadingSilenceBox );
        this.Controls.Add ( this.label3 );
        this.Controls.Add ( this.mGapBox );
        this.Controls.Add ( this.label2 );
        this.Controls.Add ( this.mThresholdBox );
        this.Controls.Add ( this.label1 );
        this.Controls.Add ( this.mCancelButton );
        this.Controls.Add ( this.mOKButton );
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SentenceDetection";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mThresholdBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mGapBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mLeadingSilenceBox;
    }
}