using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LearnDanish.ViewModel.Base;

public partial class BaseViewModel : ObservableObject
{
	public BaseViewModel()
	{
	}

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsNotBusy))]
	bool isBusy;

	[ObservableProperty]
	string title;

	public bool IsNotBusy => !isBusy;
}
