using SpeakDanish.Forms.Views;

namespace SpeakDanish
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(RecordingsPage), typeof(RecordingsPage));
        }
    }
}
