using System;
using System.Linq;
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


        public async Task<string> GetRandomSentence(string previousSentence, Task<string> getSentencesFromResources)
        {
            var sentences = await _database.GetItemsAsync<SentenceEntity>();
            if (sentences.Count < 100)
            {
                var lines = (await getSentencesFromResources).Split('\n').Where(x => !string.IsNullOrWhiteSpace(x));
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
    }
}

