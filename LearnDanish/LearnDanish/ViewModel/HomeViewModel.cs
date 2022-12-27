using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using LearnDanish.ViewModel.Base;
using System.Windows.Input;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using LearnDanish.Services;

namespace LearnDanish.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        ITtsDataInstaller _ttsDataInstaller;
        IAudioRecorder _audioRecorder;

        string _filepath;

        public HomeViewModel()
        {
            _ttsDataInstaller = DependencyService.Resolve<ITtsDataInstaller>();
            _audioRecorder = DependencyService.Resolve<IAudioRecorder>();

            Title = "Home";
            Sentence = "En hund løber gennem gaderne i en lille by.";
            IsRecording = false;

            SpeakSentenceCommand = new Command(async () => await SpeakSentenceAsync(), () => !_isSpeaking);
            StartRecordingCommand = new Command(async () => await StartRecordingAsync(), () => !_isRecording);
            StopRecordingCommand = new Command(async () => await StopRecordingAsync(), () => _isRecording);
        }

        public Command SpeakSentenceCommand { get; set; }
        public Command StartRecordingCommand { get; set; }
        public Command StopRecordingCommand { get; set; }

        private bool _isSpeaking;
        public bool IsSpeaking
        {
            get { return _isSpeaking; }
            set
            {
                _isSpeaking = true;
                OnPropertyChanged(nameof(IsSpeaking));
            }
        }

        private int _countSeconds;
        public int CountSeconds
        {
            get { return _countSeconds; }
            set
            {
                _countSeconds = value;
                OnPropertyChanged(nameof(CountSeconds));
            }
        }

        private string _sentence;
        public string Sentence
        {
            get { return _sentence; }
            set
            {
                _sentence = value;
                OnPropertyChanged(nameof(Sentence));
            }
        }

        private bool _isRecording;
        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                _isRecording = value;
                OnPropertyChanged(nameof(IsRecording));
            }
        }

        private CancellationTokenSource timerCancellationToken = null;

        public Task SpeakSentenceAsync()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var locales = await TextToSpeech.GetLocalesAsync();
                var locale = locales.FirstOrDefault(x => x.Language == "da");

                if (locale == null)
                {
                    _ttsDataInstaller.InstallTtsData();
                }
                else
                {
                    await TextToSpeech.SpeakAsync(Sentence, new SpeechOptions
                    {
                        Locale = locale
                    });
                }
            });

            return Task.FromResult(0);
        }

        public async Task StartRecordingAsync()
        {
            if (timerCancellationToken != null)
                return;

            var microphoneStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (microphoneStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                if(microphoneStatus != PermissionStatus.Granted)
                    await Permissions.RequestAsync<Permissions.Microphone>();
                if (storageStatus != PermissionStatus.Granted)
                    await Permissions.RequestAsync<Permissions.StorageWrite>();
                return;
            }

            timerCancellationToken = new CancellationTokenSource();

            _filepath = await _audioRecorder.StartRecordingAudio("recording");

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                CountSeconds = CountSeconds + 1;
                if(_countSeconds > 20)
                {
                    StopRecordingCommand.Execute(null);
                }

                var isCancelled = timerCancellationToken.IsCancellationRequested;
                if (isCancelled)
                {
                    timerCancellationToken = null;
                    return false;
                }
                return true;
            });

            IsRecording = true;
        }

        public async Task StopRecordingAsync()
        {
            await _audioRecorder.StopRecordingAudio(_filepath);

            timerCancellationToken.Cancel();
            CountSeconds = 0;
            IsRecording = false;
        }
    }
}
