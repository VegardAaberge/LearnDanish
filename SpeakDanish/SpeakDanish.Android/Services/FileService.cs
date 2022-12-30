using System;
using System.IO;
using System.Threading.Tasks;
using SpeakDanish.Services;

namespace SpeakDanish.Droid.Services
{
	public class FileService : IFileService
	{
		public FileService()
		{
		}

        public async Task<string> LoadFileAsync(string filename, string extension)
        {
            var filenameWithExtension = filename + "." + extension;

            using (var streamReader = new StreamReader(MainActivity.Instance.Assets.Open(filenameWithExtension)))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}

