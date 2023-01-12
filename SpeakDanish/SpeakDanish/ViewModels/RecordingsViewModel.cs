using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SpeakDanish.Domain;
using SpeakDanish.Domain.Models;
using SpeakDanish.Views;
using SpeakDanish.Helpers;
using SpeakDanish.ViewModels.Base;
using Xamarin.Forms;
using SpeakDanish.Domain.Services;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Contracts.Domain;
using Xamarin.Essentials.Interfaces;
using System.Timers;
using SpeakDanish.Contracts;
using Prism.Navigation;
using Prism.Events;
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

            PlaySentenceCommand = new Command<Recording>(async (r) => await PlaySentenceAsync(r));
            PlayAudioCommand = new Command<Recording>(async (r) => await PlayAudioAsync(r));
            RedoCommand = new Command<Recording>(async (r) => await RedoAsync(r));
            DeleteCommand = new Command<Recording>(async (r) => await DeleteAsync(r));

            LoadRecordingsAsync().ConfigureAwait(false);
        }

        public Command PlaySentenceCommand { get; internal set; }
        public Command PlayAudioCommand { get; internal set; }
        public Command RedoCommand { get; internal set; }
        public Command DeleteCommand { get; internal set; }

        public ObservableCollection<Recording> Recordings {
            get => _recordings;
            set => SetProperty(ref _recordings, value);
        }

        public async Task LoadRecordingsAsync()
        {
            try
            {
                IsBusy = true;
                var records = await _recordingService.GetRecordingsAsync();
                Recordings = new ObservableCollection<Recording>(records);
            }
            catch (Exception e)
            {
                await _alertService.ShowToast(e.Message);
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
                var response = await _audioUseCase.SpeakSentenceAsync(recording.Sentence, VolumeTimer_Elapsed);
                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {

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
                var response = await _audioUseCase.PlayAudioAsync(recording.FilePath);
                if (!response.Success)
                {
                    await _alertService.ShowToast(response.Message);
                }
            }
            finally
            {

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
                var response = await _recordingService.DeleteRecordingAsync(recording);
                if (response == 0)
                {
                    await _alertService.ShowToast("Was not able to delete record");
                }
            }
            finally
            {

            }
        }
    }
}

