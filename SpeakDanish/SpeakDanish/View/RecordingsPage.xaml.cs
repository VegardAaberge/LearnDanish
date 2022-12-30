using System;
using System.Collections.Generic;
using SpeakDanish.ViewModel;

using Xamarin.Forms;

namespace SpeakDanish.View
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

