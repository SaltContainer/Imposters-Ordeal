using System;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Handle unhandled exceptions
            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show($"An error occurred: {e.Exception.Message}\n\n{e.Exception.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show($"A fatal error occurred: {ex?.Message}\n\n{ex?.StackTrace}",
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            Application.Run(new MainForm());
        }
    }
}
