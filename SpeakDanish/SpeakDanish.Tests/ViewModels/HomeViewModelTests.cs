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
using SpeakDanish.Tests.ViewModels;

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

            var builder = new HomeViewModelBuilder()
                .WithGetRandomSentence((a, b) => a == previousSentence ? nextSentence : previousSentence)
                .Build();

            var actualPreviousSentence = builder.HomeViewModel.Sentence?.ToString();

            // Act
            await builder.HomeViewModel.LoadRandomSentence();

            // Arrange
            actualPreviousSentence.Should().Be(previousSentence);
            builder.HomeViewModel.Sentence.Should().Be(nextSentence);
            builder.SentenceService.Verify(x => x.GetRandomSentence(previousSentence, It.IsAny<Task<string>>()), Times.Once());
        }

        [Fact]
        public async Task SpeakSentenceAsync_FailedResponseShouldShowAlert()
        {
            // Arrange
            var sentence = "Sentence to speak";
            var errorResponse = new Response(false, "Error");

            var builder = new HomeViewModelBuilder()
                .WithSpeakSentenceAsync(errorResponse)
                .Build();
            builder.HomeViewModel.Sentence = sentence;

            // Act
            await builder.HomeViewModel.SpeakSentenceAsync();

            // Assert
            builder.AudioUseCase.Verify(x => x.SpeakSentenceAsync(sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
            builder.AlertService.Verify(x => x.ShowToast(errorResponse.Message, It.IsAny<ToastDuration>()), Times.Once());
            builder.HomeViewModel.VolumeIcon.Should().Be(MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task SpeakSentenceAsync_SuccessResponseShouldWork()
        {
            // Arrange
            var sentence = "Sentence to speak";
            var successReponse = new Response(true);

            var builder = new HomeViewModelBuilder()
                .WithSpeakSentenceAsync(successReponse)
                .Build();
            builder.HomeViewModel.Sentence = sentence;

            // Act
            await builder.HomeViewModel.SpeakSentenceAsync();

            // Assert
            builder.AudioUseCase.Verify(x => x.SpeakSentenceAsync(sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
            builder.AlertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
            builder.HomeViewModel.VolumeIcon.Should().Be(MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task StartRecordingAsync_ShouldWork()
        {
            // Arrange
            var successResponse = new Response<string> { Success = true, Data = "filepath" };

            var builder = new HomeViewModelBuilder()
                .WithStartRecordingAsync(successResponse)
                .Build();

            // Act
            await builder.HomeViewModel.StartRecordingAsync();

            // Assert
            builder.HomeViewModel.IsRecording.Should().BeTrue();
            builder.HomeViewModel.Filepath.Should().Be(successResponse.Data);
            builder.AudioUseCase.Verify(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()), Times.Once());
            builder.AlertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
        }

        [Fact]
        public async Task StartRecordingAsync_ErrorShouldShowMessage()
        {
            // Arrange
            var failureResponse = new Response<string> { Success = false, Message = "error message" };

            var builder = new HomeViewModelBuilder()
                .WithStartRecordingAsync(failureResponse)
                .Build();

            // Act
            await builder.HomeViewModel.StartRecordingAsync();

            // Assert
            builder.HomeViewModel.IsRecording.Should().BeTrue();
            builder.HomeViewModel.Filepath.Should().Be(null);
            builder.AudioUseCase.Verify(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()), Times.Once());
            builder.AlertService.Verify(x => x.ShowToast(failureResponse.Message, It.IsAny<ToastDuration>()), Times.Once());
        }

        [Fact]
        public async Task StopRecordingAsync_ShouldWork()
        {
            // Arrange
            var path = "filepath";
            var successResponse = new Response(true);

            var builder = new HomeViewModelBuilder()
                .WithStopRecordingAsync(successResponse)
                .Build()
                .UserIsRecording(path);

            // Act
            await builder.HomeViewModel.StopRecordingAsync();

            // Assert
            builder.HomeViewModel.IsRecording.Should().BeFalse();
            builder.HomeViewModel.CountSeconds.Should().Be(0);
            builder.AudioUseCase.Verify(x => x.StopRecordingAsync(path), Times.Once());
            builder.AlertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), Times.Never());
        }
    }
}

