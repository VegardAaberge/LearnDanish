using System;
using SpeakDanish.Data.Models;
using SpeakDanish.Domain.Models;

namespace SpeakDanish.Data.Mappers
{
    public static class RecordingMapper
    {
        public static RecordingEntity ToRecordingEntity(this Recording recording)
        {
            return new RecordingEntity
            {
                Id = recording.Id,
                Sentence = recording.Sentence,
                FilePath = recording.FilePath,
                Created = recording.Created,
                TranscribedText = recording.TranscribedText,
                Similarity = recording.Similarity
            };
        }

        public static Recording ToRecording(this RecordingEntity entity)
        {
            return new Recording
            {
                Id = entity.Id,
                Sentence = entity.Sentence,
                FilePath = entity.FilePath,
                Created = entity.Created,
                TranscribedText = entity.TranscribedText,
                Similarity = entity.Similarity
            };
        }
    }
}

