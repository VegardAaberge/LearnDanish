using System;
using System.ComponentModel;
using System.Windows.Input;

namespace LearnDanish.Controls
{
    public sealed class PressedGestureRecognizer : GestureRecognizer
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(PressedGestureRecognizer), null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(PressedGestureRecognizer), null);

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

        public event EventHandler Pressed;

        public void SendPressed(Microsoft.Maui.Controls.View sender)
        {
            ICommand cmd = Command;
            if (cmd != null && cmd.CanExecute(CommandParameter))
                cmd.Execute(CommandParameter);

            EventHandler handler = Pressed;
            if (handler != null)
                handler(sender, new TappedEventArgs(CommandParameter));
        }
    }
}

