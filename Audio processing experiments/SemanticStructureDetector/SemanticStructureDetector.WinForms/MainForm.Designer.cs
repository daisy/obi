namespace SemanticStructureDetector.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblInput = new Label();
            txtInputFile = new TextBox();
            btnBrowse = new Button();
            btnProcess = new Button();
            progressBar = new ProgressBar();
            txtLog = new TextBox();
            SuspendLayout();
            // 
            // lblInput
            // 
            lblInput.AutoSize = true;
            lblInput.Location = new Point(12, 32);
            lblInput.Name = "lblInput";
            lblInput.Size = new Size(122, 20);
            lblInput.TabIndex = 0;
            lblInput.Text = "Input XHTML File";
            // 
            // txtInputFile
            // 
            txtInputFile.Location = new Point(140, 29);
            txtInputFile.Name = "txtInputFile";
            txtInputFile.Size = new Size(700, 27);
            txtInputFile.TabIndex = 1;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(861, 29);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(94, 29);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnProcess
            // 
            btnProcess.Location = new Point(262, 92);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(303, 29);
            btnProcess.TabIndex = 3;
            btnProcess.Text = "Process with GPT-5";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(37, 165);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(900, 29);
            progressBar.TabIndex = 4;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(37, 239);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(900, 400);
            txtLog.TabIndex = 5;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 653);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Controls.Add(btnProcess);
            Controls.Add(btnBrowse);
            Controls.Add(txtInputFile);
            Controls.Add(lblInput);
            Name = "MainForm";
            Text = "Semantic Structure Detector";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblInput;
        private TextBox txtInputFile;
        private Button btnBrowse;
        private Button btnProcess;
        private ProgressBar progressBar;
        private TextBox txtLog;
    }
}
