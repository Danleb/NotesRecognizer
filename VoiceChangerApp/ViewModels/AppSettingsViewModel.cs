using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
using VoiceChanger.FormatParser;
using VoiceChangerApp.Models;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.ViewModels
{
    public class AppSettingsViewModel : BindableBase
    {
        public AppSettingsViewModel()
        {
            SwitchTheme = new DelegateCommand(() =>
            {
                var app = (App)Application.Current;
                app.ChangeTheme(new Uri("pack://application:,,,/Styles/DarkTheme.xaml"));
            });
        }

        #region Commands

        public DelegateCommand SwitchTheme { get; }

        #endregion

        #region Properties



        #endregion
    }
}
