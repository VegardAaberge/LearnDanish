using CommunityToolkit.Maui;
using LearnDanish.Controls;
using Microsoft.Extensions.Logging;
using MH = Microsoft.Maui.Handlers;

namespace LearnDanish;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialDesignIcons");
            })
			.ConfigureMauiHandlers(handlers =>
			{
				ConfigureHandlers(handlers);
			})
			.UseMauiCommunityToolkit();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    private static void ConfigureHandlers(IMauiHandlersCollection handlers)
    {
#if ANDROID
		handlers.AddHandler(typeof(Microsoft.Maui.Controls.Frame), typeof(LearnDanish.Platforms.Android.Renderers.PressableFrameRenderer));
#endif

#if IOS
		handlers.AddHandler(typeof(Microsoft.Maui.Controls.Frame), typeof(LearnDanish.Platforms.iOS.Renderers.PressableFrameRenderer));
#endif
    }

    private static void MauiProgram_Touch(object sender, Android.Views.View.TouchEventArgs e)
    {
        throw new NotImplementedException();
    }
}

