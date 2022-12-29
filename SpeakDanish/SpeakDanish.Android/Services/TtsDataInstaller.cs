using System;
using Android.Content;
using Android.Speech.Tts;
using SpeakDanish.Droid.Services;
using SpeakDanish.Services;
using Xamarin.Forms;

namespace SpeakDanish.Droid.Services
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

