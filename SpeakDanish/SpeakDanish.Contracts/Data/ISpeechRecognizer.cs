using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace SpeakDanish.Contracts.Data
{
    public interface ISpeechRecognizer
    {
        event EventHandler<SpeechRecognitionEventArgs> Recognized;
        event EventHandler<SessionEventArgs> SessionStopped;
        event EventHandler<SpeechRecognitionCanceledEventArgs> Canceled;

        Task StartContinuousRecognitionAsync();
        Task StopContinuousRecognitionAsync();
        void Dispose();
    }
}

