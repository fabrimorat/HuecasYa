namespace HuecasYa;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnLogin_Clicked(object sender, EventArgs e)
    {
        var banner = BannerEntry.Text?.Trim();
        var password = PasswordEntry.Text?.Trim();

        if (string.IsNullOrEmpty(banner) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Por favor ingresa tu número de banner y contraseña.", "Aceptar");
            return;
        }

        // Aquí iría tu lógica de autenticación real (llamada al backend)
        // Por ahora, solo una simulación de validación
        string loginExitoso = await new Controllers.UserController().ValidarLogin(banner, password);

        if (loginExitoso != "false")
        {
            await Shell.Current.GoToAsync($"BusquedaPage?userId={loginExitoso}");

        }
        else
        {
            await DisplayAlert("Error", "Credenciales incorrectas.", "Aceptar");
        }
    }

    private async void OnRegisterRedirect_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }

    private async void OnRecuperarContrasena_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("PasswordRecoveryPage");
    }
}