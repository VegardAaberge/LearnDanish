using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Contracts.Platform;
using SpeakDanish.Domain.UseCases;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace SpeakDanish.Tests
{
    public static class TestProviderHelper
    {
        public static ServiceProvider CreateTestProvider(Dictionary<Type, Action<Mock>> dependenciesActions)
		{
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IAudioRecorder>(p => CreateMock<IAudioRecorder>(dependenciesActions));
            serviceCollection.AddTransient<ITtsDataInstaller>(p => CreateMock<ITtsDataInstaller>(dependenciesActions));
            serviceCollection.AddTransient<ITextToSpeech>(p => CreateMock<ITextToSpeech>(dependenciesActions));
            serviceCollection.AddTransient<IPermissions>(p => CreateMock<IPermissions>(dependenciesActions));
            serviceCollection.AddTransient<IAudioUseCase, AudioUseCase>();

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

