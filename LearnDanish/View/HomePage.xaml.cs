using LearnDanish.ViewModel;

namespace LearnDanish.View;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();

		BindingContext = new HomeViewModel();
	}
}


