using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnDanish.ViewModel.Base;

namespace LearnDanish.ViewModel;

public partial class HomeViewModel : BaseViewModel
{
	public HomeViewModel()
	{
		Title = "Home";
		Sentence = "En hund løber gennem gaderne i en lille by.";
    }

	[ObservableProperty]
	string sentence;
}

