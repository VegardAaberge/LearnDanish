using System;
using SpeakDanish.ViewModels;
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
                .WithGoBackAsync()
                .WithGetEvent()
                .Build();

            // Act
            await builder.RecordingsViewModel.RedoAsync(recording);

            AssertThat(builder)
                .VerifyGoBackAsync(Times.Once)
                .VerifyGetEvent(Times.Once)
                .IsBusy(false);
        }

        [Fact]
        public void RedoAsync_ShouldNotWorkIfBusy()
        {
            // Arrange
            var recording = new Recording
            {
                Sentence = "Sentence to speak",
                FilePath = "filepath"
            };

            var builder = new RecordingsViewModelBuilder()
                .WithGoBackAsync()
                .WithGetEvent()
                .Build()
                .IsBusy();

            // Act
            builder.RecordingsViewModel.RedoCommand.Execute(recording);

            AssertThat(builder)
                .VerifyGoBackAsync(Times.Never)
                .VerifyGetEvent(Times.Never)
                .RedoCommandCanExecute(false, recording);
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

            internal RecordingsViewModelStateVerifier VerifyGoBackAsync(Func<Times> times)
            {
                _builder.Navigation.Verify(x => x.GoBackAsync(), times);
                return this;
            }

            internal RecordingsViewModelStateVerifier VerifyGetEvent(Func<Times> times)
            {
                _builder.EventAggregator.Verify(x => x.GetEvent<AppEvents.RecordingSelectedEvent>(), times);
                return this;
            }

            internal RecordingsViewModelStateVerifier RedoCommandCanExecute(bool canExecute, Recording recording)
            {
                _builder.RecordingsViewModel.RedoCommand
                    .CanExecute(recording)
                    .Should()
                    .Be(canExecute);
                return this;
            }

            internal void IsBusy(bool value)
            {
                _builder.RecordingsViewModel.IsBusy.Should().Be(value);
            }
        }
    }
}

