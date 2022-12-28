using System;
using Android.Content;
using Android.Speech.Tts;
using LearnDanish.Droid.Services;
using LearnDanish.Services;
using Xamarin.Forms;

namespace LearnDanish.Droid.Services
{
	public class TtsDataInstaller : ITtsDataInstaller
    {
        public void InstallTtsData()
        {
            Intent intent = new Intent(TextToSpeech.Engine.ActionInstallTtsData);

            // Start the intent
            MainActivity.Instance.StartActivity(intent);
        }
    }
}

