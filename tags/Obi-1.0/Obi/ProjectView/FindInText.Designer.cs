namespace Obi.ProjectView
{
    partial class FindInText
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( FindInText ) );
        this.mString = new System.Windows.Forms.TextBox ();
        this.mPreviousButton = new System.Windows.Forms.Button ();
        this.mNextButton = new System.Windows.Forms.Button ();
        this.mCloseButton = new System.Windows.Forms.Button ();
        this.label1 = new System.Windows.Forms.Label ();
        this.SuspendLayout ();
        // 
        // mString
        // 
        resources.ApplyResources ( this.mString, "mString" );
        this.mString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mString.Name = "mString";
        this.mString.TextChanged += new System.EventHandler ( this.mString_TextChanged );
        this.mString.KeyDown += new System.Windows.Forms.KeyEventHandler ( this.mString_KeyDown );
        this.mString.Enter += new System.EventHandler ( this.mString_Enter );
        // 
        // mPreviousButton
        // 
        resources.ApplyResources ( this.mPreviousButton, "mPreviousButton" );
        this.mPreviousButton.Name = "mPreviousButton";
        this.mPreviousButton.UseVisualStyleBackColor = true;
            this.mPreviousButton.Click +=  new System.EventHandler(mPreviousButton_Click);
        // 
        // mNextButton
        // 
        resources.ApplyResources ( this.mNextButton, "mNextButton" );
        this.mNextButton.Name = "mNextButton";
        this.mNextButton.UseVisualStyleBackColor = true;
        this.mNextButton.Click += new System.EventHandler ( this.mNextButton_Click );
        // 
        // mCloseButton
        // 
        resources.ApplyResources ( this.mCloseButton, "mCloseButton" );
        this.mCloseButton.Name = "mCloseButton";
        this.mCloseButton.UseVisualStyleBackColor = true;
        this.mCloseButton.Click += new System.EventHandler ( this.mCloseButton_Click );
        // 
        // label1
        // 
        resources.ApplyResources ( this.label1, "label1" );
        this.label1.Name = "label1";
        // 
        // FindInText
        // 
        resources.ApplyResources ( this, "$this" );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.Honeydew;
        this.Controls.Add ( this.label1 );
        this.Controls.Add ( this.mCloseButton );
        this.Controls.Add ( this.mNextButton );
        this.Controls.Add ( this.mPreviousButton );
        this.Controls.Add ( this.mString );
        this.Name = "FindInText";
        this.Leave += new System.EventHandler ( this.FindInText_Leave );
        this.FontChanged += new System.EventHandler ( this.FindInText_FontChanged );
        this.Enter += new System.EventHandler ( this.FindInText_Enter );
        this.SizeChanged += new System.EventHandler ( this.FindInText_SizeChanged );
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.TextBox mString;
        private System.Windows.Forms.Button mPreviousButton;
        private System.Windows.Forms.Button mNextButton;
        private System.Windows.Forms.Button mCloseButton;
        private System.Windows.Forms.Label label1;
    }
}
