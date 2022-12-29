using System;
using System.Globalization;
using System.Threading.Tasks;
using Android.Widget;
using Google.Android.Material.Snackbar;
using SpeakDanish.Services;
using SpeakDanish.Services.Enums;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace SpeakDanish.Droid.Services
{
	public class AlertService : IAlertService
	{
		public AlertService()
		{
		}

        public Task ShowToast(string message, ToastDuration duration)
        {
            ToastLength toastDuration = ToastDuration.Long == duration ? ToastLength.Long : ToastLength.Short;

            Toast.MakeText(MainActivity.Instance.ApplicationContext, message, toastDuration).Show();

            return Task.CompletedTask;
        }
    }
}

