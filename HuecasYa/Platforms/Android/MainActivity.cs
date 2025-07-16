using Android.App;
using Android.Content.PM;
using Android.OS;

namespace HuecasYa
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Verificar si la propiedad Window no es nula antes de usarla
            if (Window != null)
            {
                // Cambiar color de la barra de estado de manera compatible con versiones
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop && Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
                {
                    Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#9C092B"));
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                {
                    // Implementar lógica alternativa para versiones posteriores si es necesario
                    Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#9C092B"));
                }
            }
        }
    }
}
