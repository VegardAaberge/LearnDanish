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

namespace SpeakDanish.Tests.Domain.UseCases
{
	public class AudioUseCasesTests
	{
        private Locale _danishLocale;

        public AudioUseCasesTests()
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
            var mockTextToSpeech = new Mock<ITextToSpeech>();
            var mockPermissions = new Mock<IPermissions>();
            var mockTtsDataInstaller = new Mock<ITtsDataInstaller>();
            var mockAudioRecorder = new Mock<IAudioRecorder>();

            // Arrange
            mockTextToSpeech
                .Setup(tts => tts.GetLocalesAsync())
                .ReturnsAsync(new List<Locale> { _danishLocale });
            mockTextToSpeech
                .Setup(tts => tts.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var audioUseCase = new AudioUseCase(
                mockPermissions.Object,
                mockTtsDataInstaller.Object,
                mockAudioRecorder.Object,
                mockTextToSpeech.Object
            );

            // Act
            var result = await audioUseCase.SpeakSentenceAsync("Hej!", null);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            mockTextToSpeech.Verify(tts => tts.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async void SpeakSentenceAsync_ShouldReturnError_WhenDanishLanguageNotFound()
        {
            var mockTextToSpeech = new Mock<ITextToSpeech>();
            var mockPermissions = new Mock<IPermissions>();
            var mockTtsDataInstaller = new Mock<ITtsDataInstaller>();
            var mockAudioRecorder = new Mock<IAudioRecorder>();

            // Arrange
            mockTextToSpeech
                .Setup(tts => tts.GetLocalesAsync())
                .ReturnsAsync(new List<Locale> { });
            mockTextToSpeech
                .Setup(tts => tts.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var audioUseCase = new AudioUseCase(
                mockPermissions.Object,
                mockTtsDataInstaller.Object,
                mockAudioRecorder.Object,
                mockTextToSpeech.Object
            );

            // Act
            var result = await audioUseCase.SpeakSentenceAsync("Hej!", null);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().NotBeNullOrWhiteSpace();
            mockTextToSpeech.Verify(tts => tts.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async void StartRecordingAsync_ShouldReturnCorrectResponse()
        {
            // Arrange
            var recordingPath = "/path/to/recording.mp3";
            var permissionsMock = new Mock<IPermissions>();
            var ttsDataInstallerMock = new Mock<ITtsDataInstaller>();
            var audioRecorderMock = new Mock<IAudioRecorder>();
            var textToSpeechMock = new Mock<ITextToSpeech>();

            permissionsMock.Setup(p => p.CheckStatusAsync<Permissions.Microphone>())
                .ReturnsAsync(PermissionStatus.Granted);
            permissionsMock.Setup(p => p.CheckStatusAsync<Permissions.StorageWrite>())
                .ReturnsAsync(PermissionStatus.Granted);

            audioRecorderMock.Setup(ar => ar.StartRecordingAudio(It.IsAny<string>()))
                .ReturnsAsync(recordingPath);

            var audioUseCase = new AudioUseCase(
                permissionsMock.Object,
                ttsDataInstallerMock.Object,
                audioRecorderMock.Object,
                textToSpeechMock.Object
            );

            // Act
            var result = await audioUseCase.StartRecordingAsync(null);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().Be(recordingPath);

            permissionsMock.Verify(x => x.CheckStatusAsync<Permissions.Microphone>(), Times.Once);
            permissionsMock.Verify(x => x.CheckStatusAsync<Permissions.StorageWrite>(), Times.Once);
            audioRecorderMock.Verify(x => x.StartRecordingAudio(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void StopRecordingAsync_ShouldWork()
        {
            // Arrange
            var recordingPath = "/path/to/recording.mp3";
            var permissionsMock = new Mock<IPermissions>();
            var ttsDataInstallerMock = new Mock<ITtsDataInstaller>();
            var audioRecorderMock = new Mock<IAudioRecorder>();
            var textToSpeechMock = new Mock<ITextToSpeech>();

            audioRecorderMock.Setup(x => x.StopRecordingAudio(It.IsAny<string>()));

            var audioUseCase = new AudioUseCase(
                permissionsMock.Object,
                ttsDataInstallerMock.Object,
                audioRecorderMock.Object,
                textToSpeechMock.Object
            );

            // Act
            var result = await audioUseCase.StopRecordingAsync(recordingPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();

            audioRecorderMock.Verify(x => x.StopRecordingAudio(It.IsAny<string>()), Times.Once);
        }
    }
}

