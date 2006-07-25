using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using VirtualAudioBackend ;
using Commands ;
using Microsoft.DirectX ;
using Microsoft.DirectX.DirectSound ;

namespace TestAudioMediaAsset
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnInsert;
		private System.Windows.Forms.Button btnDetectPhrases;
		private System.Windows.Forms.ListBox lbDisplay;
		private System.Windows.Forms.Button btnGetChunk;
		private System.Windows.Forms.Button btnSplit;
		private System.Windows.Forms.Button btnMerge;
		private System.Windows.Forms.Button btnDeleteAsset;
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
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnInsert = new System.Windows.Forms.Button();
			this.btnDetectPhrases = new System.Windows.Forms.Button();
			this.lbDisplay = new System.Windows.Forms.ListBox();
			this.btnGetChunk = new System.Windows.Forms.Button();
			this.btnSplit = new System.Windows.Forms.Button();
			this.btnMerge = new System.Windows.Forms.Button();
			this.btnDeleteAsset = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(0, 0);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.TabIndex = 0;
			this.btnDelete.Text = "Delete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnInsert
			// 
			this.btnInsert.Location = new System.Drawing.Point(0, 24);
			this.btnInsert.Name = "btnInsert";
			this.btnInsert.TabIndex = 1;
			this.btnInsert.Text = "Insert";
			this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
			// 
			// btnDetectPhrases
			// 
			this.btnDetectPhrases.Location = new System.Drawing.Point(0, 48);
			this.btnDetectPhrases.Name = "btnDetectPhrases";
			this.btnDetectPhrases.TabIndex = 2;
			this.btnDetectPhrases.Text = "Detect Phrases";
			this.btnDetectPhrases.Click += new System.EventHandler(this.btnDetectPhrases_Click);
			// 
			// lbDisplay
			// 
			this.lbDisplay.Location = new System.Drawing.Point(8, 120);
			this.lbDisplay.Name = "lbDisplay";
			this.lbDisplay.Size = new System.Drawing.Size(120, 199);
			this.lbDisplay.TabIndex = 3;
			// 
			// btnGetChunk
			// 
			this.btnGetChunk.Location = new System.Drawing.Point(80, 0);
			this.btnGetChunk.Name = "btnGetChunk";
			this.btnGetChunk.TabIndex = 4;
			this.btnGetChunk.Text = "GetChunk";
			this.btnGetChunk.Click += new System.EventHandler(this.btnGetChunk_Click);
			// 
			// btnSplit
			// 
			this.btnSplit.Location = new System.Drawing.Point(80, 32);
			this.btnSplit.Name = "btnSplit";
			this.btnSplit.TabIndex = 5;
			this.btnSplit.Text = "Split";
			this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
			// 
			// btnMerge
			// 
			this.btnMerge.Location = new System.Drawing.Point(80, 56);
			this.btnMerge.Name = "btnMerge";
			this.btnMerge.TabIndex = 6;
			this.btnMerge.Text = "Merge";
			this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
			// 
			// btnDeleteAsset
			// 
			this.btnDeleteAsset.Location = new System.Drawing.Point(80, 80);
			this.btnDeleteAsset.Name = "btnDeleteAsset";
			this.btnDeleteAsset.TabIndex = 7;
			this.btnDeleteAsset.Text = "DeleteAsset";
			this.btnDeleteAsset.Click += new System.EventHandler(this.btnDeleteAsset_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.btnDeleteAsset);
			this.Controls.Add(this.btnMerge);
			this.Controls.Add(this.btnSplit);
			this.Controls.Add(this.btnGetChunk);
			this.Controls.Add(this.lbDisplay);
			this.Controls.Add(this.btnDetectPhrases);
			this.Controls.Add(this.btnInsert);
			this.Controls.Add(this.btnDelete);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion
AssetManager a = new AssetManager ("c:\\atest") ;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
				Application.Run(new Form1());
		}

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
CreateAsset () ;

double BeginTime = 00 ;
double EndTime = 8000	;
MessageBox.Show ("before delete") ;
DeleteAudioDataCommand  ComDelete = new DeleteAudioDataCommand (am0, BeginTime , EndTime ) ;
			
ComDelete.Do () ;
			
			//ComDelete.Undo () ;
//AudioMediaAsset am1 =am0.DeleteChunk (BeginTime , EndTime ) as AudioMediaAsset;
Display (am0) ;
MessageBox.Show ("Deleted Asset"+ am0.LengthInMilliseconds.ToString ()) ;
		}

		private void btnInsert_Click(object sender, System.EventArgs e)
		{
		
			AudioClip  ob_AudioClip 		 = new AudioClip ( "c:\\atest\\a\\a.wav" , 3000 , 9000) ;
			AudioMediaAsset am = new AudioMediaAsset ( ob_AudioClip.Channels , ob_AudioClip.BitDepth , ob_AudioClip.SampleRate) ;
			am.AddClip (ob_AudioClip) ;
			
			AudioClip  ob_AudioClip1 		 = new AudioClip ( "c:\\atest\\a\\a.wav" , 3000 , 6000) ;
			am.AddClip (ob_AudioClip1) ;

			AudioClip  ob_AudioClip2		 = new AudioClip ( "c:\\atest\\a\\a.wav" , 6000 , 9000) ;
			am.AddClip (ob_AudioClip2) ;

			//AudioClip  ob_AudioClip3 	 = new AudioClip ( "c:\\atest\\a\\Alpha.wav" , 9000 , 12000) ;
			//am.AddClip (ob_AudioClip3) ;

			double BeginTime = 3000 ;
			double EndTime = 8000 ;
			double InsertTime = 3000;
MessageBox.Show (" ") ;
CreateAsset () ;

InsertAudioAssetCommand ComInsert = new InsertAudioAssetCommand (am0, am , InsertTime ) ;
ComInsert.Do () ; 
//ComInsert.Undo () ;
			//am.InsertAsset (am0 , InsertTime);
			//am.InsertAsset (am.GetChunk(BeginTime , EndTime ) , InsertTime);
Display (am) ;
			MessageBox.Show (am.LengthInMilliseconds.ToString ()) ;
		
		}

		private void btnDetectPhrases_Click(object sender, System.EventArgs e)
		{
			AudioClip ob_AudioClip = new AudioClip ("c:\\s\\sil22-8-2.wav" , 0 , 2000) ;
			AudioMediaAsset am = new AudioMediaAsset  (ob_AudioClip.Channels , ob_AudioClip.BitDepth , ob_AudioClip.SampleRate ) ;
			am.AddClip (ob_AudioClip) ;
			long Sil = am.GetSilenceAmplitude (am);
			Sil = Sil + 3 ;
			MessageBox.Show (Sil.ToString ()) ;

ArrayList al = new ArrayList () ;
//AudioClip ob_AudioClip1 = new AudioClip ("c:\\s\\Detect22-8-2-new.wav" ) ;
//al.Add (ob_AudioClip1 ) ;
		
						//al.Add(ob_AudioClip) ;
AudioClip ob_AudioClip2 = new AudioClip ("c:\\s\\Detect22-8-2.wav" ) ;
al.Add (ob_AudioClip2 ) ;
			AudioMediaAsset amd = new AudioMediaAsset (al) ;
			a.AddAsset (amd) ;
			double dLength = 500 ;
double dBefore = 100 ;
//ArrayList alAsset =  amd.ApplyPhraseDetection (Sil , dLength , dBefore) ;
PhraseDetectionCommand ob_PhraseDetectionCommand = new PhraseDetectionCommand ( amd , Sil , dLength , dBefore) ;
ob_PhraseDetectionCommand.Do () ; 
ArrayList alAsset  = ob_PhraseDetectionCommand.AssetList ;
			am0 = alAsset   [1] as AudioMediaAsset ;
//MessageBox.Show (alAsset.Count.ToString ()) ;
MessageBox.Show ("Done") ;
AudioMediaAsset DisplayAsset ;
			for (int i = 0 ; i< alAsset.Count ;i++)
			{
DisplayAsset = alAsset  [i] as AudioMediaAsset ;
MessageBox.Show (DisplayAsset.LengthInMilliseconds.ToString ()) ;
			}
		}

		private void btnGetChunk_Click(object sender, System.EventArgs e)
		{
CreateAsset () ;
			//Display  (am0) ;


			double BeginTime = 4850 ;
			double EndTime = 7400 ;
			AudioMediaAsset am1 = am0.GetChunk(BeginTime , EndTime ) as AudioMediaAsset;		

			Display  (am1) ;
			MessageBox.Show (am1.LengthInMilliseconds.ToString () );

//			Display  (am0) ;
//MessageBox.Show (am0.LengthInMilliseconds.ToString () );
		}
AudioMediaAsset am0 ;
		void CreateAsset ()
		{
			ArrayList al = new ArrayList () ;
			AudioClip  ob_AudioClip 		 = new AudioClip ( "c:\\atest\\a\\A1.wav" , 0 , 8000) ;
al.Add (ob_AudioClip) ;
			AudioClip  ob_AudioClip1 		 = new AudioClip ( "c:\\atest\\a\\b1.wav" , 2000 , 9000) ;
al.Add (ob_AudioClip1) ;
			AudioClip  ob_AudioClip2		 = new AudioClip ( "c:\\atest\\a\\c1.wav" , 3000 , 10000) ;
			al.Add (ob_AudioClip2) ;

			AudioClip  ob_AudioClip3 	 = new AudioClip ( "c:\\atest\\a\\d1.wav" , 2000 , 12000) ;
al.Add (ob_AudioClip3) ;

am0 = new AudioMediaAsset (al) ;
			
		}

		void Display (AudioMediaAsset Asset )
		{
lbDisplay.Items.Clear () ;
string sTemp ;
AudioClip DisplayClip ;
			for (int i = 0 ; i< Asset.m_alClipList.Count; i++)
			{
DisplayClip = Asset.m_alClipList [i] as AudioClip ;
				sTemp = DisplayClip.Path+ "-    " +DisplayClip.BeginTime + "-    " + DisplayClip.EndTime ;
				lbDisplay.Items.Add (sTemp) ;
			}
		}

		private void btnSplit_Click(object sender, System.EventArgs e)
		{
CreateAsset () ;		
double dTime = 10000 ;
SplitAudioAssetCommand ComSplit = new SplitAudioAssetCommand  (am0 , dTime) ;
MessageBox.Show(am0.LengthInMilliseconds.ToString ()) ;
			ComSplit.Do () ;
			//ComSplit.Undo () ;
			MessageBox.Show(am0.LengthInMilliseconds.ToString ()) ;

Display (am0) ;
MessageBox.Show ("Done") ;
		}

		private void btnMerge_Click(object sender, System.EventArgs e)
		{
		CreateAsset () ;
AudioMediaAsset am1 = am0.Copy () as AudioMediaAsset;

MergeAudioAssetsCommand ComMerge = new MergeAudioAssetsCommand (am0, am1) ;
			ComMerge.Do ();
			//ComMerge.Undo () ;
MessageBox.Show (am0.LengthInMilliseconds.ToString ()  ) ;
		Display(am0) ;

		}

		private void btnDeleteAsset_Click(object sender, System.EventArgs e)
		{
		AudioClip Clip1 = new AudioClip ("c:\\atest\\a\\Delete.wav" , 0 , 1000) ;
			AudioClip Clip2 = new AudioClip ("c:\\atest\\a\\Delete.wav" , 1000 , 2000) ;
			AudioMediaAsset amdelete = new AudioMediaAsset (Clip1.Channels ,  Clip1.BitDepth , Clip1.SampleRate) ;
			amdelete.AddClip (Clip1) ;
			amdelete.AddClip (Clip2) ;
//amdelete.Delete () ;
			MessageBox.Show ("done") ;
		}


		
		

			

			

			


			
		
			

			

			
		

		

		

	}
}
