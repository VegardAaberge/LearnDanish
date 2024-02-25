using System;
using System.Threading.Tasks;

namespace SpeakDanish.Contracts.Platform
{
	public interface IAudioRecorder
	{
        Task PlayAudio(string filepath);
        Task<string?> StartRecordingAudio(string filename);
        Task StopRecordingAudio(string filepath, int attempt = 0);
    }
}

