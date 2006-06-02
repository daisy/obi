using System;
using System.Threading;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using UrakawaApplicationBackend.events.audioPlayerEvents ;
using UrakawaApplicationBackend.events.vuMeterEvents ;

namespace UrakawaApplicationBackend
{

	/// <summary>
	/// Stub for the VU Meter class
	/// </summary>
	public class VuMeter : IVuMeter
	{

		public VuMeter  ()
		{
			//m_SampleCount = Convert.ToInt32 ( m_SampleTimeLength / 50) ;
			m_SampleCount = 40 ;
			SampleArrayLeft  = new int [ 2 * m_SampleCount] ;
			SampleArrayRight  = new int [2 * m_SampleCount] ;
			m_SampleArrayPosition = 0 ;
			ob_UpdateForms.UpdateFormsEvent += new DUpdateFormsEvent ( ob_VuMeterForm.CatchUpdateForms ) ;

		}
		//Member variable used in properties
		private int m_Channels ;
		private double m_ScaleFactor = 1;
		private double m_SampleTimeLength ;
UpdateForms ob_UpdateForms = new UpdateForms () ;


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

		public double[] PeakValue
		{
			get
			{
				return null;
			}
		}

		public bool[] Overloaded
		{
			get
			{
				return null;
			}
		}
Threshold m_Threshold ;
		// properties added by app team india
		public Threshold AmplitudeThreshold
		{
			get
			{
return m_Threshold ;
			}
			set
			{
m_Threshold = value ;
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
				m_SampleTimeLength = value ;
			}
		}


		public void Reset()
		{

		}

		// member variables for internal processing
		private int [] m_arUpdatedVM  = new int  [4] ;
		private int m_FrameSize ;

		private int m_GraphPositionX = 150 ;
		private int m_GraphPositionY = 300 ;
			

private AudioPlayer ob_AudioPlayer ;
		private int  m_SampleCount  = 40 ;
		internal int m_MeanValueLeft ;
		internal int m_MeanValueRight  ;
		private  int [] SampleArrayLeft ;
		private int [] SampleArrayRight ;
		private int m_SampleArrayPosition = 0;

		public void CatchUpdateVuMeterEvent ( AudioPlayer Player , UpdateVuMeter Update) 
		{
			ob_AudioPlayer = Player ;
			m_FrameSize = Player.m_FrameSize ;
			m_Channels = Player.m_Channels ;

			for (int i = 0 ; i< 4 ; i++)
				m_arUpdatedVM [i] = Player.arUpdateVM [i] ;  
		
			Thread UpdateVMForm = new Thread(new ThreadStart (AnimationComputation  ));
			UpdateVMForm.Start()  ;

		}

		int m_UpperThreshold = 100 ;
		int m_LowerThreshold = 50 ;
		int m_PeakValueLeft = 0;
		int m_PeakValueRight = 0;
		void AnimationComputation ()
		{
			//Thread.Sleep (25) ;

			int OriginX = m_GraphPositionX -  Convert.ToInt32  ( ( m_ScaleFactor * 60 ));
			int OriginY =m_GraphPositionY- Convert.ToInt32 ( 125 * m_ScaleFactor) ;

			int [] AmpArray = new int [2] ;
			Array.Copy ( AmplitudeValue() , AmpArray , 2) ;

			//m_MeanValueLeft  = AmpArray [0] ;
			//m_MeanValueRight = AmpArray [1] ;


			SampleArrayLeft [m_SampleArrayPosition ] = AmpArray [0] ;
			SampleArrayRight [m_SampleArrayPosition ] = AmpArray [1] ;

			if (m_PeakValueLeft < m_MeanValueLeft)  
				m_PeakValueLeft = m_MeanValueLeft ;

			if (m_PeakValueRight < m_MeanValueRight )  
				m_PeakValueRight = m_MeanValueRight ;


			m_SampleArrayPosition++ ;
			if (m_SampleArrayPosition >= m_SampleCount )
			{
				m_SampleArrayPosition = 0 ;
				
			}

			// Find Mean Values of Left and Right Channels 
			m_MeanValueLeft = m_MeanValueRight = 0 ;
			for (int i = 0 ; i < m_SampleCount ; i++ )
			{
				m_MeanValueLeft = m_MeanValueLeft + SampleArrayLeft [i] ;
				m_MeanValueRight = m_MeanValueRight + SampleArrayRight [i] ;
			}
			m_MeanValueLeft = m_MeanValueLeft / m_SampleCount ;
			m_MeanValueRight = m_MeanValueRight / m_SampleCount ;
	
 // Check for Peak Overload ;
			if ( m_MeanValueLeft > 100 )
			{

PeakOverload ob_PeakOverload = new PeakOverload ( 1,ob_AudioPlayer.GetCurrentBytePosition () , ob_AudioPlayer.GetCurrentTimePosition () ) ;

ob_PeakOverload.PeakOverloadEvent += new DPeakOverloadEvent ( ob_VuMeterForm.CatchPeakOverloadEvent ) ;

ob_PeakOverload.NotifyPeakOverload ( this , ob_PeakOverload) ;
				
			}

			if ( m_MeanValueRight > 100 )
			{
				PeakOverload ob_PeakOverload = new PeakOverload ( 2 ,ob_AudioPlayer.GetCurrentBytePosition () , ob_AudioPlayer.GetCurrentTimePosition () ) ;

				ob_PeakOverload.PeakOverloadEvent += new DPeakOverloadEvent ( ob_VuMeterForm.CatchPeakOverloadEvent ) ;

				ob_PeakOverload.NotifyPeakOverload ( this , ob_PeakOverload) ;
				
			}

			if ( m_SampleArrayPosition == (m_SampleCount  - 1 ))
			{
ob_VuMeterForm.txtAmplitudeLeft.Text = m_MeanValueLeft.ToString ()  ;
ob_VuMeterForm.txtAmplitudeRight.Text = m_MeanValueRight.ToString () ;
			}


			//			int LeftGraphX= OriginX ;
			//int RightGraphX = OriginX +Convert.ToInt32 (m_ScaleFactor * 90) ;
			DisplayGraph () ;

			int ThresholdFactor  = 12500 / ( m_UpperThreshold - m_LowerThreshold) ;
			int DisplayAmpLeft = (m_MeanValueLeft * ThresholdFactor )/100 ;
			int DisplayAmpRight = (m_MeanValueRight  * ThresholdFactor )/100 ;
			int Offset = 65 - ( (m_LowerThreshold * ThresholdFactor) / 100 ) ;
			DisplayAmpLeft = DisplayAmpLeft + Offset ;
			DisplayAmpRight = DisplayAmpRight + Offset ;

			Graph.EraserLeft = OriginY + Convert.ToInt32 (m_ScaleFactor * ( 254 - DisplayAmpLeft) ) ;
			Graph.EraserRight= OriginY + Convert.ToInt32 (m_ScaleFactor * ( 254 - DisplayAmpRight)) ;

//Thread.Sleep (25) ;
			
ob_UpdateForms.NotifyUpdateForms ( this , ob_UpdateForms ) ;

		}

		int [] AmplitudeValue()
		{

			int Sum0 = 0 ;
			int Sum1 = 0 ;

			if (m_FrameSize == 1)
				Sum0 = m_arUpdatedVM[0] ;
			else if (m_FrameSize == 2)
			{
				if (m_Channels == 2)
				{
					Sum0 = (m_arUpdatedVM [0]  ) ;
					Sum1 = m_arUpdatedVM [1] ;
					
				}
				else if (m_Channels == 1)
				{
					Sum0 = m_arUpdatedVM[1] * 256 ;
					Sum0 = Sum0 + m_arUpdatedVM[0] ;
					Sum0 = (Sum0 * 256 ) / 65792 ;
				}
				// / end of if (FrameSize == 2)
			}
			else if (m_FrameSize == 4 && m_Channels == 2 )
			{
				Sum0 = m_arUpdatedVM[1] * 256 ;
				Sum0 = Sum0 + m_arUpdatedVM [0] ;
				Sum0 = (Sum0 * 256 ) /65792  ;

				Sum1 = m_arUpdatedVM[3] * 256 ;
				Sum1 = Sum1 + m_arUpdatedVM [2] ;
				Sum1 = (Sum1 * 256 ) / 65792  ;

			}
			int [] arSum= new int [2] ;
			arSum [0] = Sum0 ;
			arSum[1] = Sum1 ;

			return arSum ;
		}
		public int Counter = 0 ;
		VuMeterForm ob_VuMeterForm = new VuMeterForm () ;

		public void ShowForm ()
		{
			ob_VuMeterForm.Show () ;
		}

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
			Graph.RightGraphX = OriginX +Convert.ToInt32 (m_ScaleFactor * 90) ;

		
		}

		public struct Threshold
		{
	public int UpperThreshold ;
			public int LowerThreshold ;
		}

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

		}

	// end of class
}

}
