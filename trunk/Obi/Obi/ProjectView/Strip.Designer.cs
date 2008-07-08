namespace Obi.ProjectView
{
    partial class Strip
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
            this.mLabel = new Obi.ProjectView.EditableLabel();
            this.mBlockLayout = new Obi.ProjectView.BlockLayout();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            this.mLabel.BackColor = System.Drawing.Color.Thistle;
            this.mLabel.Editable = false;
            this.mLabel.Label = "label1";
            this.mLabel.Location = new System.Drawing.Point(4, 3);
            this.mLabel.Margin = new System.Windows.Forms.Padding(0);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(303, 75);
            this.mLabel.TabIndex = 0;
            this.mLabel.Click += new System.EventHandler(this.Strip_Click);
            this.mLabel.LabelEditedByUser += new System.EventHandler(this.Label_LabelEditedByUser);
            this.mLabel.EditableChanged += new System.EventHandler(this.Label_EditableChanged);
            // 
            // mBlockLayout
            // 
            this.mBlockLayout.Location = new System.Drawing.Point(6, 81);
            this.mBlockLayout.Name = "mBlockLayout";
            this.mBlockLayout.Size = new System.Drawing.Size(301, 102);
            this.mBlockLayout.TabIndex = 1;
            // 
            // Strip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mBlockLayout);
            this.Controls.Add(this.mLabel);
            this.DoubleBuffered = true;
            this.Name = "Strip";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(313, 189);
            this.Click += new System.EventHandler(this.Strip_Click);
            this.Enter += new System.EventHandler(this.Strip_Enter);
            this.ResumeLayout(false);

        }

        #endregion

        private EditableLabel mLabel;
        private BlockLayout mBlockLayout;




    }
}
