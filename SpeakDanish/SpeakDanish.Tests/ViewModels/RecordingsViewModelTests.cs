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
using SpeakDanish.Domain.Models;

namespace SpeakDanish.Tests.ViewModel
{
    public class RecordingsViewModelTests
    {
        public RecordingsViewModelTests()
        {
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "error")]
        public async Task PlaySentenceAsync_ShouldWork(bool success, string message)
        {
            // Arrange
            var recording = new Recording { Sentence = "Sentence to speak" };
            var response = new Response(success, message);

            var builder = new RecordingsViewModelBuilder()
                .WithSpeakSentenceAsync(response)
                .Build();

            // Act
            await builder.RecordingsViewModel.PlaySentenceAsync(recording);

            // Assert
            AssertThat(builder)
                .VerifySpeakSentenceAsync(recording.Sentence, Times.Once)
                .VerifyShowToast(success ? Times.Never : Times.Once)
                .VolumeIcon(recording, MaterialDesignIconsFont.VolumeHigh);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "error")]
        public async Task PlayAudioAsync_ShouldWork(bool success, string message)
        {
            // Arrange
            var recording = new Recording { FilePath = "filepath" };
            var response = new Response(success, message);

            var builder = new RecordingsViewModelBuilder()
                .WithPlayAudioAsync(response)
                .Build();

            // Act
            await builder.RecordingsViewModel.PlayAudioAsync(recording);

            // Assert
            AssertThat(builder)
                .VerifyPlayAudioAsync(recording.FilePath)
                .VerifyShowToast(success ? Times.Never : Times.Once)
                .VolumeIcon(recording, MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task RedoAsync_ShouldWork()
        {
            // Arrange
            var recording = new Recording {
                Sentence = "Sentence to speak",
                FilePath = "filepath"
            };

            var builder = new RecordingsViewModelBuilder()
                .WithPopAsync()
                .Build();

            // Act
            await builder.RecordingsViewModel.RedoAsync(recording);

            AssertThat(builder)
                .VerifyPopAsync(Times.Once);
        }

        [Fact]
        public async Task RedoAsync_ShouldWorkWhenDoubleTapped()
        {
            // Arrange
            var recording = new Recording
            {
                Sentence = "Sentence to speak",
                FilePath = "filepath"
            };

            var builder = new RecordingsViewModelBuilder()
                .WithPopAsync()
                .Build();

            // Act
            await builder.RecordingsViewModel.RedoAsync(recording);
            await Task.Delay(50);
            await builder.RecordingsViewModel.RedoAsync(recording);

            AssertThat(builder)
                .VerifyPopAsync(Times.Once);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "error")]
        public async Task DeleteAsync_ShouldWork(bool success, string message)
        {
            // Arrange
            var recording = new Recording { Sentence = "Sentence to speak" };
            var response = new Response(success, message);
            var rowsModified = success ? 1 : 0;

            var builder = new RecordingsViewModelBuilder()
                .WithDeleteRecordingAsync(rowsModified)
                .Build();

            // Act
            await builder.RecordingsViewModel.DeleteAsync(recording);

            AssertThat(builder)
                .VerifyDeleteRecordingAsync(recording, Times.Once)
                .VerifyShowToast(success ? Times.Never : Times.Once);
        }

        RecordingsViewModelStateVerifier AssertThat(RecordingsViewModelBuilder builder)
        {
            return new RecordingsViewModelStateVerifier(builder);
        }

        internal class RecordingsViewModelStateVerifier
        {
            private RecordingsViewModelBuilder _builder;

            public RecordingsViewModelStateVerifier(RecordingsViewModelBuilder builder)
            {
                _builder = builder;
            }

            internal RecordingsViewModelStateVerifier VerifyDeleteRecordingAsync(Recording recording, Func<Times> times)
            {
                _builder.RecordingService.Verify(x => x.DeleteRecordingAsync(recording), Times.Once);
                return this;
            }

            internal RecordingsViewModelStateVerifier VerifyPlayAudioAsync(string filePath)
            {
                _builder.AudioUseCase.Verify(x => x.PlayAudioAsync(filePath));
                return this;
            }

            internal RecordingsViewModelStateVerifier VerifyPopAsync(Func<Times> times)
            {
                _builder.Navigation.Verify(x => x.PopAsync(It.IsAny<bool>()), times);
                return this;
            }

            internal RecordingsViewModelStateVerifier VerifyShowToast(Func<Times> times)
            {
                _builder.AlertService.Verify(x => x.ShowToast(It.IsAny<string>(), It.IsAny<ToastDuration>()), times);
                return this;
            }

            internal RecordingsViewModelStateVerifier VerifySpeakSentenceAsync(string sentence, Func<Times> times)
            {
                _builder.AudioUseCase.Verify(x => x.SpeakSentenceAsync(sentence, It.IsAny<ElapsedEventHandler>()), Times.Once());
                return this;
            }

            internal RecordingsViewModelStateVerifier VolumeIcon(Recording recording, string icon)
            {
                recording.VolumeIcon.Should().Be(icon);
                return this;
            }
        }
    }
}

