using System;

namespace AudioLib
{
    public class CalculationFunctions
    {
        public static long ConvertToDecimal(int[] Ar)
        {
            //convert from mod 256 to mod 10
            return Ar[0] + (Ar[1] * 256) + (Ar[2] * 256 * 256) + (Ar[3] * 256 * 256 * 256);
        }

        public static int[] ConvertFromDecimal(long lVal)
        {
            // convert  mod 10 to 4 byte array each of mod 256
            int[] Result = new int[4];
            Result[0] = Result[1] = Result[2] = Result[3] = 0;
            for (int i = 0; i < 4; i++)
            {
                Result[i] = Convert.ToInt32(lVal % 256);
                lVal = lVal / 256;
            }
            return Result;
        }
    }
}
