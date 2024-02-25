using SpeakDanish.ViewModels;

namespace SpeakDanish.Forms.Views;

public partial class HomePage : ContentPage
{
    public HomeViewModel? ViewModel { get; private set; }

    public HomePage()
	{
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        if(BindingContext == null)
        {
            ViewModel = Handler?.MauiContext?.Services.GetServices<HomeViewModel>().FirstOrDefault();
            BindingContext = ViewModel;
        }
       
        base.OnHandlerChanged();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void LongPressEffect_Pressed(object sender, EventArgs e)
    {
        if(ViewModel != null)
            await ViewModel.StartRecordingAsync();
    }

    private async void LongPressEffect_Released(object sender, EventArgs e)
    {
        if (ViewModel != null)
            await ViewModel.StopRecordingAsync(false);
    }

    private async void LongPressEffect_Cancelled(object sender, EventArgs e)
    {
        if (ViewModel != null)
            await ViewModel.StopRecordingAsync(true);
    }
}