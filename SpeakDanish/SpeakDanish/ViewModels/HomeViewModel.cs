using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Domain.Models;
using SpeakDanish.Helpers;
using SpeakDanish.ViewModels.Base;
using SpeakDanish.Views;
using Xamarin.Forms;
using static SpeakDanish.Helpers.AppEvents;

namespace SpeakDanish.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private IAudioUseCase _audioUseCase;
        private ISentenceService _sentenceService;
        private IRecordingService<Recording> _recordingService;
        private IAlertService _alertService;
        private IEventAggregator _eventAggregator;
        private INavigationService _navigation;

        private bool _isSpeaking;
        private int _countSeconds;
        private string _sentence;
        private bool _isRecording;
        private string _volumeIcon;
        private int _volumeCounter = 1;
        private int _recordingLength;
        private string _filepathCache;

        private SubscriptionToken _recordingSelectedEvent;

        public HomeViewModel(
            IAudioUseCase audioUseCase,
            ISentenceService sentenceService,
            IRecordingService<Recording> recordingService,
            IAlertService alertService,
            IEventAggregator eventAggregator,
            INavigationService navigation)
        {
            _audioUseCase = audioUseCase;
            _sentenceService = sentenceService;
            _recordingService = recordingService;
            _alertService = alertService;
            _eventAggregator = eventAggregator;
            _navigation = navigation;

            VolumeIcon = MaterialDesignIconsFont.VolumeHigh;

            SetupCommands();

            LoadRandomSentence().Await(HandleException);
        }

        public override void Destroy()
        {
            _recordingSelectedEvent.Dispose();
            base.Destroy();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _recordingSelectedEvent = _eventAggregator.GetEvent<RecordingSelectedEvent>().Subscribe(OnRecordingSelected);
        }

        public void SetupCommands()
        {
            SpeakSentenceCommand = new DelegateCommand(() => SpeakSentenceAsync().Await(HandleException))
                .ObservesCanExecute(() => IsNotRecording);

            StartRecordingCommand = new DelegateCommand(() => StartRecordingAsync().Await(HandleException))
                .ObservesCanExecute(() => IsNotRecording);

            StopRecordingCommand = new DelegateCommand(() => StopRecordingAsync().Await(HandleException))
                .ObservesCanExecute(() => IsRecording);

            NewSentenceCommand = new DelegateCommand(() => NewSentenceAsync().Await(HandleException))
                .ObservesCanExecute(() => CanSave);

            TranscribeRecordingCommand = new DelegateCommand(() => TranscribeRecordingAsync().Await(HandleException))
                .ObservesCanExecute(() => IsNotRecording);

            NavigateToRecordingsCommand = new DelegateCommand(() => NavigateToRecordingsAsync().Await(HandleException))
                .ObservesCanExecute(() => IsNotRecording);
        }

        private void HandleException(Exception e)
        {
            Console.Write(e.ToString());
            Device.BeginInvokeOnMainThread(async () =>
            {
                await _alertService.ShowToast(e.Message);
            });
        }

        public DelegateCommand SpeakSentenceCommand { get; set; }
        public DelegateCommand StartRecordingCommand { get; set; }
        public DelegateCommand StopRecordingCommand { get; set; }
        public DelegateCommand NewSentenceCommand { get; set; }
        public DelegateCommand TranscribeRecordingCommand { get; set; }
        public DelegateCommand NavigateToRecordingsCommand { get; set; }

        public string Filepath { get; set; }

        public string VolumeIcon
        {
            get => _volumeIcon;
            set => SetProperty(ref _volumeIcon, value);
        }

        public bool IsSpeaking
        {
            get => _isSpeaking;
            set => SetProperty(ref _isSpeaking, value);
        }

        public int CountSeconds
        {
            get => _countSeconds;
            set => SetProperty(ref _countSeconds, value);
        }

        public string Sentence
        {
            get => _sentence;
            set => SetProperty(ref _sentence, value);
        }

        public bool IsRecording
        {
            get => _isRecording;
            set => SetProperty(ref _isRecording, value);
        }
        public bool IsNotRecording => !IsRecording;

        public int RecordingLength
        {
            get => _recordingLength;
            set => SetProperty(ref _recordingLength, value);
        }

        public bool CanSave
        {
            get => !string.IsNullOrEmpty(Filepath);
        }

        public async Task LoadRandomSentence()
        {
            try
            {
                IsBusy = true;
                Sentence = await _sentenceService.GetRandomSentence(Sentence, LoadFile());
            }
            finally
            {
                IsBusy = false;
            }
        }

        Task<string> LoadFile()
        {
            var assembly = typeof(HomeViewModel).GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("SpeakDanish.Resources.sentences.txt"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return Task.FromResult(reader.ReadToEnd());
                }
            }
        }

        public async Task SpeakSentenceAsync()
        {
            try
            {
                IsBusy = true; 
                var response = await _audioUseCase.SpeakSentenceAsync(Sentence, VolumeTimer_Elapsed);
                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsBusy = false;
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
            try
            {
                IsBusy = true;
                var response = await _audioUseCase.StartRecordingAsync(CountdownTimer_Elapsed);
                if (response.Success)
                {
                    _filepathCache = response.Data;
                    IsRecording = true;
                }
                else
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CountdownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CountSeconds = CountSeconds + 1;
            if (_countSeconds > 20)
            {
                StopRecordingCommand.Execute();
            }
        }

        public async Task StopRecordingAsync()
        {
            try
            {
                IsBusy = true;
                var response = await _audioUseCase.StopRecordingAsync(_filepathCache);
                if (response.Success)
                {
                    Filepath = _filepathCache;
                    RecordingLength = CountSeconds;
                    OnPropertyChanged(nameof(CanSave));
                }
                else
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsBusy = false;
                CountSeconds = 0;
                IsRecording = false;
            }
        }

        public async Task TranscribeRecordingAsync()
        {
            try
            {
                var response = await _recordingService.TranscribeDanishSpeechToText(Filepath);
                if (response.Success)
                {
                    await _alertService.ShowToast(response.Data);
                }
                else
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            catch (Exception e)
            {
                await _alertService.ShowToast(e.Message);
            }
        }

        public async Task NewSentenceAsync()
        {
            await _recordingService.InsertRecordingAsync(
                new Recording
                {
                    FilePath = Filepath,
                    Sentence = _sentence,
                    Created = DateTime.Now
                }
            );

            await _navigation.GoBackAsync(animated: false);
        }

        public async Task NavigateToRecordingsAsync()
        {
            var result = await _navigation.NavigateAsync(nameof(RecordingsPage));
            if (!result.Success)
            {
                await _alertService.ShowToast("Could not navigate to recordings page");
            }
        }

        private void OnRecordingSelected(Recording recording)
        {
            Sentence = recording.Sentence;
        }
    }
}
