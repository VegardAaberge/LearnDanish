using System;
using SpeakDanish.Data;

namespace SpeakDanish.Domain.Models
{
    public class Recording
    {
        public Recording()
        {
            VolumeIcon = "\U000f057e";
        }

        public int Id { get; set; }

        public string Sentence { get; set; }

        public string FilePath { get; set; }

        public DateTime Created { get; set; }

        public string VolumeIcon { get; set; }
    }
}

