using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Moq;
using Prism.Events;
using Prism.Navigation;
using SpeakDanish.Contracts;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Contracts.Platform.Enums;
using SpeakDanish.Domain.Models;
using SpeakDanish.Domain.Services;
using SpeakDanish.Domain.UseCases;
using SpeakDanish.Helpers;
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
        public Mock<IEventAggregator> EventAggregator = new Mock<IEventAggregator>();
        public Mock<INavigationService> Navigation = new Mock<INavigationService>();

        public RecordingsViewModel RecordingsViewModel { get; set; }

        public RecordingsViewModelBuilder()
        {
            RecordingService
                .Setup(x => x.GetRecordingsAsync())
                .ReturnsAsync(new List<Recording>());

            AlertService
                .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));
        }

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

        public RecordingsViewModelBuilder WithGoBackAsync()
        {
            Navigation.Setup(x => x.GoBackAsync());
            return this;
        }

        public RecordingsViewModelBuilder WithDeleteRecordingAsync(int rowModified)
        {
            RecordingService
                .Setup(x => x.DeleteRecordingAsync(It.IsAny<Recording>()))
                .ReturnsAsync(rowModified);
            return this;
        }

        internal RecordingsViewModelBuilder WithGetEvent()
        {
            EventAggregator
                    .Setup(x => x.GetEvent<AppEvents.RecordingSelectedEvent>())
                    .Returns(new AppEvents.RecordingSelectedEvent());

            return this;
        }

        public RecordingsViewModelBuilder Build()
		{
            RecordingsViewModel = new RecordingsViewModel(
                Navigation.Object,
                AlertService.Object,
                AudioUseCase.Object,
                EventAggregator.Object,
                RecordingService.Object
            );
            return this;
		}

        internal RecordingsViewModelBuilder IsBusy()
        {
            RecordingsViewModel.IsBusy = true;
            return this;
        }
    }
}

