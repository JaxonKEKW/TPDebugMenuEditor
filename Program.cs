using System.Runtime.InteropServices;

namespace TPMenuEditor
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
            static extern bool ShouldSystemUseDarkMode();
            //if (ShouldSystemUseDarkMode())
            //{
                Application.Run(new Form1());
            //}
            //else
            //{
            //    Application.Run(new Form2());
            //}
        }
    }
}