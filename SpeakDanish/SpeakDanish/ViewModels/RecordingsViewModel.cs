using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SpeakDanish.Domain;
using SpeakDanish.Domain.Models;
using SpeakDanish.Helpers;
using SpeakDanish.ViewModels.Base;
using Xamarin.Forms;
using SpeakDanish.Domain.Services;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Contracts.Domain;

namespace SpeakDanish.ViewModels
{
	public class RecordingsViewModel : BaseViewModel
	{
        private INavigation _navigation;
        private IAlertService _alertService;
        private IRecordingService<Recording> _recordingService;

        public ObservableCollection<Recording> _recordings;

        public RecordingsViewModel(
            INavigation navigation,
            IAlertService alertService,
            IRecordingService<Recording> recordingService)
        {
            _navigation = navigation;
            _alertService = alertService;
            _recordingService = recordingService;

            Title = "Recordings";

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
        }

        public async Task PlayAudioAsync(Recording recording)
        {
        }

        public async Task RedoAsync(Recording recording)
        {
        }

        public async Task DeleteAsync(Recording recording)
        {
        }
    }
}

