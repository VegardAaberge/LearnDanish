using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using SpeakDanish.ViewModel.Base;
using System.Windows.Input;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using SpeakDanish.View;
using SpeakDanish.Services;
using SpeakDanish.Domain;
using SpeakDanish.Domain.Models;
using Timer = System.Timers.Timer;
using SpeakDanish.Helpers;
using System.Reflection;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.CommunityToolkit.Extensions;
using SpeakDanish.Services.Enums;

namespace SpeakDanish.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private ITtsDataInstaller _ttsDataInstaller;
        private IAudioRecorder _audioRecorder;
        private IRecordingService _recordingService;
        private IAlertService _alertService;
        private INavigation _navigation;

        private string _filepath;
        private bool _isSpeaking;
        private int _countSeconds;
        private string _sentence;
        private bool _isRecording;
        private string _volumeIcon;

        private int _volumeCounter = 1;
        private Timer _volumeTimer = new Timer(300);
        private Timer _countdownTimer = new Timer(1000);

        private CancellationTokenSource _cancelSpeakTokenSource = null;

        public HomeViewModel(
            IRecordingService recordingService,
            ITtsDataInstaller ttsDataInstaller,
            IAudioRecorder audioRecorder,
            IAlertService alertService,
            INavigation navigation)
        {
            _recordingService = recordingService;
            _ttsDataInstaller = ttsDataInstaller;
            _audioRecorder = audioRecorder;
            _alertService = alertService;
            _navigation = navigation;

            Title = "Home";
            Sentence = "En hund løber gennem gaderne i en lille by. ";
            IsRecording = false;

            SpeakSentenceCommand = new Command(async () => await SpeakSentenceAsync(), () => !_isSpeaking);
            StartRecordingCommand = new Command(async () => await StartRecordingAsync(), () => !_isRecording);
            StopRecordingCommand = new Command(async () => await StopRecordingAsync(), () => _isRecording);
            NewSentenceCommand = new Command(async () => await NewSentenceAsync());
            NavigateToRecordingsCommand = new Command(async () => await NavigateToRecordingsAsync());

            VolumeIcon = MaterialDesignIconsFont.VolumeHigh;
        }

        public Command SpeakSentenceCommand { get; set; }
        public Command StartRecordingCommand { get; set; }
        public Command StopRecordingCommand { get; set; }
        public Command NewSentenceCommand { get; set; }
        public Command NavigateToRecordingsCommand { get; set; }


        public string VolumeIcon
        {
            get { return _volumeIcon; }
            set
            {
                _volumeIcon = value;
                OnPropertyChanged(nameof(VolumeIcon));
            }
        }

        public bool IsSpeaking
        {
            get { return _isSpeaking; }
            set
            {
                _isSpeaking = value;
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
            _cancelSpeakTokenSource?.Cancel();

            var locales = await TextToSpeech.GetLocalesAsync();
            var locale = locales.FirstOrDefault(x => x.Language == "da");

            if (locale == null)
            {
                _ttsDataInstaller.InstallTtsData();

                locales = await TextToSpeech.GetLocalesAsync();
                locale = locales.FirstOrDefault(x => x.Language == "da");

                if(locale == null)
                {
                    await _alertService.ShowToast("No Danish language found");
                    return;
                }
            }

            try
            {
                _cancelSpeakTokenSource = new CancellationTokenSource();
                _volumeTimer.Stop();
                _volumeTimer = new Timer(300);
                _volumeTimer.Elapsed += VolumeTimer_Elapsed;
                _volumeTimer.Start();

                await TextToSpeech.SpeakAsync(Sentence, new SpeechOptions
                {
                    Locale = locale
                }, _cancelSpeakTokenSource.Token);
            }
            finally
            {
                _volumeTimer.Stop();
                VolumeIcon = MaterialDesignIconsFont.VolumeHigh;
            }
        }

        private void VolumeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            switch (_volumeCounter)
            {
                case 1:
                    VolumeIcon = MaterialDesignIconsFont.VolumeLow;
                    break;
                case 2:
                    VolumeIcon = MaterialDesignIconsFont.VolumeMedium;
                    break;
                case 3:
                    VolumeIcon = MaterialDesignIconsFont.VolumeHigh;
                    break;
            }

            _volumeCounter++;
            if (_volumeCounter == 4)
            {
                _volumeCounter = 1;
            }
        }

        public async Task StartRecordingAsync()
        {
            if (_countdownTimer.Enabled)
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

            _filepath = await _audioRecorder.StartRecordingAudio("recording");

            _countdownTimer = new Timer(1000);
            _countdownTimer.Elapsed += CountdownTimer_Elapsed;
            _countdownTimer.Start();
            IsRecording = true;
        }

        private void CountdownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CountSeconds = CountSeconds + 1;
            if (_countSeconds > 20)
            {
                StopRecordingCommand.Execute(null);
            }
        }

        public async Task StopRecordingAsync()
        {
            await _audioRecorder.StopRecordingAudio(_filepath);

            _countdownTimer.Stop();
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
            _cancelSpeakTokenSource = null;

            IsSpeaking = false;
            CountSeconds = 0;
            IsRecording = false;
            Sentence = "New Sentence";

            var recordings = await _recordingService.GetRecordingsAsync();
        }

        public async Task NavigateToRecordingsAsync()
        {
            await _navigation.PushAsync(new RecordingsPage());
        }
    }
}
