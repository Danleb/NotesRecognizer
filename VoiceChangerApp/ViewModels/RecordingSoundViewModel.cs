using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Linq;
using System.Threading;
using VoiceChangerApp.Models;

namespace VoiceChangerApp.ViewModels
{
    public class RecordingSoundViewModel : BindableBase
    {
        public RecordingSoundViewModel()
        {

        }

        public RecordingSoundViewModel(SoundDataModel soundDataModel)
        {
            SoundDataModel = soundDataModel;
        }

        private SoundDataModel _soundDataModel;
        public SoundDataModel SoundDataModel
        {
            get { return _soundDataModel; }
            set { SetProperty(ref _soundDataModel, value); }
        }
    }
}
