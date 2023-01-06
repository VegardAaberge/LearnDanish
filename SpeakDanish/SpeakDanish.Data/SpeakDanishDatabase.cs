using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeakDanish.Data.Models;
using SQLite;

namespace SpeakDanish.Data
{
	public class SpeakDanishDatabase : ISpeakDanishDatabase
	{
        static SQLiteAsyncConnection _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        public SpeakDanishDatabase()
        {
            Task.Run(async () =>
            {
                CreateTableResult result1 = await _database.CreateTableAsync<RecordingEntity>();
                CreateTableResult result2 = await _database.CreateTableAsync<SentenceEntity>();
            }).GetAwaiter().GetResult();
        }

        public virtual Task<List<T>> GetItemsAsync<T>() where T : BaseEntity, new()
        {
            return _database.Table<T>().ToListAsync();
        }

        public virtual Task<T> GetItemAsync<T>(int id) where T : BaseEntity, new()
        {
            return _database.Table<T>().Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public virtual Task<int> InsertItemAsync<T>(T item) where T : BaseEntity, new()
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

        public virtual async Task<int> InsertAllItemsAsync<T>(List<T> items) where T : BaseEntity, new()
        {
            await _database.DeleteAllAsync<T>();

            return await _database.InsertAllAsync(items);
        }

        public virtual Task<int> DeleteItemAsync<T>(T item) where T : BaseEntity, new()
        {
            return _database.DeleteAsync(item);
        }
    }
}

