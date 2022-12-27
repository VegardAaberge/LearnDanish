using System;
using Android.Content;
using Android.Speech.Tts;
using LearnDanish.Droid.Services;
using LearnDanish.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(TtsDataInstaller))]
namespace LearnDanish.Droid.Services
{
	public class TtsDataInstaller : ITtsDataInstaller
    {
        void ITtsDataInstaller.InstallTtsData()
        {
            Intent intent = new Intent(TextToSpeech.Engine.ActionInstallTtsData);

            // Start the intent
            MainActivity.Instance.StartActivity(intent);
        }
    }
}

