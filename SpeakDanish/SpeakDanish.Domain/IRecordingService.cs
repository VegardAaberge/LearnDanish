﻿using System;
using SpeakDanish.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeakDanish.Domain.Models;

namespace SpeakDanish.Domain
{
    public interface IRecordingService
    {
        /// <summary>
        /// Retrieves a list of all recordings from the database.
        /// </summary>
        /// <returns>A list of recordings.</returns>
        Task<List<Recording>> GetRecordingsAsync();

        /// <summary>
        /// Retrieves a specific recording from the database.
        /// </summary>
        /// <param name="id">The ID of the recording to retrieve.</param>
        /// <returns>The recording with the specified ID, or null if no such recording exists.</returns>
        Task<Recording> GetRecordingAsync(int id);

        /// <summary>
        /// Inserts a new recording into the database.
        /// </summary>
        /// <param name="recording">The recording to insert.</param>
        /// <returns>The ID of the inserted recording.</returns>
        Task<int> InsertRecordingAsync(Recording recording);

        /// <summary>
        /// Deletes a recording from the database.
        /// </summary>
        /// <param name="recording">The recording to delete.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        Task<int> DeleteRecordingAsync(Recording recording);

        /// <summary>
        /// Retrieves a random sentence from a file.
        /// </summary>
        /// <param name="filepath">The path to the file containing the sentences.</param>
        /// <returns>A random sentence from the file.</returns>
        Task<string> GetRandomSentence(Task<string> getSentencesFromResources);
    }

}

