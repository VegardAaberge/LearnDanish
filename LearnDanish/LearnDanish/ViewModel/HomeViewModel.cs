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
using LearnDanish.Domain;
using LearnDanish.Domain.Models;

namespace LearnDanish.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private ITtsDataInstaller _ttsDataInstaller;
        private IAudioRecorder _audioRecorder;
        private IRecordingService _recordingService;

        private string _filepath;
        private bool _isSpeaking;
        private int _countSeconds;
        private string _sentence;
        private bool _isRecording;

        private CancellationTokenSource _timerCancellationToken = null;

        public HomeViewModel(
            IRecordingService recordingService,
            ITtsDataInstaller ttsDataInstaller,
            IAudioRecorder audioRecorder)
        {
            _recordingService = recordingService;
            _ttsDataInstaller = ttsDataInstaller;
            _audioRecorder = audioRecorder;

            Title = "Home";
            Sentence = "En hund løber gennem gaderne i en lille by.";
            IsRecording = false;

            SpeakSentenceCommand = new Command(async () => await SpeakSentenceAsync(), () => !_isSpeaking);
            StartRecordingCommand = new Command(async () => await StartRecordingAsync(), () => !_isRecording);
            StopRecordingCommand = new Command(async () => await StopRecordingAsync(), () => _isRecording);
            NewSentenceCommand = new Command(async () => await NewSentenceAsync());
        }

        public Command SpeakSentenceCommand { get; set; }
        public Command StartRecordingCommand { get; set; }
        public Command StopRecordingCommand { get; set; }
        public Command NewSentenceCommand { get; set; }

        
        public bool IsSpeaking
        {
            get { return _isSpeaking; }
            set
            {
                _isSpeaking = true;
                OnPropertyChanged(nameof(IsSpeaking));
            }
        }
        
        public int CountSeconds
        {
            get { return _countSeconds; }
            set
            {
                _countSeconds = value;
                OnPropertyChanged(nameof(CountSeconds));
            }
        }
        
        public string Sentence
        {
            get { return _sentence; }
            set
            {
                _sentence = value;
                OnPropertyChanged(nameof(Sentence));
            }
        }
        
        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                _isRecording = value;
                OnPropertyChanged(nameof(IsRecording));
            }
        }

        public async Task SpeakSentenceAsync()
        {
            var speechStatus = await Permissions.CheckStatusAsync<Permissions.Speech>();
            if(speechStatus != PermissionStatus.Granted)
            {
                speechStatus = await Permissions.RequestAsync<Permissions.Speech>();
                if (speechStatus != PermissionStatus.Granted)
                    return;
            }

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
        }

        public async Task StartRecordingAsync()
        {
            if (_timerCancellationToken != null)
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

            _timerCancellationToken = new CancellationTokenSource();

            _filepath = await _audioRecorder.StartRecordingAudio("recording");

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                CountSeconds = CountSeconds + 1;
                if(_countSeconds > 20)
                {
                    StopRecordingCommand.Execute(null);
                }

                var isCancelled = _timerCancellationToken.IsCancellationRequested;
                if (isCancelled)
                {
                    _timerCancellationToken = null;
                    return false;
                }
                return true;
            });

            IsRecording = true;
        }

        public async Task StopRecordingAsync()
        {
            await _audioRecorder.StopRecordingAudio(_filepath);

            _timerCancellationToken.Cancel();
            CountSeconds = 0;
            IsRecording = false;
        }

        public async Task NewSentenceAsync()
        {
            await _recordingService.InsertRecordingAsync(
                new Recording
                {
                    FilePath = _filepath,
                    Sentence = _sentence,
                    Created = DateTime.Now
                }
            );

            _filepath = "";
            _timerCancellationToken = null;

            IsSpeaking = false;
            CountSeconds = 0;
            IsRecording = false;
            Sentence = "New Sentence";

            var recordings = await _recordingService.GetRecordingsAsync();
        }
    }
}
