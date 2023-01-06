using System;
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
                Created = recording.Created
            };
        }

        public static Recording ToRecording(this RecordingEntity entity)
        {
            return new Recording
            {
                Id = entity.Id,
                Sentence = entity.Sentence,
                FilePath = entity.FilePath,
                Created = entity.Created
            };
        }
    }
}

