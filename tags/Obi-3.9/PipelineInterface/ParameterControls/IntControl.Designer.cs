namespace PipelineInterface.ParameterControls
    {
    partial class IntControl
        {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose ();
                }
            base.Dispose ( disposing );
            }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntControl));
                this.mNiceNameLabel = new System.Windows.Forms.Label();
                this.mIntBox = new System.Windows.Forms.TextBox();
                this.SuspendLayout();
                // 
                // mNiceNameLabel
                // 
                resources.ApplyResources(this.mNiceNameLabel, "mNiceNameLabel");
                this.mNiceNameLabel.Name = "mNiceNameLabel";
                // 
                // mIntBox
                // 
                this.mIntBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                resources.ApplyResources(this.mIntBox, "mIntBox");
                this.mIntBox.Name = "mIntBox";
                this.mIntBox.TextChanged += new System.EventHandler(this.mIntBox_TextChanged);
                // 
                // IntControl
                // 
                resources.ApplyResources(this, "$this");
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Controls.Add(this.mNiceNameLabel);
                this.Controls.Add(this.mIntBox);
                this.Name = "IntControl";
                this.Controls.SetChildIndex(this.mIntBox, 0);
                this.Controls.SetChildIndex(this.mNiceNameLabel, 0);
                this.Controls.SetChildIndex(this.mLabel, 0);
                this.ResumeLayout(false);
                this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Label mNiceNameLabel;
        private System.Windows.Forms.TextBox mIntBox;
        }
    }
