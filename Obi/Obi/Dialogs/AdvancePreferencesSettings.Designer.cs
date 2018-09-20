namespace Obi.Dialogs
{
    partial class AdvancePreferencesSettings
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
            this.m_CheckBoxListView = new System.Windows.Forms.ListView();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_CheckBoxListView
            // 
            this.m_CheckBoxListView.AccessibleName = "List box, Use up & Down arrow to move in checkboxes";
            this.m_CheckBoxListView.CheckBoxes = true;
            this.m_CheckBoxListView.Location = new System.Drawing.Point(58, 16);
            this.m_CheckBoxListView.Name = "m_CheckBoxListView";
            this.m_CheckBoxListView.Size = new System.Drawing.Size(341, 268);
            this.m_CheckBoxListView.TabIndex = 0;
            this.m_CheckBoxListView.UseCompatibleStateImageBehavior = false;
            this.m_CheckBoxListView.View = System.Windows.Forms.View.List;
            // 
            // m_btnOk
            // 
            this.m_btnOk.Location = new System.Drawing.Point(109, 319);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 1;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(234, 319);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "&Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // AdvancePreferencesSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 354);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_CheckBoxListView);
            this.Name = "AdvancePreferencesSettings";
            this.Text = "Advance Preferences Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView m_CheckBoxListView;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button button2;




    }
}