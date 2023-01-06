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
    public class RecordingServiceTests : IClassFixture<SpeakDanishDatabaseFixture>
    {
        private readonly IRecordingService _recordingServiceManySentences;
        private readonly IRecordingService _recordingServiceTwoSentences;

        private readonly string[] twoSentences = new string[] { "This is the previous sentence.", "This is another sentence." };
        private readonly string[] manySentences = Enumerable.Range(1, 200).Select(x => $"Sentence {x}").ToArray();

        public RecordingServiceTests(
            SpeakDanishDatabaseFixture fixtureManySentences,
            SpeakDanishDatabaseFixture fixtureTwoSentences)
        {
            SetupFixture(ref _recordingServiceManySentences, fixtureManySentences, manySentences);
            SetupFixture(ref _recordingServiceTwoSentences, fixtureTwoSentences, twoSentences);
        }

        private void SetupFixture(ref IRecordingService recordingService, SpeakDanishDatabaseFixture fixture, string[] sentences)
        {
            recordingService = new RecordingService(fixture.DatabaseMock.Object);

            List<SentenceEntity> sentenceEntities = sentences
                .Select(sentence => new SentenceEntity
                {
                    Sentence = sentence
                }).ToList();

            fixture.AddAllItem(sentenceEntities);
        }

        [Fact]
        public async Task TestGetRecordingsAsync()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        [Fact]
        public async Task TestGetRecordingAsync_ValidID()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        [Fact]
        public async Task TestGetRecordingAsync_InvalidID()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        [Fact]
        public async Task TestInsertRecordingAsync()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        [Fact]
        public async Task TestDeleteRecordingAsync()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        [Fact]
        public async Task TestGetRandomSentence_ValidPreviousSentence()
        {
            // Arrange
            string previousSentence = twoSentences[0];
            string exptected = twoSentences[1];
            Task<string> getSentencesFromResources = Task.FromResult(string.Join("\n", twoSentences));

            // Act
            string result = await _recordingServiceTwoSentences.GetRandomSentence(previousSentence, getSentencesFromResources);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().NotBe(previousSentence);
            result.Should().ContainAny(twoSentences);
            result.Should().Be(exptected);
        }

        [Fact]
        public async Task TestGetRandomSentence_ShouldFetchResource()
        {
            // Arrange
            string previousSentence = twoSentences[0];
            string[] resourceSentences = new string[] { "This is a sentence." };
            Task<string> getSentencesFromResources = Task.FromResult(string.Join("\n", resourceSentences));

            // Act
            string result = await _recordingServiceTwoSentences.GetRandomSentence("", getSentencesFromResources);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Be(resourceSentences.First());
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
                var selectedSentence = await _recordingServiceManySentences.GetRandomSentence("", getSentencesFromResources);
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

        [Fact]
        public async Task TestGetRandomSentence_DifferentFromPreviousSentence()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }
    }
}

