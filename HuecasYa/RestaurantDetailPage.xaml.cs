namespace HuecasYa;

public partial class RestaurantDetailPage : ContentPage
{
    private Restaurante restaurante;

    public RestaurantDetailPage(Restaurante restaurante)
    {
        InitializeComponent();
        this.restaurante = restaurante;

        // Asignar valores
        NombreLabel.Text = restaurante.Nombre;
        Imagen.Source = restaurante.Imagen;
        PresupuestoLabel.Text = $"Presupuesto: {restaurante.Presupuesto}";
        RecomendacionesLabel.Text = $"Recomendaciones: {restaurante.Recomendaciones}";
        PreferenciasLabel.Text = $"Opciones alimenticias: {restaurante.Preferencias}";
        SituacionLabel.Text = $"Situación social: {restaurante.SituacionSocial}";
        HorarioLabel.Text = $"Horario: {restaurante.Horario}";
        TiempoLabel.Text = $"Tiempo de atención: {restaurante.TiempoEstimado}";
        MetodosLabel.Text = $"Métodos de pago: {restaurante.MetodosPago}";
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // vuelve a la pantalla anterior
    }
}
