using System;
using System.Threading.Tasks;

namespace SpeakDanish.Services
{
	public interface IFileService
    {
        Task<string> LoadFileAsync(string fileName, string extension);
    }
}

