using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Domain.UseCases;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace SpeakDanish.Tests.Domain.UseCases
{
	public class AudioUseCaseBuilder
	{
        public Mock<IPermissions> Permissions = new Mock<IPermissions>();
        public Mock<ITtsDataInstaller> TtsDataInstaller = new Mock<ITtsDataInstaller>();
        public Mock<IAudioRecorder> AudioRecorder = new Mock<IAudioRecorder>();
        public Mock<ITextToSpeech> TextToSpeech = new Mock<ITextToSpeech>();

        public AudioUseCase AudioUseCase { get; set; }

        public AudioUseCaseBuilder Build()
        {
            AudioUseCase = new AudioUseCase(
                Permissions.Object,
                TtsDataInstaller.Object,
                AudioRecorder.Object,
                TextToSpeech.Object
            );
            return this;
        }

        public AudioUseCaseBuilder WithSpeakAsync(List<Locale> locales)
        {
            TextToSpeech
                    .Setup(tts => tts.GetLocalesAsync())
                    .ReturnsAsync(locales);
            TextToSpeech
                .Setup(tts => tts.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            return this;
        }

        public AudioUseCaseBuilder WithStoragePermission(PermissionStatus status, PermissionStatus response)
        {
            Permissions
                .Setup(p => p.CheckStatusAsync<Permissions.StorageWrite>())
                .ReturnsAsync(status);
            Permissions
                .Setup(p => p.RequestAsync<Permissions.StorageWrite>())
                .ReturnsAsync(response);
            return this;
        }

        public AudioUseCaseBuilder WithMicrophonePermission(PermissionStatus status, PermissionStatus response)
        {
            Permissions
                .Setup(p => p.CheckStatusAsync<Permissions.Microphone>())
                .ReturnsAsync(status);
            Permissions
                .Setup(p => p.RequestAsync<Permissions.Microphone>())
                .ReturnsAsync(response);
            return this;
        }

        public AudioUseCaseBuilder WithStartRecordingAudio(string recordingPath)
        {
            AudioRecorder
                    .Setup(x => x.StartRecordingAudio(It.IsAny<string>()))
                    .ReturnsAsync(recordingPath);
            return this;
        }

        internal AudioUseCaseBuilder WithStopRecordingAudio(string recordingPath)
        {
            AudioRecorder
                .Setup(x => x.StopRecordingAudio(It.IsAny<string>()));
            return this;
        }
    }
}

