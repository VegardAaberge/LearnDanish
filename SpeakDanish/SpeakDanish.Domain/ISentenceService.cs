using System;
using System.Threading.Tasks;

namespace SpeakDanish.Domain
{
	public interface ISentenceService
	{
        /// <summary>
        /// Returns a random sentence from the list of sentences, excluding the previous sentence.
        /// </summary>
        /// <param name="previousSentence">The previous sentence to exclude from the list of choices.</param>
        /// <param name="getSentencesFromResources">A task that returns the list of sentences to choose from.</param>
        /// <returns>A task that returns a random sentence from the list of sentences, excluding the previous sentence.</returns>
        Task<string> GetRandomSentence(string previousSentence, Task<string> getSentencesFromResources);
    }
}

