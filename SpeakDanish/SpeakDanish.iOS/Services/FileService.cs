using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using SpeakDanish.Contracts.Platform;

namespace SpeakDanish.iOS.Services
{
	public class FileService : IFileService
	{
		public FileService()
		{
		}

        public async Task<string> LoadFileAsync(string fileName, string extension)
        {
            using (var streamReader = new StreamReader(NSBundle.MainBundle.PathForResource(fileName, extension)))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}

