using System;
using System.Threading;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Obi.Audio
{
	public class VuMeter
	{
		public event Events.Audio.VuMeter.PeakOverloadHandler PeakOverload;        //
        public Events.Audio.VuMeter.LevelTooLowHandler LevelTooLowEvent; // event to notify low amplitude
		public event Events.Audio.VuMeter.ResetHandler ResetEvent;                 //
		public event Events.Audio.VuMeter.UpdateFormsHandler UpdateForms;          //
        public event Events.Audio.VuMeter.UpdatePeakMeterHandler UpdatePeakMeter;  //
        public Events.Audio.VuMeter.LevelGoodHandler LevelGoodEvent;

        private AudioPlayer mPlayer;      // associated player
        private AudioRecorder mRecorder;  // associated recorder

        private int mChannels;            // number of channels
        private int m_FrameSize;
        private int m_SamplingRate; // sampling rate of asset
        private int mUpperThreshold;      // upper threshold (?)
        private int mLowerThreshold;      // lower threshold (?)
        private int mMeanValueLeft;       // left channel mean value
        private int mMeanValueRight;      // right channel mean value
        

        // variables for detecting lower amplitude
        private double[] m_AverageValue; // array to hold average or RMS value
        private bool m_IsLowAmplitude;
        private double[] m_arLowAmpSamples ;
        private readonly int m_MaxLowAmpSamples = 10;

		/// <summary>
		/// Create the VU meter object.
		/// </summary>
        public VuMeter(AudioPlayer player, AudioRecorder recorder)
		{
            SetSampleCount(m_SampleTimeLength);
            mPlayer = player;
            mRecorder = recorder;
            SetEventHandlers();
            mChannels = 2;
            mUpperThreshold = 210;
            mLowerThreshold = 15;

            m_AverageValue = new double[2];
            m_AverageValue[0] = 0;
            m_AverageValue[1] = 0;
            m_arLowAmpSamples = new double[m_MaxLowAmpSamples];
        }


        public int UpperThreshold { get { return mUpperThreshold; } }
        public int LowerThreshold { get { return mLowerThreshold; } }
        public int ChannelValueLeft { get { return mMeanValueLeft; } }
        public int ChannelValueRight { get { return mMeanValueRight; } }
        public int Channels { get { return mChannels;  } }

        // Set the event handlers for player and recorder.
        private void SetEventHandlers()
        {
            mPlayer.UpdateVuMeter += new Events.Audio.Player.UpdateVuMeterHandler(CatchUpdateVuMeterEvent);
            mPlayer.ResetVuMeter += new Events.Audio.Player.ResetVuMeterHandler(CatchResetEvent );
            mRecorder.UpdateVuMeterFromRecorder += new Events.Audio.Recorder.UpdateVuMeterHandler(CatchUpdateVuMeterEvent);
            mRecorder.ResetVuMeter += new Events.Audio.Recorder.ResetVuMeterHandler(CatchResetEvent);
        }

		//Member variable used in properties
		private double m_SampleTimeLength = 500 ;
		internal bool m_bOverload = false ;
		private int [] arPeakOverloadValue = new int [2] ;
		private bool [] arPeakOverloadFlag = new bool [2] ;
        private double[] m_PeakDbValue;


        public double[] PeakDbValue
        {
            get
            {
                return m_PeakDbValue;
            }
        }

        public double[] AverageAmplitudeDBValue
        {
            get { return m_AverageValue; }
                        }

        public bool IsLevelTooLow
        {
            get { return m_IsLowAmplitude; }
        }

        public void CatchResetEvent(object sender, EventArgs e)
        {
            Reset();
        }

		public void Reset()
		{
            			arPeakOverloadValue [0] = 0 ;
			arPeakOverloadValue [1] = 0 ;
			arPeakOverloadFlag[0] = arPeakOverloadFlag [1] = false ;
			for (int i = 0 ; i< m_SampleCount ; i++)
			{
				SampleArrayLeft [i] = 0;
				SampleArrayRight [i] = 0;
			}
			// m_SampleArrayPosition  = 0 ;
			mMeanValueLeft = 0 ;
			mMeanValueRight = 0 ;

            m_IsLowAmplitude = false;
            m_AverageValue[0] = 0;
            m_AverageValue[1] = 0;
                        if (ResetEvent  != null)
			ResetEvent(this, new Events.Audio.VuMeter.ResetEventArgs());
					}

		// calculate and sets the count of samples to be read for computing RMS or mean value of amplitude
		private void SetSampleCount ( double TimeLengthArg ) 
		{
			m_SampleTimeLength  = TimeLengthArg ;
            m_SampleCount = Convert.ToInt32(TimeLengthArg / m_BufferReadInterval); 

			SampleArrayLeft  = new int [ 2 * m_SampleCount] ;
			SampleArrayRight  = new int [2 * m_SampleCount] ;
            
		}

		// member variables for internal processing
		private byte [] m_arUpdatedVM  ;
		private int m_UpdateVMArrayLength ;
		

		private AudioPlayer ob_AudioPlayer ;
		private AudioRecorder  ob_AudioRecorder ;
		private int  m_SampleCount    ;
		private  int [] SampleArrayLeft ;
		private int [] SampleArrayRight ;
		// jq: removed a compiler warning
        // private int m_SampleArrayPosition = 0;
        

        // avn: added on 13 March 2007, Variable to hold value of time interval for reading bytes from Buffers
        private double  m_BufferReadInterval= 50  ;

		private bool boolPlayer =false ;

		// Handles VuMeter event from player
		public void CatchUpdateVuMeterEvent ( object sender , Events.Audio.Player.UpdateVuMeterEventArgs Update) 
		{
			boolPlayer = true ;
			ob_AudioPlayer = sender as AudioPlayer;
			m_FrameSize = ob_AudioPlayer.CurrentAudio.getPCMFormat ().getBlockAlign ()  ;
			mChannels = ob_AudioPlayer.CurrentAudio.getPCMFormat ().getNumberOfChannels ()   ;
            m_SamplingRate =(int) ob_AudioPlayer.CurrentAudio.getPCMFormat().getSampleRate ();
			m_UpdateVMArrayLength = ob_AudioPlayer.arUpdateVM.Length  ;
			m_arUpdatedVM  = new byte[m_UpdateVMArrayLength ] ;

			Array.Copy ( ob_AudioPlayer.arUpdateVM  , m_arUpdatedVM , m_UpdateVMArrayLength) ;

            if (m_BufferReadInterval != ((m_arUpdatedVM.Length * 1000) / (ob_AudioPlayer.CurrentAudio.getPCMFormat().getSampleRate() *m_FrameSize  )))
            {
                m_BufferReadInterval = ((m_arUpdatedVM.Length * 1000) / (ob_AudioPlayer.CurrentAudio.getPCMFormat().getSampleRate() * m_FrameSize ));
                SetSampleCount(m_SampleTimeLength);
            }

            //AnimationComputation();
            ComputePeakDbValue();
		}


        private Thread ThreadTriggerPeakEventWithDelay;
        private byte[] m_RecorderArray;

		// handles update event from audio recorder
		public void CatchUpdateVuMeterEvent (object sender , Events.Audio.Recorder.UpdateVuMeterEventArgs UpdateVuMeter)
		{
			AudioRecorder Recorder = sender as AudioRecorder ;
			ob_AudioRecorder = Recorder ;
			m_FrameSize = ( Recorder.Channels * ( Recorder.BitDepth / 8 ) )   ;
			mChannels = Recorder.Channels ;
            m_SamplingRate = Recorder.SampleRate;
            m_UpdateVMArrayLength =  Recorder.m_UpdateVMArrayLength / 2 ;
                        m_UpdateVMArrayLength = (int) CalculationFunctions.AdaptToFrame(m_UpdateVMArrayLength, m_FrameSize);

                        m_RecorderArray = new byte[2 * m_UpdateVMArrayLength];
                        Array.Copy(Recorder.arUpdateVM, m_RecorderArray ,m_RecorderArray.Length );

			m_arUpdatedVM  = new byte [m_UpdateVMArrayLength ] ;
			Array.Copy ( m_RecorderArray , m_arUpdatedVM , m_UpdateVMArrayLength) ;

            //m_BufferReadInterval = ( m_arUpdatedVM.Length * 1000 ) / ( Recorder.SampleRate * m_FrameSize )  ;
            if (Recorder != null && m_BufferReadInterval != ((m_arUpdatedVM.Length * 1000) / (Recorder.SampleRate *  m_FrameSize  )))
            {
                m_BufferReadInterval = ((m_arUpdatedVM.Length * 1000) / (Recorder.SampleRate *  m_FrameSize  ));
                SetSampleCount(m_SampleTimeLength);
            }

            //AnimationComputation();
            ComputePeakDbValue();

            if (ThreadTriggerPeakEventWithDelay != null && ThreadTriggerPeakEventWithDelay.IsAlive)
            {
                ThreadTriggerPeakEventWithDelay.Abort();
                ThreadTriggerPeakEventWithDelay = null;
                            }

                ThreadTriggerPeakEventWithDelay = new Thread(new ThreadStart(TriggerPeakEventForSecondHalf));
                ThreadTriggerPeakEventWithDelay.IsBackground = true;
                ThreadTriggerPeakEventWithDelay.Start();
            		}
        
        /// <summary>
        ///  Compute VuMeter peak values and triggeres peak value event
        /// <see cref=""/>
        /// </summary>
        private void ComputePeakDbValue()
        {
            int bytesPerSample = m_FrameSize / mChannels;
            int noc = mChannels;
            double[] maxDbs = new double[noc];
            double full = Math.Pow(2, 8 * bytesPerSample);
            double halfFull = full / 2;

            for (int i = 0; i < m_arUpdatedVM.Length; i += bytesPerSample * noc)
            {//1
                for (int c = 0; c < noc; c++)
                { //2
                    double val = 0;
                    for (int j = 0; j < bytesPerSample; j++)
                    { //3
                        val += Math.Pow(2, 8 * j) * m_arUpdatedVM[i + (c * bytesPerSample) + j];
                    } //-3
                    if (val > halfFull)
                    { //3
                        val = full - val;
                    } //-3
                    if (val > maxDbs[c]) maxDbs[c] = val;
                } //-2
            } //-1
            for (int c = 0; c < noc; c++)
            { //1
                maxDbs[c] = 20 * Math.Log10(maxDbs[c] / halfFull);
            } // -1

            m_PeakDbValue = maxDbs;
            if ( UpdatePeakMeter != null )
                            UpdatePeakMeter(this, new Obi.Events.Audio.VuMeter.UpdatePeakMeter(maxDbs));

                        DetectOverloadForPeakMeter();
        //System.IO.File.AppendAllText("c:\\1.txt", "\n");
        //System.IO.File.AppendAllText("c:\\1.txt", maxDbs[0].ToString());
                        if ( ob_AudioRecorder != null && (ob_AudioRecorder.State == AudioRecorderState.Monitoring || ob_AudioRecorder.State == AudioRecorderState.Recording ))
                            AnimationComputation();
        }

        private void TriggerPeakEventForSecondHalf()
        {
            Thread.Sleep(66);
            if (mRecorder != null && m_RecorderArray.Length > m_arUpdatedVM.Length)
            {
                try
                {
                    Array.Copy(m_RecorderArray , m_UpdateVMArrayLength, m_arUpdatedVM, 0, m_arUpdatedVM.Length );
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                ComputePeakDbValue();
            }
        }


        double[] TempArray = new double[4];
        int tempCount = 0;
		void AnimationComputation ()
		{
            // create an local array and fill the amplitude value of both channels from function
            int[] TempAmpArray = new int[2];
            Array.Copy(AmplitudeValue1(), TempAmpArray, 2);

            //find value in db
            double MaxVal = (int)Math.Pow(2, 8 * (m_FrameSize / mChannels))/2;

            
            double left = Convert.ToDouble ( TempAmpArray [0] ) / MaxVal;
            left = 20 * Math.Log10(left);
            if (left < -90) left = -90;
                                         double right = Convert.ToDouble ( TempAmpArray [1] ) / MaxVal;
                                         right = 20 * Math.Log10(right);
                                         if (right < -90) right = -90;

                                         TempArray[tempCount] = left;
                                         tempCount++;
                                         if (tempCount >= 4) tempCount = 0;
                                         m_AverageValue[0] = (TempArray [0] + TempArray [1] + TempArray [2] ) / 3;
                                         m_AverageValue[1] = right;
            // temp disabled for new trial
                                         //m_AverageValue[0] = m_AverageValue[0] == 0 ? left : ((m_AverageValue[0] * 19) + left) / 20;
                                         //m_AverageValue[1] = m_AverageValue[1] == 0 ? right : ((m_AverageValue[1] * 19) + right) / 20; 
                                         
            //mMeanValueLeft = ( ( mMeanValueLeft * 19 ) + left ) /20 ;
            //mMeanValueRight = AmpArray[1];

                                         //Debug_WriteToTextFile(m_AverageValue[0].ToString() + "-" + m_AverageValue[1].ToString());
                        //wr.WriteLine(left.ToString());
            
                        DetectLowAmplitude();
            /*
            // update peak values if it is greater than previous value
            if (m_PeakValueLeft < mMeanValueLeft)
                arPeakOverloadValue[0] = m_PeakValueLeft = mMeanValueLeft;

            if (m_PeakValueRight < mMeanValueRight)
                arPeakOverloadValue[1] = m_PeakValueRight = mMeanValueRight;


            // Check for Peak Overload  and fire event if overloaded
            if (mMeanValueLeft > mUpperThreshold)
            {
                arPeakOverloadFlag[0] = true;
                Events.Audio.VuMeter.PeakOverloadEventArgs e;
                if (boolPlayer)
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(1,
                        ob_AudioPlayer.CurrentBytePosition , ob_AudioPlayer.CurrentTimePosition );
                }
                else
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(1, 0, 0);
                }
                PeakOverload(this, e);
            }
            else
            {
                arPeakOverloadFlag[0] = false;
            }

            if (mMeanValueRight > mUpperThreshold)
            {
                m_bOverload = true;

                arPeakOverloadFlag[1] = true;
                Events.Audio.VuMeter.PeakOverloadEventArgs e;
                if (boolPlayer)
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(2,
                        ob_AudioPlayer.CurrentBytePosition , ob_AudioPlayer.CurrentTimePosition );
                }
                else
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(2, 0, 0);
                }
                PeakOverload(this, e);
            }
            else
            {
                arPeakOverloadFlag[1] = false;
            }
            */
            // Update ccurrent graph cordinates to VuMeter display
            if ( UpdateForms != null )
            UpdateForms(this, new Events.Audio.VuMeter.UpdateFormsEventArgs());
		}

        int[] AmplitudeValue1()
        {
                        // average value to return
            int [] arAveragePeaks = new int    [2] ;
            long Left = 0;
            long Right = 0;
            

                        // number of samples from which peak is selected
            uint PeakSampleCount = Convert.ToUInt32 (m_SamplingRate / 2000);

            // number of blocks iterated
            uint Count = Convert.ToUInt32 (m_arUpdatedVM.Length / PeakSampleCount );
            if (Count * PeakSampleCount > m_arUpdatedVM.Length) Count--;

            //System.IO.StreamWriter wr = System.IO.File.AppendText("c:\\2.txt");
            //wr.WriteLine("Count" +  Count.ToString() + "-"+ "PeakSampleCount" + PeakSampleCount.ToString());
            //wr.Close();

             int   [] tempArray = new int  [2] ;
                        for (uint i = 0; i < Count; i++)
            {
                GetSpeechFragmentPeak(PeakSampleCount, i * PeakSampleCount ).CopyTo(tempArray, 0);
                Left += tempArray[0];
                Right += tempArray[1];
                                }
                                arAveragePeaks[0] = Convert.ToInt32 ( Left / Count ) ;
                                arAveragePeaks[1] = Convert.ToInt32 ( Right / Count ) ;

                                return arAveragePeaks;
        }

        private int[] GetSpeechFragmentPeak(uint FragmentSize, uint StartIndex)
        {
            int [] arPeakVal = new int[2] ;
            arPeakVal [0] =  0 ;
            arPeakVal [1] = 0 ;
                        
            for (int i = 0; i < FragmentSize ; i = i + m_FrameSize)
            {
                int SampleLeft = 0;
                int SampleRight = 0;
                SampleLeft = m_arUpdatedVM[StartIndex + i];
                if (m_FrameSize / mChannels == 2)
                {
                    SampleLeft += m_arUpdatedVM[StartIndex + i + 1] * 256;

                                        if (SampleLeft > 32768)
                        SampleLeft = SampleLeft - 65536;
                }
                if (mChannels == 2)
                {
                    SampleRight = m_arUpdatedVM[StartIndex + i + 2];
                    if (m_FrameSize / mChannels == 2)
                    {
                        SampleRight += m_arUpdatedVM[StartIndex + i + 3] * 256;

                        if (SampleRight > 32768)
                            SampleRight = SampleRight - 65536;
                    }
                }

                // Update peak values from fragment
                if (SampleLeft > arPeakVal[0]) arPeakVal[0] = SampleLeft;
                                if (SampleRight > arPeakVal[1]) arPeakVal[1] = SampleRight;
                            }
            
                                                        return arPeakVal;
        }

		// calculates the amplitude of both channels from input taken from DirectX buffers
		int [] AmplitudeValue()
		{
            int leftVal = 0;
            int rightVal = 0;
            int tmpLeftVal = 0;
            int tmpRightVal = 0;
            int[] last2LeftVals = new int[] { 0, 1 };
            int[] last2RightVals = new int[] { 0, 1 };
            System.Collections.Generic.Stack<int> leftPeaks = new System.Collections.Generic.Stack<int>();
            System.Collections.Generic.Stack<int> rightPeaks = new System.Collections.Generic.Stack<int>();

            for (int i = 0; i < m_arUpdatedVM.Length; i += m_FrameSize)
            {
                switch (m_FrameSize)
                {
                    case 1:
                        //each byte is a simple sample

                        sbyte curVal = (sbyte)m_arUpdatedVM[i];
                        tmpLeftVal = (int)curVal * (int.MaxValue / sbyte.MaxValue);
                        break;

                    case 2:
                        switch (mChannels)
                        {
                            case 1:
                                short sCurVal = BitConverter.ToInt16(m_arUpdatedVM, i);
                                tmpLeftVal = (int)sCurVal * (int.MaxValue / short.MaxValue);

                                break;
                            case 2:
                                sbyte curValLeft = (sbyte)m_arUpdatedVM[i];
                                sbyte curValRight = (sbyte)m_arUpdatedVM[i + 1];

                                tmpLeftVal = (int)curValLeft * (int.MaxValue / sbyte.MaxValue);
                                tmpRightVal = (int)curValRight * (int.MaxValue / sbyte.MaxValue);
                                break;
                            default:
                                break;
                        }
                        break;

                    case 4:
                        short sCurValLeft = BitConverter.ToInt16(m_arUpdatedVM, i);
                        short sCurValRight = BitConverter.ToInt16(m_arUpdatedVM, i + 2);
                        tmpLeftVal = (int)sCurValLeft * (int.MaxValue / short.MaxValue);
                        tmpRightVal = (int)sCurValRight * (int.MaxValue / short.MaxValue);
                        break;

                    default:
                        break;
                }
                if (tmpLeftVal == int.MinValue)
                    tmpLeftVal = int.MaxValue;
                if (tmpRightVal == int.MinValue)
                    tmpRightVal = int.MaxValue;

                tmpLeftVal = Math.Abs(tmpLeftVal);
                tmpRightVal = Math.Abs(tmpRightVal);

                //Test if this is a peak?
                if (tmpLeftVal < last2LeftVals[0] && last2LeftVals[0] > last2LeftVals[1])
                    leftPeaks.Push(tmpLeftVal);

                if (tmpLeftVal < last2LeftVals[0] && last2RightVals[0] > last2RightVals[1])
                    rightPeaks.Push(tmpRightVal);

                if (tmpLeftVal != last2LeftVals[0])
                {
                    last2LeftVals[1] = last2LeftVals[0];
                    last2LeftVals[0] = tmpLeftVal;
                }
                if (tmpRightVal != last2RightVals[0])
                {
                    last2RightVals[1] = last2RightVals[0];
                    last2RightVals[0] = tmpRightVal;
                }

            }

            long peakCount = 0;
            long peakSum = 0;
            for (; leftPeaks.Count > 0; peakCount++)
            {
                peakSum += leftPeaks.Pop();
            }
            if (peakCount > 0)
                leftVal = (int)(peakSum / peakCount);

            peakCount = 0;
            peakSum = 0;
            for (; rightPeaks.Count > 0; peakCount++)
            {
                peakSum += rightPeaks.Pop();
            }
            if (peakCount > 0)
                rightVal = (int)(peakSum / peakCount);

            int[] arSum = new int[2];
            arSum[0] = leftVal / (int.MaxValue / 256); // division done to reduce the returned value to be in the range of 0-255
            arSum[1] = rightVal / (int.MaxValue / 256);



            return arSum;
		}


        
        private void DetectOverloadForPeakMeter()
        {
        // Check for Peak Overload  and fire event if overloaded
            if ( m_PeakDbValue.Length > 0 &&    m_PeakDbValue [0]  >= 0 )
            {
                                                    arPeakOverloadFlag[0] = true;
                    Events.Audio.VuMeter.PeakOverloadEventArgs e;
                    if (boolPlayer)
                    {
                        e = new Events.Audio.VuMeter.PeakOverloadEventArgs(1,
                            ob_AudioPlayer.CurrentBytePosition, ob_AudioPlayer.CurrentTimePosition);
                    }
                    else
                    {
                        e = new Events.Audio.VuMeter.PeakOverloadEventArgs(1, 0, 0);
                    }
                if ( PeakOverload != null )
                    PeakOverload(this, e);
            }
            else
            {
                arPeakOverloadFlag[0] = false;
                            }

            if (m_PeakDbValue.Length > 1    &&   m_PeakDbValue [1]   >=  0 )
            {
                m_bOverload = true;

                arPeakOverloadFlag[1] = true;
                Events.Audio.VuMeter.PeakOverloadEventArgs e;
                if (boolPlayer)
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(2,
                        ob_AudioPlayer.CurrentBytePosition , ob_AudioPlayer.CurrentTimePosition );
                }
                else
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(2, 0, 0);
                }
                if (PeakOverload != null)
                    PeakOverload(this, e);
            }
            else
            {
                arPeakOverloadFlag[1] = false;
            }

        }

        public enum NoiseLevelSelection { High, Low, Medium } ;
        private NoiseLevelSelection m_NoiseLevel;
        /// <summary>
        ///  
                /// </summary>
        public NoiseLevelSelection NoiseLevel
        {
            get { return m_NoiseLevel; }
            set
            {
                if (value == NoiseLevelSelection.Low)
                {
                    m_LBound = -37.5;
                }
                else if (value == NoiseLevelSelection.Medium)
                {
                    m_LBound = -36;
                }
                else if (value == NoiseLevelSelection.High)
                {
                    m_LBound = -33.5;
                }
                m_NoiseLevel = value;
            }
                }

        int LowThresholdCount = 0;
        List<double> LowAmpList = new List<double>();
        bool GoneHigh = false;
        double m_UBound = -28.8;
        double m_LBound = -36;
        private void DetectLowAmplitude()
        {
                        double AmplitudeValue;
            
            if (mChannels == 1) AmplitudeValue = m_AverageValue[0];
            else AmplitudeValue = (m_AverageValue[0] + m_AverageValue[1] ) / 2;

            if (AmplitudeValue < m_LBound)
                LowThresholdCount++;
            else
            {
                                GoneHigh = true;
                LowAmpList.Add(AmplitudeValue);
                LowThresholdCount = 0;
            }

            if ( ( LowThresholdCount >= 4 || LowAmpList.Count > 40 )
                && GoneHigh )
            {
            if (LowAmpList.Count > 4)
                {
                GoneHigh = false;
                double avg = 0;
                foreach (double d in LowAmpList)
                    avg += d;

                avg = avg / LowAmpList.Count;
                if (avg < m_UBound)
                    {
                    if (LevelTooLowEvent != null) LevelTooLowEvent ( this, new Obi.Events.Audio.VuMeter.LevelTooLowEventArgs ( this, avg, 0, 0 ) );
                    m_IsLowAmplitude = true;
                    
                    //Debug_WriteToTextFile("low: " + avg.ToString());
                    }
                else if (avg > m_UBound)
                    {
                    m_IsLowAmplitude = false;
                    if (LevelGoodEvent != null) LevelGoodEvent (this ,  new Events.Audio.VuMeter.LevelGoodArgs ( avg ) );
                    }
                LowAmpList.Clear ();
                LowThresholdCount = 0;
                //Debug_WriteToTextFile(" next ");
                }
            else
                {
                LowAmpList.Clear ();
                LowThresholdCount = 0;
                }
            }
        }

        void Debug_WriteToTextFile(string s)
        {
                        if ( System.IO.File.Exists ("c:\\222111.txt") )
            {
                try
                {
                    System.IO.StreamWriter wr = System.IO.File.AppendText("c:\\222111.txt");
                    wr.WriteLine(s);
                    wr.Close();
                }
                catch (System.Exception)
                {
                    return;
                }
            }
                     }


		 // end of class
	}

}
