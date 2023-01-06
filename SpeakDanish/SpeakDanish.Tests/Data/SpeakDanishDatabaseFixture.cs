using System;
using SpeakDanish.Data;
using System.Collections.Generic;
using Moq;
using SpeakDanish.Data.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SpeakDanish.Tests.Data
{
	public class SpeakDanishDatabaseFixture : IDisposable
	{
        private readonly Dictionary<Type, object> _items = new Dictionary<Type, object>();

        public SpeakDanishDatabaseFixture()
		{
            DatabaseMock = new Mock<SpeakDanishDatabase>();

            SetupMock<SentenceEntity>();
            SetupMock<RecordingEntity>();
        }

        private void SetupMock<T>() where T : BaseEntity, new()
        {
            _items[typeof(T)] = new List<T>();

            DatabaseMock
                .Setup(x => x.GetItemsAsync<T>())
                .ReturnsAsync(() => GetItems<T>());

            DatabaseMock
                .Setup(x => x.GetItemAsync<T>(It.IsAny<int>()))
                .ReturnsAsync((int id) => GetItem<T>(id));

            DatabaseMock
                .Setup(x => x.InsertItemAsync<T>(It.IsAny<T>()))
                .ReturnsAsync(1);

            DatabaseMock
                .Setup(x => x.InsertAllItemsAsync<T>(It.IsAny<List<T>>()))
                .ReturnsAsync((List<T> items) => items.Count);

            DatabaseMock
                .Setup(x => x.DeleteItemAsync<T>(It.IsAny<T>()))
                .ReturnsAsync(1);
        }

        public Mock<SpeakDanishDatabase> DatabaseMock { get; }

        public void SetupItems<T>(List<T> items) where T : BaseEntity
        {
            _items[typeof(T)] = items;
        }

        public List<T> GetItems<T>() where T : BaseEntity
        {
            return (List<T>)_items[typeof(T)];
        }

        public T GetItem<T>(int id) where T : BaseEntity
        {
            return ((List<T>)_items[typeof(T)]).FirstOrDefault(i => i.Id == id);
        }

        public void Dispose()
        {
        }
    }
}

