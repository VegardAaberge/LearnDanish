using System;
using Microsoft.Extensions.DependencyInjection;
using SpeakDanish.ViewModels;
using SpeakDanish.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("materialdesignicons-webfont.ttf", Alias = "MaterialDesignIcons")]
namespace SpeakDanish
{
    public partial class App : Application
    {
        public App (Action<IServiceCollection> addPlatformServices = null)
        {
            InitializeComponent();

            AppContainer.SetupServices(addPlatformServices);

            MainPage = new NavigationPage()
            {
                BarBackgroundColor = Color.Blue,
                BarTextColor = Color.White,
            };
            (MainPage as NavigationPage).PushAsync(new HomePage(), false);
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

