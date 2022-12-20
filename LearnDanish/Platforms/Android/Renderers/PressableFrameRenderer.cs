using System;
using Android.Content;
using Android.Views;
using LearnDanish.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;

namespace LearnDanish.Platforms.Android.Renderers
{
    public class PressableFrameRenderer : FrameRenderer
    {
        public PressableFrameRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                if (e.NewElement == null || !e.NewElement.GestureRecognizers.Any())
                    return;

                if (e.NewElement.GestureRecognizers.Any(x => x.GetType() == typeof(PressedGestureRecognizer)
                                                            || x.GetType() == typeof(ReleasedGestureRecognizer)))
                {
                    Touch += Control_Touch;
                }
            }
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

