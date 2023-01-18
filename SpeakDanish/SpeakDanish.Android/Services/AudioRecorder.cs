using System;
using System.IO;
using System.Threading.Tasks;
using AOE = Android.OS.Environment;
using Android.Media;
using SpeakDanish.Droid.Services;
using Xamarin.Forms;
using Android.OS;
using SpeakDanish.Contracts.Platform;

namespace SpeakDanish.Droid.Services
{
	public class AudioRecorder : IAudioRecorder
	{
        private MediaRecorder _mediaRecorder;
        private MediaPlayer _mediaPlayer;

        public async Task<string> StartRecordingAudio(string filename)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                _mediaRecorder = new MediaRecorder(MainActivity.Instance);
            }
            else
            {
                _mediaRecorder = new MediaRecorder();
            }
                
            _mediaRecorder.SetAudioSource(AudioSource.Mic);
            _mediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
            _mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
            _mediaRecorder.SetAudioChannels(1);
            _mediaRecorder.SetAudioSamplingRate(16000);

            string directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            string filePath = Path.Combine(directory, filename + ".mp4");
            _mediaRecorder.SetOutputFile(filePath);
            await Task.Run(() => _mediaRecorder.Prepare());
            _mediaRecorder.Start();

            return filePath;
        }

        public Task StopRecordingAudio(string filepath)
        {
            _mediaRecorder.Stop();
            _mediaRecorder.Reset();
            _mediaRecorder.Release();

            return Task.FromResult(0);
        }

        public Task PlayAudio(string filepath)
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetDataSource(filepath);
            _mediaPlayer.Prepare();

            _mediaPlayer.Start();

            _mediaPlayer.Completion += (sender, e) =>
            {
                _mediaPlayer.Release();
                _mediaPlayer = null;
            };

            return Task.CompletedTask;
        }
    }
}

