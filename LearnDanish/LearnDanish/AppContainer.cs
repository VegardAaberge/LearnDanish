using System;
using LearnDanish.Domain;
using LearnDanish.Services;
using LearnDanish.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace LearnDanish
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
			services.AddTransient<IRecordingService, RecordingService>();

			services.AddSingleton<HomeViewModel>();
        }

		public static T GetService<T>()
		{
			return _serviceProvider.GetService<T>();
		}
    }
}

