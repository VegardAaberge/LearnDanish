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

        [Fact]
        public async Task SpeakSentenceAsync_FailedResponseShouldShowAlert()
        {
            // Arrange
            var errorResponse = new Response(false, "Error");
            var recording = new Recording { Sentence = "Sentence to speak" };

            var builder = new RecordingsViewModelBuilder()
                .WithSpeakSentenceAsync(errorResponse)
                .Build();

            // Act
            await builder.RecordingsViewModel.PlaySentenceAsync(recording);

            // Assert
            AssertThat(builder)
                .VerifySpeakSentenceAsync(recording.Sentence, Times.Once)
                .VerifyShowToast(Times.Once)
                .VolumeIcon(recording, MaterialDesignIconsFont.VolumeHigh);
        }

        [Fact]
        public async Task SpeakSentenceAsync_SuccessResponseShouldWork()
        {
            // Arrange
            var recording = new Recording { Sentence = "Sentence to speak" };
            var successReponse = new Response(true);

            var builder = new RecordingsViewModelBuilder()
                .WithSpeakSentenceAsync(successReponse)
                .Build();

            // Act
            await builder.RecordingsViewModel.PlaySentenceAsync(recording);

            // Assert
            AssertThat(builder)
                .VerifySpeakSentenceAsync(recording.Sentence, Times.Once)
                .VerifyShowToast(Times.Never)
                .VolumeIcon(recording, MaterialDesignIconsFont.VolumeHigh);
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

            internal void VolumeIcon(Recording recording, string icon)
            {
                recording.VolumeIcon.Should().Be(icon);
            }
        }
    }
}

