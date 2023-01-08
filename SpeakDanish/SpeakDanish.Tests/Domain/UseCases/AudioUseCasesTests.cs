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
            // Arrange
            var mockTextToSpeech = new Mock<ITextToSpeech>();

            var setupDictionary = new Dictionary<Type, Action<Mock>>();
            setupDictionary.Add(typeof(ITextToSpeech), mock =>
            {
                mockTextToSpeech = mock as Mock<ITextToSpeech>;
                mockTextToSpeech
                    .Setup(tts => tts.GetLocalesAsync())
                    .ReturnsAsync(new List<Locale> { _danishLocale });
                mockTextToSpeech
                    .Setup(tts => tts.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            });

            var audioUseCase = TestProviderHelper
                .CreateTestProvider(setupDictionary)
                .GetService<IAudioUseCase>();

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
            // Arrange
            var mockTextToSpeech = new Mock<ITextToSpeech>();

            var setupDictionary = new Dictionary<Type, Action<Mock>>();
            setupDictionary.Add(typeof(ITextToSpeech), mock =>
            {
                mockTextToSpeech = mock as Mock<ITextToSpeech>;
                mockTextToSpeech
                    .Setup(tts => tts.GetLocalesAsync())
                    .ReturnsAsync(new List<Locale> { });
                mockTextToSpeech
                    .Setup(tts => tts.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            });

            var audioUseCase = TestProviderHelper
                .CreateTestProvider(setupDictionary)
                .GetService<IAudioUseCase>();

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
            var audioRecorderMock = new Mock<IAudioRecorder>();
            var permissionsMock = new Mock<IPermissions>();

            var setupDictionary = new Dictionary<Type, Action<Mock>>();
            setupDictionary.Add(typeof(IPermissions), mock =>
            {
                permissionsMock = mock as Mock<IPermissions>;
                permissionsMock.Setup(p => p.CheckStatusAsync<Permissions.Microphone>())
                .ReturnsAsync(PermissionStatus.Granted);
                permissionsMock.Setup(p => p.CheckStatusAsync<Permissions.StorageWrite>())
                    .ReturnsAsync(PermissionStatus.Granted);
            });
            setupDictionary.Add(typeof(IAudioRecorder), mock =>
            {
                audioRecorderMock = mock as Mock<IAudioRecorder>;
                audioRecorderMock
                    .Setup(x => x.StartRecordingAudio(It.IsAny<string>()))
                    .ReturnsAsync(recordingPath);
            });

            var audioUseCase = TestProviderHelper
                .CreateTestProvider(setupDictionary)
                .GetService<IAudioUseCase>();

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
            Mock<IAudioRecorder> audioRecorderMock = new Mock<IAudioRecorder>();

            var setupDictionary = new Dictionary<Type, Action<Mock>>();
            setupDictionary.Add(typeof(IAudioRecorder), mock =>
            {
                audioRecorderMock = mock as Mock<IAudioRecorder>;
                audioRecorderMock.Setup(x => x.StopRecordingAudio(It.IsAny<string>()));
            });

            var audioUseCase = TestProviderHelper
                .CreateTestProvider(setupDictionary)
                .GetService<IAudioUseCase>();

            // Act
            var result = await audioUseCase.StopRecordingAsync(recordingPath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            audioRecorderMock.Verify(x => x.StopRecordingAudio(It.IsAny<string>()), Times.Once);
        }
    }
}

