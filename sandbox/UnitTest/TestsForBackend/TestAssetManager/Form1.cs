using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using VirtualAudioBackend ;

namespace TestAssetManager
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lbHashTable;
		private System.Windows.Forms.ListBox lbExists;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnRename;
		private System.Windows.Forms.Button btnGetAssetList;
		private System.Windows.Forms.Button btnCopy;
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
			this.lbHashTable = new System.Windows.Forms.ListBox();
			this.lbExists = new System.Windows.Forms.ListBox();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnRename = new System.Windows.Forms.Button();
			this.btnGetAssetList = new System.Windows.Forms.Button();
			this.btnCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbHashTable
			// 
			this.lbHashTable.Location = new System.Drawing.Point(0, 0);
			this.lbHashTable.Name = "lbHashTable";
			this.lbHashTable.Size = new System.Drawing.Size(120, 95);
			this.lbHashTable.TabIndex = 0;
			// 
			// lbExists
			// 
			this.lbExists.Location = new System.Drawing.Point(120, 0);
			this.lbExists.Name = "lbExists";
			this.lbExists.Size = new System.Drawing.Size(120, 95);
			this.lbExists.TabIndex = 1;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(0, 104);
			this.btnStart.Name = "btnStart";
			this.btnStart.TabIndex = 2;
			this.btnStart.Text = "Start";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Location = new System.Drawing.Point(0, 128);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 3;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(8, 152);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.TabIndex = 4;
			this.btnDelete.Text = "Delete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnRename
			// 
			this.btnRename.Location = new System.Drawing.Point(0, 176);
			this.btnRename.Name = "btnRename";
			this.btnRename.TabIndex = 5;
			this.btnRename.Text = "Rename";
			this.btnRename.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnGetAssetList
			// 
			this.btnGetAssetList.Location = new System.Drawing.Point(0, 200);
			this.btnGetAssetList.Name = "btnGetAssetList";
			this.btnGetAssetList.TabIndex = 6;
			this.btnGetAssetList.Text = "GetAssetList";
			this.btnGetAssetList.Click += new System.EventHandler(this.button2_Click);
			// 
			// btnCopy
			// 
			this.btnCopy.Location = new System.Drawing.Point(0, 224);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.TabIndex = 7;
			this.btnCopy.Text = "Copy";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.btnCopy);
			this.Controls.Add(this.btnGetAssetList);
			this.Controls.Add(this.btnRename);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.lbExists);
			this.Controls.Add(this.lbHashTable);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion
AssetManager am = new AssetManager ("c:\\atest\\a") ;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnStart_Click(object sender, System.EventArgs e)
		{
AudioClip ob_Clip = new AudioClip ("c:\\atest\\a\\Num.wav" , 100 , 3000) ;
			AudioClip ob_Clip1 = new AudioClip ("c:\\atest\\a\\Alpha.wav" , 100 , 3000) ;
ArrayList al = new ArrayList () ;
al.Add (ob_Clip) ;
			al.Add (ob_Clip1) ;
am.NewAudioMediaAsset (2 , 8 , 22050) ;		
am.NewAudioMediaAsset (al) ;
			am.NewAudioMediaAsset (2 , 8 , 22050) ;		
			MessageBox.Show (" ") ;
AudioMediaAsset Asset = new AudioMediaAsset (1,16,44100) ;
//Asset.Name = "Added" ;
am.AddAsset (Asset) ;
			
AddList (am.Assets, am.m_htExists) ;
MessageBox.Show ("Done") ;
		}


		void AddList (Hashtable hashtable, Hashtable Exists)
		{
lbExists.Items.Clear () ;
			lbHashTable.Items.Clear () ;
			IDictionaryEnumerator mEnumerator = hashtable.GetEnumerator();
			while(mEnumerator.MoveNext())
			{
				lbHashTable.Items.Add (mEnumerator .Key) ; 
			}
					
			IDictionaryEnumerator en = Exists.GetEnumerator();
			while(en.MoveNext())
			{
				lbExists.Items.Add (en.Key) ; 
			}
					
			
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
AudioMediaAsset a = am.GetAsset("amMediaAsset2") as AudioMediaAsset ;
			MessageBox.Show (a.Name) ;
		am.RemoveAsset (a) ;
AddList (am.Assets , am.m_htExists) ;
MessageBox.Show ("Removed") ;
		}

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			AudioMediaAsset a = am.GetAsset("amMediaAsset2") as AudioMediaAsset ;
			MessageBox.Show (a.Name) ;
			am.DeleteAsset(a) ;
			AddList (am.Assets , am.m_htExists) ;
			MessageBox.Show ("Deleted") ;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			AudioMediaAsset a = am.GetAsset("amMediaAsset2") as AudioMediaAsset ;
			MessageBox.Show (a.Name) ;
			am.RenameAsset(a, "Name") ;
			AddList (am.Assets , am.m_htExists) ;
			MessageBox.Show ("Renamed") ;
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
		Hashtable h = am.GetAssets (MediaType.Audio) ;

			IDictionaryEnumerator en = h.GetEnumerator () ;

			while (en.MoveNext() )
			{
MessageBox.Show(en.Key.ToString ()) ;
			}
MessageBox.Show("Done") ;
		}

		private void btnCopy_Click(object sender, System.EventArgs e)
		{

					AudioMediaAsset a = am.GetAsset("amMediaAsset3") as AudioMediaAsset ;
			a.Name = null ;
MessageBox.Show (a.Manager.ToString()) ;
am.CopyAsset (a) ;
						AddList (am.Assets , am.m_htExists) ;
MessageBox.Show ("Asset copied") ;

		}
	}
}
