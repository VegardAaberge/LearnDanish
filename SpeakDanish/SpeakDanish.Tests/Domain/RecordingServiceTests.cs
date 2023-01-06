using System;
using System.Threading.Tasks;
using SpeakDanish.Domain;
using SpeakDanish.Tests.Data;
using Xunit;

namespace SpeakDanish.Tests.Domain
{
	public class RecordingServiceTests : IClassFixture<SpeakDanishDatabaseFixture>
	{
        private RecordingService _recordingService;

        public RecordingServiceTests(SpeakDanishDatabaseFixture fixture)
		{
			_recordingService = new RecordingService(fixture.DatabaseMock.Object);
		}

        public async Task TestGetRecordingsAsync()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        public async Task TestGetRecordingAsync_ValidID()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        public async Task TestGetRecordingAsync_InvalidID()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        public async Task TestInsertRecordingAsync()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

        public async Task TestDeleteRecordingAsync()
        {
            // Arrange
            // TODO

            // Act
            // TODO

            // Assert
            // TODO
        }

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

