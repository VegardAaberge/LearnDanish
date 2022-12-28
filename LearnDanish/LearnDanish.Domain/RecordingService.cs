using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnDanish.Data;
using LearnDanish.Data.Mappers;
using LearnDanish.Domain;
using LearnDanish.Domain.Models;

namespace LearnDanish.Domain
{
    public class RecordingService : IRecordingService
    {
        private  LearnDanishDatabase _database;

        public RecordingService()
        {
            _database = LearnDanishDatabase.Instance.GetAwaiter().GetResult();
        }

        public async Task<List<Recording>> GetRecordingsAsync()
        {
            var recordings = await _database.GetItemsAsync<RecordingEntity>();

            return recordings
                .Select(r => r.ToRecording())
                .ToList();
        }

        public async Task<Recording> GetRecordingAsync(int id)
        {
            var recording = await _database.GetItemAsync<RecordingEntity>(id);

            return recording.ToRecording();
        }

        public Task<int> InsertRecordingAsync(Recording recording)
        {
            return _database.InsertItemAsync(recording.ToRecordingEntity());
        }

        public Task<int> DeleteRecordingAsync(Recording recording)
        {
            return _database.DeleteItemAsync(recording.ToRecordingEntity());
        }
    }
}

