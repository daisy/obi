namespace Obi.Dialogs
{
    partial class PhraseDetectionMethodDialog
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
            m_LblPhraseDetectionMethod = new System.Windows.Forms.Label();
            m_RadioTraditional = new System.Windows.Forms.RadioButton();
            m_RadioAI = new System.Windows.Forms.RadioButton();
            m_BtnOK = new System.Windows.Forms.Button();
            m_BtnCancel = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // m_LblPhraseDetectionMethod
            // 
            m_LblPhraseDetectionMethod.AccessibleName = "Phrase Detection Method";
            m_LblPhraseDetectionMethod.AutoSize = true;
            m_LblPhraseDetectionMethod.Location = new System.Drawing.Point(146, 60);
            m_LblPhraseDetectionMethod.Name = "m_LblPhraseDetectionMethod";
            m_LblPhraseDetectionMethod.Size = new System.Drawing.Size(177, 20);
            m_LblPhraseDetectionMethod.TabIndex = 0;
            m_LblPhraseDetectionMethod.Text = "Phrase Detection Method";
            // 
            // m_RadioTraditional
            // 
            m_RadioTraditional.AutoSize = true;
            m_RadioTraditional.Location = new System.Drawing.Point(43, 120);
            m_RadioTraditional.Name = "m_RadioTraditional";
            m_RadioTraditional.Size = new System.Drawing.Size(217, 24);
            m_RadioTraditional.TabIndex = 1;
            m_RadioTraditional.TabStop = true;
            m_RadioTraditional.Text = "&Traditional Phrase Detection";
            m_RadioTraditional.UseVisualStyleBackColor = true;
            // 
            // m_RadioAI
            // 
            m_RadioAI.AutoSize = true;
            m_RadioAI.Location = new System.Drawing.Point(320, 120);
            m_RadioAI.Name = "m_RadioAI";
            m_RadioAI.Size = new System.Drawing.Size(160, 24);
            m_RadioAI.TabIndex = 2;
            m_RadioAI.TabStop = true;
            m_RadioAI.Text = "&AI Phrase Detection";
            m_RadioAI.UseVisualStyleBackColor = true;
            // 
            // m_BtnOK
            // 
            m_BtnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_BtnOK.Location = new System.Drawing.Point(109, 204);
            m_BtnOK.Name = "m_BtnOK";
            m_BtnOK.Size = new System.Drawing.Size(94, 29);
            m_BtnOK.TabIndex = 3;
            m_BtnOK.Text = "&OK";
            m_BtnOK.UseVisualStyleBackColor = true;
            m_BtnOK.Click += m_BtnOK_Click;
            // 
            // m_BtnCancel
            // 
            m_BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_BtnCancel.Location = new System.Drawing.Point(277, 204);
            m_BtnCancel.Name = "m_BtnCancel";
            m_BtnCancel.Size = new System.Drawing.Size(94, 29);
            m_BtnCancel.TabIndex = 4;
            m_BtnCancel.Text = "&Cancel";
            m_BtnCancel.UseVisualStyleBackColor = true;
            m_BtnCancel.Click += m_BtnCancel_Click;
            // 
            // PhraseDetectionMethodDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(516, 261);
            Controls.Add(m_BtnCancel);
            Controls.Add(m_BtnOK);
            Controls.Add(m_RadioAI);
            Controls.Add(m_RadioTraditional);
            Controls.Add(m_LblPhraseDetectionMethod);
            Name = "PhraseDetectionMethodDialog";
            Text = "Phrase Detection Method";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label m_LblPhraseDetectionMethod;
        private System.Windows.Forms.RadioButton m_RadioTraditional;
        private System.Windows.Forms.RadioButton m_RadioAI;
        private System.Windows.Forms.Button m_BtnOK;
        private System.Windows.Forms.Button m_BtnCancel;
    }
}