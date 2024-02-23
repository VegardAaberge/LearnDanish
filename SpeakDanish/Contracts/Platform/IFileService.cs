using System;
using System.Threading.Tasks;

namespace SpeakDanish.Contracts.Platform
{
	public interface IFileService
    {
        Task<string> LoadFileAsync(string fileName, string extension);
    }
}

