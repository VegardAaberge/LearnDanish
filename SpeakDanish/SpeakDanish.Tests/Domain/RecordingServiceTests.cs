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

namespace SpeakDanish.Tests
{
    public class RecordingServiceTests : IClassFixture<SpeakDanishDatabaseFixture>
    {
        private SpeakDanishDatabaseFixture _fixture;
        private IRecordingService _recordingService;

        public RecordingServiceTests(SpeakDanishDatabaseFixture fixture)
        {
            _fixture = fixture;
            _recordingService = new RecordingService(fixture.DatabaseMock.Object);
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
            string[] sentences = new string[] { "This is the previous sentence.", "This is another sentence." };
            List<SentenceEntity> sentenceEntities = sentences.Select(sentence => new SentenceEntity
            {
                Sentence = sentence
            }).ToList();
            _fixture.AddAllItem(sentenceEntities);
            string previousSentence = sentences[0];
            string exptected = sentences[1];
            Task<string> getSentencesFromResources = Task.FromResult(string.Join("\n", sentences));

            // Act
            string result = await _recordingService.GetRandomSentence(previousSentence, getSentencesFromResources);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().NotBe(previousSentence);
            result.Should().ContainAny(sentences);
            result.Should().Be(exptected);
        }

        [Fact]
        public async Task TestGetRandomSentence_InvalidPreviousSentence()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
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

