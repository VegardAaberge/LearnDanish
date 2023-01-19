using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using Xamarin.Essentials.Interfaces;
using SpeakDanish.Domain.Models;
using SpeakDanish.Contracts;
using Xamarin.Essentials;
using SpeakDanish.Contracts.Shared;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Data.Api;
using Microsoft.CognitiveServices.Speech;

namespace SpeakDanish.Domain.UseCases
{
	public class AudioUseCase : IAudioUseCase
    {
        private IPermissions _permissions;
        private ITtsDataInstaller _ttsDataInstaller;
        private IAudioRecorder _audioRecorder;
        private ISpeechService<TranscriptionResult> _speechServices;
        private ITextToSpeech _textToSpeech;

        private CancellationTokenSource _cancelSpeakTokenSource = new CancellationTokenSource();
        private Timer _volumeTimer = new Timer();
        private Timer _countTimer = new Timer();

        public AudioUseCase(
            IPermissions permissions,
            ITtsDataInstaller ttsDataInstaller,
            IAudioRecorder audioRecorder,
            ISpeechService<TranscriptionResult> speechServices,
            ITextToSpeech textToSpeech)
		{
            _permissions = permissions;
            _ttsDataInstaller = ttsDataInstaller;
            _audioRecorder = audioRecorder;
            _speechServices = speechServices;
            _textToSpeech = textToSpeech;
        }

        public async Task<Response> SpeakSentenceAsync(string sentence, ElapsedEventHandler volumeTimerCallback)
        {
            _cancelSpeakTokenSource?.Cancel();

            var locales = await _textToSpeech.GetLocalesAsync();
            var locale = locales.FirstOrDefault(x => x.Language == "da");

            if (locale == null)
            {
                _ttsDataInstaller.InstallTtsData();

                locales = await _textToSpeech.GetLocalesAsync();
                locale = locales.FirstOrDefault(x => x.Language == "da");

                if (locale == null)
                {
                    return new Response(false, "No Danish language found");
                }
            }

            _cancelSpeakTokenSource = new CancellationTokenSource();
            _volumeTimer.Stop();
            _volumeTimer = new Timer(300);
            _volumeTimer.Elapsed += volumeTimerCallback;
            _volumeTimer.Start();

            await _textToSpeech.SpeakAsync(sentence, new SpeechOptions
            {
                Locale = locale
            }, _cancelSpeakTokenSource.Token);

            _volumeTimer.Stop();

            return new Response(true);
        }

        public async Task<Response<string>> StartRecordingAsync(ElapsedEventHandler countTimer)
        {
            if (_countTimer.Enabled)
                return new Response<string>(false, "Already recording");

            var microphoneStatus = await _permissions.CheckStatusAsync<Permissions.Microphone>();
            var storageStatus = await _permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (microphoneStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                if (microphoneStatus != PermissionStatus.Granted)
                    await _permissions.RequestAsync<Permissions.Microphone>();
                if (storageStatus != PermissionStatus.Granted)
                    await _permissions.RequestAsync<Permissions.StorageWrite>();

                return new Response<string>(false, "Permission denied");
            }

            var filepath = await _audioRecorder.StartRecordingAudio("recording");
            StartTimer(countTimer);

            return new Response<string>
            {
                Success = true,
                Data = filepath
            };
        }

        public async Task<Response> StopRecordingAsync(string filepath)
        {
            await _audioRecorder.StopRecordingAudio(filepath);

            StopTimer();

            return new Response(true);
        }

        public async Task<Response> PlayAudioAsync(string filepath)
        {
            await _audioRecorder.PlayAudio(filepath);

            return new Response(true);
        }

        public async Task StartTranscribingDanish(ElapsedEventHandler countTimer, Action<string> recognizedCallback)
        {
            StartTimer(countTimer);

            await _speechServices.StartTranscribingDanish(result =>
            {
                if(result.Reason == ResultReason.RecognizedSpeech)
                {
                    recognizedCallback(result.Text);
                }
            });
        }

        public async Task StopTranscribingDanish()
        {
            StopTimer();

            await _speechServices.StopTranscribingDanish();
        }

        private void StartTimer(ElapsedEventHandler countTimer)
        {
            _countTimer = new Timer(1000);
            _countTimer.Elapsed += countTimer;
            _countTimer.Start();
        }

        private void StopTimer()
        {
            _countTimer.Stop();
        }
    }
}

