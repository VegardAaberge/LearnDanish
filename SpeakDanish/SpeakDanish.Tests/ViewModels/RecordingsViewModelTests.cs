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
using SpeakDanish.Domain.Models;

namespace SpeakDanish.Tests.ViewModel
{
    public class RecordingsViewModelTests
    {
        private Action<ServiceCollection> _addRecordingsViewModelAction;

        public RecordingsViewModelTests()
        {
            _addRecordingsViewModelAction = (serviceCollection) =>
            {
                serviceCollection.AddTransient<RecordingsViewModel>();
            };
        }

        [Fact]
        public async Task PlaySentenceAsync_FailedResponseShouldShowAlert()
        {
            // Arrange
            var recording = new Recording { Sentence = "Sentence to speak" };
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

            var recordingsViewModel = TestUtils
                .CreateTestProvider(setup, _addRecordingsViewModelAction)
                .GetService<RecordingsViewModel>();

            // Act
            await recordingsViewModel.PlaySentenceAsync(recording);

            // Assert
            audioUseCase.Verify(x => x.SpeakSentenceAsync(recording.Sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(errorResponse, It.IsAny<ToastDuration>()), Times.Once());
            recordingsViewModel.VolumeIcon.Should().Be(MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task PlaySentenceAsync_SuccessResponseShouldWork()
        {
            // Arrange
            var recording = new Recording { Sentence = "Sentence to speak" };

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

            var recordingsViewModel = TestUtils
                .CreateTestProvider(setup, _addRecordingsViewModelAction)
                .GetService<RecordingsViewModel>();

            // Act
            await recordingsViewModel.PlaySentenceAsync(recording);

            // Assert
            audioUseCase.Verify(x => x.SpeakSentenceAsync(recording.Sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
            recordingsViewModel.VolumeIcon.Should().Be(MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task PlayAudioAsync_ShouldWork()
        {
            // Arrange
            var recording = new Recording { Sentence = "Sentence to speak" };
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

            var recordingsViewModel = TestUtils
                .CreateTestProvider(setup, _addRecordingsViewModelAction)
                .GetService<RecordingsViewModel>();

            // Act
            await recordingsViewModel.PlayAudioAsync(recording);

            // Assert
            //recordingsViewModel.IsRecording.Should().BeTrue();
            //recordingsViewModel.Filepath.Should().Be(successResponse.Data);
            audioUseCase.Verify(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
        }

        [Fact]
        public async Task PlayAudioAsync_ErrorShouldShowMessage()
        {
            // Arrange
            var recording = new Recording { Sentence = "Sentence to speak" };
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

            var recordingsViewModel = TestUtils
                .CreateTestProvider(setup, _addRecordingsViewModelAction)
                .GetService<RecordingsViewModel>();

            // Act
            await recordingsViewModel.PlayAudioAsync(recording);

            // Assert
            //recordingsViewModel.IsRecording.Should().BeTrue();
            //recordingsViewModel.Filepath.Should().Be(null);
            audioUseCase.Verify(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()), Times.Once());
            alertService.Verify(x => x.ShowToast(failureResponse.Message, It.IsAny<ToastDuration>()), Times.Once());
        }
    }
}

