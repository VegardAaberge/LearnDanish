using SpeakDanish.Forms.Services;
using SpeakDanish.Forms.Views;
using System;

namespace SpeakDanish.Forms.Views
{
	public class RootPage : ContentPage
	{
        public RootPage ()
		{
        }

        protected override async void OnAppearing()
        {
            await Shell.Current.GoToAsync(nameof(HomePage));

            base.OnAppearing();
        }
    }
}


