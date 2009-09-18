namespace Obi.PipelineInterface.ParameterControls
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
                this.mNiceNameLabel = new System.Windows.Forms.Label();
                this.mIntBox = new System.Windows.Forms.TextBox();
                this.SuspendLayout();
                // 
                // mNiceNameLabel
                // 
                this.mNiceNameLabel.AutoSize = true;
                this.mNiceNameLabel.Location = new System.Drawing.Point(3, 25);
                this.mNiceNameLabel.Name = "mNiceNameLabel";
                this.mNiceNameLabel.Size = new System.Drawing.Size(39, 16);
                this.mNiceNameLabel.TabIndex = 1;
                this.mNiceNameLabel.Text = "Nice:";
                // 
                // mIntBox
                // 
                this.mIntBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                this.mIntBox.Location = new System.Drawing.Point(48, 23);
                this.mIntBox.Name = "mIntBox";
                this.mIntBox.Size = new System.Drawing.Size(74, 22);
                this.mIntBox.TabIndex = 2;
                this.mIntBox.TextChanged += new System.EventHandler(this.mIntBox_TextChanged);
                // 
                // IntControl
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Controls.Add(this.mNiceNameLabel);
                this.Controls.Add(this.mIntBox);
                this.Margin = new System.Windows.Forms.Padding(5);
                this.Name = "IntControl";
                this.Size = new System.Drawing.Size(287, 55);
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
