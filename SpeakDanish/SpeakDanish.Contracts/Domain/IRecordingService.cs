using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeakDanish.Contracts.Domain
{
    public interface IRecordingService<T> where T : class
    {
        /// <summary>
        /// Retrieves a list of all recordings from the database.
        /// </summary>
        /// <returns>A list of recordings.</returns>
        Task<List<T>> GetRecordingsAsync();

        /// <summary>
        /// Retrieves a specific recording from the database.
        /// </summary>
        /// <param name="id">The ID of the recording to retrieve.</param>
        /// <returns>The recording with the specified ID, or null if no such recording exists.</returns>
        Task<T?> GetRecordingAsync(int id);

        /// <summary>
        /// Inserts a new recording into the database.
        /// </summary>
        /// <param name="recording">The recording to insert.</param>
        /// <returns>The ID of the inserted recording.</returns>
        Task<int> InsertRecordingAsync(T recording);

        /// <summary>
        /// Deletes a recording from the database.
        /// </summary>
        /// <param name="recording">The recording to delete.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        Task<int> DeleteRecordingAsync(T recording);
    }
}

