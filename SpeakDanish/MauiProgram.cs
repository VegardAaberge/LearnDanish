using Microsoft.Extensions.Logging;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Data.Api;
using SpeakDanish.Data.Database;
using SpeakDanish.Domain.Models;
using SpeakDanish.Domain.Services;
using SpeakDanish.Domain.UseCases;
using SpeakDanish.ViewModels;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using SpeakDanish.Forms.Services;
using SpeakDanish.Controls.Entries;
using SpeakDanish.Forms.Effects;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Media;




#if ANDROID
using SpeakDanish.Platforms.Android.Handlers;
using SpeakDanish.Droid.Services;
using SpeakDanish.Platforms.Android.Effects;
#endif

namespace SpeakDanish
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .RegisterTypes()
                .RegisterPlatformTypes()
                .RegisterPlatformHandlers()
                .RegisterEffects()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialDesignIcons");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterTypes(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<INavigationService, NavigationService>();
            mauiAppBuilder.Services.AddSingleton<ISpeakDanishDatabase, SpeakDanishDatabase>();
            mauiAppBuilder.Services.AddSingleton<ISpeechRecognizer, SpeechRecognizerWrapper>();
            mauiAppBuilder.Services.AddSingleton<ISpeechService<SpeechToTextResult>, ToolkitSpeechService>();
            mauiAppBuilder.Services.AddSingleton<ISentenceService, SentenceService>();
            mauiAppBuilder.Services.AddSingleton<IRecordingService<Recording>, RecordingService>();

            mauiAppBuilder.Services.AddTransient<IAudioUseCase, AudioUseCase>();
            mauiAppBuilder.Services.AddTransient<Microsoft.Maui.Media.ITextToSpeech, TextToSpeechImplementation>();
            mauiAppBuilder.Services.AddTransient<IPermissions, PermissionsImplementation>();
            mauiAppBuilder.Services.AddSingleton<ISpeechToText>(SpeechToText.Default);

            mauiAppBuilder.Services.AddTransient<HomeViewModel>();
            mauiAppBuilder.Services.AddTransient<RecordingsViewModel>();

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterPlatformTypes(this MauiAppBuilder mauiAppBuilder)
        {
#if ANDROID
            mauiAppBuilder.Services.AddSingleton<IAudioRecorder, AudioRecorder>();
            mauiAppBuilder.Services.AddSingleton<ITtsDataInstaller, TtsDataInstaller>();
            mauiAppBuilder.Services.AddSingleton<IAlertService, AlertService>();
            mauiAppBuilder.Services.AddSingleton<IFileService, FileService>();
#endif

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterPlatformHandlers(this MauiAppBuilder mauiAppBuilder)
        {
#if ANDROID
            mauiAppBuilder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(BorderlessEntry), typeof(BorderlessEntryHandler));
            });
#endif

            return mauiAppBuilder;
        }

        public static MauiAppBuilder RegisterEffects(this MauiAppBuilder mauiAppBuilder)
        {
#if ANDROID
            mauiAppBuilder.ConfigureEffects(effectts =>
            {
                effectts.Add(typeof(LongPressEffect), typeof(LongPressEffectImplementation));
            });
#endif

            return mauiAppBuilder;
        }
    }
}
