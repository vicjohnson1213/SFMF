using SFMFManager.Util;
using System.IO;
using System.Windows;

namespace SFMFLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            using (var w = File.AppendText("errors.log"))
            {
                w.WriteLine(e.Exception.Message);
                w.WriteLine(e.Exception.StackTrace);
                w.WriteLine();
            }

            e.Handled = false;
        }
    }
}
