using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using SpeakDanish.Domain.Models;
using SpeakDanish.Contracts;

namespace SpeakDanish.Domain.UseCases
{
	public class AudioUseCase : IAudioUseCase
    {
        private ITtsDataInstaller _ttsDataInstaller;
        private IAudioRecorder _audioRecorder;
        private ITextToSpeech _textToSpeech;

        private CancellationTokenSource _cancelSpeakTokenSource = new CancellationTokenSource();
        private Timer _volumeTimer = new Timer();
        private Timer _countTimer = new Timer();

        public AudioUseCase(
            ITtsDataInstaller ttsDataInstaller,
            IAudioRecorder audioRecorder,
            ITextToSpeech textToSpeech)
		{
            _ttsDataInstaller = ttsDataInstaller;
            _audioRecorder = audioRecorder;
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

            await TextToSpeech.SpeakAsync(sentence, new SpeechOptions
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

            var microphoneStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (microphoneStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                if (microphoneStatus != PermissionStatus.Granted)
                    await Permissions.RequestAsync<Permissions.Microphone>();
                if (storageStatus != PermissionStatus.Granted)
                    await Permissions.RequestAsync<Permissions.StorageWrite>();

                return new Response<string>(false, "Permission denied");
            }

            var filepath = await _audioRecorder.StartRecordingAudio("recording");
            _countTimer = new Timer(1000);
            _countTimer.Elapsed += countTimer;
            _countTimer.Start();

            return new Response<string>
            {
                Success = true,
                Data = filepath
            };
        }

        public async Task<Response> StopRecordingAsync(string filepath)
        {
            await _audioRecorder.StopRecordingAudio(filepath);

            _countTimer.Stop();

            return new Response(true);
        }
    }
}

