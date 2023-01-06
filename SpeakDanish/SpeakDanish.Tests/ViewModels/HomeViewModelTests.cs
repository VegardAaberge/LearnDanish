using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SpeakDanish.Domain.Services;
using SpeakDanish.Services;
using SpeakDanish.ViewModels;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xunit;

namespace SpeakDanish.Tests.ViewModel
{
	public class RecordingsViewModelTests
	{
        private HomeViewModel _homeViewModel;

        public RecordingsViewModelTests()
		{
            Type localeType = typeof(Locale);
            ConstructorInfo constructor = localeType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string)
            }, null);

            object[] constructorArgs = { "da", "", "Danish", "" };
            Locale danishLocale = (Locale)constructor.Invoke(constructorArgs);

            var textToSpeech = new Mock<ITextToSpeech>();
            textToSpeech
                .Setup(x => x.GetLocalesAsync())
                .ReturnsAsync(new [] { danishLocale });

            textToSpeech.Setup(x => x.SpeakAsync(It.IsAny<string>(), It.IsAny<SpeechOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            ITtsDataInstaller ttsDataInstaller = new Mock<ITtsDataInstaller>().Object;
            IAudioRecorder audioRecorder = new Mock<IAudioRecorder>().Object;
            ISentenceService sentenceService = new Mock<ISentenceService>().Object;
            IRecordingService recordingService = new Mock<IRecordingService>().Object;
            IAlertService alertService = new Mock<IAlertService>().Object;
            INavigation navigation = new Mock<INavigation>().Object;

            _homeViewModel = new HomeViewModel(sentenceService, recordingService, ttsDataInstaller, audioRecorder, alertService, navigation);
        }

        [Fact]
        public async Task SpeakSentenceAsync_ShouldSpeakSentenceInDanish()
        {
            // Act
        }

    }
}

