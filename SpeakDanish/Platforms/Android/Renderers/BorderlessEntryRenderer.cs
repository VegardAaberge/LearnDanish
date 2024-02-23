using Android.Content;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using SpeakDanish.Controls.Entries;

namespace SpeakDanish.Platforms.Android.Handlers
{
    public partial class BorderlessEntryHandler
    {
        public static IPropertyMapper<BorderlessEntry, BorderlessEntryHandler> PropertyMapper = new PropertyMapper<BorderlessEntry, BorderlessEntryHandler>(ViewHandler.ViewMapper)
        {
        };

        public BorderlessEntryHandler() : base(PropertyMapper)
        {
        }
    }

    public partial class BorderlessEntryHandler : ViewHandler<BorderlessEntry, AppCompatEditText>
    {
        public BorderlessEntryHandler(IPropertyMapper mapper, CommandMapper? commandMapper = null) : base(mapper, commandMapper)
        {
        }

        protected override AppCompatEditText CreatePlatformView() => new AppCompatEditText(Context);

        protected override void ConnectHandler(AppCompatEditText nativeView)
        {
            base.ConnectHandler(nativeView);

            nativeView.Background = null;
            nativeView.SetHintTextColor(Colors.Black.ToDefaultColorStateList());
        }
    }
}