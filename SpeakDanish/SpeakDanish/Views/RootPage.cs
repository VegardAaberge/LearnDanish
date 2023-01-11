using System;
using Prism.Navigation;
using Xamarin.Forms;

namespace SpeakDanish.Views
{
	public class RootPage : ContentPage
	{
        private INavigationService _navigation;

        public RootPage (INavigationService navigation)
		{
			_navigation = navigation;
		}

        protected override async void OnAppearing()
        {
            await _navigation.NavigateAsync(nameof(HomePage));
            base.OnAppearing();
        }
    }
}


