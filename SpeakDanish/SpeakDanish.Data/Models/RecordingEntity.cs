using System;
using SpeakDanish.Data.Models;
using SQLite;

namespace SpeakDanish.Data.Models
{
    public class RecordingEntity : BaseEntity
    {
        public string Sentence { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
    }
}

