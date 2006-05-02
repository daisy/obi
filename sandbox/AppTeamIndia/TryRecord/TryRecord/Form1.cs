using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
namespace TryRecord
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		ArrayList aFormat =  new ArrayList();
		ArrayList aDevice = new ArrayList();
		WaveFormat InputFormat = new WaveFormat();
		WaveFormat format = new WaveFormat();
		private System.Windows.Forms.Label lblInputFormat; 
		Capture applicationDevice = null;
		

		private System.Windows.Forms.ComboBox comboDevice;
		private System.Windows.Forms.Button btnDevice;
		private System.Windows.Forms.ListBox listSelect;
		private System.Windows.Forms.Button btnFormat;
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
			this.btnDevice = new System.Windows.Forms.Button();
			this.listSelect = new System.Windows.Forms.ListBox();
			this.btnFormat = new System.Windows.Forms.Button();
			this.lblInputFormat = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboDevice
			// 
			this.comboDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDevice.Location = new System.Drawing.Point(16, 24);
			this.comboDevice.Name = "comboDevice";
			this.comboDevice.Size = new System.Drawing.Size(200, 21);
			this.comboDevice.TabIndex = 0;
			// 
			// btnDevice
			// 
			this.btnDevice.Location = new System.Drawing.Point(24, 72);
			this.btnDevice.Name = "btnDevice";
			this.btnDevice.TabIndex = 1;
			this.btnDevice.Text = "Set Device";
			this.btnDevice.Click += new System.EventHandler(this.btnDevice_Click);
			// 
			// listSelect
			// 
			this.listSelect.Location = new System.Drawing.Point(232, 24);
			this.listSelect.Name = "listSelect";
			this.listSelect.Size = new System.Drawing.Size(192, 95);
			this.listSelect.TabIndex = 2;
			this.listSelect.SelectedIndexChanged += new System.EventHandler(this.listSelect_SelectedIndexChanged);
			// 
			// btnFormat
			// 
			this.btnFormat.Location = new System.Drawing.Point(240, 144);
			this.btnFormat.Name = "btnFormat";
			this.btnFormat.TabIndex = 3;
			this.btnFormat.Text = "Set Format";
			this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
			// 
			// lblInputFormat
			// 
			this.lblInputFormat.Location = new System.Drawing.Point(248, 184);
			this.lblInputFormat.Name = "lblInputFormat";
			this.lblInputFormat.TabIndex = 4;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 269);
			this.Controls.Add(this.lblInputFormat);
			this.Controls.Add(this.btnFormat);
			this.Controls.Add(this.listSelect);
			this.Controls.Add(this.btnDevice);
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
Trial tr = new Trial();
			aDevice= tr.GetInputDevice();
			comboDevice.DataSource = aDevice;
			comboDevice.SelectedIndex = 1;
						
			
			aFormat = tr.GetFormatList();
			listSelect.DataSource = aFormat;
}

		private void btnDevice_Click(object sender, System.EventArgs e)
		{
		Trial tr = new Trial();
			Guid g;
			g =tr.SetInputDeviceGuid();
			MessageBox.Show(g.ToString());
applicationDevice = tr.InitDirectSound();			
		}

		private void btnFormat_Click(object sender, System.EventArgs e)
		{
Trial tr = new Trial();
InputFormat = tr.GetInputFormat(listSelect.SelectedIndex);
lblInputFormat.Text = string.Format("{0} Hz, {1}-bit ", 
				InputFormat.SamplesPerSecond, 
				InputFormat.BitsPerSample) +
				((1 == InputFormat.Channels) ? "Mono" : "Stereo");
				MessageBox.Show(InputFormat.ToString());
		}

		private void listSelect_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnFormat.Enabled = true;
		}
	}
	class Trial
	{
		private ArrayList m_aSelect = new ArrayList();
		private ArrayList m_aGuid = new ArrayList();
		private Capture m_cApplicationDevice = null;
		private int m_iCaptureBufferSize = 0;
		private int m_iNotifySize = 0;					
		private Guid m_gCaptureDeviceGuid = Guid.Empty;  
		private bool[] m_bInputFormatSupported = new bool[12];
		private ArrayList m_aformats= new ArrayList();
		
		// returns a list of input devices
		public ArrayList GetInputDevice()
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();  // gathers the available capture devices
			foreach (DeviceInformation info in devices)
			{
				m_aSelect.Add(info.Description);
			}
			return m_aSelect;
		}

		//returns the guid of the selected device, in this case the default 
		//device guid has been returned
		public Guid SetInputDeviceGuid()
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();
			m_aGuid = GetInputDevice();
			m_gCaptureDeviceGuid = devices[1].DriverGuid;
			return m_gCaptureDeviceGuid;
		}
		
		//this return type is capture, where the device guid is used to set a capture which
		//will be later on used to create the capture buffer for recording
		public Capture InitDirectSound()
		{
			m_iCaptureBufferSize = 0;
			m_iNotifySize = 0;
			// Create DirectSound.Capture using the preferred capture device
			m_gCaptureDeviceGuid = SetInputDeviceGuid();
			m_cApplicationDevice = new Capture(m_gCaptureDeviceGuid);
			return m_cApplicationDevice;
		}
		// structure for format information
		private struct FormatInfo
		{
			public WaveFormat format;
			public override string ToString()
			{
				return  ConvertWaveFormatToString(format);
			}
		};
		
		//static method to convert the WaveFormat to string
		private static string ConvertWaveFormatToString(WaveFormat format)
		{
			// Name: ConvertWaveFormatToString()
			// Desc: Converts a wave format to a text string
			return format.SamplesPerSecond + " Hz, " + format.BitsPerSample + "-bit " + 
				((format.Channels == 1) ? "Mono" : "Stereo");
		}

		//Returns 12different wave formats based on Index
		public void GetWaveFormatFromIndex(int m_iIndex, ref WaveFormat m_wFormat)
		{
			int SampleRate = m_iIndex / 4;
			int iType = m_iIndex %4 ;
			switch (SampleRate)
			{
				case 0: m_wFormat.SamplesPerSecond = 11025; break;
				case 1:  m_wFormat.SamplesPerSecond = 22050; break;
				case 2: m_wFormat.SamplesPerSecond = 44100; break;
			}				
			switch (iType)
			{
				case 0: m_wFormat.BitsPerSample =  8; m_wFormat.Channels = 1; break;
				case 1: m_wFormat.BitsPerSample = 16; m_wFormat.Channels = 1; break;
				case 2: m_wFormat.BitsPerSample =  8; m_wFormat.Channels = 2; break;
				case 3: m_wFormat.BitsPerSample = 16; m_wFormat.Channels = 2; break;

			}
			// BlockAlign Retrieves and sets the minimum atomic unit of data, in bytes, for the format type.
			m_wFormat.BlockAlign = (short)(m_wFormat.Channels * (m_wFormat.BitsPerSample / 8)); 
			// AverageBytesPerSecond Retrieves and sets the required average data-transfer rate, in bytes per second, for the format type.			
			m_wFormat.AverageBytesPerSecond = m_wFormat.BlockAlign * m_wFormat.SamplesPerSecond;
		}
		//collects the list of all the formats in an ArrayList and returns the ArrayList
		public ArrayList GetFormatList()
		{
			FormatInfo  info			= new FormatInfo();			
			WaveFormat	m_wFormat			= new WaveFormat();
			for (int m_iIndex = 0; m_iIndex < m_bInputFormatSupported.Length; m_iIndex++)
			{
				//Turn the m_iIndex into a WaveFormat 
				GetWaveFormatFromIndex(m_iIndex, ref m_wFormat);
				info.format = m_wFormat;
				m_aformats.Add(info);
			}
			return m_aformats;
		}
		//it returns a selected format on the basis of the selected index
		public WaveFormat GetInputFormat(int m_iIndex)
		{
			WaveFormat m_wFormat = new WaveFormat();
			FormatInfo Info = new FormatInfo();
			m_aformats= GetFormatList();
			GetWaveFormatFromIndex(m_iIndex, ref m_wFormat);
			Info.format = m_wFormat;
			return Info.format;
		}
		


	}
}
