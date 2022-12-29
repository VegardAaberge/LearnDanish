using System;
using SQLite;

namespace SpeakDanish.Data.Models
{
	public class BaseEntity
	{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}

