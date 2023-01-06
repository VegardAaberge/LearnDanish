using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SpeakDanish.Data;
using SpeakDanish.Data.Database;
using SpeakDanish.Data.Mappers;
using SpeakDanish.Data.Models;
using SpeakDanish.Domain;
using SpeakDanish.Domain.Models;
using SpeakDanish.Domain.Services;
using SpeakDanish.Tests.Data;
using SpeakDanish.Tests.Data.Database;
using Xunit;

namespace SpeakDanish.Tests.Domain.Services
{
    public class RecordingServiceTests : IClassFixture<SpeakDanishDatabaseFixture>
    {
        private Mock<SpeakDanishDatabase> _mockDatabase;
        private RecordingService _recordingService;

        private readonly List<RecordingEntity> _recordings = new List<RecordingEntity>{
            new RecordingEntity { Id=1, Sentence = "Sentence 1", Created=new DateTime(2022,11,1), FilePath="Path1" },
            new RecordingEntity { Id=2, Sentence = "Sentence 2", Created=new DateTime(2023,11,1), FilePath="Path2" },
            new RecordingEntity { Id=3, Sentence = "Sentence 3", Created=new DateTime(2024,11,1), FilePath="Path3" }
        };

        public RecordingServiceTests(SpeakDanishDatabaseFixture fixture)
        {
            _mockDatabase = fixture.DatabaseMock;
            _recordingService = new RecordingService(fixture.DatabaseMock.Object);

            fixture.SetupItems(_recordings);
        }

        [Fact]
        public async Task TestGetRecordingsAsync_ShouldWork()
        {
            // Arrange
            var expected = _recordings;

            // Act
            var actual = await _recordingService.GetRecordingsAsync();

            // Assert
            _mockDatabase.Verify(x => x.GetItemsAsync<RecordingEntity>(), Times.Once);
            actual.Should().NotBeNullOrEmpty();
            actual.Should().HaveCount(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task TestGetRecordingAsync_ValidIDShouldWork()
        {
            // Arrange
            int recordingId = 1;
            RecordingEntity expected = _recordings.First(x => x.Id == 1);

            // Act
            var actual = await _recordingService.GetRecordingAsync(recordingId);

            // Assert
            _mockDatabase.Verify(x => x.GetItemAsync<RecordingEntity>(recordingId), Times.Once);
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task TestGetRecordingAsync_InvalidIDShouldNotWork()
        {
            // Arrange
            int recordingId = 999;

            // Act
            var actual = await _recordingService.GetRecordingAsync(recordingId);

            // Assert
            _mockDatabase.Verify(x => x.GetItemAsync<RecordingEntity>(recordingId), Times.Once);
            actual.Should().BeNull();
        }

        [Fact]
        public async Task TestInsertRecordingAsync_ShouldWork()
        {
            // Arrange
            Recording recording = new Recording();

            // Act
            var actual = await _recordingService.InsertRecordingAsync(recording);

            // Assert
            _mockDatabase.Verify(x => x.InsertItemAsync<RecordingEntity>(It.IsAny<RecordingEntity>()), Times.Once);
            actual.Should().Be(1);
            recording.Should().BeEquivalentTo(recording.ToRecordingEntity().ToRecording());
        }

        [Fact]
        public async Task TestDeleteRecordingAsync_ShouldWork()
        {
            // Arrange
            Recording recording = new Recording();

            // Act
            var actual = await _recordingService.DeleteRecordingAsync(recording);

            // Assert
            _mockDatabase.Verify(x => x.DeleteItemAsync<RecordingEntity>(It.IsAny<RecordingEntity>()), Times.Once);
            actual.Should().Be(1);
        }
    }
}

