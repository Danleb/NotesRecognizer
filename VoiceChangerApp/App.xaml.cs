using NLog;
using Prism.Ioc;
using Prism.Unity;
using System.Runtime.InteropServices;
using System.Windows;
using VoiceChangerApp.Models;
using VoiceChangerApp.ViewModels;
using VoiceChangerApp.Views;

namespace VoiceChangerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private static Logger logger;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<SoundDataModel, SoundDataModel>();
            containerRegistry.Register<FileInfoViewModel, FileInfoViewModel>();
            containerRegistry.Register<EditorWindowViewModel, EditorWindowViewModel>();
            containerRegistry.Register<RawSoundViewModel, RawSoundViewModel>();
        }

        protected override Window CreateShell()
        {
#if DEBUG
            AllocConsole();
#endif
            logger = LogManager.GetCurrentClassLogger();
            logger.Info("App loaded.");

            var w = Container.Resolve<EditorWindow>();
            return w;
        }
    }
}
