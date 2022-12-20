using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnDanish.ViewModel.Base;

namespace LearnDanish.ViewModel;

public partial class HomeViewModel : BaseViewModel
{
	public HomeViewModel()
	{
		Title = "Home";
		Sentence = "En hund løber gennem gaderne i en lille by.";
		IsRecording = true;
    }

	[ObservableProperty]
	string sentence;

	[ObservableProperty]
	bool isRecording;

	[RelayCommand]
	public async Task StartRecordingAsync()
	{
		await Task.Yield();
	}

	[RelayCommand]
    public async Task StopRecordingAsync()
    {
        await Task.Yield();
    }
}

