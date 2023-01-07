using System;
using SpeakDanish.Domain;
using SpeakDanish.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using SpeakDanish.Data;
using SpeakDanish.Data.Database;
using SpeakDanish.Domain.Services;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Domain.Models;
using SpeakDanish.Domain.UseCases;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials;
using Xamarin.Essentials.Implementation;

namespace SpeakDanish
{
    public class AppContainer
    {
        private static ServiceProvider _serviceProvider;

        public static void SetupServices(Action<IServiceCollection> addPlatformServices = null)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            addPlatformServices?.Invoke(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISpeakDanishDatabase, SpeakDanishDatabase>();
            services.AddSingleton<ISentenceService, SentenceService>();
            services.AddSingleton<IRecordingService<Recording>, RecordingService>();

            services.AddTransient<IAudioUseCase, AudioUseCase>();
            services.AddTransient<ITextToSpeech, TextToSpeechImplementation>();
            services.AddTransient<IPermissions, PermissionsImplementation>();

            services.AddTransient<HomeViewModel>();
            services.AddTransient<RecordingsViewModel>();

            services.AddSingleton<INavigation>(s => Application.Current.MainPage.Navigation);
        }

        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}

