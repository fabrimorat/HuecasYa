using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Web;

namespace HuecasYa;

public partial class RegisterPage : ContentPage
{
    private readonly HttpClient _httpClient = new();
    private readonly string _baseUrl = AppConfig.apiUrl;

    public RegisterPage()
    {
        InitializeComponent();
    }

    private void OnCheckPoliticaChanged(object sender, CheckedChangedEventArgs e)
    {
        RegisterButton.IsEnabled = e.Value;
    }


    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();
        string banner = BannerEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(banner) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Campos requeridos", "Asegurate de llenar todos los campos antes de continuar", "OK");
            return;
        }

        if (!email.EndsWith("@udla.edu.ec"))
        {
            await DisplayAlert("Correo no permitido", "Recuerda que debes usar un correo institucional (@udla.edu.ec).", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Contraseñas no coinciden", "Asegurate que la contraseña sea igual a la ingresada previamente.", "OK");
            return;
        }

        var usuario = new
        {
            name,
            email,
            banner,
            password,
            favoritos = new List<string>()
        };

        try
        {
            string json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{_baseUrl}/api/users";
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Bienvenido a HuecasYa", "Ahora inicia sesión con tu cuenta y diviértete!", "OK");
                await Shell.Current.GoToAsync("///LoginPage");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"No se pudo registrar: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error inesperado: {ex.Message}", "OK");
        }
    }
}
