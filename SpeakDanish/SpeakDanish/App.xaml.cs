using System;
using SpeakDanish.Views;
using Prism.Unity;
using Xamarin.Forms;
using Prism.Ioc;
using Prism;
using SpeakDanish.Data.Database;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.ViewModels;
using SpeakDanish.Domain.UseCases;
using SpeakDanish.Domain.Services;
using SpeakDanish.Domain.Models;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using System.Threading.Tasks;
using Prism.Navigation;

[assembly: ExportFont("materialdesignicons-webfont.ttf", Alias = "MaterialDesignIcons")]
namespace SpeakDanish
{
    public partial class App : PrismApplication
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync($"NavigationPage/{nameof(RootPage)}/{nameof(HomePage)}");
        }

        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterForNavigation<NavigationPage>();
            container.RegisterForNavigation<RootPage>();

            container.RegisterSingleton<ISpeakDanishDatabase, SpeakDanishDatabase>();
            container.RegisterSingleton<ISentenceService, SentenceService>();
            container.RegisterSingleton<IRecordingService<Recording>, RecordingService>();

            container.Register<IAudioUseCase, AudioUseCase>();
            container.Register<ITextToSpeech, TextToSpeechImplementation>();
            container.Register<IPermissions, PermissionsImplementation>();

            container.RegisterForNavigation<HomePage, HomeViewModel>();
            container.RegisterForNavigation<RecordingsPage, RecordingsViewModel>();
        }
    }
}

