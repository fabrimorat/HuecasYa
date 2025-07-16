using System.Text.Json.Nodes;
using System.Web;

namespace HuecasYa;

public partial class BusquedaPage : ContentPage, IQueryAttributable
{
    private readonly HttpClient _httpClient = new();
    private readonly string _baseUrl = AppConfig.apiUrl;

    private string usuarioId; // Establece dinámicamente al iniciar sesión

    public BusquedaPage()
    {
        InitializeComponent();
        _ = CargarCategoriasAsync();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("userId"))
        {
            usuarioId = query["userId"].ToString();
            CargarFavoritos(); // ahora sí puedes usar _userId
        }
    }

    private async void CargarFavoritos()
    {
        try
        {
            var url = $"{_baseUrl}/api/users/favoritos/plato/{usuarioId}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var favoritos = JsonNode.Parse(json)?.AsArray();

                if (favoritos != null)
                {
                    var favoritosList = favoritos.Select(f => f.AsObject()).ToList();
                    FavoritosList.ItemsSource = favoritosList;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar favoritos: {ex.Message}", "OK");
        }
    }


    private async Task CargarCategoriasAsync()
    {
        try
        {
            var url = $"{_baseUrl}/api/restaurantes/categorias";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var categorias = JsonNode.Parse(json)?.AsArray();

                if (categorias != null)
                {
                    PreferenciaPicker.ItemsSource = categorias.Select(c => c.ToString()).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar categorías: {ex.Message}", "OK");
        }
    }


    private async void OnBuscarClicked(object sender, EventArgs e)
    {
        try
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            // Capturar datos de filtros
            string? presupuestoStr = PresupuestoEntry.Text;
            string? situacion = SituacionPicker.SelectedIndex >= 0 ? SituacionPicker.SelectedItem.ToString() : null;

            if (!string.IsNullOrWhiteSpace(presupuestoStr))
                query["presupuesto"] = presupuestoStr;

            if (PreferenciaPicker.SelectedIndex >= 0)
                query["preferencia"] = PreferenciaPicker.SelectedItem.ToString();

            if (!string.IsNullOrWhiteSpace(situacion))
                query["situacion"] = situacion;

            if (!string.IsNullOrWhiteSpace(TiempoAtencionEntry.Text))
                query["tiempo"] = TiempoAtencionEntry.Text;

            if (MetodoPagoPicker.SelectedIndex >= 0)
                query["metodo"] = MetodoPagoPicker.SelectedItem.ToString();

            // ✅ Si la situación es "cumpleaños" y no se ingresó presupuesto, detener y mostrar alerta
            if (situacion == "cumpleaños" && string.IsNullOrWhiteSpace(presupuestoStr))
            {
                await DisplayAlert("Presupuesto requerido", "Para cumpleaños debes ingresar un presupuesto estimado.", "OK");
                return;
            }

            // Construir URL
            string url = $"{_baseUrl}/api/restaurantes/filtrarPlatos";
            string queryString = query.ToString();
            if (!string.IsNullOrWhiteSpace(queryString))
                url += "?" + queryString;

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var resultados = JsonNode.Parse(json)?.AsArray();

                ResultadosList.ItemsSource = resultados;

                // ✅ Si la situación es "cumpleaños", pedir número de personas y calcular el pago
                if (situacion == "cumpleaños")
                {
                    string numPersonasStr = await DisplayPromptAsync(
                        "¿Se van de cumpleaños? 🎉",
                        "¿Cuántas personas asistirán al festejo?",
                        keyboard: Keyboard.Numeric,
                        placeholder: "Ej. 5");

                    if (int.TryParse(numPersonasStr, out int numPersonas) &&
                        numPersonas > 1 &&
                        decimal.TryParse(presupuestoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal presupuesto))
                    {
                        decimal pagoAprox = (presupuesto) / (numPersonas - 1);
                        await DisplayAlert("Que viva el Cumpleañero! 🎉", $"En base al presupuesto ingresado de ${presupuesto}, recuerda que el cumpleañero no paga, así que cada uno debería aportar aproximadamente ${pagoAprox:F2}.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Dato inválido", "Por favor ingresa un número válido mayor a 1.", "OK");
                    }
                }

                // ✅ Si la situación es "cita", pedir descripción del plan en texto
                if (situacion == "cita")
                {
                    string planCita = await DisplayPromptAsync(
                        "Cita 👥",
                        "¿Qué tienes planeado para la cita?",
                        keyboard: Keyboard.Chat,
                        placeholder: "Ej. Cenar");

                    if (!string.IsNullOrWhiteSpace(planCita))
                    {
                        await DisplayAlert("¡Plan listo! 💕", $"Tu plan para la cita es: {planCita}. ¡Que la pasen increíble!", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Entrada inválida", "Por favor ingresa una descripción válida para tu cita.", "OK");
                    }
                }

                // ✅ Si la situación es "grupo", pedir número de personas y calcular el pago
                if (situacion == "grupo")
                {
                    string numPersonasSt = await DisplayPromptAsync(
                        "Salida en Grupo? 👌",
                        "¿Cuántas personas hay en el grupo?",
                        keyboard: Keyboard.Numeric,
                        placeholder: "Ej. 5");

                    if (int.TryParse(numPersonasSt, out int numPersona) &&
                        numPersona > 1 &&
                        decimal.TryParse(presupuestoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal presupuesto))
                    {
                        decimal pagoAprox = (presupuesto) / (numPersona);
                        await DisplayAlert("Disfruten en Grupo! ✌️", $"En base al presupuesto ingresado de ${presupuesto}, cada uno debería aportar ${pagoAprox:F2}.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Dato inválido", "Por favor ingresa un número válido mayor a 1.", "OK");
                    }
                }

            }
            else
            {
                await DisplayAlert("Sin resultados", "No se encontraron platos que cumplan con los filtros.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al buscar: {ex.Message}", "OK");
        }
    }


    private async void ResultadosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is JsonObject restaurante)
        {
            // Convertimos JsonObject a string JSON plano
            var jsonString = restaurante.ToJsonString();

            // Navegamos enviando el objeto por parámetro codificado
            var encoded = Uri.EscapeDataString(jsonString);
            await Shell.Current.GoToAsync($"DetalleRestaurantePage?userId={usuarioId}&data={encoded}");


            // Deseleccionar el ítem
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnPlatoTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is JsonObject plato)
        {
            var json = plato.ToJsonString();
            var encoded = Uri.EscapeDataString(json);
            await Shell.Current.GoToAsync($"DetalleRestaurantePage?userId={usuarioId}&data={encoded}");
        }
    }

}