using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using SpeakDanish.Data;
using SpeakDanish.Data.Mappers;
using SpeakDanish.Data.Models;
using SpeakDanish.Domain;
using SpeakDanish.Domain.Models;

namespace SpeakDanish.Domain
{
    public class RecordingService : IRecordingService
    {
        private SpeakDanishDatabase _database;

        public RecordingService()
        {
            _database = SpeakDanishDatabase.Instance.GetAwaiter().GetResult();
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

        public async Task<string> GetRandomSentence(Task<string> getSentencesFromResources)
        {
            var sentences = await _database.GetItemsAsync<SentenceEntity>();
            if(sentences.Count < 100)
            {
                var lines = (await getSentencesFromResources).Split('\n').Where(x => !string.IsNullOrWhiteSpace(x));
                sentences = lines.Select(line => new SentenceEntity
                {
                    Sentence = line
                }).ToList();

                await _database.InsertAllItemsAsync(sentences);
            }

            if (sentences.Count == 0)
                return string.Empty;

            int index = new Random().Next(0, sentences.Count - 1);

            return sentences[index].Sentence;
        }
    }
}

