using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeakDanish.Forms.Effects
{
    public class LongPressEffect : RoutingEffect
    {
        public event EventHandler Pressed;
        public event EventHandler Released;

        public LongPressEffect() 
        {
        }

        internal void OnPressed()
        {
            Pressed?.Invoke(this, EventArgs.Empty);
        }

        internal void OnReleased()
        {
            Released?.Invoke(this, EventArgs.Empty);
        }
    }
}
