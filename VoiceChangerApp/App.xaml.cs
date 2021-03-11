using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Prism.Unity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
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
        private static ILogger _logger;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<SoundDataModel, SoundDataModel>();
            containerRegistry.Register<FileInfoViewModel, FileInfoViewModel>();
            containerRegistry.Register<EditorWindowViewModel, EditorWindowViewModel>();
            containerRegistry.Register<RawSoundViewModel, RawSoundViewModel>();
            var container = (UnityContainer)containerRegistry.GetContainer();

            var loggerFactory = new LoggerFactory().AddNLog();
            var factoryMethod = typeof(LoggerFactoryExtensions).
                          GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                          .First(x => x.ContainsGenericParameters);
            container.RegisterInstance(loggerFactory, new ContainerControlledLifetimeManager());
            container.RegisterFactory(typeof(ILogger<>), null, (c, t, n) =>
            {
                var factory = c.Resolve<ILoggerFactory>();
                var genericType = t.GetGenericArguments().First();
                var mi = typeof(LoggerFactoryExtensions).GetMethods().Single(m => m.Name == "CreateLogger" && m.IsGenericMethodDefinition);
                var gi = mi.MakeGenericMethod(t.GetGenericArguments().First());
                return gi.Invoke(null, new[] { factory });
            });

            _logger = container.Resolve<ILogger<App>>();
        }

        protected override Window CreateShell()
        {
#if DEBUG
            AllocConsole();
#endif
            _logger.LogInformation("App loaded.");
            var w = Container.Resolve<EditorWindow>();
            return w;
        }
    }
}
