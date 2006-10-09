using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.AudioVideoPlayback;

namespace DirectS1
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Button btnPlayBack;
		private System.Windows.Forms.TextBox txtVal;
		private System.Windows.Forms.Button btnGetTime;
		private System.Windows.Forms.Button btnJumpTime;
		private System.Windows.Forms.Button btnStopPlayBack;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog2;
		public System.Windows.Forms.TextBox txtStep;
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
			this.btnPlayBack = new System.Windows.Forms.Button();
			this.txtVal = new System.Windows.Forms.TextBox();
			this.btnGetTime = new System.Windows.Forms.Button();
			this.btnJumpTime = new System.Windows.Forms.Button();
			this.btnStopPlayBack = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
			this.txtStep = new System.Windows.Forms.TextBox();
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
			// btnPlayBack
			// 
			this.btnPlayBack.Location = new System.Drawing.Point(8, 32);
			this.btnPlayBack.Name = "btnPlayBack";
			this.btnPlayBack.TabIndex = 1;
			this.btnPlayBack.Text = "PlayBack";
			this.btnPlayBack.Click += new System.EventHandler(this.btnPlayBack_Click);
			// 
			// txtVal
			// 
			this.txtVal.Location = new System.Drawing.Point(104, 56);
			this.txtVal.Name = "txtVal";
			this.txtVal.TabIndex = 2;
			this.txtVal.Text = "textBox1";
			// 
			// btnGetTime
			// 
			this.btnGetTime.Location = new System.Drawing.Point(8, 56);
			this.btnGetTime.Name = "btnGetTime";
			this.btnGetTime.TabIndex = 2;
			this.btnGetTime.Text = "Get time";
			this.btnGetTime.Click += new System.EventHandler(this.btnVal_Click);
			// 
			// btnJumpTime
			// 
			this.btnJumpTime.Location = new System.Drawing.Point(8, 80);
			this.btnJumpTime.Name = "btnJumpTime";
			this.btnJumpTime.TabIndex = 4;
			this.btnJumpTime.Text = "Jump to time";
			this.btnJumpTime.Click += new System.EventHandler(this.btnJumpTime_Click);
			// 
			// btnStopPlayBack
			// 
			this.btnStopPlayBack.Location = new System.Drawing.Point(8, 104);
			this.btnStopPlayBack.Name = "btnStopPlayBack";
			this.btnStopPlayBack.TabIndex = 5;
			this.btnStopPlayBack.Text = "Stop playback";
			this.btnStopPlayBack.Click += new System.EventHandler(this.btnStopPlayBack_Click);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(80, 0);
			this.btnStop.Name = "btnStop";
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop Play";
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "FileName";
			this.openFileDialog1.Title = "Open File";
			// 
			// txtStep
			// 
			this.txtStep.Location = new System.Drawing.Point(104, 80);
			this.txtStep.Name = "txtStep";
			this.txtStep.TabIndex = 4;
			this.txtStep.Text = "";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.txtStep);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnStopPlayBack);
			this.Controls.Add(this.btnJumpTime);
			this.Controls.Add(this.btnGetTime);
			this.Controls.Add(this.txtVal);
			this.Controls.Add(this.btnPlayBack);
			this.Controls.Add(this.btnPlay);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion
		private SecondaryBuffer sound = null;
private Device dSound = null;		
private Audio ourAudio = null ;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnPlay_Click(object sender, System.EventArgs e)
		{
			//SecondaryBuffer sound = null;
//Device dSound = null;		
			try
			{
				dSound = new Device();
				//dSound.SetCooperartiveLevel(this,
				dSound.SetCooperativeLevel(this, CooperativeLevel.Priority);
				
			}
			catch
			{
				MessageBox.Show("Unable to create sound device. Sample will now exit.");
				this.Close();
			}
			//try
			//{
				BufferDescription d = new BufferDescription();
			//}
			//catch
			//{
				//MessageBox.Show("Problem is d description");
			//}
d.ControlPan = true;
			d.ControlVolume = true;
d.ControlFrequency = true;
			d.ControlEffects = true;
			openFileDialog1.ShowDialog();
string SPath = openFileDialog1.FileName ;
			sound = new SecondaryBuffer(SPath, d, dSound);
try
{
	//sound = new SecondaryBuffer(SPath, d, dSound);
	
	sound.Play(0, BufferPlayFlags.Default );

}
			catch
			{
			MessageBox.Show("In play");
			}
			


		
		}

		private void btnPlayBack_Click(object sender, System.EventArgs e)
		{
		string SPathPlayBack ;
openFileDialog2.ShowDialog();
			SPathPlayBack = openFileDialog2.FileName ;		
			ourAudio = new Audio(SPathPlayBack);
ourAudio.Play ();
		}

		private void btnVal_Click(object sender, System.EventArgs e)
		{
		double Seek;
Seek = ourAudio.CurrentPosition ;
		txtVal.Text = Seek.ToString();

		}

		private void btnJumpTime_Click(object sender, System.EventArgs e)
		{
			double Step ;
				//Step = ToDouble ("111");
			Step = 60 ;
		ourAudio.CurrentPosition = Step + ourAudio.CurrentPosition;
		}

		private void btnStopPlayBack_Click(object sender, System.EventArgs e)
		{
		ourAudio.Stop();
		}

		private void btnStop_Click(object sender, System.EventArgs e)
		{
sound.Stop		();
		}
	}
}
