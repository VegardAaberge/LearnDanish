using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using LearnDanish.ViewModel.Base;
using System.Windows.Input;

namespace LearnDanish.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "Home";
            Sentence = "En hund løber gennem gaderne i en lille by.";
            IsRecording = true;

            StartRecordingCommand = new Command(async () => await StartRecordingAsync());
            StopRecordingCommand = new Command(async () => await StopRecordingAsync());
        }

        public Command StartRecordingCommand { get; set; }
        public Command StopRecordingCommand { get; set; }

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

        public async Task StartRecordingAsync()
        {
            await Task.Yield();
        }

        public async Task StopRecordingAsync()
        {
            await Task.Yield();
        }
    }
}
