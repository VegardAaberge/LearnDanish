using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SpeakDanish.Controls
{
	public sealed class ReleasedGestureRecognizer : GestureRecognizer
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(ReleasedGestureRecognizer), null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(ReleasedGestureRecognizer), null);

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, (object)value); }
        }

        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        public event EventHandler Released;

        public void SendReleased(View sender)
        {
            ICommand cmd = Command;
            if (cmd != null && cmd.CanExecute(CommandParameter))
                cmd.Execute(CommandParameter);

            EventHandler handler = Released;
            if (handler != null)
                handler(sender, new TappedEventArgs(CommandParameter));
        }
    }
}

