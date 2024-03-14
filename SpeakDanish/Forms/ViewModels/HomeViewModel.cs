using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using SpeakDanish.Extensions;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Data.Api;
using SpeakDanish.Domain.Models;
using SpeakDanish.Helpers;
using SpeakDanish.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;
using static SpeakDanish.Helpers.AppEvents;
using SpeakDanish.Domain.Utility;
using SpeakDanish.Forms.Services;
using SpeakDanish.Forms.Views;
using CommunityToolkit.Maui.Alerts;

namespace SpeakDanish.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Private Fields
        private IAudioUseCase _audioUseCase;
        private ISentenceService _sentenceService;
        private IRecordingService<Recording> _recordingService;
        private IAlertService _alertService;
        //private IEventAggregator _eventAggregator;
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
        //private SubscriptionToken _recordingSelectedEvent;
        private int _similarity;
        #endregion

        public HomeViewModel(
            IAudioUseCase audioUseCase,
            ISentenceService sentenceService,
            IRecordingService<Recording> recordingService,
            IAlertService alertService,
            //IEventAggregator eventAggregator,
            INavigationService navigation)
        {
            _audioUseCase = audioUseCase;
            _sentenceService = sentenceService;
            _recordingService = recordingService;
            _alertService = alertService;
            //_eventAggregator = eventAggregator;
            _navigation = navigation;

            SetupCommands();

            LoadRandomSentence();
        }

        #region Setup
        public void Destroy()
        {
            //_recordingSelectedEvent.Dispose();
        }

        public void Initialize()
        {
            //_recordingSelectedEvent = _eventAggregator.GetEvent<RecordingSelectedEvent>().Subscribe(OnRecordingSelected);
        }

        public void SetupCommands()
        {
            SpeakSentenceCommand = new AsyncRelayCommand(() => SpeakSentenceAsync(), () => IsNotRecording);
            PlayAudioCommand = new AsyncRelayCommand(() => PlayAudioAsync(), () => IsNotBusy);
            NewSentenceCommand = new AsyncRelayCommand(() => NewSentenceAsync(), () => CanSave);
            NavigateToRecordingsCommand = new AsyncRelayCommand(() => NavigateToRecordingsAsync(), () => IsNotRecording);
        }

        private void HandleException(Exception e)
        {
            Console.Write(e.ToString());
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _alertService.ShowToast(e.Message);
            });
        }
        
        public AsyncRelayCommand SpeakSentenceCommand { get; set; }
        public AsyncRelayCommand PlayAudioCommand { get; set; }
        public AsyncRelayCommand NewSentenceCommand { get; set; }
        public AsyncRelayCommand NavigateToRecordingsCommand { get; set; }
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
                    var color = Colors.Green;
                    if (IsRecording)
                        color = color.Darker(0.7f);
                    return color;
                }
                else if(Application.Current.Resources.TryGetValue("Primary", out var primary) && primary is Color color)
                {
                    if (IsRecording)
                        color = color.Darker(0.7f);
                    return color;
                }
                else
                {
                    return Colors.Black;
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
            get => IsRecordingAccepted;
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
                        if (result.Success)
                        {
                            TranscribedText = result.Data?.ToString() ?? "";

                            var trimmedSentence = Sentence.Trim(new char[] { ' ', '.', '!', '?' }).ToLower();
                            Similarity = StringUtils.GetSimilarity(trimmedSentence, TranscribedText.ToLower());
                        }
                        else
                        {
                            TranscribedText = "";
                            Similarity = 0;
                            Toast.Make("Failed: " + result.Message, CommunityToolkit.Maui.Core.ToastDuration.Short);
                        }
                        
                        IsBusy = false;
                        CountSeconds = 0;
                        OnPropertyChanged(nameof(CircleIcon));
                        OnPropertyChanged(nameof(CanSave));
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

        private async void CountdownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CountSeconds = CountSeconds + 1;
            if (_countSeconds > 20)
            {
                await StopRecordingAsync(false);
            }
        }

        public async Task StopRecordingAsync(bool isCancelled)
        {
            try
            {
                IsBusy = true;
                IsRecording = false;

                if (!IsTranscribingAccepted)
                {
                    await _audioUseCase.StopTranscribingDanish(isCancelled);
                }
                else
                {
                    var response = await _audioUseCase.StopRecordingAsync(_filepathCache);
                    if (isCancelled)
                    {
                    }
                    else if (response.Success)
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

            await Shell.Current.Navigation.PopToRootAsync(animated: false);
        }

        public async Task NavigateToRecordingsAsync()
        {
            var result = await _navigation.NavigateAsync(nameof(RecordingsPage));
            if (!result)
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
