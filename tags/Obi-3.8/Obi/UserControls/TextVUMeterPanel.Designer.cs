namespace Obi.UserControls
{
    partial class TextVUMeterPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextVUMeterPanel));
            this.lblLeft = new System.Windows.Forms.Label();
            this.mLeftBox = new System.Windows.Forms.TextBox();
            this.lblRight = new System.Windows.Forms.Label();
            this.mRightBox = new System.Windows.Forms.TextBox();
            this.mResetButton = new System.Windows.Forms.Button();
            this.tmUpdateText = new System.Windows.Forms.Timer(this.components);
            this.mVUMeterPaneltoolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblLeft
            // 
            resources.ApplyResources(this.lblLeft, "lblLeft");
            this.lblLeft.Name = "lblLeft";
            // 
            // mLeftBox
            // 
            resources.ApplyResources(this.mLeftBox, "mLeftBox");
            this.mLeftBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mLeftBox.Name = "mLeftBox";
            this.mLeftBox.ReadOnly = true;
            // 
            // lblRight
            // 
            resources.ApplyResources(this.lblRight, "lblRight");
            this.lblRight.Name = "lblRight";
            // 
            // mRightBox
            // 
            resources.ApplyResources(this.mRightBox, "mRightBox");
            this.mRightBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mRightBox.Name = "mRightBox";
            this.mRightBox.ReadOnly = true;
            // 
            // mResetButton
            // 
            resources.ApplyResources(this.mResetButton, "mResetButton");
            this.mResetButton.BackColor = System.Drawing.SystemColors.Control;
            this.mResetButton.MaximumSize = new System.Drawing.Size(32, 28);
            this.mResetButton.Name = "mResetButton";
            this.mResetButton.UseVisualStyleBackColor = false;
            this.mResetButton.Click += new System.EventHandler(this.mResetButton_Click);
            // 
            // tmUpdateText
            // 
            this.tmUpdateText.Enabled = true;
            this.tmUpdateText.Interval = 400;
            this.tmUpdateText.Tick += new System.EventHandler(this.tmUpdateText_Tick);
            // 
            // mVUMeterPaneltoolTip
            // 
            this.mVUMeterPaneltoolTip.IsBalloon = true;
            this.mVUMeterPaneltoolTip.ToolTipTitle = "Transport Bar";
            // 
            // TextVUMeterPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.mResetButton);
            this.Controls.Add(this.mRightBox);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.mLeftBox);
            this.Controls.Add(this.lblLeft);
            this.Name = "TextVUMeterPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.TextBox mLeftBox;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.TextBox mRightBox;
        private System.Windows.Forms.Button mResetButton;
        private System.Windows.Forms.Timer tmUpdateText;
        private System.Windows.Forms.ToolTip mVUMeterPaneltoolTip;

    }
}
