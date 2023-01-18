using System;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Shared;

namespace SpeakDanish.Data.Api
{
    public class SpeechRecognizerWrapper : ISpeechRecognizer
    {
        private SpeechRecognizer _speechRecognizer;

        private Timer _stopTimer = new Timer();
        private readonly int _stopTimerDuration = 30000;

        public SpeechRecognizerWrapper()
        {
            var speechConfig = SpeechConfig.FromSubscription(Secrets.SPEECH_SUBSCRIPTION_KEY, AppSettings.SPEECH_REGION)
            //speechConfig.SpeechRecognitionLanguage = AppSettings.SPEECH_RECOGNITION_LANGUAGE;
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

            _speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        }

        public event EventHandler<SpeechRecognitionEventArgs> Recognized
        {
            add => _speechRecognizer.Recognized += value;
            remove => _speechRecognizer.Recognized -= value;
        }

        public event EventHandler<SessionEventArgs> SessionStopped
        {
            add => _speechRecognizer.SessionStopped += value;
            remove => _speechRecognizer.SessionStopped -= value;
        }

        public event EventHandler<SpeechRecognitionCanceledEventArgs> Canceled
        {
            add => _speechRecognizer.Canceled += value;
            remove => _speechRecognizer.Canceled -= value;
        }

        public void Dispose() => _speechRecognizer.Dispose();

        public Task StartContinuousRecognitionAsync()
        {
            _stopTimer = new Timer(_stopTimerDuration);
            _stopTimer.Elapsed += async (s, e) =>
            {
                await StopContinuousRecognitionAsync();
            };
            _stopTimer.Start();

            return _speechRecognizer.StartContinuousRecognitionAsync();

        }

        public Task StopContinuousRecognitionAsync()
        {
            _stopTimer?.Stop();
            _stopTimer?.Dispose();
            return _speechRecognizer.StopContinuousRecognitionAsync();
        }
    }
}

