using System;
using System.IO;
using System.Windows.Forms;

namespace Obi
{
    static class Program
    {
        internal static readonly string OBI_NS = "http://www.daisy.org/urakawa/obi";  // Obi namespace URI
        
        /// <summary>
        /// The main entry point for the application.
        /// Open the first file given as argument, or just start.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
                Application.Run(new ObiForm());
            }
            else
            {
                Application.Run(new ObiForm(args[0]));
            }
        }

        /// <summary>
        /// Convenience event handler doing nothing, useful for visitors.
        /// </summary>
        public static void Noop(object sender, EventArgs e)
        {
        }
    }
}