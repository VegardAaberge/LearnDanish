using Android.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using SpeakDanish.Forms.Effects;
using SpeakDanish.Platforms.Android.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

[assembly: ExportEffect(typeof(LongPressEffectImplementation), nameof(LongPressEffect))]
namespace SpeakDanish.Platforms.Android.Effects
{
    public class LongPressEffectImplementation : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Container != null)
            {
                Container.Touch += OnTouch; ;
            }
        }

    
        protected override void OnDetached()
        {
            if (Container != null)
            {
                Container.Touch -= OnTouch;
            }
        }

        private void OnTouch(object? sender, global::Android.Views.View.TouchEventArgs e)
        {
            if(e.Event is MotionEvent motionEvent)
            {
                var effect = (LongPressEffect)Element.Effects.FirstOrDefault(e => e is LongPressEffect);

                switch (e.Event.Action)
                {
                    case MotionEventActions.Down:
                        effect?.OnPressed();
                        break;
                    case MotionEventActions.Up:
                        effect?.OnReleased();
                        break;
                }
            }
        }
    }
}
