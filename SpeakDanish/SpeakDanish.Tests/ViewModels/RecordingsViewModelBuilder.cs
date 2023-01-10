using System;
using System.Threading.Tasks;
using System.Timers;
using Moq;
using SpeakDanish.Contracts;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Contracts.Platform.Enums;
using SpeakDanish.Domain.Models;
using SpeakDanish.Domain.Services;
using SpeakDanish.Domain.UseCases;
using SpeakDanish.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace SpeakDanish.Tests.ViewModels
{
	public class RecordingsViewModelBuilder
    {
        public Mock<IAudioUseCase> AudioUseCase = new Mock<IAudioUseCase>();
        public Mock<IRecordingService<Recording>> RecordingService = new Mock<IRecordingService<Recording>>();
        public Mock<IAlertService> AlertService = new Mock<IAlertService>();
        public Mock<INavigation> Navigation = new Mock<INavigation>();

        public RecordingsViewModel RecordingsViewModel { get; set; }

        public RecordingsViewModelBuilder WithSpeakSentenceAsync(Response response)
        {
            AudioUseCase
                    .Setup(x => x.SpeakSentenceAsync(It.IsAny<string>(), It.IsAny<ElapsedEventHandler>()))
                    .ReturnsAsync(response);
            return this;
        }

        public RecordingsViewModelBuilder WithPlayAudioAsync(Response response)
        {
            AudioUseCase
                    .Setup(x => x.PlayAudioAsync(It.IsAny<string>()))
                    .ReturnsAsync(response);
            return this;
        }

        public RecordingsViewModelBuilder WithPopAsync()
        {
            Navigation
                    .Setup(x => x.PopAsync(It.IsAny<bool>()));
            return this;
        }

        public RecordingsViewModelBuilder WithDeleteRecordingAsync(int rowModified)
        {
            RecordingService
                .Setup(x => x.DeleteRecordingAsync(It.IsAny<Recording>()))
                .ReturnsAsync(rowModified);
            return this;
        }

        public RecordingsViewModelBuilder Build()
		{
            AlertService
                .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));

            RecordingsViewModel = new RecordingsViewModel(
                Navigation.Object,
                AlertService.Object,
                RecordingService.Object
            );
            return this;
		}
    }
}

