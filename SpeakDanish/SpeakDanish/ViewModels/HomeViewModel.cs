using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using SpeakDanish.ViewModels.Base;
using System.Windows.Input;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using SpeakDanish.Views;
using SpeakDanish.Domain;
using SpeakDanish.Domain.Models;
using Timer = System.Timers.Timer;
using SpeakDanish.Helpers;
using System.Reflection;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.CommunityToolkit.Extensions;
using System.IO;
using SpeakDanish.Domain.Services;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Domain.UseCases;
using SpeakDanish.Contracts;

namespace SpeakDanish.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private IAudioUseCase _audioUseCase;
        private ISentenceService _sentenceService;
        private IRecordingService<Recording> _recordingService;
        private IAlertService _alertService;
        private INavigation _navigation;

        private bool _isSpeaking;
        private int _countSeconds;
        private string _sentence;
        private bool _isRecording;
        private string _volumeIcon;
        private int _volumeCounter = 1;

        public HomeViewModel(
            IAudioUseCase audioUseCase,
            ISentenceService sentenceService,
            IRecordingService<Recording> recordingService,
            IAlertService alertService,
            INavigation navigation)
        {
            _audioUseCase = audioUseCase;
            _sentenceService = sentenceService;
            _recordingService = recordingService;
            _alertService = alertService;
            _navigation = navigation;

            Title = "Home";
            VolumeIcon = MaterialDesignIconsFont.VolumeHigh;

            SpeakSentenceCommand = new Command(async () => await SpeakSentenceAsync(), () => !_isSpeaking);
            StartRecordingCommand = new Command(async () => await StartRecordingAsync(), () => !_isRecording);
            StopRecordingCommand = new Command(async () => await StopRecordingAsync(), () => _isRecording);
            NewSentenceCommand = new Command(async () => await NewSentenceAsync());
            NavigateToRecordingsCommand = new Command(async () => await NavigateToRecordingsAsync());

            LoadRandomSentence().ConfigureAwait(false);
        }

        public Command SpeakSentenceCommand { get; set; }
        public Command StartRecordingCommand { get; set; }
        public Command StopRecordingCommand { get; set; }
        public Command NewSentenceCommand { get; set; }
        public Command NavigateToRecordingsCommand { get; set; }

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


        public bool HasRecording
        {
            get => !string.IsNullOrEmpty(Filepath);
        }

        public async Task LoadRandomSentence()
        {
            try
            {
                Sentence = await _sentenceService.GetRandomSentence(Sentence, LoadFile());
            }
            catch (Exception e)
            {
                await _alertService.ShowToast(e.Message);
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
                var response = await _audioUseCase.SpeakSentenceAsync(Sentence, VolumeTimer_Elapsed);
                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
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
                var response = await _audioUseCase.StartRecordingAsync(CountdownTimer_Elapsed);
                if (response.Success)
                {
                    Filepath = response.Data;
                }
                else
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsRecording = true;
            }
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
            try
            {
                var response = await _audioUseCase.StopRecordingAsync(Filepath);
                if (response.Success)
                {
                    OnPropertyChanged(nameof(HasRecording));
                }
                else
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                CountSeconds = 0;
                IsRecording = false;
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

            await _navigation.PopAsync(false);
            await _navigation.PushAsync(new HomePage());
        }

        public async Task NavigateToRecordingsAsync()
        {
            await _navigation.PushAsync(new RecordingsPage());
        }
    }
}
