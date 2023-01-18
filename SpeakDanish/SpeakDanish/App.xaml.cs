using Prism;
using Prism.Ioc;
using Prism.Unity;
using SpeakDanish.Contracts.Data;
using SpeakDanish.Contracts.Domain;
using SpeakDanish.Data.Api;
using SpeakDanish.Data.Database;
using SpeakDanish.Domain.Models;
using SpeakDanish.Domain.Services;
using SpeakDanish.Domain.UseCases;
using SpeakDanish.ViewModels;
using SpeakDanish.Views;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

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
            container.RegisterSingleton<ISpeechRecognizer, SpeechRecognizerWrapper>();
            container.RegisterSingleton<ISpeechService<TranscriptionResult>, SpeechService>();
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

