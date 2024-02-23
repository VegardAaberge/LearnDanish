using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Data.Database;
using SpeakDanish.Data.Models;

namespace SpeakDanish.Domain.Services
{
	public class SentenceService : ISentenceService
	{
        private ISpeakDanishDatabase _database;

        public SentenceService(ISpeakDanishDatabase database)
		{
            _database = database;
        }


        public async Task<string> GetRandomSentence<T>(string previousSentence)
        {
            var sentences = await _database.GetItemsAsync<SentenceEntity>();
            if (sentences.Count < 100)
            {
                var sentencesData = await LoadSentences<T>();
                var lines = sentencesData.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x));
                sentences = lines.Select(line => new SentenceEntity
                {
                    Sentence = line
                }).ToList();

                await _database.InsertAllItemsAsync(sentences);
            }

            if (sentences.Count <= 1)
                return sentences.FirstOrDefault()?.Sentence ?? string.Empty;

            Random random = new Random();
            while (true)
            {
                lock (random)
                {
                    int index = random.Next(0, sentences.Count);
                    string newSentence = sentences[index].Sentence;

                    if (previousSentence != newSentence)
                        return sentences[index].Sentence;
                }
            }
        }

        async Task<string> LoadSentences<T>()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("sentences.txt");

            if (stream == null) return null;

            using (StreamReader reader = new StreamReader(stream))
            {
                // Read and return the content asynchronously
                return await reader.ReadToEndAsync();
            }
        }
    }
}

