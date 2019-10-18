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
            this.mBlockLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.mLabel = new Obi.ProjectView.EditableLabel();
            this.SuspendLayout();
            // 
            // mBlockLayout
            // 
            this.mBlockLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mBlockLayout.BackColor = System.Drawing.Color.GreenYellow;
            this.mBlockLayout.Location = new System.Drawing.Point(3, 78);
            this.mBlockLayout.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mBlockLayout.Name = "mBlockLayout";
            this.mBlockLayout.Size = new System.Drawing.Size(303, 109);
            this.mBlockLayout.TabIndex = 1;
            this.mBlockLayout.WrapContents = false;
            this.mBlockLayout.Click += new System.EventHandler(this.Strip_Enter);
            // 
            // mLabel
            // 
            this.mLabel.BackColor = System.Drawing.Color.DodgerBlue;
            this.mLabel.Editable = true;
            this.mLabel.Label = "Untitled section";
            this.mLabel.Location = new System.Drawing.Point(3, 3);
            this.mLabel.MinimumSize = new System.Drawing.Size(184, 0);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(184, 72);
            this.mLabel.TabIndex = 0;
            this.mLabel.Tag = "";
            this.mLabel.Click += new System.EventHandler(this.Strip_Enter);
            this.mLabel.Enter += new System.EventHandler(this.Strip_Enter);
            this.mLabel.LabelEditedByUser += new System.EventHandler(this.Label_LabelEditedByUser);
            this.mLabel.EditableChanged += new System.EventHandler(this.Label_EditableChanged);
            this.mLabel.SizeChanged += new System.EventHandler(this.Label_SizeChanged);
            // 
            // Strip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Yellow;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mBlockLayout);
            this.Controls.Add(this.mLabel);
            this.DoubleBuffered = true;
            this.Name = "Strip";
            this.Size = new System.Drawing.Size(309, 187);
            this.LocationChanged += new System.EventHandler(this.Strip_LocationChanged);
            this.Click += new System.EventHandler(this.Strip_Enter);
            this.Enter += new System.EventHandler(this.Strip_Enter);
            this.ResumeLayout(false);

        }

        #endregion

        private EditableLabel mLabel;
        private System.Windows.Forms.FlowLayoutPanel mBlockLayout;




    }
}
