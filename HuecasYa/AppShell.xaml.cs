namespace HuecasYa
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("PasswordRecoveryPage", typeof(PasswordRecoveryPage));
            Routing.RegisterRoute("StaticMapPage", typeof(StaticMapPage));
            Routing.RegisterRoute(nameof(RestaurantDetailPage), typeof(RestaurantDetailPage));
            Routing.RegisterRoute(nameof(BusquedaPage), typeof(BusquedaPage));
            Routing.RegisterRoute(nameof(DetalleRestaurantePage), typeof(DetalleRestaurantePage));

        }
    }
}
