using System;
using System.Threading;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Obi.Audio
{
	public class VuMeter
	{
		// public event Events.Audio.VuMeter.LevelTooLowHandler LevelTooLow;
		public event Events.Audio.VuMeter.PeakOverloadHandler PeakOverload;
		public event Events.Audio.VuMeter.ResetHandler ResetEvent;
		public event Events.Audio.VuMeter.UpdateFormsHandler UpdateForms;

		/// <summary>
		/// Create the VU meter object.
		/// </summary>
        public VuMeter()
		{
			SetSampleCount(m_SampleTimeLength);
			// m_SampleArrayPosition = 0;
		}

        /// <summary>
        /// Set the VU meter to listen to the audio player and recorder.
        /// </summary>
        public void SetEventHandlers()
        {
            AudioPlayer.Instance.UpdateVuMeter +=
                new Events.Audio.Player.UpdateVuMeterHandler(CatchUpdateVuMeterEvent);
            AudioRecorder.Instance.UpdateVuMeterFromRecorder +=
                new Events.Audio.Recorder.UpdateVuMeterHandler(CatchUpdateVuMeterEvent);
        }

		//Member variable used in properties
		private int m_Channels =2 ;
		private double m_ScaleFactor = 2 ;
		private double m_SampleTimeLength = 500 ;
		internal bool m_bOverload = false ;
		private int m_UpperThreshold = 210 ;
		private int m_LowerThreshold = 15  ;
		private int [] arPeakOverloadValue = new int [2] ;
		private bool [] arPeakOverloadFlag = new bool [2] ;
		
		public int Channels
		{
			get
			{
				return m_Channels ;
			}
			set
			{
				m_Channels = value ;
			}
		}

		public object Stream
		{
			set
			{
			}
		}
		
		public UserControl VisualControl
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public UserControl TextControl
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int [] PeakValue
		{
			get
			{
				return arPeakOverloadValue ;
			}
		}

		public bool[] Overloaded
		{
			get
			{
				return arPeakOverloadFlag ;
			}
		}

		// properties added by app team india
		public int UpperThreshold 
		{
			get
			{
				return m_UpperThreshold ;
			}
			set
			{
				m_UpperThreshold = value ;
			}
		}

		public int LowerThreshold
		{
			get
			{
				return m_LowerThreshold ;
			}
			set
			{
				m_LowerThreshold = value ;
			}
		}

		public double ScaleFactor
		{
			get
			{
				return m_ScaleFactor ;
			}
			set
			{
				m_ScaleFactor = value ;
			}
		}


		public double SampleTimeLength
		{
			get
			{
				return m_SampleTimeLength ;
			}
			set
			{
				SetSampleCount (value) ;
			}
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
			m_MeanValueLeft = 0 ;
			m_MeanValueRight = 0 ;

            if (ResetEvent  != null)
			ResetEvent(this, new Events.Audio.VuMeter.ResetEventArgs());
					}

		// calculate and sets the count of samples to be read for computing RMS or mean value of amplitude
		void SetSampleCount ( double TimeLengthArg ) 
		{
			m_SampleTimeLength  = TimeLengthArg ;
//			m_SampleCount  = Convert.ToInt32 (TimeLengthArg / 50 ); // 50 is byte reading interval 
            m_SampleCount = Convert.ToInt32(TimeLengthArg / m_BufferReadInterval); 

			SampleArrayLeft  = new int [ 2 * m_SampleCount] ;
			SampleArrayRight  = new int [2 * m_SampleCount] ;
            
		}

		// member variables for internal processing
		private byte [] m_arUpdatedVM  ;
		private int m_UpdateVMArrayLength ;
		private int m_FrameSize ;

		// for position of graph which is centre of graph in Y and centre of left edges of two graph for x
		private int m_GraphPositionX = 170 ;
		private int m_GraphPositionY = 300 ;
			

		private AudioPlayer ob_AudioPlayer ;
		private AudioRecorder  ob_AudioRecorder ;
		private int  m_SampleCount    ;
		internal int m_MeanValueLeft ;
		internal int m_MeanValueRight  ;
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
			m_FrameSize = ob_AudioPlayer.m_FrameSize ;
			m_Channels = ob_AudioPlayer.m_Channels ;
			m_UpdateVMArrayLength = ob_AudioPlayer.m_UpdateVMArrayLength ;
			m_arUpdatedVM  = new byte[m_UpdateVMArrayLength ] ;

			Array.Copy ( ob_AudioPlayer.arUpdateVM  , m_arUpdatedVM , m_UpdateVMArrayLength) ;

            //m_BufferReadInterval =( ( m_arUpdatedVM.Length * 1000 ) / (ob_AudioPlayer.CurrentAsset.SampleRate * m_FrameSize))  ;

            if (m_BufferReadInterval != ((m_arUpdatedVM.Length * 1000) / (ob_AudioPlayer.CurrentAsset.SampleRate * ob_AudioPlayer.m_FrameSize )))
            {
                m_BufferReadInterval = ((m_arUpdatedVM.Length * 1000) / (ob_AudioPlayer.CurrentAsset.SampleRate * ob_AudioPlayer.m_FrameSize ));
                SetSampleCount(m_SampleTimeLength);
            }

            AnimationComputation();
//			Thread UpdateVMForm = new Thread(new ThreadStart (AnimationComputation  ));
//            UpdateVMForm.IsBackground = true ;
//			UpdateVMForm.Start()  ;
            

		}
        
		// handles update event from audio recorder
		public void CatchUpdateVuMeterEvent (object sender , Events.Audio.Recorder.UpdateVuMeterEventArgs UpdateVuMeter)
		{
			AudioRecorder Recorder = sender as AudioRecorder ;
			ob_AudioRecorder = Recorder ;
			m_FrameSize = Recorder.m_FrameSize ;
			m_Channels = Recorder.m_Channels ;
			m_UpdateVMArrayLength = Recorder.m_UpdateVMArrayLength ;
			m_arUpdatedVM  = new byte [m_UpdateVMArrayLength ] ;
			Array.Copy ( Recorder.arUpdateVM  , m_arUpdatedVM , m_UpdateVMArrayLength) ;

            //m_BufferReadInterval = ( m_arUpdatedVM.Length * 1000 ) / ( Recorder.SampleRate * m_FrameSize )  ;
            if (Recorder != null && m_BufferReadInterval != ((m_arUpdatedVM.Length * 1000) / (Recorder.SampleRate * Recorder.m_FrameSize )))
            {
                m_BufferReadInterval = ((m_arUpdatedVM.Length * 1000) / (Recorder.SampleRate * Recorder.m_FrameSize  ));
                SetSampleCount(m_SampleTimeLength);
            }

            AnimationComputation();
            //Thread UpdateVMForm = new Thread(new ThreadStart (AnimationComputation  ));
            //UpdateVMForm.IsBackground = true;
            //UpdateVMForm.Start()  ;

		}
			


		int m_PeakValueLeft = 0;
		int m_PeakValueRight = 0;
		void AnimationComputation ()
		{
            //Thread.Sleep (25) ;
            // finds the origin i.e upper left corner of graph display rectangle
            //origin is computed from centre position of graph
            int OriginX = m_GraphPositionX - Convert.ToInt32((m_ScaleFactor * 60));
            int OriginY = m_GraphPositionY - Convert.ToInt32(125 * m_ScaleFactor);

            // create an local array and fill the amplitude value of both channels from function
            int[] AmpArray = new int[2];
            Array.Copy(AmplitudeValue(), AmpArray, 2);

            m_MeanValueLeft = AmpArray[0];
            m_MeanValueRight = AmpArray[1];

            // update peak values if it is greater than previous value
            if (m_PeakValueLeft < m_MeanValueLeft)
                arPeakOverloadValue[0] = m_PeakValueLeft = m_MeanValueLeft;

            if (m_PeakValueRight < m_MeanValueRight)
                arPeakOverloadValue[1] = m_PeakValueRight = m_MeanValueRight;


            // Check for Peak Overload  and fire event if overloaded
            if (m_MeanValueLeft > m_UpperThreshold)
            {
                arPeakOverloadFlag[0] = true;
                Events.Audio.VuMeter.PeakOverloadEventArgs e;
                if (boolPlayer)
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(1,
                        ob_AudioPlayer.GetCurrentBytePosition(), ob_AudioPlayer.GetCurrentTimePosition());
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

            if (m_MeanValueRight > m_UpperThreshold)
            {
                m_bOverload = true;

                arPeakOverloadFlag[1] = true;
                Events.Audio.VuMeter.PeakOverloadEventArgs e;
                if (boolPlayer)
                {
                    e = new Events.Audio.VuMeter.PeakOverloadEventArgs(2,
                        ob_AudioPlayer.GetCurrentBytePosition(), ob_AudioPlayer.GetCurrentTimePosition());
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

            // compute the cordinates of graph and animation
            DisplayGraph();

            int ThresholdFactor = 12500 / (m_UpperThreshold - m_LowerThreshold);
            int DisplayAmpLeft = (m_MeanValueLeft * ThresholdFactor) / 100;
            int DisplayAmpRight = (m_MeanValueRight * ThresholdFactor) / 100;
            int Offset = 65 - ((m_LowerThreshold * ThresholdFactor) / 100);
            DisplayAmpLeft = DisplayAmpLeft + Offset;
            DisplayAmpRight = DisplayAmpRight + Offset;

            Graph.EraserLeft = OriginY + Convert.ToInt32(m_ScaleFactor * (254 - DisplayAmpLeft));
            Graph.EraserRight = OriginY + Convert.ToInt32(m_ScaleFactor * (254 - DisplayAmpRight));

            //Thread.Sleep (25) ;
            // Update ccurrent graph cordinates to VuMeter display
            if ( UpdateForms != null )
            UpdateForms(this, new Events.Audio.VuMeter.UpdateFormsEventArgs());
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
                        switch (m_Channels)
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

//            System.Diagnostics.Debug.WriteLine(leftVal.ToString().PadLeft(10, ' ') + " : " + rightVal.ToString().PadLeft(10, ' '));

            return arSum;
		}

		// calculates the values of graph cordinates with respect to position and scale factor
		public GraphPts Graph = new GraphPts() ;
		public void DisplayGraph	 ()
		{
			int OriginX = m_GraphPositionX -  Convert.ToInt32  ( ( m_ScaleFactor * 60 ));
			int OriginY =m_GraphPositionY- Convert.ToInt32 ( 125 * m_ScaleFactor) ;

			// Position of BackGround color of graph 
			Graph.BackGroundTop = OriginY - (Convert.ToInt32 (50 * m_ScaleFactor) ) ;
			Graph.BackGroundBottom = OriginY + (Convert.ToInt32 ( 304 * m_ScaleFactor)) ;
			Graph.BackGroundX= OriginX - (Convert.ToInt32(50 * m_ScaleFactor)) ;
			Graph.BackGroundWidth = Convert.ToInt32 ( 220 * m_ScaleFactor) ;

			// Position of tri colored vertical lines

			Graph.LineWidth = Convert.ToInt32 (30 * m_ScaleFactor) ;

			Graph.HighTop = OriginY ;
			Graph.HighBottom = OriginY + Convert.ToInt32 (m_ScaleFactor * 60) ;

			Graph.NormalTop = OriginY + Convert.ToInt32 (m_ScaleFactor * 67) ;
			Graph.NormalBottom = OriginY + Convert.ToInt32 (m_ScaleFactor * 187 ) ;

			Graph.LowTop  = OriginY + Convert.ToInt32 (m_ScaleFactor * 194) ;
			Graph.LowBottom = OriginY + Convert.ToInt32 (m_ScaleFactor * 254) ;

			Graph.LeftGraphX = OriginX ;
			Graph.RightGraphX = OriginX +Convert.ToInt32 (m_ScaleFactor * 60) ;

			Graph.PeakOverloadLightX = 240 ;
			Graph.PeakOverloadLightY = 30 ;
		}
		/*
				internal class Threshold
				{
			public int UpperThreshold ;
					public int LowerThreshold ;
				}
		*/
		public struct GraphPts
		{

			public int HighTop ;
			public int HighBottom ;

			public int NormalTop ;
			public int NormalBottom ;

			public int LowTop ;
			public int LowBottom ;

			public int LineWidth ;

			public int ScaleFactor ;
			public int LeftGraphX ;
			public int RightGraphX ;

			public int BackGroundWidth ;
			public int BackGroundTop ;
			public int BackGroundBottom ;
			public int BackGroundX ;

			public int EraserLeft ;
			public int EraserRight ;

			public int PeakOverloadLightX ;
			public int PeakOverloadLightY ;
		}
		// end of class
	}

}
