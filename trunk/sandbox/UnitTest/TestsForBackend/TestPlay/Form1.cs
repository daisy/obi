using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
 using VirtualAudioBackend ;
using Microsoft.DirectX ;
using Microsoft.DirectX.DirectSound ;

namespace TestPlay
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnPause;
		private System.Windows.Forms.Button btnResume;
		private System.Windows.Forms.Button btnGepPosition;
		private System.Windows.Forms.TextBox txtPosition;
		private System.Windows.Forms.Button btnSetPosition;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button btnAddClip;
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
			this.btnPlay = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnPause = new System.Windows.Forms.Button();
			this.btnResume = new System.Windows.Forms.Button();
			this.btnGepPosition = new System.Windows.Forms.Button();
			this.txtPosition = new System.Windows.Forms.TextBox();
			this.btnSetPosition = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.btnAddClip = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnPlay
			// 
			this.btnPlay.Location = new System.Drawing.Point(0, 0);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.TabIndex = 0;
			this.btnPlay.Text = "Play";
			this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(80, 0);
			this.btnStop.Name = "btnStop";
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop";
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnPause
			// 
			this.btnPause.Location = new System.Drawing.Point(0, 24);
			this.btnPause.Name = "btnPause";
			this.btnPause.TabIndex = 2;
			this.btnPause.Text = "Pause";
			this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
			// 
			// btnResume
			// 
			this.btnResume.Location = new System.Drawing.Point(80, 24);
			this.btnResume.Name = "btnResume";
			this.btnResume.TabIndex = 3;
			this.btnResume.Text = "Resume";
			this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
			// 
			// btnGepPosition
			// 
			this.btnGepPosition.Location = new System.Drawing.Point(0, 48);
			this.btnGepPosition.Name = "btnGepPosition";
			this.btnGepPosition.TabIndex = 4;
			this.btnGepPosition.Text = "GetPosition";
			this.btnGepPosition.Click += new System.EventHandler(this.btnGepPosition_Click);
			// 
			// txtPosition
			// 
			this.txtPosition.Location = new System.Drawing.Point(0, 72);
			this.txtPosition.Name = "txtPosition";
			this.txtPosition.TabIndex = 5;
			this.txtPosition.Text = "";
			// 
			// btnSetPosition
			// 
			this.btnSetPosition.Location = new System.Drawing.Point(0, 96);
			this.btnSetPosition.Name = "btnSetPosition";
			this.btnSetPosition.TabIndex = 6;
			this.btnSetPosition.Text = "SetPosition";
			this.btnSetPosition.Click += new System.EventHandler(this.btnSetPosition_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(16, 136);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(208, 20);
			this.textBox1.TabIndex = 7;
			this.textBox1.Text = "";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(24, 176);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(208, 20);
			this.textBox2.TabIndex = 8;
			this.textBox2.Text = "";
			// 
			// btnAddClip
			// 
			this.btnAddClip.Location = new System.Drawing.Point(136, 96);
			this.btnAddClip.Name = "btnAddClip";
			this.btnAddClip.TabIndex = 9;
			this.btnAddClip.Text = "Add Clip";
			this.btnAddClip.Click += new System.EventHandler(this.btnOpen_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.btnAddClip);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btnSetPosition);
			this.Controls.Add(this.txtPosition);
			this.Controls.Add(this.btnGepPosition);
			this.Controls.Add(this.btnResume);
			this.Controls.Add(this.btnPause);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnPlay);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion
AudioPlayer ap = new AudioPlayer () ;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
AudioMediaAsset am ;
		private void btnPlay_Click(object sender, System.EventArgs e)
		{
			/*
OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "wave files(*.wav|*.wav";
			ofd.ShowDialog();
textBox1.Text = ofd.FileName;
			textBox2.Text = ofd.FileName;

			//AudioClip		 ob_Clip = new AudioClip (textBox1.Text) ;
			
			AudioMediaAsset am = new AudioMediaAsset (ob_Clip.Channels ,  ob_Clip.BitDepth , ob_Clip.SampleRate) ;
			

am.AddClip (ob_Clip) ;

AudioClip		 ob_Clip1 = new AudioClip (textBox2.Text) ;
am.AddClip (ob_Clip1) ;

			AudioClip		 ob_Clip2 = new AudioClip ("F:\\My Documents\\Project\\0.wav" , 0, 5000) ;
			am.AddClip (ob_Clip2) ;
*/

VuMeter ob_VuMeter = new VuMeter () ;
ob_VuMeter.ScaleFactor = 2 ;
			ob_VuMeter.SampleTimeLength = 2000 ;
			ob_VuMeter.LowerThreshold = 10 ;
			ob_VuMeter.UpperThreshold = 80 ;
ob_VuMeter.ShowForm () ;
ap.VuMeterObject = ob_VuMeter ;

			Microsoft.DirectX.DirectSound.Device dSound = new  Microsoft.DirectX.DirectSound.Device ();
		
			dSound.SetCooperativeLevel(this, CooperativeLevel.Priority);
			ap.OutputDevice = dSound ;
ap.CompFactor =  1 ;
			ap.Play (am) ;
//MessageBox.Show ("Done") ;
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
		ap.Stop () ;
		}

		private void btnPause_Click(object sender, System.EventArgs e)
		{
		ap.Pause () ;
		}

		private void btnResume_Click(object sender, System.EventArgs e)
		{
		ap.Resume () ;
		}

		private void btnGepPosition_Click(object sender, System.EventArgs e)
		{
		txtPosition.Text = ap.CurrentTimePosition.ToString () ;
		}

		private void btnSetPosition_Click(object sender, System.EventArgs e)
		{
		ap.CurrentTimePosition = Convert.ToDouble (txtPosition.Text) ;
		}

		private void btnOpen_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "wave files(*.wav|*.wav";
			ofd.ShowDialog();
AudioClip ob_AudioClip  ;
			
				if (textBox1.Text == "" && textBox2.Text == "")
ob_AudioClip = new AudioClip( ofd.FileName) ;
				else
			ob_AudioClip = new AudioClip( ofd.FileName, Convert.ToDouble(textBox1.Text ) , Convert.ToDouble (textBox2.Text )) ;
				
			
if (am == null)
			am = new AudioMediaAsset (ob_AudioClip.Channels , ob_AudioClip.BitDepth , ob_AudioClip.SampleRate) ;

am.AddClip (ob_AudioClip) ;
textBox1.Clear			 ();
			textBox2.Clear ();
			

			
			ofd.Dispose();
/*
			ofd.ShowDialog();
			textBox2.Text = ofd.FileName;
			*/
		}
	}
}
