using System;
using SpeakDanish.Data;

namespace SpeakDanish.Domain.Models
{
	public class Recording
	{
        public int Id { get; set; }

        public string Sentence { get; set; }

        public string FilePath { get; set; }

        public DateTime Created { get; set; }

        public Recording FromRecording(Recording recording)
        {
            return new Recording
            {
                Sentence = recording.Sentence,
                FilePath = recording.FilePath,
                Created = recording.Created
            };
        }
    }
}

