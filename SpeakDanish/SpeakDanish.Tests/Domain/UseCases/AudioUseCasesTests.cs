using System;
using Moq;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Domain.UseCases;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xunit;
using System.Reflection;
using System.Threading;
using FluentAssertions;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Tests.ViewModels;
using SpeakDanish.Contracts;

namespace SpeakDanish.Tests.Domain.UseCases
{
    public class AudioUseCaseTests
    {
        private Locale _danishLocale;
        private Action<ServiceCollection> _addAudioUseCase;

        public AudioUseCaseTests()
        {
            Type localeType = typeof(Locale);
            ConstructorInfo constructor = localeType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string)
            }, null);

            object[] constructorArgs = { "da", "", "Danish", "" };
            _danishLocale = (Locale)constructor.Invoke(constructorArgs);
        }

        [Fact]
        public async void SpeakSentenceAsync_ShouldSpeakSentence_WhenCalled()
        {
            // Arrange
            var sentence = "Sentence to speak";
            var builder = new AudioUseCaseBuilder()
                .WithSpeakAsync(new List<Locale>() { _danishLocale })
                .Build();

            // Act
            var result = await builder.AudioUseCase.SpeakSentenceAsync(sentence, null);

            // Assert
            AssertThat(builder)
                .IsSuccessful(result)
                .VerifySpeakAsync(sentence, _danishLocale, Times.Once);
        }

        [Fact]
        public async void SpeakSentenceAsync_ShouldReturnError_WhenDanishLanguageNotFound()
        {
            // Arrange
            var sentence = "Sentence to speak";
            var builder = new AudioUseCaseBuilder()
                .WithSpeakAsync(new List<Locale>())
                .Build();

            // Act
            var result = await builder.AudioUseCase.SpeakSentenceAsync(sentence, null);

            // Assert
            AssertThat(builder)
                .IsNotSuccessful(result)
                .VerifySpeakAsync(sentence, null, Times.Never);   
        }

        [Fact]
        public async void StartRecordingAsync_ShouldReturnCorrectResponse()
        {
            // Arrange
            var recordingPath = "/path/to/recording.mp3";

            var builder = new AudioUseCaseBuilder()
                .WithMicrophonePermission(PermissionStatus.Granted, PermissionStatus.Granted)
                .WithStoragePermission(PermissionStatus.Granted, PermissionStatus.Granted)
                .WithStartRecordingAudio(recordingPath)
                .Build();

            // Act
            var result = await builder.AudioUseCase.StartRecordingAsync(null);

            // Assert
            AssertThat(builder)
                .IsSuccessful(result)
                .ResultHasData(result, recordingPath)
                .MicrophonePermission(PermissionStatus.Granted, PermissionStatus.Granted)
                .StorageWritePermission(PermissionStatus.Granted, PermissionStatus.Granted)
                .VerifyStartRecordingAudio("recording", Times.Once);
        }

        [Fact]
        public async void StopRecordingAsync_ShouldWork()
        {
            // Arrange
            var recordingPath = "/path/to/recording.mp3";

            var builder = new AudioUseCaseBuilder()
                .WithStopRecordingAudio(recordingPath)
                .Build();

            // Act
            var result = await builder.AudioUseCase.StopRecordingAsync(recordingPath);

            // Assert
            AssertThat(builder)
                .IsSuccessful(result)
                .VerifyStopRecordingAudio(recordingPath, Times.Once);
        }

        [Fact]
        public async void PlayAudio_ShouldWork()
        {
            // Arrange
            var recordingPath = "/path/to/recording.mp3";

            var builder = new AudioUseCaseBuilder()
                .WithStopRecordingAudio(recordingPath)
                .Build();

            // Act
            var result = await builder.AudioUseCase.PlayAudioAsync(recordingPath);

            // Assert
            AssertThat(builder)
                .IsSuccessful(result)
                .VerifyPlayAudio(recordingPath, Times.Once);
        }

        AudioUseCaseVerifier AssertThat(AudioUseCaseBuilder builder)
        {
            return new AudioUseCaseVerifier(builder);
        }

        internal class AudioUseCaseVerifier
        {
            private AudioUseCaseBuilder _builder;

            public AudioUseCaseVerifier(AudioUseCaseBuilder builder)
            {
                _builder = builder;
            }

            internal AudioUseCaseVerifier IsSuccessful(Response result)
            {
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                return this;
            }

            internal AudioUseCaseVerifier ResultHasData(Response<string> result, string path)
            {
                result.Data.Should().Be(path);
                return this;
            }

            internal AudioUseCaseVerifier IsNotSuccessful(Response result)
            {
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.Message.Should().NotBeNullOrWhiteSpace();
                return this;
            }

            internal AudioUseCaseVerifier VerifySpeakAsync(string sentence, Locale locale, Func<Times> times)
            {
                _builder.TextToSpeech.Verify(
                    tts => tts.SpeakAsync(
                        sentence,
                        It.Is<SpeechOptions>(x => x.Locale == locale),
                        It.IsAny<CancellationToken>()
                    ), times
                );
                return this;
            }

            internal AudioUseCaseVerifier MicrophonePermission(PermissionStatus checkStatus, PermissionStatus requestStatus)
            {
                Func<Times> requestTimes = checkStatus == PermissionStatus.Granted ? Times.Never : Times.Once;
                _builder.Permissions.Verify(x => x.CheckStatusAsync<Permissions.Microphone>(), Times.Once);
                _builder.Permissions.Verify(x => x.RequestAsync<Permissions.Microphone>(), requestTimes);
                return this;
            }

            internal AudioUseCaseVerifier StorageWritePermission(PermissionStatus checkStatus, PermissionStatus requestStatus)
            {
                Func<Times> requestTimes = checkStatus == PermissionStatus.Granted ? Times.Never : Times.Once;
                _builder.Permissions.Verify(x => x.CheckStatusAsync<Permissions.StorageWrite>(), Times.Once);
                _builder.Permissions.Verify(x => x.RequestAsync<Permissions.StorageWrite>(), requestTimes);
                return this;
            }

            internal AudioUseCaseVerifier VerifyStartRecordingAudio(string filename, Func<Times> times)
            {
                _builder.AudioRecorder.Verify(x => x.StartRecordingAudio(filename), times);
                return this;
            }

            internal AudioUseCaseVerifier VerifyStopRecordingAudio(string filepath, Func<Times> times)
            {
                _builder.AudioRecorder.Verify(x => x.StopRecordingAudio(filepath), times);
                return this;
            }

            internal AudioUseCaseVerifier VerifyPlayAudio(string filepath, Func<Times> times)
            {
                _builder.AudioRecorder.Verify(x => x.PlayAudio(filepath), times);
                return this;
            }
        }
    }
}

