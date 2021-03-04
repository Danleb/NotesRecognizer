using NLog;
using System.Runtime.InteropServices;
using System.Windows;

namespace VoiceChangerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger;

        void App_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
            AllocConsole();
#endif
            logger = LogManager.GetCurrentClassLogger();
            logger.Info("App loaded.");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
