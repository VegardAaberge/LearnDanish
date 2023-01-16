using System;
using SpeakDanish.Contracts.Shared;
using System.Threading.Tasks;

namespace SpeakDanish.Contracts.Data
{
	public interface ISpeechService
	{
        Task<Response<string>> TranscribeDanishSpeechToText(string filepath);
    }
}

