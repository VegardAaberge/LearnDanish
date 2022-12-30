using System;
using System.Collections.ObjectModel;
using SpeakDanish.ViewModel.Base;
using Xamarin.Forms;

namespace SpeakDanish.ViewModel
{
	public class RecordingsViewModel : BaseViewModel
	{
        private INavigation _navigation;

        public RecordingsViewModel(INavigation navigation)
		{
			_navigation = navigation;

			Title = "Recordings";

			Items = new ObservableCollection<RecordingItem>
			{
				new RecordingItem{ RecordingName="Item 1"},
				new RecordingItem{ RecordingName="Item 2"}
			};
		}

        public ObservableCollection<RecordingItem> Items { get; set; }
    }
}

