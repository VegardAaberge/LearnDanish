using System;
using Microsoft.CognitiveServices.Speech;

namespace SpeakDanish.Domain.Models
{
	public class TranscriptionResult
	{
        public string Text { get; set; }
        public ResultReason Reason { get; internal set; }
        public PropertyCollection Properties { get; internal set; }
        public long OffsetInTicks { get; internal set; }
        public TimeSpan Duration { get; internal set; }
    }
}

