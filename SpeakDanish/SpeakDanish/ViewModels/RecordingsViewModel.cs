using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Timers;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Domain.Models;
using SpeakDanish.ViewModels.Base;
using Xamarin.Forms;
using static SpeakDanish.Helpers.AppEvents;

namespace SpeakDanish.ViewModels
{
    public class RecordingsViewModel : BaseViewModel
	{
        private INavigationService _navigation;
        private IAlertService _alertService;
        private IAudioUseCase _audioUseCase;
        private IEventAggregator _eventAggregator;
        private IRecordingService<Recording> _recordingService;

        public ObservableCollection<Recording> _recordings;

        public RecordingsViewModel(
            INavigationService navigation,
            IAlertService alertService,
            IAudioUseCase audioUseCase,
            IEventAggregator eventAggregator,
            IRecordingService<Recording> recordingService)
        {
            _navigation = navigation;
            _alertService = alertService;
            _audioUseCase = audioUseCase;
            _eventAggregator = eventAggregator;
            _recordingService = recordingService;

            SetupCommands();

            LoadRecordingsAsync().Await(HandleException);
        }

        public DelegateCommand<Recording> PlaySentenceCommand { get; internal set; }
        public DelegateCommand<Recording> PlayAudioCommand { get; internal set; }
        public DelegateCommand<Recording> RedoCommand { get; internal set; }
        public DelegateCommand<Recording> DeleteCommand { get; internal set; }

        public ObservableCollection<Recording> Recordings
        {
            get => _recordings;
            set => SetProperty(ref _recordings, value);
        }

        public void SetupCommands()
        {
            PlaySentenceCommand = new DelegateCommand<Recording>((r) => PlaySentenceAsync(r).Await(HandleException))
                .ObservesCanExecute(() => IsNotBusy);

            PlayAudioCommand = new DelegateCommand<Recording>((r) => PlayAudioAsync(r).Await(HandleException))
                .ObservesCanExecute(() => IsNotBusy);

            RedoCommand = new DelegateCommand<Recording>((r) => RedoAsync(r).Await(HandleException))
                .ObservesCanExecute(() => IsNotBusy);

            DeleteCommand = new DelegateCommand<Recording>((r) => DeleteAsync(r).Await(HandleException))
                .ObservesCanExecute(() => IsNotBusy);
        }

        private void HandleException(Exception e)
        {
            Device.BeginInvokeOnMainThread(async () =>
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
                var response = await _audioUseCase.PlayAudioAsync(recording.FilePath);
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
            _eventAggregator.GetEvent<RecordingSelectedEvent>().Publish(recording);
            await _navigation.GoBackAsync();
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

