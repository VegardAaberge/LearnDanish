using System;
using System.Collections.Generic;
using SpeakDanish.ViewModels;

using Xamarin.Forms;

namespace SpeakDanish.Views
{
    public partial class RecordingsPage : ContentPage
    {
        public RecordingsPage()
        {
            InitializeComponent();

            BindingContext = AppContainer.GetService<RecordingsViewModel>();
        }
    }
}

