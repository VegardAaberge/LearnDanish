using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Domain.Models;
using SpeakDanish.Domain.Services;
using SpeakDanish.Domain.UseCases;
using SpeakDanish.ViewModels;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace SpeakDanish.Tests
{
    public static class TestUtils
    {
        public static Dictionary<Type, Action<Mock>> CreateSetupDictionary()
        {
            return new Dictionary<Type, Action<Mock>>();
        }

        public static ServiceProvider CreateTestProvider(Dictionary<Type, Action<Mock>> dependenciesActions, Action<ServiceCollection> addTransient)
		{
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IAudioRecorder>(p => CreateMock<IAudioRecorder>(dependenciesActions));
            serviceCollection.AddTransient<ITtsDataInstaller>(p => CreateMock<ITtsDataInstaller>(dependenciesActions));
            serviceCollection.AddTransient<ITextToSpeech>(p => CreateMock<ITextToSpeech>(dependenciesActions));
            serviceCollection.AddTransient<IPermissions>(p => CreateMock<IPermissions>(dependenciesActions));

            serviceCollection.AddTransient<ISentenceService>(p => CreateMock<ISentenceService>(dependenciesActions));
            serviceCollection.AddTransient<IRecordingService<Recording>>(p => CreateMock<IRecordingService<Recording>>(dependenciesActions));
            serviceCollection.AddTransient<IAlertService>(p => CreateMock<IAlertService>(dependenciesActions));
            serviceCollection.AddTransient<INavigation>(p => CreateMock<INavigation>(dependenciesActions));
            serviceCollection.AddTransient<IAudioUseCase>(p => CreateMock<IAudioUseCase>(dependenciesActions));

            addTransient(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private static T CreateMock<T> (Dictionary<Type, Action<Mock>> dependenciesActions) where T : class
        {
            var mock = new Mock<T>();
            if (dependenciesActions.ContainsKey(typeof(T)))
            {
                var action = dependenciesActions[typeof(T)];
                action(mock);
            }
            return mock.Object;
        }
    }
}

