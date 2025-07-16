using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace HuecasYa;

public partial class PasswordRecoveryPage : ContentPage
{
    private readonly HttpClient _httpClient = new();
    private readonly string _baseUrl = AppConfig.apiUrl;
    private string _codigoGenerado;
    private string _emailUsuario;

    public PasswordRecoveryPage()
    {
        InitializeComponent();
    }

    private async void OnBackToLogin_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("LoginPage");
    }

    private async void OnRecover_Clicked(object sender, EventArgs e)
    {
        _emailUsuario = EmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(_emailUsuario) || !_emailUsuario.EndsWith("@udla.edu.ec"))
        {
            await DisplayAlert("Correo inválido", "Ingresa un correo institucional válido (@udla.edu.ec).", "OK");
            return;
        }

        try
        {
            // Consultar si el usuario existe
            var url = $"{_baseUrl}/api/users/por-correo/{Uri.EscapeDataString(_emailUsuario)}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("No encontrado", "No existe un usuario con ese correo.", "OK");
                return;
            }

            // Simular envío de código por correo (generar y mostrar en consola)
            _codigoGenerado = new Random().Next(100000, 999999).ToString();
            Console.WriteLine($"📧 Código enviado al correo: {_codigoGenerado}");
            await new Controllers.UserController().EnviarCodigoPorCorreo(_emailUsuario, _codigoGenerado);

            // Pedir el código y la nueva contraseña
            string codigoIngresado = await DisplayPromptAsync("Código de recuperación", "Revisa tu correo (simulado) e ingresa el código:");
            if (codigoIngresado != _codigoGenerado)
            {
                await DisplayAlert("Código incorrecto", "El código ingresado no es válido.", "OK");
                return;
            }

            string nuevaContrasena = await DisplayPromptAsync(
                                                "Nueva contraseña",
                                                "Ingresa tu nueva contraseña:",
                                                accept: "Aceptar",
                                                cancel: "Cancelar",
                                                placeholder: "••••••",
                                                maxLength: 30,
                                                keyboard: Keyboard.Text);


            if (string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                await DisplayAlert("Contraseña inválida", "La contraseña no puede estar vacía.", "OK");
                return;
            }

            // Enviar nueva contraseña al backend
            var cambio = new
            {
                email = _emailUsuario,
                nuevaPassword = nuevaContrasena
            };

            var json = JsonSerializer.Serialize(cambio);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var cambioResponse = await _httpClient.PutAsync($"{_baseUrl}/api/users/cambiar-password", content);

            if (cambioResponse.IsSuccessStatusCode)
            {
                await DisplayAlert("Éxito", "Tu contraseña ha sido restablecida. Inicia sesión con la nueva contraseña.", "OK");
                await Shell.Current.GoToAsync("LoginPage");
            }
            else
            {
                var errorText = await cambioResponse.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"No se pudo actualizar la contraseña: {errorText}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Hubo un error: {ex.Message}", "OK");
        }
    }
}
