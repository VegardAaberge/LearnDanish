using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeakDanish.ViewModels;
using Xamarin.Forms;

namespace SpeakDanish.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            (Parent as NavigationPage).BarBackgroundColor = Color.Blue;
            (Parent as NavigationPage).BarTextColor = Color.White;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}

