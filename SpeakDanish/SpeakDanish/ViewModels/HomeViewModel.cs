using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using SpeakDanish.Extensions;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Data.Api;
using SpeakDanish.Domain.Models;
using SpeakDanish.Helpers;
using SpeakDanish.ViewModels.Base;
using SpeakDanish.Views;
using Xamarin.Forms;
using static SpeakDanish.Helpers.AppEvents;
using SpeakDanish.Domain.Utility;

namespace SpeakDanish.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Private Fields
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
        private string _volumeIconSentence = MaterialDesignIconsFont.VolumeHigh;
        private string _volumeIconRecording = MaterialDesignIconsFont.VolumeHigh;
        private int _volumeCounter = 1;
        private int _recordingLength;
        private string _filepathCache;
        private string _filepath;
        private bool _isTranscribing;
        private bool _isRecordingAccepted;
        private bool _isTranscribingAccepted;
        private string _transcribedText;

        private string _filename;
        private SubscriptionToken _recordingSelectedEvent;
        private int _similarity;
        #endregion

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

            SetupCommands();

            LoadRandomSentence().Await(HandleException);
        }

        #region Setup
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

            PlayAudioCommand = new DelegateCommand(() => PlayAudioAsync().Await(HandleException))
                .ObservesCanExecute(() => IsNotBusy);

            StartRecordingCommand = new DelegateCommand(() => StartRecordingAsync().Await(HandleException))
                .ObservesCanExecute(() => IsNotRecording);

            StopRecordingCommand = new DelegateCommand(() => StopRecordingAsync().Await(HandleException))
                .ObservesCanExecute(() => IsRecording);

            NewSentenceCommand = new DelegateCommand(() => NewSentenceAsync().Await(HandleException))
                .ObservesCanExecute(() => CanSave);

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
        public DelegateCommand PlayAudioCommand { get; set; }
        public DelegateCommand StartRecordingCommand { get; set; }
        public DelegateCommand StopRecordingCommand { get; set; }
        public DelegateCommand NewSentenceCommand { get; set; }
        public DelegateCommand NavigateToRecordingsCommand { get; set; }
        #endregion

        #region Public Fields
        public string Filepath
        {
            get => _filepath;
            set => SetProperty(ref _filepath, value);
        }

        public string VolumeIconSentence
        {
            get => _volumeIconSentence;
            set => SetProperty(ref _volumeIconSentence, value);
        }

        public string VolumeIconRecording
        {
            get => _volumeIconRecording;
            set => SetProperty(ref _volumeIconRecording, value);
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

        public int Similarity
        {
            get => _similarity;
            set => SetProperty(ref _similarity, value);
        }

        public string Sentence
        {
            get => _sentence;
            set => SetProperty(ref _sentence, value);
        }

        public bool IsRecording
        {
            get => _isRecording;
            set {
                SetProperty(ref _isRecording, value);
                OnPropertyChanged(nameof(CircleColor));
                OnPropertyChanged(nameof(CircleIcon));
            }
        }
        public bool IsNotRecording => !IsRecording;

        public int RecordingLength
        {
            get => _recordingLength;
            set => SetProperty(ref _recordingLength, value);
        }

        public bool IsTranscribingAccepted
        {
            get => _isTranscribingAccepted;
            set {
                SetProperty(ref _isTranscribingAccepted, value);
                OnPropertyChanged(nameof(CircleColor));
                OnPropertyChanged(nameof(CircleIcon));
            }
        }
        public bool IsRecordingAccepted
        {
            get => _isRecordingAccepted;
            set
            {
                SetProperty(ref _isRecordingAccepted, value);
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public Color CircleColor
        {
            get
            {
                if (IsTranscribingAccepted)
                {
                    var color = Color.Green;
                    if (IsRecording)
                        color = color.Darker(0.7f);
                    return color;
                }
                else
                {
                    var color = Color.Blue;
                    if (IsRecording)
                        color = color.Darker(0.7f);
                    return color;
                }
            }
        }

        public string CircleIcon
        {
            get
            {
                if (!IsTranscribingAccepted)
                {
                    if(Sentence != null && TranscribedText == null && IsBusy && !IsRecording)
                        return MaterialDesignIconsFont.DotsHorizontal;
                    else
                        return MaterialDesignIconsFont.MicrophoneMessage;
                }
                else
                {
                    if (IsBusy && !IsRecording)
                        return MaterialDesignIconsFont.VolumeHigh;
                    else
                        return MaterialDesignIconsFont.Microphone;
                }
            }
        }

        public bool CanSave
        {
            get => !string.IsNullOrEmpty(Filepath) && IsRecordingAccepted;
        }

        public string TranscribedText {
            get => _transcribedText;
            set => SetProperty(ref _transcribedText, value);
        }
        #endregion

        #region Methods
        public async Task LoadRandomSentence()
        {
            try
            {
                IsBusy = true;
                Sentence = await _sentenceService.GetRandomSentence<HomeViewModel>(Sentence);
                _filename = StringUtils.CreateUniqueFileName(Sentence);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SpeakSentenceAsync()
        {
            try
            {
                IsBusy = true;
                var response = await _audioUseCase.SpeakSentenceAsync(Sentence, (s, e) => VolumeTimer_Elapsed(newValue =>
                {
                    VolumeIconSentence = newValue;
                }));

                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsBusy = false;
                VolumeIconSentence = MaterialDesignIconsFont.VolumeHigh;
            }
        }

        public async Task PlayAudioAsync()
        {
            try
            {
                IsBusy = true;
                var response = await _audioUseCase.PlayAudioAsync(Filepath, (s, e) => VolumeTimer_Elapsed(newValue =>
                {
                    VolumeIconRecording = newValue;
                }));

                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsBusy = false;
                VolumeIconRecording = MaterialDesignIconsFont.VolumeHigh;
            }
        }

        private void VolumeTimer_Elapsed(Action<string> setValue)
        {
            switch (_volumeCounter)
            {
                case 1:
                    setValue(MaterialDesignIconsFont.VolumeLow);
                    break;
                case 2:
                    setValue(MaterialDesignIconsFont.VolumeMedium);
                    break;
                case 3:
                    setValue(MaterialDesignIconsFont.VolumeHigh);
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
                IsRecording = true;

                if (!IsTranscribingAccepted)
                {
                    TranscribedText = null;
                    await _audioUseCase.StartTranscribingDanish(CountdownTimer_Elapsed, result =>
                    {
                        TranscribedText = result;
                        Similarity = StringUtils.LevenshteinSimilarity(Sentence, result);
                    });
                }
                else
                {
                    var response = await _audioUseCase.StartRecordingAsync(_filename, CountdownTimer_Elapsed);
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
                IsRecording = false;

                if (!IsTranscribingAccepted)
                {
                    await _audioUseCase.StopTranscribingDanish();
                }
                else
                {
                    var response = await _audioUseCase.StopRecordingAsync(_filepathCache);
                    if (response.Success)
                    {
                        Filepath = _filepathCache;
                        RecordingLength = CountSeconds;

                        await PlayAudioAsync();
                    }
                    else
                    {
                        await _alertService.ShowToast(response.Message);
                    }
                }
            }
            finally
            {
                IsBusy = false;
                CountSeconds = 0;
                OnPropertyChanged(nameof(CircleIcon));
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public async Task NewSentenceAsync()
        {
            await _recordingService.InsertRecordingAsync(
                new Recording
                {
                    FilePath = _filepath,
                    Sentence = _sentence,
                    Created = DateTime.Now,
                    TranscribedText = _transcribedText,
                    Similarity = _similarity
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
        #endregion
    }
}
