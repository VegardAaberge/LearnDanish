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
        public HomeViewModelTests()
        {
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
            AssertThat(builder)
                .Sentence(nextSentence)
                .VerifyGetRandomSentence(previousSentence, Times.Once);
        }

        [Fact]
        public async Task SpeakSentenceAsync_FailedResponseShouldShowAlert()
        {
            // Arrange
            var sentence = "Sentence to speak";
            var errorResponse = new Response(false, "Error");

            var builder = new HomeViewModelBuilder()
                .WithSpeakSentenceAsync(errorResponse)
                .Build()
                .UpdateSentence(sentence);

            // Act
            await builder.HomeViewModel.SpeakSentenceAsync();

            // Assert
            AssertThat(builder)
                .VerifySpeakSentenceAsync(sentence, Times.Once)
                .VerifyShowToast(Times.Once)
                .VolumeIcon(MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task SpeakSentenceAsync_SuccessResponseShouldWork()
        {
            // Arrange
            var sentence = "Sentence to speak";
            var successReponse = new Response(true);

            var builder = new HomeViewModelBuilder()
                .WithSpeakSentenceAsync(successReponse)
                .Build()
                .UpdateSentence(sentence);

            // Act
            await builder.HomeViewModel.SpeakSentenceAsync();

            // Assert
            AssertThat(builder)
                .VerifySpeakSentenceAsync(sentence, Times.Once)
                .VerifyShowToast(Times.Never)
                .VolumeIcon(MaterialDesignIconsFont.VolumeHigh);
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
            AssertThat(builder)
                .IsRecording(true)
                .FilePath(successResponse.Data)
                .VerifyStartRecordingAsync(Times.Once)
                .VerifyShowToast(Times.Never);
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
            AssertThat(builder)
                .IsRecording(false)
                .FilePath(null)
                .VerifyStartRecordingAsync(Times.Once)
                .VerifyShowToast(Times.Once);
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
                .UpdateUserIsRecording(path);

            // Act
            await builder.HomeViewModel.StopRecordingAsync();

            // Assert
            AssertThat(builder)
                .IsRecording(false)
                .VerifyStopRecordingAsync(path, Times.Once)
                .VerifyShowToast(Times.Never);
        }

        HomeViewModelStateVerifier AssertThat(HomeViewModelBuilder builder)
        {
            return new HomeViewModelStateVerifier(builder);
        }

        internal class HomeViewModelStateVerifier
        {
            private HomeViewModelBuilder _builder;

            public HomeViewModelStateVerifier(HomeViewModelBuilder builder)
            {
                _builder = builder;
            }

            internal HomeViewModelStateVerifier IsRecording(bool value)
            {
                _builder.HomeViewModel.IsRecording.Should().Be(value);
                if(!value)
                    _builder.HomeViewModel.CountSeconds.Should().Be(0);
                return this;
            }

            internal HomeViewModelStateVerifier IsCountSecond(int value)
            {
                _builder.HomeViewModel.CountSeconds.Should().Be(value);
                return this;
            }

            internal HomeViewModelStateVerifier VerifyStopRecordingAsync(string path, Func<Times> times)
            {
                _builder.AudioUseCase.Verify(x => x.StopRecordingAsync(path), times);
                return this;
            }

            internal HomeViewModelStateVerifier VerifyShowToast(Func<Times> times)
            {
                _builder.AlertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), times);
                return this;
            }

            internal HomeViewModelStateVerifier FilePath(string value)
            {
                _builder.HomeViewModel.Filepath.Should().Be(value);
                return this;
            }

            internal HomeViewModelStateVerifier VerifyStartRecordingAsync(Func<Times> times)
            {
                _builder.AudioUseCase.Verify(x => x.StartRecordingAsync(It.IsAny<ElapsedEventHandler>()), times);
                return this;
            }

            internal HomeViewModelStateVerifier VerifySpeakSentenceAsync(string sentence, Func<Times> times)
            {
                _builder.AudioUseCase.Verify(x => x.SpeakSentenceAsync(sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
                return this;
            }

            internal HomeViewModelStateVerifier VolumeIcon(string icon)
            {
                _builder.HomeViewModel.VolumeIcon.Should().Be(icon);
                return this;
            }

            internal HomeViewModelStateVerifier Sentence(string sentence)
            {
                _builder.HomeViewModel.Sentence.Should().Be(sentence);
                return this;
            }

            internal HomeViewModelStateVerifier VerifyGetRandomSentence(string sentence, Func<Times> times)
            {
                _builder.SentenceService.Verify(x => x.GetRandomSentence(sentence, It.IsAny<Task<string>>()), times);
                return this;
            }
        }
    }
}

