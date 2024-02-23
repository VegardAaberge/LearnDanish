using AndroidX.Lifecycle;
using SpeakDanish.ViewModels;

namespace SpeakDanish.Forms.Views;

public partial class RecordingsPage : ContentPage
{
	public RecordingsPage()
	{
		InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        if (BindingContext == null)
        {
            BindingContext = Handler?.MauiContext?.Services.GetServices<RecordingsViewModel>().FirstOrDefault();
        }

        base.OnHandlerChanged();
    }

    private void MyCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        MyCollectionView.SelectedItem = null;
    }
}