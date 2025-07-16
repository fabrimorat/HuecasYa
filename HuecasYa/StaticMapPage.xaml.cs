namespace HuecasYa;

public partial class StaticMapPage : ContentPage
{
	public StaticMapPage()
	{
		InitializeComponent();
        HuecasList.ItemsSource = new List<Hueca>
        {
            new Hueca
            {
                Nombre = "La Sazón de Doña Mary",
                Presupuesto = "Presupuesto: $3 - $7",
                Calificacion = "Calificación: ★★★★☆",
                Imagen = "comida1.png"
            },
            new Hueca
            {
                Nombre = "El Rey del Seco",
                Presupuesto = "Presupuesto: $5 - $10",
                Calificacion = "Calificación: ★★★★★",
                Imagen = "comida2.png"
            },
            new Hueca
            {
                Nombre = "Vegano Feliz",
                Presupuesto = "Presupuesto: $4 - $8",
                Calificacion = "Calificación: ★★★☆☆",
                Imagen = "comida3.png"
            },
            new Hueca
            {
                Nombre = "Casa RES",
                Presupuesto = "Presupuesto: $15 - $30",
                Calificacion = "Calificación: ★★★★☆",
                Imagen = "comida3.png"
            }
        };
    }

    public class Hueca
    {
        public string Nombre { get; set; }
        public string Presupuesto { get; set; }
        public string Calificacion { get; set; }
        public string Imagen { get; set; }  // ruta de imagen (local)
    }

    private async void HuecasList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Hueca hueca)
        {
            var restaurante = new Restaurante
            {
                Nombre = hueca.Nombre,
                Imagen = hueca.Imagen,
                Presupuesto = hueca.Presupuesto,
                Recomendaciones = "Seco de pollo, jugo natural",
                Preferencias = "Vegano, Vegetariano, Sin gluten",
                SituacionSocial = "Grupo, Cita",
                Horario = "7:00 – 22:00",
                TiempoEstimado = "15 – 20 min",
                MetodosPago = "Efectivo, Transferencia, Tarjeta"
            };

            await Navigation.PushAsync(new RestaurantDetailPage(restaurante));
            HuecasList.SelectedItem = null;
        }
    }
}
