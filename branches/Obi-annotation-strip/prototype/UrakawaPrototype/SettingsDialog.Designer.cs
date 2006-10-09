namespace UrakawaPrototype
{
    partial class SettingsDialog
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
            this.defaultPeakLevel = new System.Windows.Forms.TrackBar();
            this.peakWarningThreshold = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.oneAudioChannel = new System.Windows.Forms.RadioButton();
            this.twoAudioChannels = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.inputDevices = new System.Windows.Forms.ComboBox();
            this.outputDevices = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.audioPage = new System.Windows.Forms.TabPage();
            this.displayPage = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.colors = new System.Windows.Forms.GroupBox();
            this.font = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.vuMeterTab = new System.Windows.Forms.TabPage();
            this.vuMeter1 = new UrakawaPrototype.VuMeter();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.defaultPeakLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peakWarningThreshold)).BeginInit();
            this.tabs.SuspendLayout();
            this.audioPage.SuspendLayout();
            this.displayPage.SuspendLayout();
            this.colors.SuspendLayout();
            this.font.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.vuMeterTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // defaultPeakLevel
            // 
            this.defaultPeakLevel.LargeChange = 1;
            this.defaultPeakLevel.Location = new System.Drawing.Point(6, 94);
            this.defaultPeakLevel.Maximum = -10;
            this.defaultPeakLevel.Minimum = -50;
            this.defaultPeakLevel.Name = "defaultPeakLevel";
            this.defaultPeakLevel.Size = new System.Drawing.Size(172, 42);
            this.defaultPeakLevel.TabIndex = 0;
            this.defaultPeakLevel.Value = -26;
            // 
            // peakWarningThreshold
            // 
            this.peakWarningThreshold.Location = new System.Drawing.Point(6, 19);
            this.peakWarningThreshold.Maximum = 0;
            this.peakWarningThreshold.Minimum = -10;
            this.peakWarningThreshold.Name = "peakWarningThreshold";
            this.peakWarningThreshold.Size = new System.Drawing.Size(172, 42);
            this.peakWarningThreshold.TabIndex = 1;
            this.peakWarningThreshold.Value = -3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Peak warning threshold";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Minimum mean volume level";
            // 
            // oneAudioChannel
            // 
            this.oneAudioChannel.AutoSize = true;
            this.oneAudioChannel.Location = new System.Drawing.Point(9, 159);
            this.oneAudioChannel.Name = "oneAudioChannel";
            this.oneAudioChannel.Size = new System.Drawing.Size(45, 17);
            this.oneAudioChannel.TabIndex = 4;
            this.oneAudioChannel.TabStop = true;
            this.oneAudioChannel.Text = "One";
            this.oneAudioChannel.UseVisualStyleBackColor = true;
            // 
            // twoAudioChannels
            // 
            this.twoAudioChannels.AutoSize = true;
            this.twoAudioChannels.Location = new System.Drawing.Point(9, 182);
            this.twoAudioChannels.Name = "twoAudioChannels";
            this.twoAudioChannels.Size = new System.Drawing.Size(46, 17);
            this.twoAudioChannels.TabIndex = 5;
            this.twoAudioChannels.TabStop = true;
            this.twoAudioChannels.Text = "Two";
            this.twoAudioChannels.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Number of Audio Channels";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 223);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Set Input Device";
            // 
            // inputDevices
            // 
            this.inputDevices.FormattingEnabled = true;
            this.inputDevices.Items.AddRange(new object[] {
            "Line In",
            "Internal Microphone"});
            this.inputDevices.Location = new System.Drawing.Point(17, 243);
            this.inputDevices.Name = "inputDevices";
            this.inputDevices.Size = new System.Drawing.Size(121, 21);
            this.inputDevices.TabIndex = 8;
            this.inputDevices.Text = "Microphone";
            // 
            // outputDevices
            // 
            this.outputDevices.FormattingEnabled = true;
            this.outputDevices.Items.AddRange(new object[] {
            "Speakers",
            "Headphones",
            "Line Out"});
            this.outputDevices.Location = new System.Drawing.Point(16, 299);
            this.outputDevices.Name = "outputDevices";
            this.outputDevices.Size = new System.Drawing.Size(121, 21);
            this.outputDevices.TabIndex = 10;
            this.outputDevices.Text = "Speakers";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 279);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Set Output Device";
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.audioPage);
            this.tabs.Controls.Add(this.displayPage);
            this.tabs.Controls.Add(this.vuMeterTab);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(248, 360);
            this.tabs.TabIndex = 11;
            // 
            // audioPage
            // 
            this.audioPage.Controls.Add(this.label1);
            this.audioPage.Controls.Add(this.outputDevices);
            this.audioPage.Controls.Add(this.peakWarningThreshold);
            this.audioPage.Controls.Add(this.label5);
            this.audioPage.Controls.Add(this.label2);
            this.audioPage.Controls.Add(this.inputDevices);
            this.audioPage.Controls.Add(this.defaultPeakLevel);
            this.audioPage.Controls.Add(this.label4);
            this.audioPage.Controls.Add(this.label3);
            this.audioPage.Controls.Add(this.twoAudioChannels);
            this.audioPage.Controls.Add(this.oneAudioChannel);
            this.audioPage.Location = new System.Drawing.Point(4, 22);
            this.audioPage.Name = "audioPage";
            this.audioPage.Padding = new System.Windows.Forms.Padding(3);
            this.audioPage.Size = new System.Drawing.Size(240, 334);
            this.audioPage.TabIndex = 0;
            this.audioPage.Text = "Audio settings";
            this.audioPage.UseVisualStyleBackColor = true;
            // 
            // displayPage
            // 
            this.displayPage.Controls.Add(this.font);
            this.displayPage.Controls.Add(this.colors);
            this.displayPage.Location = new System.Drawing.Point(4, 22);
            this.displayPage.Name = "displayPage";
            this.displayPage.Padding = new System.Windows.Forms.Padding(3);
            this.displayPage.Size = new System.Drawing.Size(240, 346);
            this.displayPage.TabIndex = 1;
            this.displayPage.Text = "Display properties";
            this.displayPage.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Structure point";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Phrase";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Unused portion";
            // 
            // colors
            // 
            this.colors.Controls.Add(this.button3);
            this.colors.Controls.Add(this.button2);
            this.colors.Controls.Add(this.button1);
            this.colors.Controls.Add(this.label6);
            this.colors.Controls.Add(this.label8);
            this.colors.Controls.Add(this.label7);
            this.colors.Location = new System.Drawing.Point(8, 6);
            this.colors.Name = "colors";
            this.colors.Size = new System.Drawing.Size(214, 147);
            this.colors.TabIndex = 3;
            this.colors.TabStop = false;
            this.colors.Text = "Colors";
            // 
            // font
            // 
            this.font.Controls.Add(this.label10);
            this.font.Controls.Add(this.numericUpDown1);
            this.font.Controls.Add(this.label9);
            this.font.Controls.Add(this.comboBox1);
            this.font.Location = new System.Drawing.Point(8, 165);
            this.font.Name = "font";
            this.font.Size = new System.Drawing.Size(214, 176);
            this.font.TabIndex = 4;
            this.font.TabStop = false;
            this.font.Text = "Display font";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LimeGreen;
            this.button1.Location = new System.Drawing.Point(118, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 24);
            this.button1.TabIndex = 3;
            this.button1.Text = "Color [Green]";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.CornflowerBlue;
            this.button2.Location = new System.Drawing.Point(118, 47);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 24);
            this.button2.TabIndex = 4;
            this.button2.Text = "Color [Blue]";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.DimGray;
            this.button3.Location = new System.Drawing.Point(118, 87);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 24);
            this.button3.TabIndex = 5;
            this.button3.Text = "Color [Gray]";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Times New Roman",
            "Verdana",
            "MS Sans Serif"});
            this.comboBox1.Location = new System.Drawing.Point(9, 51);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(187, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.Text = "MS Sans Serif";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Font name";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(9, 108);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(38, 20);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 92);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Font size";
            // 
            // vuMeterTab
            // 
            this.vuMeterTab.Controls.Add(this.vuMeter1);
            this.vuMeterTab.Location = new System.Drawing.Point(4, 22);
            this.vuMeterTab.Name = "vuMeterTab";
            this.vuMeterTab.Padding = new System.Windows.Forms.Padding(3);
            this.vuMeterTab.Size = new System.Drawing.Size(240, 346);
            this.vuMeterTab.TabIndex = 2;
            this.vuMeterTab.Text = "VU Meter";
            this.vuMeterTab.UseVisualStyleBackColor = true;
            // 
            // vuMeter1
            // 
            this.vuMeter1.Location = new System.Drawing.Point(51, 91);
            this.vuMeter1.Name = "vuMeter1";
            this.vuMeter1.Size = new System.Drawing.Size(130, 160);
            this.vuMeter1.TabIndex = 0;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(84, 366);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(72, 31);
            this.button4.TabIndex = 12;
            this.button4.Text = "Close";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 397);
            this.ControlBox = false;
            this.Controls.Add(this.button4);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.Text = "Settings Dialog";
            ((System.ComponentModel.ISupportInitialize)(this.defaultPeakLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peakWarningThreshold)).EndInit();
            this.tabs.ResumeLayout(false);
            this.audioPage.ResumeLayout(false);
            this.audioPage.PerformLayout();
            this.displayPage.ResumeLayout(false);
            this.colors.ResumeLayout(false);
            this.colors.PerformLayout();
            this.font.ResumeLayout(false);
            this.font.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.vuMeterTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar defaultPeakLevel;
        private System.Windows.Forms.TrackBar peakWarningThreshold;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton oneAudioChannel;
        private System.Windows.Forms.RadioButton twoAudioChannels;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox inputDevices;
        private System.Windows.Forms.ComboBox outputDevices;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage audioPage;
        private System.Windows.Forms.TabPage displayPage;
        private System.Windows.Forms.GroupBox colors;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox font;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabPage vuMeterTab;
        private VuMeter vuMeter1;
        private System.Windows.Forms.Button button4;

    }
}