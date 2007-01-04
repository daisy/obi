using System;
using System.Windows.Forms;

namespace Obi
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ObiForm());
        }

        /// <summary>
        /// Convenience event handler doing nothing, useful for visitors.
        /// </summary>
        public static void Noop(object sender, EventArgs e)
        {
        }
    }
}