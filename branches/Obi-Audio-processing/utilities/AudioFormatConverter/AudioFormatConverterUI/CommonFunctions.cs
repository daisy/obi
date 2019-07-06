using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AudioFormatConverterUI
    {
    public class CommonFunctions
        {
        /// <summary>
        /// create xml document instance and load xml file in it
        /// </summary>
        /// <param name="path"></param>
        /// <returns> instance of XmlDocument </returns>
        public static XmlDocument CreateXmlDocument ( string path )
            {
            XmlTextReader Reader = null;
            XmlDocument XmlDoc = null;

            // create xml reader and load xml document
            try
                {
                Reader = new XmlTextReader ( path );
                Reader.XmlResolver = null;

                XmlDoc = new XmlDocument ();
                XmlDoc.XmlResolver = null;
                XmlDoc.Load ( Reader );
                }
            finally
                {
                Reader.Close ();
                Reader = null;
                }
            return XmlDoc;
            }


        /// <summary>
        /// write xml document in file passed as parameter
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="path"></param>
        public static void WriteXmlDocumentToFile ( XmlDocument xmlDoc, string path )
            {
            XmlTextWriter writer = null;
            try
                {
                writer = new XmlTextWriter ( path, null );
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save ( writer );
                }
            finally
                {
                writer.Close ();
                writer = null;
                }
            }

        /// <summary>
        /// converts DTB string representation of time into time span
        /// </summary>
        /// <param name="timeString"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan ( string timeString )
            {
            
            if (timeString.EndsWith ( "ms" ))
                {
                double temp = Convert.ToDouble ( timeString.Replace ( "ms", "" ) );
                return new TimeSpan ( (long)temp * 10000 );
                }
            else if (timeString.StartsWith ( "npt=" ))
                {
                double temp = double.Parse ( timeString.Replace ( "s", "" ) );
                return new TimeSpan ( (long)temp * 10000000 );
                }
            else
                {
                return TimeSpan.Parse ( timeString );
                }
            }

        public static string GetStringTotalTimeRoundedOff ( TimeSpan time )
            {
            string strMS = time.Milliseconds.ToString ();
            int compareDigit = 0;
            int.TryParse ( strMS.Substring ( 0, 1 ), out compareDigit );
            if (compareDigit >= 5)
                {
                TimeSpan additiveSpan = new TimeSpan ( Convert.ToInt64 ( .5 * 10000000 ) );
                time = time.Add ( additiveSpan );
                }
            return time.ToString ().Split ( '.' )[0];
            }

        }
    }
