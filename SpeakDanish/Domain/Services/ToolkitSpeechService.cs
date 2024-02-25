using System.Globalization;
using CommunityToolkit.Maui.Media;
using SpeakDanish.Contracts.Data;

namespace SpeakDanish.Data.Api
{
    public class ToolkitSpeechService(ISpeechToText speechToText) : ISpeechService<SpeechToTextResult>
    {
        private bool _isTranscribing;
        private SpeechToTextResult _recognitionResult;
        private CancellationToken cancellationToken;
        private ISpeechToText _speechToText = speechToText;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public async Task StartTranscribingDanish(Action<SpeechToTextResult> recognizedCallback)
        {
            try
            {
                if (_isTranscribing)
                    return;

                _isTranscribing = true;
                _recognitionResult = await _speechToText.ListenAsync(
                                                    CultureInfo.GetCultureInfo("da-DK"),
                                                    new Progress<string>(partialText =>
                                                    {
                                                    }), cancellationToken);
            }
            catch (Exception)
            {
            }
            finally 
            {
                _isTranscribing = false;
                recognizedCallback(_recognitionResult);
            }
        }

        public async Task StopTranscribingDanish(bool isCancelled)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (!_isTranscribing)
                    return;

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                if (!isCancelled)
                {
                    try
                    {
                        while (_isTranscribing)
                        {
                            cts.Token.ThrowIfCancellationRequested();
                            await Task.Delay(100, cts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }

                if (isCancelled || cts.IsCancellationRequested)
                {
                    await Task.Delay(100);
                    await _speechToText.StopListenAsync();
                    await _speechToText.DisposeAsync();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

