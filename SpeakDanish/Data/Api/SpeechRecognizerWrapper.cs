using System;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Shared;

namespace SpeakDanish.Data.Api
{
    public class SpeechRecognizerWrapper : ISpeechRecognizer
    {
        private SpeechRecognizer _speechRecognizer;

        private Timer _stopTimer;
        private readonly int _stopTimerDuration = 30000;

        public SpeechRecognizerWrapper()
        {
            var speechConfig = SpeechConfig.FromSubscription(Secrets.SPEECH_SUBSCRIPTION_KEY, AppSettings.SPEECH_REGION);
            speechConfig.SpeechRecognitionLanguage = AppSettings.SPEECH_RECOGNITION_LANGUAGE;
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
            TimerCallback callback = async state =>
            {
                await StopContinuousRecognitionAsync();
            };

            _stopTimer?.Dispose();
            _stopTimer = new Timer(callback, null, _stopTimerDuration, Timeout.Infinite);

            return _speechRecognizer.StartContinuousRecognitionAsync();

        }

        public Task StopContinuousRecognitionAsync()
        {
            if (_stopTimer != null)
            {
                _stopTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _stopTimer.Dispose();
                _stopTimer = null;
            }

            // Stop the continuous recognition task
            return _speechRecognizer.StopContinuousRecognitionAsync();
        }
    }
}

