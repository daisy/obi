namespace Obi.Dialogs
{
    partial class ProjectStatistics
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
            this.m_lblDuration = new System.Windows.Forms.Label();
            this.m_txtDuration = new System.Windows.Forms.TextBox();
            this.m_lblSectionCount = new System.Windows.Forms.Label();
            this.m_txtSectionsCount = new System.Windows.Forms.TextBox();
            this.m_lblPhraseCount = new System.Windows.Forms.Label();
            this.m_txtPhraseCount = new System.Windows.Forms.TextBox();
            this.m_lblPageCount = new System.Windows.Forms.Label();
            this.m_txtPageCount = new System.Windows.Forms.TextBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_lblTitle = new System.Windows.Forms.Label();
            this.m_txtTitle = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_lblDuration
            // 
            this.m_lblDuration.AutoSize = true;
            this.m_lblDuration.Location = new System.Drawing.Point(0, 50);
            this.m_lblDuration.Name = "m_lblDuration";
            this.m_lblDuration.Size = new System.Drawing.Size(86, 13);
            this.m_lblDuration.TabIndex = 2;
            this.m_lblDuration.Text = "Project &Duration:";
            // 
            // m_txtDuration
            // 
            this.m_txtDuration.AccessibleName = "Project Duration:";
            this.m_txtDuration.Location = new System.Drawing.Point(100, 50);
            this.m_txtDuration.Name = "m_txtDuration";
            this.m_txtDuration.ReadOnly = true;
            this.m_txtDuration.Size = new System.Drawing.Size(100, 20);
            this.m_txtDuration.TabIndex = 3;
            // 
            // m_lblSectionCount
            // 
            this.m_lblSectionCount.AutoSize = true;
            this.m_lblSectionCount.Location = new System.Drawing.Point(0, 80);
            this.m_lblSectionCount.Name = "m_lblSectionCount";
            this.m_lblSectionCount.Size = new System.Drawing.Size(83, 13);
            this.m_lblSectionCount.TabIndex = 4;
            this.m_lblSectionCount.Text = "No. of &Sections:";
            // 
            // m_txtSectionsCount
            // 
            this.m_txtSectionsCount.AccessibleName = "Number of sections:";
            this.m_txtSectionsCount.Location = new System.Drawing.Point(100, 80);
            this.m_txtSectionsCount.Name = "m_txtSectionsCount";
            this.m_txtSectionsCount.ReadOnly = true;
            this.m_txtSectionsCount.Size = new System.Drawing.Size(100, 20);
            this.m_txtSectionsCount.TabIndex = 5;
            // 
            // m_lblPhraseCount
            // 
            this.m_lblPhraseCount.AutoSize = true;
            this.m_lblPhraseCount.Location = new System.Drawing.Point(0, 110);
            this.m_lblPhraseCount.Name = "m_lblPhraseCount";
            this.m_lblPhraseCount.Size = new System.Drawing.Size(104, 13);
            this.m_lblPhraseCount.TabIndex = 6;
            this.m_lblPhraseCount.Text = "Total no. of &phrases:";
            // 
            // m_txtPhraseCount
            // 
            this.m_txtPhraseCount.AccessibleName = "Total number of phrases:";
            this.m_txtPhraseCount.Location = new System.Drawing.Point(100, 110);
            this.m_txtPhraseCount.Name = "m_txtPhraseCount";
            this.m_txtPhraseCount.ReadOnly = true;
            this.m_txtPhraseCount.Size = new System.Drawing.Size(100, 20);
            this.m_txtPhraseCount.TabIndex = 7;
            // 
            // m_lblPageCount
            // 
            this.m_lblPageCount.AutoSize = true;
            this.m_lblPageCount.Location = new System.Drawing.Point(0, 140);
            this.m_lblPageCount.Name = "m_lblPageCount";
            this.m_lblPageCount.Size = new System.Drawing.Size(96, 13);
            this.m_lblPageCount.TabIndex = 8;
            this.m_lblPageCount.Text = "Total no. of pages:";
            // 
            // m_txtPageCount
            // 
            this.m_txtPageCount.AccessibleName = "Total number of pages:";
            this.m_txtPageCount.Location = new System.Drawing.Point(100, 140);
            this.m_txtPageCount.Name = "m_txtPageCount";
            this.m_txtPageCount.ReadOnly = true;
            this.m_txtPageCount.Size = new System.Drawing.Size(100, 20);
            this.m_txtPageCount.TabIndex = 9;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.Location = new System.Drawing.Point(205, 200);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 10;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_lblTitle
            // 
            this.m_lblTitle.AutoSize = true;
            this.m_lblTitle.Location = new System.Drawing.Point(0, 10);
            this.m_lblTitle.Name = "m_lblTitle";
            this.m_lblTitle.Size = new System.Drawing.Size(30, 13);
            this.m_lblTitle.TabIndex = 0;
            this.m_lblTitle.Text = "Title:";
            // 
            // m_txtTitle
            // 
            this.m_txtTitle.AccessibleName = "Title:";
            this.m_txtTitle.Location = new System.Drawing.Point(100, 10);
            this.m_txtTitle.Name = "m_txtTitle";
            this.m_txtTitle.Size = new System.Drawing.Size(100, 20);
            this.m_txtTitle.TabIndex = 1;
            // 
            // ProjectStatistics
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.m_txtTitle);
            this.Controls.Add(this.m_lblTitle);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_txtPageCount);
            this.Controls.Add(this.m_lblPageCount);
            this.Controls.Add(this.m_txtPhraseCount);
            this.Controls.Add(this.m_lblPhraseCount);
            this.Controls.Add(this.m_txtSectionsCount);
            this.Controls.Add(this.m_lblSectionCount);
            this.Controls.Add(this.m_txtDuration);
            this.Controls.Add(this.m_lblDuration);
            this.Name = "ProjectStatistics";
            this.Text = "Project Statistics";
            this.Load += new System.EventHandler(this.ProjectStatistics_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDuration;
        private System.Windows.Forms.TextBox m_txtDuration;
        private System.Windows.Forms.Label m_lblSectionCount;
        private System.Windows.Forms.TextBox m_txtSectionsCount;
        private System.Windows.Forms.Label m_lblPhraseCount;
        private System.Windows.Forms.TextBox m_txtPhraseCount;
        private System.Windows.Forms.Label m_lblPageCount;
        private System.Windows.Forms.TextBox m_txtPageCount;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Label m_lblTitle;
        private System.Windows.Forms.TextBox m_txtTitle;
    }
}