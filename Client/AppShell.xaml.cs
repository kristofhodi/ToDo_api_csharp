namespace Client
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EditToDoPage), typeof(EditToDoPage));
        }
    }
}
