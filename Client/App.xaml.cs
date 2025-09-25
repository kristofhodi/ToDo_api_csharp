namespace Client
{
    public partial class App : Application
    {
        public App()
        {
            Routing.RegisterRoute("details", typeof(DetailsPage));
            Routing.RegisterRoute("AddToDoPage", typeof(AddToDoPage));

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}