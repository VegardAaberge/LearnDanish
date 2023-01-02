using System;
using SpeakDanish.Domain;
using System.Threading.Tasks;
using Xunit;

namespace SpeakDanish.Tests
{
    public class RecordingServiceTests
    {
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
            // TODO
        }

        [Fact]
        public async Task TestGetRandomSentence_ValidPreviousSentence()
        {
            // Arrange
            string previousSentence = "This is the previous sentence.";
            Task<string> getSentencesFromResources = Task.FromResult("This is a sentence. This is another sentence.");

            IRecordingService recordingService = new RecordingService();

            // Act
            Task<string> result = recordingService.GetRandomSentence(previousSentence, getSentencesFromResources);
            string resultString = await result;

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(previousSentence, resultString);
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

