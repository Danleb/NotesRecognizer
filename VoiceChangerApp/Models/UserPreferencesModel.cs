using System.Configuration;
using System.Reactive.Subjects;
using System.Threading;
using VoiceChangerApp.Utils;

namespace VoiceChangerApp.Models
{
    public class UserPreferencesModel : IUserPreferencesModel
    {
        private Configuration _config;
        private Mutex _configSaveMutex = new();

        public UserPreferencesModel()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = _config.AppSettings.Settings;

            WorkDirectory = appSettings[UserPreferencesKeys.WorkDirectoryKey]?.Value;
            OnWorkDirectoryChanged.OnNext(WorkDirectory);

            LastOpenedFile = appSettings[UserPreferencesKeys.LastOpenedFileKey]?.Value;
            OnLastOpenedFileChanged.OnNext(LastOpenedFile);

            SetWorkDirectory.SubscribeAsync(v => UpdatePreference(UserPreferencesKeys.WorkDirectoryKey, v));
            SetLastOpenedFile.SubscribeAsync(v => UpdatePreference(UserPreferencesKeys.LastOpenedFileKey, v));
        }

        #region Properties

        public SoundSource LastSoundSourceTab { get; private set; }

        public Theme Theme { get; private set; }

        public string WorkDirectory { get; private set; }

        public string LastOpenedFile { get; private set; }

        #endregion

        #region Commands

        public Subject<Theme> SetTheme { get; } = new();
        public Subject<SoundSource> SetSoundSource { get; } = new();
        public Subject<string> SetWorkDirectory { get; } = new();
        public Subject<string> SetLastOpenedFile { get; } = new();

        #endregion

        #region Events

        public Subject<Theme> OnThemeChanged { get; } = new();
        public Subject<SoundSource> OnSoundSourceChanged { get; } = new();
        public BehaviorSubject<string> OnWorkDirectoryChanged { get; } = new(string.Empty);
        public BehaviorSubject<string> OnLastOpenedFileChanged { get; } = new(string.Empty);

        #endregion

        private void UpdatePreference(string key, string value)
        {
            SetPreferenceValue(key, value);
            AfterValueChanged();
        }

        private void AfterValueChanged()
        {
            _configSaveMutex.WaitOne();
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(_config.AppSettings.SectionInformation.Name);
            _configSaveMutex.ReleaseMutex();
        }

        private void SetPreferenceValue(string key, string value)
        {
            if (_config.AppSettings.Settings[key] == null)
            {
                _config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                _config.AppSettings.Settings[key].Value = value;
            }
        }
    }
}
