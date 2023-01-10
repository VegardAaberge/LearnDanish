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

            AssertThat(builder)
                .Sentence(previousSentence);

            // Act
            await builder.HomeViewModel.LoadRandomSentence();

            // Arrange
            AssertThat(builder)
                .Sentence(nextSentence)
                .VerifyGetRandomSentence(previousSentence, Times.Once);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "error")]
        public async Task SpeakSentenceAsync_ShouldWork(bool success, string message)
        {
            // Arrange
            var sentence = "Sentence to speak";
            var errorResponse = new Response(success, message);
            Func<Times> times = success ? Times.Never : Times.Once;

            var builder = new HomeViewModelBuilder()
                .WithSpeakSentenceAsync(errorResponse)
                .Build()
                .UpdateSentence(sentence);

            // Act
            await builder.HomeViewModel.SpeakSentenceAsync();

            // Assert
            AssertThat(builder)
                .VerifySpeakSentenceAsync(sentence, Times.Once)
                .VerifyShowToast(times)
                .VolumeIcon(MaterialDesignIconsFont.VolumeHigh);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "error")]
        public async Task StartRecordingAsync_ShouldWork(bool success, string message)
        {
            // Arrange
            var data = success ? "filepath" : null;
            var response = new Response<string>(success, message, data);


            var builder = new HomeViewModelBuilder()
                .WithStartRecordingAsync(response)
                .Build();

            // Act
            await builder.HomeViewModel.StartRecordingAsync();

            // Assert
            AssertThat(builder)
                .IsRecording(success ? true : false)
                .FilePath(data)
                .VerifyStartRecordingAsync(Times.Once)
                .VerifyShowToast(success ? Times.Never : Times.Once);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "error")]
        public async Task StopRecordingAsync_ShouldWork(bool success, string message)
        {
            // Arrange
            var path = "filepath";
            var successResponse = new Response(success, message);

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
                .VerifyShowToast(success ? Times.Never : Times.Once);
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

