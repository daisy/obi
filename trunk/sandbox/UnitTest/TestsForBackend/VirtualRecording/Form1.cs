using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using VirtualAudioBackend;

namespace VirtualRecording
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
	ArrayList mDev = new ArrayList();
	AudioRecorder ar = new AudioRecorder();
	VuMeter ob_VuMeter = new VuMeter () ;
		
		private System.Windows.Forms.ComboBox comboDevice;
		private System.Windows.Forms.Button btnSetDevice;
		private System.Windows.Forms.Button btnRecord;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnListen;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboDevice = new System.Windows.Forms.ComboBox();
			this.btnSetDevice = new System.Windows.Forms.Button();
			this.btnRecord = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnListen = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboDevice
			// 
			this.comboDevice.Location = new System.Drawing.Point(16, 16);
			this.comboDevice.Name = "comboDevice";
			this.comboDevice.Size = new System.Drawing.Size(240, 21);
			this.comboDevice.TabIndex = 1;
			// 
			// btnSetDevice
			// 
			this.btnSetDevice.Location = new System.Drawing.Point(24, 112);
			this.btnSetDevice.Name = "btnSetDevice";
			this.btnSetDevice.TabIndex = 2;
			this.btnSetDevice.Text = "Set Device";
			this.btnSetDevice.Click += new System.EventHandler(this.btnSetDevice_Click);
			// 
			// btnRecord
			// 
			this.btnRecord.Location = new System.Drawing.Point(32, 168);
			this.btnRecord.Name = "btnRecord";
			this.btnRecord.TabIndex = 3;
			this.btnRecord.Text = "Record";
			this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(40, 208);
			this.btnStop.Name = "btnStop";
			this.btnStop.TabIndex = 4;
			this.btnStop.Text = "Stop";
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnListen
			// 
			this.btnListen.Location = new System.Drawing.Point(176, 120);
			this.btnListen.Name = "btnListen";
			this.btnListen.TabIndex = 5;
			this.btnListen.Text = "Listen";
			this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 269);
			this.Controls.Add(this.btnListen);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnRecord);
			this.Controls.Add(this.btnSetDevice);
			this.Controls.Add(this.comboDevice);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			mDev = ar.GetInputDevices();
			comboDevice.DataSource = mDev;
		}

		private void btnSetDevice_Click(object sender, System.EventArgs e)
		{
			ar.InitDirectSound(comboDevice.SelectedIndex);
		}

		private void btnRecord_Click(object sender, System.EventArgs e)
		{
			ob_VuMeter.ScaleFactor = 2;
			ob_VuMeter.SampleTimeLength = 2000 ;
			ob_VuMeter.UpperThreshold = 120;
			ob_VuMeter.LowerThreshold = 100;
			// Displays the VuMeter form
			ob_VuMeter.ShowForm () ;
			// assigns the VuMeter object to AudioPlayer property as it is routed through AudioPlayer for integrating it with AudioPlayer
			ar.VuMeterObject = ob_VuMeter ;			
			try
			{
				AssetManager aManager = new AssetManager("C:\\Project");
				AudioMediaAsset am = new AudioMediaAsset(2, 16, 44100);
				aManager.AddAsset(am);
				ar.StartRecording(am);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
			try
			{
				ar.StopRecording();
			}
			catch(Exception ext)
			{
				MessageBox.Show(ext.ToString());
			}
		}

		


		private void btnListen_Click(object sender, System.EventArgs e)
		{
			ob_VuMeter.ScaleFactor = 2;
			ob_VuMeter.SampleTimeLength = 2000 ;
ob_VuMeter.UpperThreshold = 150;
			ob_VuMeter.LowerThreshold = 100;
			// Displays the VuMeter form
			ob_VuMeter.ShowForm () ;
			

			// assigns the VuMeter object to AudioPlayer property as it is routed through AudioPlayer for integrating it with AudioPlayer
			ar.VuMeterObject = ob_VuMeter ;			
			try
			{
				AssetManager aManager = new AssetManager("C:\\Project");
				AudioMediaAsset am2 = new AudioMediaAsset(2, 16, 22050);
				aManager.AddAsset(am2);
ar.StartListening(am2);
			}
			catch(Exception exl)
			{
				MessageBox.Show(exl.ToString());
			}
		}
	}
}
