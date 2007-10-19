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
    }
}