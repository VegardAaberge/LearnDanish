using System;
using LearnDanish.Data.Models;
using SQLite;

namespace LearnDanish.Data
{
    public class RecordingEntity : BaseEntity
    {
        public string Sentence { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
    }
}

