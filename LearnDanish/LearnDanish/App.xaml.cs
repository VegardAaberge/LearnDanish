using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("materialdesignicons-webfont.ttf", Alias = "MaterialDesignIcons")]
namespace LearnDanish
{
    public partial class App : Application
    {
        public App (Action<IServiceCollection> addPlatformServices = null)
        {
            InitializeComponent();

            AppContainer.SetupServices(addPlatformServices);

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.Blue,
                BarTextColor = Color.White,
            };
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

