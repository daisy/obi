namespace Obi.Dialogs
{
    partial class AudioProcessingDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioProcessingDialog));
            this.m_gpbox_Process = new System.Windows.Forms.GroupBox();
            this.m_txt_info = new System.Windows.Forms.TextBox();
            this.m_numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.m_lbl_Parameters = new System.Windows.Forms.Label();
            this.m_cb_Process = new System.Windows.Forms.ComboBox();
            this.m_lbl_Process = new System.Windows.Forms.Label();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_InfoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_gpbox_Process.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // m_gpbox_Process
            // 
            this.m_gpbox_Process.Controls.Add(this.m_txt_info);
            this.m_gpbox_Process.Controls.Add(this.m_numericUpDown1);
            this.m_gpbox_Process.Controls.Add(this.m_lbl_Parameters);
            this.m_gpbox_Process.Controls.Add(this.m_cb_Process);
            this.m_gpbox_Process.Controls.Add(this.m_lbl_Process);
            resources.ApplyResources(this.m_gpbox_Process, "m_gpbox_Process");
            this.m_gpbox_Process.Name = "m_gpbox_Process";
            this.m_gpbox_Process.TabStop = false;
            // 
            // m_txt_info
            // 
            resources.ApplyResources(this.m_txt_info, "m_txt_info");
            this.m_txt_info.Name = "m_txt_info";
            this.m_txt_info.ReadOnly = true;
            // 
            // m_numericUpDown1
            // 
            this.m_numericUpDown1.DecimalPlaces = 1;
            resources.ApplyResources(this.m_numericUpDown1, "m_numericUpDown1");
            this.m_numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.m_numericUpDown1.Maximum = new decimal(new int[] {
            40,
            0,
            0,
            65536});
            this.m_numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.m_numericUpDown1.Name = "m_numericUpDown1";
            this.m_numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // m_lbl_Parameters
            // 
            resources.ApplyResources(this.m_lbl_Parameters, "m_lbl_Parameters");
            this.m_lbl_Parameters.Name = "m_lbl_Parameters";
            // 
            // m_cb_Process
            // 
            this.m_cb_Process.FormattingEnabled = true;
            this.m_cb_Process.Items.AddRange(new object[] {
            resources.GetString("m_cb_Process.Items"),
            resources.GetString("m_cb_Process.Items1"),
            resources.GetString("m_cb_Process.Items2")});
            resources.ApplyResources(this.m_cb_Process, "m_cb_Process");
            this.m_cb_Process.Name = "m_cb_Process";
            // 
            // m_lbl_Process
            // 
            resources.ApplyResources(this.m_lbl_Process, "m_lbl_Process");
            this.m_lbl_Process.Name = "m_lbl_Process";
            // 
            // m_btn_OK
            // 
            resources.ApplyResources(this.m_btn_OK, "m_btn_OK");
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btn_Cancel, "m_btn_Cancel");
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // AudioProcessingDialog
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_gpbox_Process);
            this.MaximizeBox = false;
            this.Name = "AudioProcessingDialog";
            this.m_gpbox_Process.ResumeLayout(false);
            this.m_gpbox_Process.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox m_gpbox_Process;
        private System.Windows.Forms.ComboBox m_cb_Process;
        private System.Windows.Forms.Label m_lbl_Process;
        private System.Windows.Forms.NumericUpDown m_numericUpDown1;
        private System.Windows.Forms.Label m_lbl_Parameters;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.TextBox m_txt_info;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ToolTip m_InfoToolTip;
    }
}