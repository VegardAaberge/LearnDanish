using System;
using SQLite;

namespace SpeakDanish.Data.Database
{
	public class BaseEntity
	{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}

