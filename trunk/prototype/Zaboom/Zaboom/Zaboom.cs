using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Zaboom
{
    static class Zaboom
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ZaboomForm());
        }
    }
}