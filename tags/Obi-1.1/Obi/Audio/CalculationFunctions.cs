using System;
using System.IO;

namespace Obi.Audio
{
	/// <summary>
	/// Summary description for CalculationFunctions.
	/// </summary>
	public class CalculationFunctions
	{

		public static long AdaptToFrame  (long lVal, int FrameSize)
		{
			long  lTemp = lVal / FrameSize ;
			return lTemp * FrameSize ;
		}

		public static long ConvertToDecimal (int [] Ar)
		{
			//convert from mod 256 to mod 10
			return Ar[0] + (Ar[1] * 256) + (Ar[2] *256 *256) + (Ar[3] *256 *256 *256) ;
		}
		
		public static int [] ConvertFromDecimal (long lVal) 
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
		public static long ConvertTimeToByte (double dTime, int SamplingRate , int FrameSize)
		{
			return Convert.ToInt64(( dTime * SamplingRate  * FrameSize)/1000) ;
		}

		public static double ConvertByteToTime (long lByte, int SamplingRate, int FrameSize)
		{
			long lTemp = (1000 * lByte) / (SamplingRate * FrameSize) ;
			return Convert.ToDouble (lTemp) ;
		}

        // end of cal function
	}
	// end of namespace
}
