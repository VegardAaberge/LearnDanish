using System;
using System.Threading.Tasks;

namespace LearnDanish.Services
{
	public interface IAudioRecorder
	{
        Task<String> StartRecordingAudio(string filename);
        Task StopRecordingAudio(string filepath);
    }
}

