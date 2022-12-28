using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnDanish.Data.Models;
using SQLite;

namespace LearnDanish.Data
{
	public class LearnDanishDatabase
	{
		public LearnDanishDatabase()
		{
            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        static SQLiteAsyncConnection _database;

        public static readonly AsyncLazy<LearnDanishDatabase> Instance = new AsyncLazy<LearnDanishDatabase>(async () => 
        {
            var instance = new LearnDanishDatabase();
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

