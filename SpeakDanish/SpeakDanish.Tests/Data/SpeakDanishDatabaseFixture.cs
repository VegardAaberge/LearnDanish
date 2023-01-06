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
            DatabaseMock
                .Setup(x => x.GetItemsAsync<T>())
                .ReturnsAsync(() => GetItems<T>());

            DatabaseMock
                .Setup(x => x.GetItemAsync<T>(It.IsAny<int>()))
                .ReturnsAsync((int id) => GetItem<T>(id));

            DatabaseMock
                .Setup(x => x.InsertItemAsync<T>(It.IsAny<T>()))
                .ReturnsAsync((T item) => AddItem<T>(item));

            DatabaseMock
                .Setup(x => x.InsertAllItemsAsync<T>(It.IsAny<List<T>>()))
                .ReturnsAsync((List<T> items) => AddAllItem<T>(items));

            DatabaseMock
                .Setup(x => x.DeleteItemAsync<T>(It.IsAny<T>()))
                .ReturnsAsync((T item) => RemoveItem<T>(item));
        }

        public Mock<SpeakDanishDatabase> DatabaseMock { get; }

        public List<T> GetItems<T>() where T : BaseEntity
        {
            if (!_items.ContainsKey(typeof(T)))
            {
                _items[typeof(T)] = new List<T>();
            }

            return (List<T>)_items[typeof(T)];
        }

        public T GetItem<T>(int id) where T : BaseEntity
        {
            if (!_items.ContainsKey(typeof(T)))
            {
                return default(T);
            }

            return ((List<T>)_items[typeof(T)]).FirstOrDefault(i => i.Id == id);
        }

        public int AddItem<T>(T item) where T : BaseEntity
        {
            if (!_items.ContainsKey(typeof(T)))
            {
                _items[typeof(T)] = new List<T>();
            }

            ((List<T>)_items[typeof(T)]).Add(item);
            return 1;
        }

        public int AddAllItem<T>(List<T> items) where T : BaseEntity
        {
            if (!_items.ContainsKey(typeof(T)))
            {
                _items[typeof(T)] = new List<T>();
            }

            _items[typeof(T)] = items;
            return items.Count;
        }

        public int RemoveItem<T>(T item) where T : BaseEntity
        {
            if (!_items.ContainsKey(typeof(T)))
            {
                return 0;
            }

            ((List<T>)_items[typeof(T)]).Remove(item);
            return 1;
        }

        public void Dispose()
        {
        }
    }
}

