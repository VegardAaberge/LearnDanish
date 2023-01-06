using System;
using System.Threading.Tasks;

namespace SpeakDanish.Contracts.Platform
{
	public interface IAudioRecorder
	{
        Task<String> StartRecordingAudio(string filename);
        Task StopRecordingAudio(string filepath);
    }
}

