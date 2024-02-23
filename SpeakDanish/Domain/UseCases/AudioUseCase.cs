using System;
using System.Linq;
using System.Timers;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using EI = Xamarin.Essentials.Interfaces;
using SpeakDanish.Domain.Models;
using SpeakDanish.Contracts.Shared;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Data.Api;
using Microsoft.CognitiveServices.Speech;

namespace SpeakDanish.Domain.UseCases
{
	public class AudioUseCase : IAudioUseCase
    {
        private EI.IPermissions _permissions;
        private ITtsDataInstaller _ttsDataInstaller;
        private IAudioRecorder _audioRecorder;
        private ISpeechService<TranscriptionResult> _speechServices;
        private ITextToSpeech _textToSpeech;

        private CancellationTokenSource _cancelSpeakTokenSource = new CancellationTokenSource();
        private System.Timers.Timer _countTimer = new System.Timers.Timer();
        public AudioUseCase(
            EI.IPermissions permissions,
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

            Locale? locale = await FindDanishLocaleAsync();

            if (locale == null)
            {
                _ttsDataInstaller.InstallTtsData();

                locale = await FindDanishLocaleAsync();

                if (locale == null)
                {
                    return new Response(false, "No Danish language found");
                }
            }

            _cancelSpeakTokenSource = new CancellationTokenSource();
            StartTimer(volumeTimerCallback, 300);

            await _textToSpeech.SpeakAsync(sentence, new SpeechOptions
            {
                Locale = locale
            }, _cancelSpeakTokenSource.Token);

            StopTimer();

            return new Response(true);
        }

        public async Task<Locale?> FindDanishLocaleAsync()
        {
            List<Locale> locales = (await TextToSpeech.GetLocalesAsync()).ToList();

            // Attempt to find a locale with the language code "da" (ISO 639-1) or "dan" (ISO 639-2/T)
            return locales.FirstOrDefault(x => x.Language.Equals("da", StringComparison.OrdinalIgnoreCase) || x.Language.Equals("dan", StringComparison.OrdinalIgnoreCase));
        }

        private async Task<Response<string>> VerifyUserPermissions()
        {
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

            return new Response<string>(true, "Permissions accepted");
        }

        public async Task<Response<string>> StartRecordingAsync(string filename, ElapsedEventHandler countTimer)
        {
            if (_countTimer.Enabled)
                return new Response<string>(false, "Already recording");

            var response = await VerifyUserPermissions();
            if (!response.Success)
                return response;

            var filepath = await _audioRecorder.StartRecordingAudio(filename);
            StartTimer(countTimer, 1000);

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

        public async Task<Response> PlayAudioAsync(string filepath, ElapsedEventHandler volumeTimerCallback)
        {
            StartTimer(volumeTimerCallback, 300);
            await _audioRecorder.PlayAudio(filepath);
            StopTimer();

            return new Response(true);
        }

        public async Task StartTranscribingDanish(ElapsedEventHandler countTimer, Action<string> recognizedCallback)
        {
            var response = await VerifyUserPermissions();
            if (!response.Success)
                return;

            StartTimer(countTimer, 1000);

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

        private void StartTimer(ElapsedEventHandler countTimer, int duration)
        {
            _countTimer.Stop();
            _countTimer = new System.Timers.Timer(duration);
            _countTimer.Elapsed += countTimer;
            _countTimer.Start();
        }

        private void StopTimer()
        {
            _countTimer.Stop();
        }
    }
}

