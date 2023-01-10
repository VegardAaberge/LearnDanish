using System;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using SpeakDanish.Contracts.Platform;

namespace SpeakDanish.iOS.Services
{
	public class AudioRecorder : IAudioRecorder
    {
		public AudioRecorder()
		{
		}

        private AVAudioRecorder _audioRecorder;
        private AVAudioPlayer _audioPlayer;

        public Task<string> StartRecordingAudio(string filename)
        {
            var audioSession = AVAudioSession.SharedInstance();
            var error = audioSession.SetCategory(AVAudioSessionCategory.Record);
            if (error != null)
            {
                // Handle error
            }
            error = audioSession.SetActive(true);
            if (error != null)
            {
                // Handle error
            }

            var audioFileUrl = NSUrl.FromString(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename + ".m4a"));

            var audioSettings = new AudioSettings
            {
                SampleRate = 44100,
                Format = AudioToolbox.AudioFormatType.MPEG4AAC,
                NumberChannels = 1,
                AudioQuality = AVAudioQuality.High
            };
            _audioRecorder = AVAudioRecorder.Create(audioFileUrl, audioSettings, out error);
            if (error != null)
            {
                // Handle error
            }

            _audioRecorder.Record();

            return Task.FromResult(audioFileUrl.AbsoluteString);
        }

        public Task StopRecordingAudio(string filepath)
        {
            // Stop the recorder
            _audioRecorder.Stop();
            _audioRecorder.Dispose();
            _audioRecorder = null;

            // Replay the audio
            PlayAudio(filepath);

            return Task.CompletedTask;
        }

        public Task PlayAudio(string filepath)
        {
            _audioPlayer = AVAudioPlayer.FromUrl(NSUrl.FromString(filepath));
            _audioPlayer.FinishedPlaying += (sender, e) =>
            {
                _audioPlayer.Stop();
                _audioPlayer = null;
            };
            _audioPlayer.Play();

            return Task.CompletedTask;
        }
    }
}

