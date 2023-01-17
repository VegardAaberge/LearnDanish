using System;
using SpeakDanish.Contracts.Shared;
using System.Threading.Tasks;

namespace SpeakDanish.Contracts.Data
{
	public interface ISpeechService<T> where T: class
	{
        Task<Response<string>> TranscribeDanishSpeechFromFile(string filepath);

        public void StartTranscribingDanish(Action<T> recognizedCallback);

        public void StopTranscribingDanish();
    }
}

