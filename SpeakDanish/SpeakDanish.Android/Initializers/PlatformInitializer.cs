using System;
using Prism;
using Prism.Ioc;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Droid.Services;

namespace SpeakDanish.Droid.Initializers
{
    public class PlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterSingleton<IAudioRecorder, AudioRecorder>();
            container.RegisterSingleton<ITtsDataInstaller, TtsDataInstaller>();
            container.RegisterSingleton<IAlertService, AlertService>();
            container.RegisterSingleton<IFileService, FileService>();
        }
    }
}

