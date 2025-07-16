using Android.App;
using Android.Runtime;

namespace HuecasYa
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override void OnCreate()
        {
            base.OnCreate();
            // Aquí puedes inicializar cualquier cosa que necesites al inicio de la aplicación
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=   (sender, cert, chain, sslPolicyErrors) => true;
        }
    }
}
