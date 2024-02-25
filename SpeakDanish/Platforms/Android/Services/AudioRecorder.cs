using Android.Media;
using Android.OS;
using Java.Lang;
using SpeakDanish.Contracts.Platform;

namespace SpeakDanish.Droid.Services
{
    public class AudioRecorder : IAudioRecorder
	{
        private MediaRecorder _mediaRecorder;
        private MediaPlayer _mediaPlayer;

        private bool _isRecording;

        public async Task<string?> StartRecordingAudio(string filename)
        {
            if (_isRecording)
                return null;

            try
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
                File.Delete(filePath);

                _mediaRecorder.SetOutputFile(filePath);
                await Task.Run(() => _mediaRecorder.Prepare());
                _mediaRecorder.Start();
                _isRecording = true;

                return filePath;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public async Task StopRecordingAudio(string filepath, int attempt = 0)
        {
            if (!_isRecording)
                return;

            try
            {
                await Task.Delay(100);

                _mediaRecorder.Stop();
                _mediaRecorder.Reset();
                _mediaRecorder.Release();

                await Task.Delay(100);
                _isRecording = false;
            }
            catch (System.Exception)
            {
                if (attempt == 3)
                    throw;

                attempt++;
                await StopRecordingAudio(filepath, attempt);
            }
        }

        public async Task PlayAudio(string filepath)
        {
            try
            {
                _mediaPlayer = new MediaPlayer();
                _mediaPlayer.SetDataSource(filepath);
                _mediaPlayer.Prepare();

                _mediaPlayer.Start();

                var mediaPlayerTask = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                _mediaPlayer.Completion += (sender, e) =>
                {
                    _mediaPlayer.Release();
                    _mediaPlayer = null;
                    mediaPlayerTask.SetResult(0);
                };

                await mediaPlayerTask.Task;
            }
            catch (System.Exception ex)
            {
            }
        }
    }
}

