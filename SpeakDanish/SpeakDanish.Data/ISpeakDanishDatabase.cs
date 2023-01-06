using System;
using SpeakDanish.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeakDanish.Data
{
    /// <summary>
    /// Interface for a database that stores information about Danish language recordings.
    /// </summary>
    public interface ISpeakDanishDatabase
    {
        /// <summary>
        /// Asynchronously retrieves a list of items of type T from the database.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve. Must be a subclass of BaseEntity.</typeparam>
        /// <returns>A list of items of type T.</returns>
        Task<List<T>> GetItemsAsync<T>() where T : BaseEntity, new();

        /// <summary>
        /// Asynchronously retrieves a single item of type T from the database with the given ID.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve. Must be a subclass of BaseEntity.</typeparam>
        /// <param name="id">The ID of the item to retrieve.</param>
        /// <returns>The item of type T with the given ID, or null if no such item exists.</returns>
        Task<T> GetItemAsync<T>(int id) where T : BaseEntity, new();

        /// <summary>
        /// Asynchronously inserts the given item into the database. If the item already has an ID, updates the existing item in the database.
        /// </summary>
        /// <typeparam name="T">The type of entity to insert. Must be a subclass of BaseEntity.</typeparam>
        /// <param name="item">The item to insert or update.</param>
        /// <returns>The ID of the inserted or updated item.</returns>
        Task<int> InsertItemAsync<T>(T item) where T : BaseEntity, new();

        /// <summary>
        /// Asynchronously inserts all the given items into the database, replacing any existing items of the same type.
        /// </summary>
        /// <typeparam name="T">The type of entity to insert. Must be a subclass of BaseEntity.</typeparam>
        /// <param name="items">The items to insert.</param>
        /// <returns>The number of items inserted.</returns>
        Task<int> InsertAllItemsAsync<T>(List<T> items) where T : BaseEntity, new();

        /// <summary>
        /// Asynchronously deletes the given item from the database.
        /// </summary>
        /// <typeparam name="T">The type of entity to delete. Must be a subclass of BaseEntity.</typeparam>
        /// <param name="item">The item to delete.</param>
        /// <returns>The number of items deleted (0 or 1).</returns>
        Task<int> DeleteItemAsync<T>(T item) where T : BaseEntity, new();
    }
}

