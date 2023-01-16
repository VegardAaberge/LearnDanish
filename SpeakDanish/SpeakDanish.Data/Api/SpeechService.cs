using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SpeakDanish.Contracts;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Shared;
using static SQLite.SQLite3;

namespace SpeakDanish.Data.Api
{
	public class SpeechService : ISpeechService
    {
		public SpeechService()
		{
		}

        public async Task<Response<string>> TranscribeDanishSpeechToText(string filepath)
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

