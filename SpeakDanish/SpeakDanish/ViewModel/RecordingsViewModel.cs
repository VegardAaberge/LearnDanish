using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SpeakDanish.Domain;
using SpeakDanish.Domain.Models;
using SpeakDanish.Services;
using SpeakDanish.Helpers;
using SpeakDanish.ViewModel.Base;
using Xamarin.Forms;

namespace SpeakDanish.ViewModel
{
	public class RecordingsViewModel : BaseViewModel
	{
        private INavigation _navigation;
        private IAlertService _alertService;
        private IRecordingService _recordingService;

        private string _volumeIcon;
        public ObservableCollection<Recording> _recordings;

        public RecordingsViewModel(
            INavigation navigation,
            IAlertService alertService,
            IRecordingService recordingService)
        {
            _navigation = navigation;
            _alertService = alertService;
            _recordingService = recordingService;

            Title = "Recordings";
            VolumeIcon = MaterialDesignIconsFont.VolumeHigh;

            PlaySentenceCommand = new Command<Recording>(async (r) => await PlaySentenceAsync(r));
            PlayAudioCommand = new Command<Recording>(async (r) => await PlayAudioAsync(r));
            RedoCommand = new Command<Recording>(async (r) => await RedoAsync(r));

            LoadRecordingsAsync().ConfigureAwait(false);
        }

        public Command PlaySentenceCommand { get; internal set; }
        public Command PlayAudioCommand { get; internal set; }
        public Command RedoCommand { get; internal set; }

        public ObservableCollection<Recording> Recordings {
            get => _recordings;
            set => SetProperty(ref _recordings, value);
        }

        public string VolumeIcon
        {
            get => _volumeIcon;
            set => SetProperty(ref _volumeIcon, value);
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

        private async Task PlaySentenceAsync(Recording recording)
        {
        }

        private async Task PlayAudioAsync(Recording recording)
        {
        }

        private async Task RedoAsync(Recording recording)
        {
        }
    }
}

