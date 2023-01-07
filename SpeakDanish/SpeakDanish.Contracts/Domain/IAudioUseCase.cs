using System;
using System.Threading.Tasks;
using System.Timers;

namespace SpeakDanish.Contracts.Domain
{
	public interface IAudioUseCase
	{
		Task<Response> SpeakSentenceAsync(string sentence, ElapsedEventHandler volumeTimerCallback);

		Task<Response<string>> StartRecordingAsync(ElapsedEventHandler countTimer);

        Task<Response> StopRecordingAsync(string filepath);
    }
}

