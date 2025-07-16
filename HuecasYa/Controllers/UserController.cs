using HuecasYa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HuecasYa.Controllers
{
    public class UserController
    {
        public async Task<String> ValidarLogin(string banner, string password)
        {
            var loginRequest = new LoginRequest
            {
                Banner = banner,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 👇 HttpClientHandler que ignora la validación del certificado (¡solo en desarrollo!)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(10);
            client.BaseAddress = new Uri(AppConfig.apiUrl); // ej. "https://192.168.68.128:5295/"

            try
            {
                var response = await client.PostAsync("/api/users/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    return responseBody;
                }

                return "false";
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo conectar: {ex.Message}", "Aceptar");
                return "false";
            }
        }

        public async Task<bool> EnviarCodigoPorCorreo(string correo, string codigo)
        {
            var formData = new Dictionary<string, string>
            {
                { "correo", correo },
                { "codigo", codigo }
            };

            var content = new FormUrlEncodedContent(formData);

            // 👇 HttpClientHandler que ignora la validación del certificado (¡solo en desarrollo!)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(10);
            client.BaseAddress = new Uri(AppConfig.apiUrl); // Asegúrate de terminar en "/"

            try
            {
                var response = await client.PostAsync("/api/users/enviar-codigo", content);

                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "✅ Código enviado al correo", "OK");
                    return true;
                }

                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Error", $"❌ No se pudo enviar: {error}", "OK");
                return false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"❌ Error de red: {ex.Message}", "OK");
                return false;
            }
        }
    }
}
