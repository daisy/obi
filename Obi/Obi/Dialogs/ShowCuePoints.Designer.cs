namespace Obi.Dialogs
{
    partial class ShowCuePoints
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
            this.label1 = new System.Windows.Forms.Label();
            this.m_chCuePosition = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_chCueLabel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_CuePointsListView = new System.Windows.Forms.ListView();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(70, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(228, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cue Points in the audio files to be imported are:";
            // 
            // m_chCuePosition
            // 
            this.m_chCuePosition.Text = "Cue Position(Seconds)";
            this.m_chCuePosition.Width = -2;
            // 
            // m_chCueLabel
            // 
            this.m_chCueLabel.Text = "Cue Label";
            this.m_chCueLabel.Width = -2;
            // 
            // m_CuePointsListView
            // 
            this.m_CuePointsListView.AccessibleName = "Cue Points in the audio files to be imported";
            this.m_CuePointsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_chCuePosition,
            this.m_chCueLabel});
            this.m_CuePointsListView.Location = new System.Drawing.Point(49, 63);
            this.m_CuePointsListView.Name = "m_CuePointsListView";
            this.m_CuePointsListView.Size = new System.Drawing.Size(280, 246);
            this.m_CuePointsListView.TabIndex = 1;
            this.m_CuePointsListView.UseCompatibleStateImageBehavior = false;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnOk.Location = new System.Drawing.Point(144, 345);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 2;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // ShowCuePoints
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnOk;
            this.ClientSize = new System.Drawing.Size(387, 393);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_CuePointsListView);
            this.Controls.Add(this.label1);
            this.Name = "ShowCuePoints";
            this.Text = "ShowCuePoints";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader m_chCuePosition;
        private System.Windows.Forms.ColumnHeader m_chCueLabel;
        private System.Windows.Forms.ListView m_CuePointsListView;
        private System.Windows.Forms.Button m_btnOk;
    }
}