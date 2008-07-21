using System;
using System.IO;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(args.Length == 0 ? new ObiForm() : new ObiForm(args[0]));
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
    }
}