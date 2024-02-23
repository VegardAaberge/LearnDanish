using System;
using System.Threading.Tasks;
using SpeakDanish.Contracts.Platform.Enums;

namespace SpeakDanish.Contracts.Platform
{
	public interface IAlertService
	{
		Task ShowToast(String message, ToastDuration duration = ToastDuration.Short);
	}
}

