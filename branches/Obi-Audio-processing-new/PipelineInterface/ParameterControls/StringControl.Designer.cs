namespace PipelineInterface.ParameterControls
    {
    partial class StringControl
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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StringControl));
                this.label1 = new System.Windows.Forms.Label();
                this.textBox1 = new System.Windows.Forms.TextBox();
                this.SuspendLayout();
                // 
                // label1
                // 
                resources.ApplyResources(this.label1, "label1");
                this.label1.Name = "label1";
                // 
                // textBox1
                // 
                resources.ApplyResources(this.textBox1, "textBox1");
                this.textBox1.Name = "textBox1";
                this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
                // 
                // StringControl
                // 
                resources.ApplyResources(this, "$this");
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Controls.Add(this.label1);
                this.Controls.Add(this.textBox1);
                this.Name = "StringControl";
                this.Controls.SetChildIndex(this.textBox1, 0);
                this.Controls.SetChildIndex(this.label1, 0);
                this.Controls.SetChildIndex(this.mLabel, 0);
                this.ResumeLayout(false);
                this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        }
    }
