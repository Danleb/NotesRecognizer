using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Unity;
using Unity.Lifetime;
using VoiceChangerApp.Models;
using VoiceChangerApp.ViewModels;
using VoiceChangerApp.Views;
using VoiceChangerApp.Views.DataSourceViews;

namespace VoiceChangerApp
{
    public partial class App : PrismApplication
    {
        private static ILogger _logger;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterModel(containerRegistry);
            RegisterViewModel(containerRegistry);
            RegisterView();
            RegisterGenericLogger(containerRegistry);
        }

        private static void RegisterView()
        {
            ViewModelLocationProvider.Register(typeof(DataSourceView).ToString(), typeof(DataSourceViewModel));
            ViewModelLocationProvider.Register(typeof(SoundGenerationView).ToString(), typeof(SoundGenerationViewModel));
            ViewModelLocationProvider.Register(typeof(WaveletGenerationView).ToString(), typeof(WaveletGenerationViewModel));
        }

        private static void RegisterViewModel(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<DataSourceViewModel, DataSourceViewModel>();
            containerRegistry.Register<EditorWindowViewModel, EditorWindowViewModel>();
            containerRegistry.Register<RawSoundViewModel, RawSoundViewModel>();
            containerRegistry.Register<WaveletGenerationViewModel, WaveletGenerationViewModel>();
        }

        private static void RegisterModel(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IErrorModel, ErrorModel>();
            containerRegistry.RegisterSingleton<SoundDataModel, SoundDataModel>();
            containerRegistry.RegisterSingleton<UserPreferencesModel, UserPreferencesModel>();
            containerRegistry.RegisterSingleton<ScalogramModel, ScalogramModel>();
        }

        private static void RegisterGenericLogger(IContainerRegistry containerRegistry)
        {
            var container = (UnityContainer)containerRegistry.GetContainer();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddNLog());
            var factoryMethod = typeof(LoggerFactoryExtensions).
                          GetMethods(BindingFlags.Static | BindingFlags.Public)
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

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            //{
            //    var viewName = viewType.FullName.Replace(".ViewModels.", ".CustomNamespace.");
            //    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            //    var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
            //    return Type.GetType(viewModelName);
            //});
        }
    }
}
