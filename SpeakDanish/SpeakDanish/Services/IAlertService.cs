using System;
using System.Threading.Tasks;
using SpeakDanish.Services.Enums;

namespace SpeakDanish.Services
{
	public interface IAlertService
	{
		Task ShowToast(String message, ToastDuration duration = ToastDuration.Short);
	}
}

