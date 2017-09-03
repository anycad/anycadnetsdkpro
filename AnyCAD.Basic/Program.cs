using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AnyCAD.Basic
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
            AnyCAD.Platform.GlobalInstance.Application.SetLogFileName(new AnyCAD.Platform.Path("anycad.net.sdk.log"));
            Application.Run(new FormMain());
        }
    }
}
