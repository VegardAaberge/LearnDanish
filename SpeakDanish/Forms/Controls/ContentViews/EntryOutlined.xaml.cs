using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace SpeakDanish.Controls.ContentViews
{
    public partial class EntryOutlined : ContentView
    {
        public EntryOutlined()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryOutlined), default(string));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EntryOutlined), default(string));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(EntryOutlined), Colors.Blue);

        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(EntryOutlined), Colors.Blue);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        async void TextBox_Focused(object sender, FocusEventArgs e)
        {
            await TranslateLabelToTitle();
        }

        async void TextBox_Unfocused(object sender, FocusEventArgs e)
        {
            await TranslateLabelToPlaceHolder();
        }

        async Task TranslateLabelToTitle()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                var placeHolder = this.PlaceHolderLabel;
                var distance = GetPlaceholderDistance(placeHolder);
                await placeHolder.TranslateTo(0, -distance);
            }

        }

        async Task TranslateLabelToPlaceHolder()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                await this.PlaceHolderLabel.TranslateTo(0, 0);
            }
        }

        double GetPlaceholderDistance(Label control)
        {
            // In Android we need to move the label slightly up so it's centered in the border frame.
            var distance = 0d;
            // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            if (Device.RuntimePlatform == Device.iOS) distance = 0;
            else distance = 5;

            distance = control.Height + distance;
            return distance;
        }
    }
}

