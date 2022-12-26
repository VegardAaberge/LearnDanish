using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using LearnDanish.ViewModel.Base;
using System.Windows.Input;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace LearnDanish.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "Home";
            Sentence = "En hund løber gennem gaderne i en lille by.";
            IsRecording = false;

            StartRecordingCommand = new Command(async () => await StartRecordingAsync(), () => !_isRecording);
            StopRecordingCommand = new Command(async () => await StopRecordingAsync(), () => _isRecording);
        }

        public Command StartRecordingCommand { get; set; }
        public Command StopRecordingCommand { get; set; }

        private int _countSeconds;
        public int CountSeconds
        {
            get { return _countSeconds; }
            set
            {
                _countSeconds = value;
                OnPropertyChanged(nameof(CountSeconds));
            }
        }

        private string _sentence;
        public string Sentence
        {
            get { return _sentence; }
            set
            {
                _sentence = value;
                OnPropertyChanged(nameof(Sentence));
            }
        }

        private bool _isRecording;
        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                _isRecording = value;
                OnPropertyChanged(nameof(IsRecording));
            }
        }

        private CancellationTokenSource timerCancellationToken = null;

        public async Task StartRecordingAsync()
        {
            if (timerCancellationToken != null)
                return;

            await Task.Yield();
            timerCancellationToken = new CancellationTokenSource();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                CountSeconds = CountSeconds + 1;
                if(_countSeconds > 20)
                {
                    StopRecordingCommand.Execute(null);
                }

                var isCancelled = timerCancellationToken.IsCancellationRequested;
                if (isCancelled)
                {
                    timerCancellationToken = null;
                    return false;
                }
                return true;
            });

            IsRecording = true;
        }

        public async Task StopRecordingAsync()
        {
            await Task.Yield();

            timerCancellationToken.Cancel();
            CountSeconds = 0;
            IsRecording = false;
        }
    }
}
