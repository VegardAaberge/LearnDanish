using System;
using System.Threading.Tasks;
using SpeakDanish.Services;
using SpeakDanish.Services.Enums;
using UIKit;

namespace SpeakDanish.iOS.Services
{
	public class AlertService : IAlertService
	{
		public AlertService()
		{
		}

        public async Task ShowToast(string message, ToastDuration duration)
        {
            double durationSeconds = ToastDuration.Long == duration ? 3.0 : 2.0;
            var alertController = UIAlertController.Create(null, message, UIAlertControllerStyle.ActionSheet);
            alertController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;

            UIViewController viewController = AppDelegate.GetVisibleViewController();
            viewController.PresentViewController(alertController, true, null);

            await Task.Delay((int)(durationSeconds * 1000));

            alertController.DismissViewController(true, null);
        }
    }
}

