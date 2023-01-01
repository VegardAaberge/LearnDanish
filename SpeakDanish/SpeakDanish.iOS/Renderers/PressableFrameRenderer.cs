using System;
using System.Linq;
using Foundation;
using SpeakDanish.Controls.GestureRecognizers;
using SpeakDanish.Platforms.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Frame), typeof(PressableFrameRenderer))]
namespace SpeakDanish.Platforms.iOS.Renderers
{
    public class PressableFrameRenderer : FrameRenderer
    {
        public PressableFrameRenderer()
        {
            UserInteractionEnabled = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                GetControl().Layer.ShadowOpacity = 0.3f;
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            if (this.Element?.GestureRecognizers == null)
                return;

            var pressedGestureRecognizer = this.Element.GestureRecognizers.OfType<PressedGestureRecognizer>().FirstOrDefault();
            if (pressedGestureRecognizer != null)
                pressedGestureRecognizer.SendPressed(this.Element);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (this.Element?.GestureRecognizers == null)
                return;

            var pressedGestureRecognizer = this.Element.GestureRecognizers.OfType<ReleasedGestureRecognizer>().FirstOrDefault();
            if (pressedGestureRecognizer != null)
                pressedGestureRecognizer.SendReleased(this.Element);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            if (this.Element?.GestureRecognizers == null)
                return;

            var pressedGestureRecognizer = this.Element.GestureRecognizers.OfType<ReleasedGestureRecognizer>().FirstOrDefault();
            if (pressedGestureRecognizer != null)
                pressedGestureRecognizer.SendReleased(this.Element);
        }
    }
}

