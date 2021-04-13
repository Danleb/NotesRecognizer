using System.Configuration;
using System.Reactive.Subjects;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Models
{
    public class UserPreferencesModel : IUserPreferencesModel
    {
        private const string _themeKey = "Theme";
        private const string _workDirectoryKey = "WorkDirectory";
        private Configuration _config;

        public UserPreferencesModel()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = _config.AppSettings.Settings;

            WorkDirectory = appSettings[_workDirectoryKey]?.Value;
            OnWorkDirectoryChanged.OnNext(WorkDirectory);

            SetWorkDirectory.SubscribeAsync(v =>
            {
                _config.AppSettings.Settings.Add(_workDirectoryKey, v);
                AfterValueChanged();
            });
        }

        #region Properties

        public SoundSource LastSoundSourceTab { get; private set; }

        public Theme Theme { get; private set; }

        public string WorkDirectory { get; private set; }

        #endregion

        #region Commands

        public Subject<Theme> SetTheme { get; } = new();
        public Subject<SoundSource> SetSoundSource { get; } = new();
        public Subject<string> SetWorkDirectory { get; } = new();

        #endregion

        #region Events

        public Subject<Theme> OnThemeChanged { get; } = new();
        public Subject<SoundSource> OnSoundSourceChanged { get; } = new();
        public BehaviorSubject<string> OnWorkDirectoryChanged { get; } = new(string.Empty);

        #endregion

        private void AfterValueChanged()
        {
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(_config.AppSettings.SectionInformation.Name);
        }
    }
}
