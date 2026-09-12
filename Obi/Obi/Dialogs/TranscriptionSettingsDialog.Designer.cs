namespace Obi.Dialogs
{
    partial class TranscriptionSettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed;
        /// otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing &&
                (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support -
        /// do not modify the contents of this method
        /// with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(
                    typeof(TranscriptionSettingsDialog));

            m_ModelCb =
                new System.Windows.Forms.ComboBox();

            m_btnOK =
                new System.Windows.Forms.Button();

            label1 =
                new System.Windows.Forms.Label();

            lblBookLanguage =
                new System.Windows.Forms.Label();

            m_BookLanguageCb =
                new System.Windows.Forms.ComboBox();

            m_TranscriptionEngineCb =
                new System.Windows.Forms.ComboBox();

            lblTranscriptionEngine =
                new System.Windows.Forms.Label();

            m_btnCancel =
                new System.Windows.Forms.Button();

            SuspendLayout();

            // 
            // m_ModelCb
            // 
            resources.ApplyResources(
                m_ModelCb,
                "m_ModelCb");

            m_ModelCb.FormattingEnabled =
                true;

            m_ModelCb.Name =
                "m_ModelCb";

            // 
            // m_btnOK
            // 
            resources.ApplyResources(
                m_btnOK,
                "m_btnOK");

            m_btnOK.Name =
                "m_btnOK";

            m_btnOK.UseVisualStyleBackColor =
                true;

            m_btnOK.Click +=
                m_btnOK_Click;

            // 
            // label1
            // 
            resources.ApplyResources(
                label1,
                "label1");

            label1.Name =
                "label1";

            // 
            // lblBookLanguage
            // 
            resources.ApplyResources(
                lblBookLanguage,
                "lblBookLanguage");

            lblBookLanguage.Name =
                "lblBookLanguage";

            // 
            // m_BookLanguageCb
            // 
            resources.ApplyResources(
                m_BookLanguageCb,
                "m_BookLanguageCb");

            m_BookLanguageCb.FormattingEnabled =
                true;

            m_BookLanguageCb.Name =
                "m_BookLanguageCb";

            // 
            // m_TranscriptionEngineCb
            // 
            resources.ApplyResources(
                m_TranscriptionEngineCb,
                "m_TranscriptionEngineCb");

            m_TranscriptionEngineCb.FormattingEnabled =
                true;

            m_TranscriptionEngineCb.Name =
                "m_TranscriptionEngineCb";

            // 
            // lblTranscriptionEngine
            // 
            resources.ApplyResources(
                lblTranscriptionEngine,
                "lblTranscriptionEngine");

            lblTranscriptionEngine.Name =
                "lblTranscriptionEngine";

            // 
            // m_btnCancel
            // 
            resources.ApplyResources(
                m_btnCancel,
                "m_btnCancel");

            m_btnCancel.Name =
                "m_btnCancel";

            m_btnCancel.UseVisualStyleBackColor =
                true;

            m_btnCancel.Click +=
                m_btnCancel_Click;

            // 
            // TranscriptionSettingsDialog
            // 
            resources.ApplyResources(
                this,
                "$this");

            AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            CancelButton =
                m_btnCancel;

            Controls.Add(
                m_btnCancel);

            Controls.Add(
                m_btnOK);

            Controls.Add(
                m_ModelCb);

            Controls.Add(
                label1);

            Controls.Add(
                m_BookLanguageCb);

            Controls.Add(
                lblBookLanguage);

            Controls.Add(
                m_TranscriptionEngineCb);

            Controls.Add(
                lblTranscriptionEngine);

            MaximizeBox =
                false;

            MinimizeBox =
                false;

            Name =
                "TranscriptionSettingsDialog";

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        private System.Windows.Forms.ComboBox m_ModelCb;

        private System.Windows.Forms.Button m_btnOK;

        private System.Windows.Forms.Button m_btnCancel;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Label lblBookLanguage;

        private System.Windows.Forms.ComboBox m_BookLanguageCb;

        private System.Windows.Forms.ComboBox m_TranscriptionEngineCb;

        private System.Windows.Forms.Label lblTranscriptionEngine;
    }
}