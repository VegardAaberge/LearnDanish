using System;
using SpeakDanish.Controls.Entries;
using SpeakDanish.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace SpeakDanish.iOS.Renderers
{
	public class BorderlessEntryRenderer : EntryRenderer
	{
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            //Configure Native control (UITextField)
            if (Control != null)
            {
                Control.Layer.BorderWidth = 0;
                Control.BorderStyle = UIKit.UITextBorderStyle.None;
            }
        }
    }
}
