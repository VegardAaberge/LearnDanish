using System;
using SQLite;

namespace LearnDanish.Data.Models
{
	public class BaseEntity
	{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}

