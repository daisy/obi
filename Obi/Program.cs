using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Obi
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Open the first file given as argument, or just start.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
            {
            //language switch, loads culture from settings 
            // strCulture is "hi-IN" for hindi, "en-US" for english
                Settings settings = Settings.GetSettings();
            string strCulture = settings.UserProfile.Culture.Name;

            //if ( !string.IsNullOrEmpty (strCulture )
                //&& (strCulture == "en-US" || strCulture == "hi-IN" || strCulture == "fr-FR"))
                                //{
            bool errorInSettingCulture = false;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(strCulture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(strCulture);
            }
            catch (System.Exception)
            {
                errorInSettingCulture = true;
                Console.WriteLine("error in setting culture: " + strCulture);
            }
            if (errorInSettingCulture)
            {
                string defaultCulture = "en-US";
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(defaultCulture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(defaultCulture);
                settings.UserProfile.Culture = System.Globalization.CultureInfo.GetCultureInfo(defaultCulture);
                try
                {
                    settings.SaveSettings();
                }
                catch (System.Exception) { Console.WriteLine("error in saving default culture in settings file"); }
            }
                //}
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault ( false );
            Application.Run ( args.Length == 0 ? new ObiForm () : new ObiForm ( args[0] ) );
            }


        /// <summary>
        /// Format a duration in milliseconds in a verbose way.
        /// </summary>
        public static string FormatDuration_Long(double durationMs)
        {
            double seconds = durationMs / 1000.0;
            if (seconds < 60.0)
            {
                return string.Format(Localizer.Message("long_s_ms"),
                    seconds, Localizer.Message(seconds > 1 ? "seconds" : "second"));
            }
            else
            {
                int minutes = (int)Math.Floor(seconds / 60.0);
                int seconds_ = (int)Math.Round(seconds - minutes * 60.0);
                if (minutes < 60)
                {
                    return string.Format(Localizer.Message("long_mm_ss"),
                        minutes, Localizer.Message(minutes > 1 ? "minutes" : "minute"),
                        string.Format(seconds_ == 0 ? "" : string.Format(Localizer.Message("long_addl_time"),
                            seconds_, Localizer.Message(seconds_ > 1 ? "seconds" : "second"))));
                }
                else
                {
                    int hours = minutes / 60;
                    int minutes_ = minutes % 60;
                    return string.Format(Localizer.Message("long_h_mm_ss"),
                        hours, Localizer.Message(hours > 1 ? "hours" : "hour"),
                        string.Format(minutes_ == 0 ? "" : string.Format(Localizer.Message("long_addl_time"),
                            minutes_, Localizer.Message(minutes_ > 1 ? "minutes" : "minute"))),
                        string.Format(seconds_ == 0 ? "" : string.Format(Localizer.Message("long_addl_time"),
                            seconds_, Localizer.Message(seconds_ > 1 ? "seconds" : "second"))));
                }
            }
        }

        /// <summary>
        /// Format a duration in milliseconds in a smart way.
        /// </summary>
        public static string FormatDuration_Smart(double durationMs)
        {
            double seconds = durationMs / 1000.0;
            if (seconds < 60.0)
            {
                return string.Format(Localizer.Message("duration_s_ms"), seconds);
            }
            else
            {
                int minutes = (int)Math.Floor(seconds / 60.0);
                int seconds_ = (int)Math.Round(seconds - minutes * 60.0);
                if (minutes < 60)
                {
                    return string.Format(Localizer.Message("duration_mm_ss"), minutes, seconds_);
                }
                else
                {
                    return string.Format(Localizer.Message("duration_h_mm_ss"), minutes / 60, minutes % 60, seconds_);
                }
            }
        }


        /// <summary>
        /// Get a safename for the project directory from the title.
        /// </summary>
        public static string SafeName(string title)
        {
            string invalid = "[";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) invalid += String.Format("\\x{0:x2}", (int)c);
            invalid += "]+";
            string safe = Regex.Replace(title, invalid, "_");
            safe = Regex.Replace(safe, "^_", "");
            safe = Regex.Replace(safe, "_$", "");
            safe = Regex.Replace ( safe, ",", "_" );
            return safe;
        }

        public static string GetObiRoamingUserDirectory()
        {
            string appDataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string obiRoamingDir = System.IO.Path.Combine(appDataDir, "Obi");
            if (!System.IO.Directory.Exists(obiRoamingDir)) System.IO.Directory.CreateDirectory(obiRoamingDir);
            return obiRoamingDir;
        }

    }
}