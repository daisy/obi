using System;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Summary description for CalculationFunctions.
	/// </summary>
	public class CalculationFunctions
	{

		public long AdaptToFrame  (long lVal, int FrameSize)
		{
			long  lTemp = lVal / FrameSize ;
			return lTemp * FrameSize ;
		}

		public long ConvertToDecimal (int [] Ar)
		{
			//convert from mod 256 to mod 10
			return Ar[0] + (Ar[1] * 256) + (Ar[2] *256 *256) + (Ar[3] *256 *256 *256) ;
		}
		
		public int [] ConvertFromDecimal (long lVal) 
		{
			// convert  mod 10 to 4 byte array each of mod 256
			int [] Result = new int [4] ;
			Result [0] = Result [1] = Result [2] = Result [3] = 0;
			for (int i = 0 ;i<4 ; i++)
			{
				Result [i] = Convert.ToInt32 (lVal % 256 );
				lVal = lVal / 256 ;
			}
			return  Result ;
		}



		// function for converting time into bytes
		public long ConvertTimeToByte (double dTime, int SamplingRate , int FrameSize)
		{
			return Convert.ToInt64(( dTime * SamplingRate  * FrameSize)/1000) ;
		}

		public double ConvertByteToTime (long lByte, int SamplingRate, int FrameSize)
		{
			long lTemp = (1000 * lByte) / (SamplingRate * FrameSize) ;
			return Convert.ToDouble (lTemp) ;
		}


		internal string GenerateNewAssetName ( IMediaAsset Asset)
		{
			
			long i = 0 ;

			string sTemp ;

			sTemp = "MediaAsset" ;
			string sTempName ;
			sTempName = sTemp + i.ToString () ;
AssetManager manager = Asset.Manager as AssetManager ;
			while ( manager.m_htExists.ContainsKey (sTempName)  && i<9000000)
			{

				i++;
				sTempName = sTemp + i.ToString () ;
				
			}

			if (i<9000000)
			{
manager.m_htExists.Add (sTempName , Asset) ;
				return sTempName ;
			}
			else
			{
				return null ;
			}
		}
		
			

		// end of cal function
	}
	// end of namespace
}
