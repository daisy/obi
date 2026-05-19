namespace AudioTranscriber
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
            lblAudio = new Label();
            lblOutput = new Label();
            lblStatus = new Label();
            txtAudioPath = new TextBox();
            txtOutputPath = new TextBox();
            btnBrowseAudio = new Button();
            btnBrowseOutput = new Button();
            btnStart = new Button();
            btnCancel = new Button();
            progressBar = new ProgressBar();
            dgvTranscript = new DataGridView();
            StartTime = new DataGridViewTextBoxColumn();
            EndTime = new DataGridViewTextBoxColumn();
            Word = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvTranscript).BeginInit();
            SuspendLayout();
            // 
            // lblAudio
            // 
            lblAudio.AutoSize = true;
            lblAudio.Location = new Point(20, 20);
            lblAudio.Name = "lblAudio";
            lblAudio.Size = new Size(76, 20);
            lblAudio.TabIndex = 0;
            lblAudio.Text = "Audio File";
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Location = new Point(20, 70);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(82, 20);
            lblOutput.TabIndex = 1;
            lblOutput.Text = "Output File";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20, 620);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(50, 20);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Ready";
            // 
            // txtAudioPath
            // 
            txtAudioPath.AllowDrop = true;
            txtAudioPath.Location = new Point(120, 20);
            txtAudioPath.Name = "txtAudioPath";
            txtAudioPath.Size = new Size(700, 27);
            txtAudioPath.TabIndex = 3;
            // 
            // txtOutputPath
            // 
            txtOutputPath.Location = new Point(120, 70);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new Size(700, 27);
            txtOutputPath.TabIndex = 4;
            // 
            // btnBrowseAudio
            // 
            btnBrowseAudio.Location = new Point(850, 18);
            btnBrowseAudio.Name = "btnBrowseAudio";
            btnBrowseAudio.Size = new Size(120, 35);
            btnBrowseAudio.TabIndex = 5;
            btnBrowseAudio.Text = "Browse";
            btnBrowseAudio.UseVisualStyleBackColor = true;
            // 
            // btnBrowseOutput
            // 
            btnBrowseOutput.Location = new Point(850, 68);
            btnBrowseOutput.Name = "btnBrowseOutput";
            btnBrowseOutput.Size = new Size(120, 35);
            btnBrowseOutput.TabIndex = 6;
            btnBrowseOutput.Text = "Save As";
            btnBrowseOutput.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(120, 120);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(120, 40);
            btnStart.TabIndex = 7;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(260, 120);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 40);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(20, 180);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1000, 30);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 9;
            progressBar.Visible = false;
            // 
            // dgvTranscript
            // 
            dgvTranscript.AllowUserToAddRows = false;
            dgvTranscript.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTranscript.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTranscript.Columns.AddRange(new DataGridViewColumn[] { StartTime, EndTime, Word });
            dgvTranscript.Location = new Point(20, 240);
            dgvTranscript.Name = "dgvTranscript";
            dgvTranscript.ReadOnly = true;
            dgvTranscript.RowHeadersWidth = 51;
            dgvTranscript.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTranscript.Size = new Size(1000, 350);
            dgvTranscript.TabIndex = 10;
            // 
            // StartTime
            // 
            StartTime.HeaderText = "Start";
            StartTime.MinimumWidth = 6;
            StartTime.Name = "StartTime";
            StartTime.ReadOnly = true;
            // 
            // EndTime
            // 
            EndTime.HeaderText = "End";
            EndTime.MinimumWidth = 6;
            EndTime.Name = "EndTime";
            EndTime.ReadOnly = true;
            // 
            // Word
            // 
            Word.HeaderText = "Word";
            Word.MinimumWidth = 6;
            Word.Name = "Word";
            Word.ReadOnly = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1082, 653);
            Controls.Add(dgvTranscript);
            Controls.Add(progressBar);
            Controls.Add(btnCancel);
            Controls.Add(btnStart);
            Controls.Add(btnBrowseOutput);
            Controls.Add(btnBrowseAudio);
            Controls.Add(txtOutputPath);
            Controls.Add(txtAudioPath);
            Controls.Add(lblStatus);
            Controls.Add(lblOutput);
            Controls.Add(lblAudio);
            Location = new Point(20, 20);
            MinimumSize = new Size(1100, 700);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Audio Transcriber";
            ((System.ComponentModel.ISupportInitialize)dgvTranscript).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAudio;
        private Label lblOutput;
        private Label lblStatus;
        private TextBox txtAudioPath;
        private TextBox txtOutputPath;
        private Button btnBrowseAudio;
        private Button btnBrowseOutput;
        private Button btnStart;
        private Button btnCancel;
        private ProgressBar progressBar;
        private DataGridView dgvTranscript;
        private DataGridViewTextBoxColumn StartTime;
        private DataGridViewTextBoxColumn EndTime;
        private DataGridViewTextBoxColumn Word;
    }
}
