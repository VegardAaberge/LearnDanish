using System;
using System.IO;
using System.Threading.Tasks;
using AOE = Android.OS.Environment;
using Android.Media;
using SpeakDanish.Droid.Services;
using SpeakDanish.Services;
using Xamarin.Forms;
using Android.OS;

namespace SpeakDanish.Droid.Services
{
	public class AudioRecorder : IAudioRecorder
	{
        private MediaRecorder _mediaRecorder;
        private MediaPlayer _mediaPlayer;

        public async Task<string> StartRecordingAudio(string filename)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                _mediaRecorder = new MediaRecorder(MainActivity.Instance);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                _mediaRecorder = new MediaRecorder();
#pragma warning restore CS0618 // Type or member is obsolete
            }
                
            _mediaRecorder.SetAudioSource(AudioSource.Mic);
            _mediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
            _mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);

            _mediaRecorder.SetOutputFile(Path.Combine(AOE.ExternalStorageDirectory.AbsolutePath, filename + ".mp4"));
            await Task.Run(() => _mediaRecorder.Prepare());
            _mediaRecorder.Start();

            return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, filename + ".mp4");
        }

        public Task StopRecordingAudio(string filepath)
        {
            // Stop the recorder
            _mediaRecorder.Stop();
            _mediaRecorder.Reset();
            _mediaRecorder.Release();

            // Replay the audio
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetDataSource(filepath);
            _mediaPlayer.Prepare();

            _mediaPlayer.Start();

            _mediaPlayer.Completion += (sender, e) =>
            {
                _mediaPlayer.Release();
                _mediaPlayer = null;
            };

            return Task.FromResult(0);
        }
    }
}

