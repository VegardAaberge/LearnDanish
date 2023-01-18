using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Shared;
using SpeakDanish.Domain.Models;

namespace SpeakDanish.Data.Api
{
    public class SpeechService : ISpeechService<TranscriptionResult>
    {
        private ISpeechRecognizer _recognizer;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public SpeechService(ISpeechRecognizer recognizer)
		{
            _recognizer = recognizer;
        }

        private bool _isTranscribing;
        private TaskCompletionSource<int> stopRecognition = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task StartTranscribingDanish(Action<TranscriptionResult> recognizedCallback)
        {
            if (_isTranscribing)
                return;

            _recognizer.Recognized += (s, e) =>
            {
                recognizedCallback(new TranscriptionResult
                {
                    Text = e.Result.Text,
                    Reason = e.Result.Reason,
                    Properties = e.Result.Properties,
                    OffsetInTicks = e.Result.OffsetInTicks,
                    Duration = e.Result.Duration
                });
            };
            _recognizer.SessionStopped += async (s, e) => await StopTranscribingDanish();
            _recognizer.Canceled += async (s, e) => await StopTranscribingDanish();

            await _recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
            _isTranscribing = true;

            await Task.Run(() =>
            {
                Task.WaitAny(new[] { stopRecognition.Task });  
            }).ConfigureAwait(false);
        }

        public async Task StopTranscribingDanish()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_isTranscribing)
                    return;

                await _recognizer.StopContinuousRecognitionAsync();
                stopRecognition.TrySetResult(0);
                _isTranscribing = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<string>> TranscribeDanishSpeechFromFile(string filepath)
        {
            using (var fileStream = System.IO.File.OpenRead(filepath))
            {
                var audioFormat = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1);
                var audioConfig = AudioConfig.FromStreamInput(new PullAudioInputStream(fileStream), audioFormat);
                var speechConfig = SpeechConfig.FromSubscription(Secrets.SPEECH_SUBSCRIPTION_KEY, AppSettings.SPEECH_REGION);
                speechConfig.SpeechRecognitionLanguage = AppSettings.SPEECH_RECOGNITION_LANGUAGE;

                using (var recognizer = new SpeechRecognizer(speechConfig, audioConfig))
                {

                    var result = await recognizer.RecognizeOnceAsync();

                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        return new Response<string>
                        {
                            Success = true,
                            Data = result.Text
                        };
                    }
                    else
                    {
                        return new Response<string>(false, result.Reason.ToString());
                    }
                }
            }
        }

        internal class PullAudioInputStream : PullAudioInputStreamCallback
        {
            private readonly Stream _stream;

            public PullAudioInputStream(Stream stream)
            {
                _stream = stream;
            }

            public override void Close()
            {
                _stream.Dispose();
                base.Close();
            }

            public override int Read(byte[] dataBuffer, uint size)
            {
                return _stream.Read(dataBuffer, 0, (int)size);
            }
        }
    }
}

