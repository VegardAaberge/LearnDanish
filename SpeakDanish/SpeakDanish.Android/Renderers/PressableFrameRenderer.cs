using System;
using Android.Content;
using Android.Views;
using System.Linq;
using SpeakDanish.Controls.GestureRecognizers;
using SpeakDanish.Platforms.Android.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(Frame), typeof(PressableFrameRenderer))]
namespace SpeakDanish.Platforms.Android.Renderers
{
    public class PressableFrameRenderer : FrameRenderer
    {
        public PressableFrameRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            Touch += Control_Touch;
        }

        private void Control_Touch(object sender, TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    var pressedGestureRecognizer = this.Element.GestureRecognizers.OfType<PressedGestureRecognizer>().FirstOrDefault();
                    if (pressedGestureRecognizer != null)
                        pressedGestureRecognizer.SendPressed(this.Element);
                    break;

                case MotionEventActions.Up:
                    var releasedGestureRecognizer = this.Element.GestureRecognizers.OfType<ReleasedGestureRecognizer>().FirstOrDefault();
                    if (releasedGestureRecognizer != null)
                        releasedGestureRecognizer.SendReleased(this.Element);
                    break;

                default:
                    break;
            }
        }
    }
}

