using System.Net.Http;
using System.Text.Json.Nodes;
using System.Web;

namespace HuecasYa;

public partial class DetalleRestaurantePage : ContentPage, IQueryAttributable
{
    private readonly HttpClient _httpClient = new();
    private readonly string _baseUrl = AppConfig.apiUrl;

    private JsonObject _restaurante;
    private string _usuarioId;

    public DetalleRestaurantePage()
    {
        InitializeComponent();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("userId", out var userEncoded))
        {
            _usuarioId = Uri.UnescapeDataString(userEncoded.ToString());
        }

        if (query.TryGetValue("data", out var dataEncoded))
        {
            var jsonStr = Uri.UnescapeDataString(dataEncoded.ToString());
            var platoParcial = JsonNode.Parse(jsonStr)?.AsObject();

            string? nombreRestaurante = platoParcial?["restaurante"]?.ToString();

            if (!string.IsNullOrEmpty(nombreRestaurante))
            {
                try
                {
                    string url = $"{_baseUrl}/api/restaurantes/por-nombre/{Uri.EscapeDataString(nombreRestaurante)}";

                    var response = await _httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonCompleto = await response.Content.ReadAsStringAsync();
                        _restaurante = JsonNode.Parse(jsonCompleto)?.AsObject();
                        MostrarInformacion();
                        MostrarMenu();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo cargar la información del restaurante.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Hubo un problema al buscar el restaurante: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No se recibió el nombre del restaurante.", "OK");
            }
        }
    }

    private void MostrarInformacion()
    {
        NombreLabel.Text = _restaurante["nombre"]?.ToString() ?? "";
        UbicacionLabel.Text = "📍 " + (_restaurante["ubicacion"]?.ToString() ?? "");
        CalificacionLabel.Text = "⭐ " + (_restaurante["calificacion"]?.ToString() ?? "");

        // 🕒 Horario
        string apertura = _restaurante["horarioApertura"]?.ToString() ?? "00:00";
        string cierre = _restaurante["horarioCierre"]?.ToString() ?? "00:00";
        HorarioLabel.Text = $"🕒 Horario: {apertura} - {cierre}";

        // 💲 Precio promedio
        string precioPromedio = _restaurante["precioPromedio"]?["$numberDecimal"]?.ToString() ?? "0.00";
        PrecioPromedioLabel.Text = $"💲 Precio promedio: ${precioPromedio}";

        // 💳 Métodos de pago
        var metodos = _restaurante["metodosPago"]?.AsArray()?.Select(x => x?.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToList();
        if (metodos != null && metodos.Any())
            MetodosPagoLabel.Text = $"💳 Métodos de pago: {string.Join(", ", metodos)}";

        // 🥗 Preferencias alimenticias
        var preferencias = _restaurante["preferenciasAlimenticias"]?.AsArray()?.Select(x => x?.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToList();
        if (preferencias != null && preferencias.Any())
            PreferenciasLabel.Text = $"🥗 Preferencias: {string.Join(", ", preferencias)}";

        // 🎉 Situaciones sociales
        var situaciones = _restaurante["situacionesSociales"]?.AsArray()?.Select(x => x?.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToList();
        if (situaciones != null && situaciones.Any())
            SituacionesLabel.Text = $"🎉 Ideal para: {string.Join(", ", situaciones)}";

        // 🌍 Mapa (opcional)
        string latOrigen = "-0.180653";  // UDLA
        string lonOrigen = "-78.467838";
        string latDestino = _restaurante["latitud"]?.ToString() ?? "0";
        string lonDestino = _restaurante["longitud"]?.ToString() ?? "0";

        string osmUrl = $"https://staticmap.openstreetmap.de/staticmap.php" +
                        $"?size=600x300" +
                        $"&markers={latOrigen},{lonOrigen},blue" +
                        $"&markers={latDestino},{lonDestino},red" +
                        $"&path={latOrigen},{lonOrigen}|{latDestino},{lonDestino}";

        //MapaRuta.Source = ImageSource.FromUri(new Uri(osmUrl));
    }


    private void MostrarMenu()
    {
        var categorias = _restaurante["categoriasMenu"]?.AsArray();
        if (categorias == null) return;

        var grouped = new List<MenuCategoria>();

        foreach (var categoria in categorias)
        {
            string nombreCategoria = categoria?["nombreCategoria"]?.ToString() ?? "Categoría";
            var platos = categoria?["platos"]?.AsArray();
            var listaPlatos = new List<Dictionary<string, string>>();

            if (platos != null)
            {
                foreach (var p in platos)
                {
                    listaPlatos.Add(new Dictionary<string, string>
                    {
                        { "nombre", p?["nombre"]?.ToString() ?? "" },
                        { "descripcion", p?["descripcion"]?.ToString() ?? "" },
                        { "precio", p?["precio"]?["$numberDecimal"]?.ToString() ?? "0.00" }
                    });
                }
            }

            grouped.Add(new MenuCategoria(nombreCategoria, listaPlatos));
        }

        MenuList.ItemsSource = grouped;
    }

    private async void OnAbrirMapaClicked(object sender, EventArgs e)
    {
        string lat = _restaurante["latitud"]?.ToString();
        string lon = _restaurante["longitud"]?.ToString();
        string mapsUrl = $"https://www.google.com/maps/dir/?api=1&destination={lat},{lon}";
        await Launcher.Default.OpenAsync(mapsUrl);
    }

    public class MenuCategoria : List<Dictionary<string, string>>
    {
        public string Key { get; set; }

        public MenuCategoria(string key, IEnumerable<Dictionary<string, string>> items) : base(items)
        {
            Key = key;
        }
    }

    private async void OnAgregarPlatoFavoritoClicked(object sender, EventArgs e)
    {
        if (sender is Button boton && boton.BindingContext is Dictionary<string, string> plato)
        {
            try
            {
                string nombrePlato = plato.GetValueOrDefault("nombre") ?? "";
                string categoria = ((MenuCategoria)((CollectionView)MenuList).BindingContext)?.Key ?? "";
                string restaurante = _restaurante["nombre"]?.ToString() ?? "";

                if (string.IsNullOrEmpty(_usuarioId) || string.IsNullOrEmpty(nombrePlato) || string.IsNullOrEmpty(restaurante))
                {
                    await DisplayAlert("Error", "No se pudo identificar el usuario o el plato.", "OK");
                    return;
                }

                // Formato identificador: Restaurante::Categoría::Plato
                string idPlato = $"{restaurante}::{categoria}::{nombrePlato}";

                var content = new StringContent($"\"{idPlato}\"", System.Text.Encoding.UTF8, "application/json");

                string url = $"{_baseUrl}/api/users/favoritos/plato/{_usuarioId}";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("¡Listo!", "Plato guardado como favorito ❤️", "OK");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"No se pudo guardar: {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Hubo un problema al guardar el plato: {ex.Message}", "OK");
            }
        }
    }

}
