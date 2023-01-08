using System;
using SpeakDanish.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SpeakDanish.Contracts.Platform;
using System.Collections.Generic;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts;
using SpeakDanish.Helpers;
using System.Timers;
using SpeakDanish.Contracts.Platform.Enums;

namespace SpeakDanish.Tests.ViewModel
{
    public class HomeViewModelTests
    {
        private Action<ServiceCollection> _addHomeViewModelAction;

        public HomeViewModelTests()
        {
            _addHomeViewModelAction = (serviceCollection) =>
            {
                serviceCollection.AddTransient<HomeViewModel>();
            };
        }

        [Fact]
        public async Task LoadRandomSentence_ShouldWork()
        {
            // Arrange
            var previousSentence = "Sentence 1";
            var nextSentence = "Sentence 2";
            var setup = TestUtils.CreateSetupDictionary();
            Mock<ISentenceService> sentenceService = new Mock<ISentenceService>();

            setup.Add(typeof(ISentenceService), mock =>
            {
                sentenceService = mock as Mock<ISentenceService>;
                sentenceService
                    .Setup(x => x.GetRandomSentence(It.IsAny<string>(), It.IsAny<Task<string>>()))
                    .ReturnsAsync((string a, Task<string> b) => a == previousSentence ? nextSentence : previousSentence);
            });

            var homeViewModel = TestUtils
                .CreateTestProvider(setup, _addHomeViewModelAction)
                .GetService<HomeViewModel>();

            var actualPreviousSentence = homeViewModel.Sentence?.ToString();

            // Act
            await homeViewModel.LoadRandomSentence();

            // Arrange
            actualPreviousSentence.Should().Be(previousSentence);
            homeViewModel.Sentence.Should().Be(nextSentence);
            sentenceService.Verify(x => x.GetRandomSentence(previousSentence, It.IsAny<Task<string>>()), Times.Once());
        }

        [Fact]
        public async Task SpeakSentenceAsync_FailedResponseShouldShowAlert()
        {
            // Arrange
            var sentence = "Sentence to speak";
            var errorResponse = "Error";

            var setup = TestUtils.CreateSetupDictionary();
            Mock<IAudioUseCase> audioUseCase = new Mock<IAudioUseCase>();
            Mock<IAlertService> alertService = new Mock<IAlertService>();

            setup.Add(typeof(IAudioUseCase), mock =>
            {
                audioUseCase = mock as Mock<IAudioUseCase>;
                audioUseCase
                    .Setup(x => x.SpeakSentenceAsync(It.IsAny<string>(), It.IsAny<ElapsedEventHandler>()))
                    .ReturnsAsync((string s, ElapsedEventHandler a) => new Response(false, errorResponse));
            });
            setup.Add(typeof(IAlertService), mock =>
            {
                alertService = mock as Mock<IAlertService>;
                alertService
                    .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));
            });

            var homeViewModel = TestUtils
                .CreateTestProvider(setup, _addHomeViewModelAction)
                .GetService<HomeViewModel>();
            homeViewModel.Sentence = sentence;

            // Act
            await homeViewModel.SpeakSentenceAsync();

            // Assert
            audioUseCase.Verify(x => x.SpeakSentenceAsync(sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(errorResponse, It.IsAny<ToastDuration>()), Times.Once());
            homeViewModel.VolumeIcon.Should().Be(MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task SpeakSentenceAsync_SuccessResponseShouldWork()
        {
            // Arrange
            var sentence = "Sentence to speak";

            var setup = TestUtils.CreateSetupDictionary();
            Mock<IAudioUseCase> audioUseCase = new Mock<IAudioUseCase>();
            Mock<IAlertService> alertService = new Mock<IAlertService>();

            setup.Add(typeof(IAudioUseCase), mock =>
            {
                audioUseCase = mock as Mock<IAudioUseCase>;
                audioUseCase
                    .Setup(x => x.SpeakSentenceAsync(It.IsAny<string>(), It.IsAny<ElapsedEventHandler>()))
                    .ReturnsAsync((string s, ElapsedEventHandler a) => new Response(true, null));
            });
            setup.Add(typeof(IAlertService), mock =>
            {
                alertService = mock as Mock<IAlertService>;
                alertService
                    .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));
            });

            var homeViewModel = TestUtils
                .CreateTestProvider(setup, _addHomeViewModelAction)
                .GetService<HomeViewModel>();
            homeViewModel.Sentence = sentence;

            // Act
            await homeViewModel.SpeakSentenceAsync();

            // Assert
            audioUseCase.Verify(x => x.SpeakSentenceAsync(sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
            homeViewModel.VolumeIcon.Should().Be(MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task StartRecordingAsync_ShouldWork()
        {
            // Arrange
            var successResponse = new Response<string> { Success = true, Data = "filepath" };

            var setup = TestUtils.CreateSetupDictionary();
            Mock<IAudioUseCase> audioUseCase = new Mock<IAudioUseCase>();
            Mock<IAlertService> alertService = new Mock<IAlertService>();

            setup.Add(typeof(IAudioUseCase), mock =>
            {
                audioUseCase = mock as Mock<IAudioUseCase>;
                audioUseCase
                    .Setup(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()))
                    .ReturnsAsync(successResponse);
            });
            setup.Add(typeof(IAlertService), mock =>
            {
                alertService = mock as Mock<IAlertService>;
                alertService
                    .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));
            });

            var homeViewModel = TestUtils
                .CreateTestProvider(setup, _addHomeViewModelAction)
                .GetService<HomeViewModel>();

            // Act
            await homeViewModel.StartRecordingAsync();

            // Assert
            homeViewModel.IsRecording.Should().BeTrue();
            homeViewModel.Filepath.Should().Be(successResponse.Data);
            audioUseCase.Verify(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
        }

        [Fact]
        public async Task StartRecordingAsync_ErrorShouldShowMessage()
        {
            // Arrange
            var failureResponse = new Response<string> { Success = false, Message = "error message" };

            var setup = TestUtils.CreateSetupDictionary();
            Mock<IAudioUseCase> audioUseCase = new Mock<IAudioUseCase>();
            Mock<IAlertService> alertService = new Mock<IAlertService>();

            setup.Add(typeof(IAudioUseCase), mock =>
            {
                audioUseCase = mock as Mock<IAudioUseCase>;
                audioUseCase
                    .Setup(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()))
                    .ReturnsAsync(failureResponse);
            });
            setup.Add(typeof(IAlertService), mock =>
            {
                alertService = mock as Mock<IAlertService>;
                alertService
                    .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));
            });

            var homeViewModel = TestUtils
                .CreateTestProvider(setup, _addHomeViewModelAction)
                .GetService<HomeViewModel>();

            // Act
            await homeViewModel.StartRecordingAsync();

            // Assert
            homeViewModel.IsRecording.Should().BeTrue();
            homeViewModel.Filepath.Should().Be(null);
            audioUseCase.Verify(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(failureResponse.Message, It.IsAny<ToastDuration>()), Times.Once());
        }

        [Fact]
        public async Task StopRecordingAsync_ShouldWork()
        {
            // Arrange
            var path = "filepath";
            var successResponse = new Response(true);

            var setup = TestUtils.CreateSetupDictionary();
            Mock<IAudioUseCase> audioUseCase = new Mock<IAudioUseCase>();
            Mock<IAlertService> alertService = new Mock<IAlertService>();

            setup.Add(typeof(IAudioUseCase), mock =>
            {
                audioUseCase = mock as Mock<IAudioUseCase>;
                audioUseCase
                    .Setup(x => x.StopRecordingAsync(It.IsAny<string>()))
                    .ReturnsAsync(successResponse);
            });
            setup.Add(typeof(IAlertService), mock =>
            {
                alertService = mock as Mock<IAlertService>;
                alertService
                    .Setup(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()));
            });

            var homeViewModel = TestUtils
                .CreateTestProvider(setup, _addHomeViewModelAction)
                .GetService<HomeViewModel>();
            homeViewModel.Filepath = path;
            homeViewModel.IsRecording = true;
            homeViewModel.CountSeconds = 6;

            // Act
            await homeViewModel.StopRecordingAsync();

            // Assert
            homeViewModel.IsRecording.Should().BeFalse();
            homeViewModel.CountSeconds.Should().Be(0);
            audioUseCase.Verify(x => x.StopRecordingAsync(path), Times.Once());
            alertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
        }
    }
}

