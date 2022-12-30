using System;
using SpeakDanish.Domain;
using SpeakDanish.Services;
using SpeakDanish.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

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
			services.AddSingleton<IRecordingService, RecordingService>();

			services.AddSingleton<HomeViewModel>();
			services.AddTransient<RecordingsViewModel>();

            services.AddSingleton<INavigation>(s => Application.Current.MainPage.Navigation);
        }

		public static T GetService<T>()
		{
			return _serviceProvider.GetService<T>();
		}
    }
}

