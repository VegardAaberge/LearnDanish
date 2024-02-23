using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.Input;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Domain.Models;
using SpeakDanish.Forms.Services;
using SpeakDanish.ViewModels.Base;

namespace SpeakDanish.ViewModels
{
    public class RecordingsViewModel : BaseViewModel
	{
        private INavigationService _navigation;
        private IAlertService _alertService;
        private IAudioUseCase _audioUseCase;
        //private IEventAggregator _eventAggregator;
        private IRecordingService<Recording> _recordingService;

        public ObservableCollection<Recording> _recordings;
        private string _testString;

        public RecordingsViewModel(
            INavigationService navigation,
            IAlertService alertService,
            IAudioUseCase audioUseCase,
            //IEventAggregator eventAggregator,
            IRecordingService<Recording> recordingService)
        {
            _navigation = navigation;
            _alertService = alertService;
            _audioUseCase = audioUseCase;
            //_eventAggregator = eventAggregator;
            _recordingService = recordingService;

            SetupCommands();

            TestString = "Test Data";
            LoadRecordingsAsync();
        }

        public AsyncRelayCommand<Recording> PlaySentenceCommand { get; internal set; }
        public AsyncRelayCommand<Recording> PlayAudioCommand { get; internal set; }
        public AsyncRelayCommand<Recording> RedoCommand { get; internal set; }
        public AsyncRelayCommand<Recording> DeleteCommand { get; internal set; }

        public ObservableCollection<Recording> Recordings
        {
            get => _recordings;
            set => SetProperty(ref _recordings, value);
        }

        public string TestString
        {
            get => _testString;
            set => SetProperty(ref _testString, value);
        }

        public void SetupCommands()
        {
            PlaySentenceCommand = new AsyncRelayCommand<Recording>(r => PlaySentenceAsync(r), r => IsNotBusy);

            PlayAudioCommand = new AsyncRelayCommand<Recording>(r => PlayAudioAsync(r), r => IsNotBusy);

            RedoCommand = new AsyncRelayCommand<Recording>(r => RedoAsync(r), r => IsNotBusy);

            DeleteCommand = new AsyncRelayCommand<Recording>(r => DeleteAsync(r), r => IsNotBusy);
        }

        private void HandleException(Exception e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _alertService.ShowToast(e.Message);
            });
        }

        public async Task LoadRecordingsAsync()
        {
            try
            {
                IsBusy = true;
                var records = await _recordingService.GetRecordingsAsync();
                Recordings = new ObservableCollection<Recording>(records);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task PlaySentenceAsync(Recording recording)
        {
            try
            {
                IsBusy = true;
                var response = await _audioUseCase.SpeakSentenceAsync(recording.Sentence, VolumeTimer_Elapsed);
                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void VolumeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // TODO
        }

        public async Task PlayAudioAsync(Recording recording)
        {
            try
            {
                IsBusy = true;
                var response = await _audioUseCase.PlayAudioAsync(recording.FilePath, (s, e) => { });
                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task RedoAsync(Recording recording)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                //_eventAggregator.GetEvent<RecordingSelectedEvent>()?.Publish(recording);

                var result = await _navigation.GoBackAsync();
                if (!result)
                {
                    await _alertService.ShowToast("Could not return");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DeleteAsync(Recording recording)
        {
            try
            {
                IsBusy = true;
                var response = await _recordingService.DeleteRecordingAsync(recording);
                if (response == 0)
                {
                    await _alertService.ShowToast("Was not able to delete record");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

