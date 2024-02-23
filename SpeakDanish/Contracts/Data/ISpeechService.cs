using System;
using SpeakDanish.Contracts.Shared;
using System.Threading.Tasks;

namespace SpeakDanish.Contracts.Data
{
	public interface ISpeechService<T> where T: class
	{
        Task<Response<string>> TranscribeDanishSpeechFromFile(string filepath);

        Task StartTranscribingDanish(Action<T> recognizedCallback);

        Task StopTranscribingDanish();
    }
}

