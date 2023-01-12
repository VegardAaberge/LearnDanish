using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using Prism.Navigation;

namespace SpeakDanish.ViewModels.Base
{
    public partial class BaseViewModel : BindableBase, INavigationAware, IDestructible
    {
        public BaseViewModel()
        {
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private int _busyCount;
        public bool IsBusy
        {
            get => _busyCount > 0;
            set
            {
                if (value)
                    _busyCount++;
                else if (_busyCount > 0)
                    _busyCount--;

                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void Destroy()
        {
        }
    }
}
