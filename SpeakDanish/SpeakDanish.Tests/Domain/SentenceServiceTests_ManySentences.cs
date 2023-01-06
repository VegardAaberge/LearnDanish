using System;
using SpeakDanish.Domain;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using SpeakDanish.Data;
using SpeakDanish.Domain.Models;
using System.Collections.Generic;
using SpeakDanish.Tests.Data;
using System.Linq;
using SpeakDanish.Data.Models;
using MathNet.Numerics.Distributions;

namespace SpeakDanish.Tests
{
    public class SentenceServiceTests_ManySentences : IClassFixture<SpeakDanishDatabaseFixture>
    {
        private readonly ISentenceService _sentenceService;
        private readonly string[] sentences = Enumerable.Range(1, 200).Select(x => $"Sentence {x}").ToArray();

        public SentenceServiceTests_ManySentences(SpeakDanishDatabaseFixture fixture)
        {
            _sentenceService = new SentenceService(fixture.DatabaseMock.Object);

            List<SentenceEntity> sentenceEntities = sentences
                .Select(sentence => new SentenceEntity
                {
                    Sentence = sentence
                }).ToList();

            fixture.AddAllItem(sentenceEntities);
        }



        [Fact]
        public async Task TestGetRandomSentence_ShouldNotFetchResource()
        {
            // Arrange
            string previousSentence = sentences[0];
            string[] resourceSentences = new string[] { "This is a sentence." };
            Task<string> getSentencesFromResources = Task.FromResult(string.Join("\n", resourceSentences));

            // Act
            string result = await _sentenceService.GetRandomSentence(previousSentence, getSentencesFromResources);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().NotBe(resourceSentences.First());
            result.Should().StartWith("Sentence");
        }

        [Fact]
        public async Task TestGetRandomSentence_ShouldBeRandom()
        {
            // Arrange
            string[] sentences = Enumerable.Range(1, 200).Select(x => $"Sentence {x}").ToArray();
            
            Task<string> getSentencesFromResources = Task.FromResult(string.Join("\n", sentences));

            int numTrials = 20;
            Dictionary<string, int> observedFrequencies = sentences.ToDictionary(key => key, value => 0);

            // Act
            for (int i = 0; i < numTrials; i++)
            {
                var selectedSentence = await _sentenceService.GetRandomSentence("", getSentencesFromResources);
                observedFrequencies[selectedSentence] = observedFrequencies[selectedSentence] + 1;
            }

            // Assert
            double chiSquaredValue = 0.0;
            double expectedFrequency = 1.0 / sentences.Count();
            foreach (var sentence in sentences)
            {
                double observedFrequency = (double)observedFrequencies[sentence] / sentences.Count();
                double diff = observedFrequency - expectedFrequency;
                chiSquaredValue += diff * diff / expectedFrequency;
            }

            int degreesOfFreedom = sentences.Count() - 1;
            ChiSquared chiSquared = new ChiSquared(degreesOfFreedom);
            double pValue = 1 - chiSquared.CumulativeDistribution(chiSquaredValue);

            pValue.Should().BeGreaterThan(0.05);
        }
    }
}

