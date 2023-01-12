using System;
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
using SpeakDanish.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace SpeakDanish.Tests.ViewModels
{
	public class HomeViewModelBuilder
    {
        public Mock<IAudioUseCase> AudioUseCase = new Mock<IAudioUseCase>();
        public Mock<ISentenceService> SentenceService = new Mock<ISentenceService>();
        public Mock<IRecordingService<Recording>> RecordingService = new Mock<IRecordingService<Recording>>();
        public Mock<IAlertService> AlertService = new Mock<IAlertService>();
        public Mock<IEventAggregator> EventAggregator = new Mock<IEventAggregator>();
        public Mock<INavigationService> Navigation = new Mock<INavigationService>();

        public HomeViewModel HomeViewModel { get; set; }

        public HomeViewModelBuilder WithGetRandomSentence(Func<string, Task<string>, string> returnFunc)
        {
            SentenceService
                .Setup(x => x.GetRandomSentence(It.IsAny<string>(), It.IsAny<Task<string>>()))
                .ReturnsAsync(returnFunc);
            return this;
        }

        public HomeViewModelBuilder WithSpeakSentenceAsync(Response response)
        {
            AudioUseCase
                    .Setup(x => x.SpeakSentenceAsync(It.IsAny<string>(), It.IsAny<ElapsedEventHandler>()))
                    .ReturnsAsync(response);
            return this;
        }

        public HomeViewModelBuilder WithStartRecordingAsync(Response<string> response)
        {
            AudioUseCase
                    .Setup(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()))
                    .ReturnsAsync(response);
            return this;
        }

        internal HomeViewModelBuilder WithStopRecordingAsync(Response successResponse)
        {
            AudioUseCase
                    .Setup(x => x.StopRecordingAsync(It.IsAny<string>()))
                    .ReturnsAsync(successResponse);
            return this;
        }

        public HomeViewModelBuilder Build()
		{
            AlertService
                .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));

            HomeViewModel = new HomeViewModel(
                AudioUseCase.Object,
                SentenceService.Object,
                RecordingService.Object,
                AlertService.Object,
                EventAggregator.Object,
                Navigation.Object
            );
            return this;
		}

        public HomeViewModelBuilder UpdateUserIsRecording(string path)
        {
            HomeViewModel.Filepath = path;
            HomeViewModel.IsRecording = true;
            HomeViewModel.CountSeconds = 6;
            return this;
        }

        internal HomeViewModelBuilder UpdateSentence(string sentence)
        {
            HomeViewModel.Sentence = sentence;
            return this;
        }
    }
}

