using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.AudioVideoPlayback;
using Microsoft.DirectX.DirectInput ;

using UrakawaApplicationBackend.events.audioPlayerEvents;
using UrakawaApplicationBackend.events.vuMeterEvents ;

namespace UrakawaApplicationBackend
{
	public class AudioPlayer : IAudioPlayer
	{
	
		// declare member variables
		private IAudioMediaAsset m_Asset ;
		private SecondaryBuffer SoundBuffer;
		private Microsoft.DirectX.DirectSound.Device 			 SndDevice= null ;
		private BufferDescription BufferDesc = null ;
		private FileStream fs ;		private int m_BufferCheck ;
		private int m_SizeBuffer ;
		private 		int m_RefreshLength ;
		private 		long m_lLength;
		private 		long m_lPlayed ;
		Thread  RefreshThread ;
		
		private 		byte [] 	ByteBuffer ;
		private 		bool m_PlayFile ;
		byte [] RefreshArray;
		private 		long m_lArrayPosition;
		private 		bool m_FastPlay ;
		private 		int m_Step ;
		internal int m_FrameSize ;
		internal int m_Channels ;
		private 		int m_SamplingRate ;
		private AudioPlayerState  m_State;

		//static counter to implement singleton
		private static int m_ConstructorCounter =0;

// constructor 
		public 		AudioPlayer ()
		{
			if (m_ConstructorCounter == 0)
			{
				m_ConstructorCounter++ ;
				m_State= AudioPlayerState .stopped ;
				m_PlayFile = true ;
				m_FastPlay = false ;
				
AssociateEvents  () ;
				
			}
			else
			{
				throw new Exception("This class is Singleton");
				
			}
		}

		


// Create objects for triggering events
//StateChanged  ob_StateChanged = new StateChanged   ( AudioPlayerState.stopped) ;
		
EndOfAudioAsset  ob_EndOfAudioAsset  = new EndOfAudioAsset () ;
EndOfAudioBuffer ob_EndOfAudioBuffer = new EndOfAudioBuffer () ;
		UpdateVuMeter ob_UpdateVuMeter = new UpdateVuMeter () ;

// create objects for handling events
CatchEvents ob_CatchEvents = new CatchEvents () ;
VuMeter ob_VuMeter ;


// bool variable to enable or disable event
bool m_EventsEnabled = true ;
		void AssociateEvents ()
		{
//ob_StateChanged.StateChangedEvent+=new DStateChangedEvent (ob_CatchEvents.CatchStateChangedEvent) ;
ob_EndOfAudioAsset.EndOfAudioAssetEvent+=new DEndOfAudioAssetEvent(ob_CatchEvents.CatchEndOfAudioEvent) ;
ob_EndOfAudioBuffer.EndOfAudioBufferEvent+=new DEndOfAudioBufferEvent  (ob_CatchEvents.CatchEndOfAudioEvent) ;
			
		}
			
// Set VuMeter object
		public 		void SetVuMeterObject ( VuMeter ob_VuMeterArg )
		{
			ob_VuMeter = ob_VuMeterArg ;
			ob_UpdateVuMeter.UpdateVuMeterEvent += new DUpdateVuMeterEvent (ob_VuMeter.CatchUpdateVuMeterEvent ) ;	
			
		}

		public VuMeter VuMeterObject
		{
			get
			{
return ob_VuMeter ;
			}
			set
			{
SetVuMeterObject (value) ;
			}
		}


		void TriggerStateChangedEvent ()
		{

		}

void TriggerStateChangedEvent ( StateChanged ob)
{
			if (m_EventsEnabled == true)
			{
				ob.StateChangedEvent+=new DStateChangedEvent (ob_CatchEvents.CatchStateChangedEvent) ;

				ob.NotifyStateChanged ( this, ob) ;
			}
		}

		

// array for update current amplitude to VuMeter
		internal byte [] arUpdateVM ;
internal int m_UpdateVMArrayLength ;

// gets the current AudioPlayer state
		public AudioPlayerState State
		{
			get
			{
				return m_State ;
			}
		}
		
// Output  device object
		public Microsoft.DirectX.DirectSound.Device 			   OutputDevice
		{
			get
			{
				return SndDevice ;
			}
			set
			{
				SndDevice = value ;
			}
		}

		public IAudioMediaAsset CurrentAsset
		{
			get
			{
				return m_Asset ;
			}
		}

		public int CompFactor
		{
			get
			{
return m_Step ;
			}
			set
			{
Set_m_Step (value) ;
			}
		}

		public long CurrentBytePosition
		{
			get
			{
				return GetCurrentBytePosition();
			}
			set
			{
				SetCurrentBytePosition (value) ;
			}
		}

		public double CurrentTimePosition
		{
			get
			{
				return GetCurrentTimePosition() ;
			}
			set
			{
				SetCurrentTimePosition (value) ;
			}
		}

// checks the input value of compression factor and sets it for fast play
		// Default value  is 10 i.e. 80% time compression
		/// </summary>
		/// <param name="l_Step"></param>
		void Set_m_Step(int l_Step)
		{
			if (l_Step == 1)
			{
m_FastPlay = false ;
			}
			else if (l_Step >2&& l_Step <20)
			{
				m_FastPlay = true ;
				m_Step = l_Step ;
			}
			else
			{
MessageBox.Show("Invalid compression factor") ;
			}
		}

		public ArrayList GetOutputDevices()
		{
			CollectOutputDevices() ;
			ArrayList OutputDevices = new ArrayList ();

for(int i = 0; i < devList.Count; i++) 
{
	OutputDevices.Add (devList[i].Description);
	
}
			return OutputDevices ;
		}


		public void SetDevice (Control FormHandle, int Index)
		{
Microsoft.DirectX.DirectSound.Device dSound = new  Microsoft.DirectX.DirectSound.Device ( devList [Index].DriverGuid);
			dSound.SetCooperativeLevel(FormHandle, CooperativeLevel.Priority);
			SndDevice  = dSound ;
		}

						DevicesCollection devList ;
		DevicesCollection  CollectOutputDevices()
		{
			devList = new DevicesCollection();
return devList  ;
			}

// Functions


		public void Play(IAudioMediaAsset asset )
		{
			m_Asset = asset ;
VuMeter ob_VuMeter  = new VuMeter () ;
ob_VuMeter.DisplayGraph () ;
InitPlay(0, 0) ;
		}

		void InitPlay(long lStartPosition, long lEndPosition)
		{
			CalculationFunctions calc = new CalculationFunctions() ;
// Check if anything is playing or input length is out of bound
			if (m_State.Equals  (AudioPlayerState .stopped))
			{
				// Adjust the start and end position according to frame size
				lStartPosition = calc.AdaptToFrame(lStartPosition, m_Asset.FrameSize) ;
				lEndPosition = calc.AdaptToFrame(lEndPosition, m_Asset.FrameSize) ;
m_SamplingRate = m_Asset.SampleRate ;
				// creates file stream from file
				fs = new FileStream (m_Asset .Path, FileMode.Open,  								FileAccess.Read) ;

				// lEndPosition = 0 means that file is played to end
				if (lEndPosition != 0)
				{
					m_lLength = (lEndPosition )- lStartPosition;
				}
				else
				{
					m_lLength = (m_Asset .SizeInBytes - 44 - lStartPosition ) ;
				}

				// suet the file pointer position ahead header of file
				fs.Position= lStartPosition + 44 ;

				WaveFormat newFormat = new WaveFormat () ;				
				BufferDesc = new BufferDescription();

				// retrieve format from file
				m_FrameSize = m_Asset.FrameSize ;
m_Channels = m_Asset.Channels ;
				newFormat.AverageBytesPerSecond = m_Asset .SampleRate * m_Asset .FrameSize ;
				newFormat.BitsPerSample = Convert.ToInt16(m_Asset .BitDepth) ;
				newFormat.BlockAlign = Convert.ToInt16(m_Asset .FrameSize );
				newFormat.Channels  = Convert.ToInt16(m_Asset .Channels );

				newFormat.FormatTag = WaveFormatTag.Pcm ;

				newFormat.SamplesPerSecond = m_Asset .SampleRate ;

				// loads  format to buffer description
				BufferDesc.Format = newFormat ;

				// calculate size of buffer so as to contain 1 second of audio
				m_SizeBuffer = m_Asset .SampleRate *  m_Asset .FrameSize ;
				m_RefreshLength = (m_Asset .SampleRate / 2 ) * m_Asset .FrameSize ;
// calculate the size of VuMeter Update array length
				m_UpdateVMArrayLength = m_SizeBuffer  / 50 ;
m_UpdateVMArrayLength = Convert.ToInt32 (calc.AdaptToFrame ( Convert.ToInt32 ( m_UpdateVMArrayLength ),  m_FrameSize)  );
arUpdateVM = new byte [ m_UpdateVMArrayLength ] ;

				// sets the calculated size of buffer
				BufferDesc.BufferBytes = m_SizeBuffer ;

// Global focus is set to true so that the sound can be played in background also
				BufferDesc.GlobalFocus = true ;

				// initialising secondary buffer
				SoundBuffer= new SecondaryBuffer(BufferDesc, SndDevice);

				// check for fast play
				int reduction = 0 ;
				if (m_FastPlay == false)
				{
					SoundBuffer.Write (0 , fs , m_SizeBuffer, 0) ;
				}
				else
				{
					// for fast play buffer is filled in parts with new part overlapping previous part
					for (int i = 0 ; i< m_SizeBuffer ; i = i + (m_Step * m_FrameSize))
					{
						SoundBuffer.Write (i , fs , m_Step * m_FrameSize, 0) ;
						i = i - (2*m_FrameSize) ;
						// compute the difference in bytes skipped
						reduction = reduction + (2*m_FrameSize) ;
					}

				}
				// Adds the length (count) of file played into a variable
				m_lPlayed = m_SizeBuffer + reduction;

				m_PlayFile = true ;
				
// Change the state and trigger event
StateChanged ob_StateChanged = new StateChanged (m_State) ;
m_State= AudioPlayerState .playing ;

TriggerStateChangedEvent (ob_StateChanged) ;

				// starts playing
				SoundBuffer.Play(0, BufferPlayFlags.Looping);
				m_BufferCheck = 1 ;

				//initialise and start thread for refreshing buffer
				RefreshThread = new Thread(new ThreadStart (RefreshBuffer));
				RefreshThread.Start() ;

// end of playing check 
			}
			// end of function
		}


		
		void RefreshBuffer ()
		{
		CalculationFunctions calc = new CalculationFunctions () ;
			int ReadPosition;
// variable to count byte difference in compressed and non compressed data of audio file
int reduction = 0 ;
			while (m_lPlayed < m_lLength)
			{//1
				if (SoundBuffer.Status.BufferLost  )
					SoundBuffer.Restore () ;

reduction = 0 ;
				Thread.Sleep (50) ;
				ReadPosition = SoundBuffer.PlayPosition  ;
				if (ReadPosition < ((m_SizeBuffer)- m_UpdateVMArrayLength  ) )
				{
Array.Copy ( SoundBuffer.Read (ReadPosition , typeof (byte) , LockFlag.None , m_UpdateVMArrayLength  ) , arUpdateVM , m_UpdateVMArrayLength  ) ;				
ob_UpdateVuMeter.NotifyUpdateVuMeter ( this, ob_UpdateVuMeter ) ;
				}
				// check if play cursor is in second half , then refresh first half else second
				if ((m_BufferCheck% 2) == 1 &&  SoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
					// Checks if file is played or byteBuffer is played
					if (m_PlayFile == true)
					{//3
						// check for normal or fast play
						if (m_FastPlay == false)
						{//4
							SoundBuffer.Write (0 , fs , m_RefreshLength, 0) ;
						}//-4
						else
						{//4
							for (int i = 0 ; i< m_RefreshLength; i=i+(m_Step*m_FrameSize))
							{
								SoundBuffer.Write ( i, fs , m_Step*m_FrameSize, 0) ;
i = i-(2*m_FrameSize );
								reduction = reduction + (2* m_FrameSize);
							}//-4

						}//-3
					}//-2
					else
					{//2
for (int i = 0 ; i< m_RefreshLength ; i++)
RefreshArray[i] = ByteBuffer [m_lArrayPosition + i] ;

SoundBuffer.Write (0 , RefreshArray,  0) ;
m_lArrayPosition = m_lArrayPosition + m_RefreshLength ;
					}//-2
					m_lPlayed = m_lPlayed + m_RefreshLength+ reduction;

					m_BufferCheck++ ;
				}//-1
				else if ((m_BufferCheck % 2 == 0) &&  SoundBuffer.PlayPosition < m_RefreshLength)
				{//1
					if (m_PlayFile == true)
					{//2
						if (m_FastPlay == false)
						{//3
							SoundBuffer.Write (m_RefreshLength, fs , m_RefreshLength, 0)  ;						}//-3						else						{//3							for (int i = 0 ; i< m_RefreshLength; i=i+(m_Step*m_FrameSize))
							{//4
								SoundBuffer.Write ( i+ m_RefreshLength, fs , m_Step*m_FrameSize, 0) ;
								i = i-(2*m_FrameSize) ;
								reduction = reduction + (2* m_FrameSize);
															}						//-4
							
							// end of FastPlay check
						}//-3					}//-2					else					{//2						for (int i = 0 ; i< m_RefreshLength ; i++)
							RefreshArray[i] = ByteBuffer [m_lArrayPosition + i] ;						SoundBuffer.Write (m_RefreshLength , RefreshArray,  0) ;
						m_lArrayPosition = m_lArrayPosition + m_RefreshLength ;						// end of file check					}//-2
						m_lPlayed = m_lPlayed + m_RefreshLength+ reduction;

					m_BufferCheck++ ;

// end of even/ odd part of buffer;
				}//-1
				
					
// end of while
			}
// calculate time to stop according to remaining data
			int time ;
			long lRemaining = (m_lPlayed - m_lLength);
			double dTemp ;
			
				dTemp= ((m_RefreshLength + m_RefreshLength - lRemaining			)*1000)/m_RefreshLength ;
time = Convert.ToInt32(dTemp * 0.48 );

//if (m_FastPlay == true)
//time = (time-250) * (1- (2/m_Step));

			Thread.Sleep (time) ;
			
// Stopping process begins
			SoundBuffer.Stop () ;
			if (m_PlayFile == true)
			{
				fs.Close();
ob_EndOfAudioAsset.NotifyEndOfAudioAsset ( this , ob_EndOfAudioAsset) ;
			}
			else
			{
				m_lArrayPosition = 0 ;
				ByteBuffer = null ;
ob_EndOfAudioBuffer.NotifyEndOfAudioBuffer (this, ob_EndOfAudioBuffer ) ;
			}
//Stop () ;
// changes the state and trigger events
			//ob_StateChanged = new StateChanged (m_State) ;

StateChanged ob_StateChanged = new StateChanged (m_State) ;
m_State= AudioPlayerState .stopped ;

TriggerStateChangedEvent (ob_StateChanged) ;
//ob_EndOfFile.TriggerEndOfFileEvent ( m_Asset.Path);
		
			// RefreshBuffer ends
		}
		
		public void Play(IAudioMediaAsset  asset, double timeFrom)
		{

			m_Asset = asset ;
CalculationFunctions calc = new CalculationFunctions () ;
long lPosition = calc.ConvertTimeToByte (timeFrom, m_Asset .SampleRate, m_Asset .FrameSize) ;
			lPosition = calc.AdaptToFrame(lPosition, m_Asset .FrameSize) ;

			if(lPosition>0   && lPosition < m_Asset.AudioLengthBytes)
			{
				InitPlay ( lPosition, 0 );
			}
			else
			{
MessageBox.Show ("Parameters out of range") ;
			}
		}

// contains the end position  to play to be used in starting playing  after seeking
long lByteTo = 0 ;		
		public void Play(IAudioMediaAsset asset , double timeFrom, double timeTo)
		{
			m_Asset = asset ;
			CalculationFunctions calc = new CalculationFunctions () ;

			long lStartPosition = calc.ConvertTimeToByte (timeFrom, m_Asset .SampleRate, m_Asset .FrameSize) ;
			lStartPosition = calc.AdaptToFrame(lStartPosition, m_Asset .FrameSize) ;

			long lEndPosition = calc.ConvertTimeToByte (timeTo , m_Asset.SampleRate, m_Asset.FrameSize) ;
			lByteTo = lEndPosition ;

// check for valid arguments
if (lStartPosition>0 && lStartPosition < lEndPosition && lEndPosition <= m_Asset.AudioLengthBytes)
																						   {
						InitPlay ( lStartPosition, lEndPosition );
																						   }
			else
			{
				MessageBox.Show("Arguments out of range") ;
			}
		}


byte [] ByteArrayCopy ;
		public void Play(byte [] byteArray)
		{
ByteArrayCopy = new byte[byteArray.LongLength] ;
			for (long i = 0 ; i< byteArray.LongLength; i++)
ByteArrayCopy[i] = byteArray [i] ;

Play(byteArray, 0);
		}


		void Play(byte [] byteArray, long lStartPosition)
		{

CalculationFunctions calc = new CalculationFunctions () ;
			
			//declare   array variable of size 4 as the max chunk in header is 4 bytes long
			int [] Ar = new int[4] ;

			// retrieve the format of audio from header of byte stream
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
Ar[0] = Convert.ToInt32(byteArray [22]);
			Ar[1] = Convert.ToInt32(byteArray [23]);
long lTemp = calc.ConvertToDecimal (Ar) ;
		short shChannels = Convert.ToInt16 (lTemp) ;
			m_Channels = Convert.ToInt32 (shChannels) ;


			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
			Ar[0] = Convert.ToInt32(byteArray [24]);
			Ar[1] = Convert.ToInt32(byteArray [25]);
			lTemp = calc.ConvertToDecimal (Ar) ;
			int iSamplingRate = Convert.ToInt32 (lTemp) ;
			m_SamplingRate = iSamplingRate ;


			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
			Ar[0] = Convert.ToInt32(byteArray [32]);
			Ar[1] = Convert.ToInt32(byteArray [33]);
			lTemp = calc.ConvertToDecimal (Ar) ;
			short shFrameSize = Convert.ToInt16 (lTemp) ;
m_FrameSize = Convert.ToInt32 (shFrameSize) ;


			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
			Ar[0] = Convert.ToInt32(byteArray [34]);
			Ar[1] = Convert.ToInt32(byteArray [35]);
			lTemp = calc.ConvertToDecimal (Ar) ;
			short shBitDepth = Convert.ToInt16 (lTemp) ;

// Adjust start position according frame size
lStartPosition = calc.AdaptToFrame(lStartPosition, m_FrameSize) ;

			//check for out of range value
			if ( lStartPosition < (byteArray.LongLength - 44) )
			{			
			// get the maximum length to play
			m_lLength = byteArray.LongLength  - lStartPosition;
			
			// deduct header length from total length
			m_lLength = m_lLength - 44 ;

long lAdditionalLength = m_SamplingRate*m_FrameSize/2 ;
			// fill the byteBuffer to be played  from byte array
			ByteBuffer = new byte [m_lLength + lAdditionalLength] ;
			for (long i = 0 ; i< m_lLength ; i++)
				ByteBuffer[i] = byteArray [i+44+ lStartPosition] ;

for (long i = 0; i< lAdditionalLength; i++)
ByteBuffer [i+m_lLength] = 0 ;

			// sets the wave format of secondary buffer
			WaveFormat newFormat = new WaveFormat () ;				
			BufferDesc = new BufferDescription();


			newFormat.AverageBytesPerSecond = iSamplingRate * shFrameSize ;
			newFormat.BitsPerSample = shBitDepth ;
			newFormat.BlockAlign = shFrameSize ;
			newFormat.Channels  = shChannels ;

			newFormat.FormatTag = WaveFormatTag.Pcm ;

			newFormat.SamplesPerSecond = iSamplingRate ;

			// gets the length to be played
			m_lLength = calc.AdaptToFrame(m_lLength , Convert.ToInt32(shFrameSize)) ;

			BufferDesc.Format = newFormat ;

				// Global focus is set to true so that the sound can be played in background also
				BufferDesc.GlobalFocus = true ;

			// gets the size of buffer to be created
			m_SizeBuffer = iSamplingRate *   shFrameSize;
			m_RefreshLength = (iSamplingRate / 2 ) * shFrameSize;
				// calculate the size of VuMeter Update array length
				m_UpdateVMArrayLength = m_SizeBuffer  / 50 ;
				m_UpdateVMArrayLength = Convert.ToInt32 (calc.AdaptToFrame ( Convert.ToInt32 ( m_UpdateVMArrayLength ),  m_FrameSize)  );
				arUpdateVM = new byte [ m_UpdateVMArrayLength ] ;

//m_lLength = (m_lLength/m_RefreshLength)*m_RefreshLength ;

			BufferDesc.BufferBytes = m_SizeBuffer ;


			// initialise the buffer
			SoundBuffer= new SecondaryBuffer(BufferDesc, SndDevice);

			//creates temporary byte array to fill secondary buffer
			RefreshArray = new byte[m_SizeBuffer] ;
			for (int i = 0 ; i< m_SizeBuffer ; i++)
				RefreshArray[i] = ByteBuffer [i];

			// set the current position to be filled next time
			m_lArrayPosition = m_SizeBuffer ;

			SoundBuffer.Write (0 , RefreshArray, 0) ;
			m_lPlayed = m_SizeBuffer ;
			RefreshArray = new byte [m_RefreshLength] ;

			m_PlayFile = false ;

				StateChanged ob_StateChanged = new StateChanged (m_State) ;
			m_State= AudioPlayerState .playing ;
TriggerStateChangedEvent (ob_StateChanged) ;

			SoundBuffer.Play(0, BufferPlayFlags.Looping);
			m_BufferCheck= 1 ;
			RefreshThread = new Thread(new ThreadStart (RefreshBuffer));
			RefreshThread.Start() ;

// end of out of range check
		}
			// end of play byteBuffer
		}



		public void Pause()
		{
			if (m_State.Equals(AudioPlayerState .playing))
			{
				SoundBuffer.Stop () ;
				// Change the state and trigger event

StateChanged ob_StateChanged = new StateChanged (m_State) ;
				m_State= AudioPlayerState .paused ;
				TriggerStateChangedEvent (ob_StateChanged) ;
			}
		}

		public void Resume()
		{
			if (m_State.Equals (AudioPlayerState .paused))
			{
				

				StateChanged ob_StateChanged = new StateChanged (m_State) ;
				m_State= AudioPlayerState .playing ;
				TriggerStateChangedEvent (ob_StateChanged) ;
				SoundBuffer.Play(0, BufferPlayFlags.Looping);
			}			
		}

		public void Stop()
		{
			if (!m_State.Equals(AudioPlayerState .stopped))			
			{
				SoundBuffer.Stop () ;



				RefreshThread.Abort () ;
			
				if (m_PlayFile == true)
					fs.Close();
				else
				{
					m_lArrayPosition = 0 ;
					ByteBuffer = null ;
				}
			}

			StateChanged ob_StateChanged = new StateChanged (m_State) ;
m_State= AudioPlayerState .stopped ;
			TriggerStateChangedEvent (ob_StateChanged) ;
		}

		internal long GetCurrentBytePosition()
		{
int PlayPosition  = SoundBuffer.PlayPosition;
			
			long lCurrentPosition ;
			if (PlayPosition < m_RefreshLength)
			{ 
				// takes the lPlayed position and subtract the part of buffer played from it
				lCurrentPosition = m_lPlayed - ( 2 * m_RefreshLength) + PlayPosition ;
			}
			else
			{
lCurrentPosition = m_lPlayed - (3 * m_RefreshLength) + PlayPosition ;
			}
return lCurrentPosition ;
		}

		internal double GetCurrentTimePosition()
		{
			CalculationFunctions calc = new CalculationFunctions () ;
			long lPos = GetCurrentBytePosition ();
			double dTime = calc.ConvertByteToTime (lPos , m_SamplingRate, m_FrameSize) ;
			return dTime ;
		}		

		void SetCurrentBytePosition (long localPosition) 
		{
m_EventsEnabled = false ;
			if (SoundBuffer.Status.Looping)
			{
				if ( m_PlayFile== true)
				{

					Stop();
					Thread.Sleep (30) ;
					InitPlay(localPosition , lByteTo);
				}
				else
				{
					
Stop () ;
Play (ByteArrayCopy, localPosition) ;
				}
			}
			
			else if(m_State.Equals (AudioPlayerState .paused) )
			{
				if (m_PlayFile == true)
				{
					Stop();
Thread.Sleep (20) ;
					InitPlay(localPosition , lByteTo);
					Thread.Sleep(30) ;
					SoundBuffer.Stop () ;
					// Stop () also change the m_Stateto stopped so change it to paused
					m_State=AudioPlayerState .paused ;
				}
				else
				{
					Stop () ;
					Play (ByteArrayCopy, localPosition) ;
					Thread.Sleep(30) ;
					SoundBuffer.Stop () ;
					// Stop () also change the m_Stateto stopped so change it to paused
										m_State=AudioPlayerState .paused ;
				}
			}
m_EventsEnabled = true ;
// end of set byte position
		}


		void SetCurrentTimePosition (double localPosition) 
		{
CalculationFunctions calc = new CalculationFunctions() ;
			long lTemp = calc.ConvertTimeToByte (localPosition, m_SamplingRate, m_FrameSize);
SetCurrentBytePosition(lTemp) ;
		}

		public void  test ()
		{
if (m_State.Equals(AudioPlayerState .stopped))
	MessageBox.Show("Stopped") ;
		}
// End Class
	}
	// End NameSpace
}
