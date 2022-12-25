using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnDanish.ViewModel;
using Xamarin.Forms;

namespace LearnDanish
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel();
        }
    }
}

