using Android.Content;
using Android.Speech.Tts;
using SpeakDanish.Contracts.Platform;

namespace SpeakDanish.Droid.Services
{
    public class TtsDataInstaller : ITtsDataInstaller
    {
        public void InstallTtsData()
        {
            Intent intent = new Intent(Android.Speech.Tts.TextToSpeech.Engine.ActionInstallTtsData);

            // Start the intent
            MainActivity.Instance.StartActivity(intent);
        }
    }
}

