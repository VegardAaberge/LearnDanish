using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeakDanish.Data.Models;
using SQLite;

namespace SpeakDanish.Data
{
	public class SpeakDanishDatabase
	{
		public SpeakDanishDatabase()
		{
            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        static SQLiteAsyncConnection _database;

        public static readonly AsyncLazy<SpeakDanishDatabase> Instance = new AsyncLazy<SpeakDanishDatabase>(async () => 
        {
            var instance = new SpeakDanishDatabase();
            CreateTableResult result = await _database.CreateTableAsync<RecordingEntity>();
            return instance;
        });

        public Task<List<T>> GetItemsAsync<T>() where T : BaseEntity, new()
        {
            return _database.Table<T>().ToListAsync();
        }

        public Task<T> GetItemAsync<T>(int id) where T : BaseEntity, new()
        {
            return _database.Table<T>().Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> InsertItemAsync<T>(T item) where T : BaseEntity, new()
        {
            if (item.Id != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync<T>(T item) where T : BaseEntity, new()
        {
            return _database.DeleteAsync(item);
        }
    }
}

