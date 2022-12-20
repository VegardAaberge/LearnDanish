using System;
using Foundation;
using LearnDanish.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using UIKit;

namespace LearnDanish.Platforms.iOS.Renderers
{
    public class PressableFrameRenderer : FrameRenderer
    {
        public PressableFrameRenderer()
        {
            UserInteractionEnabled = true;
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

