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
        SituacionLabel.Text = $"Situaci�n social: {restaurante.SituacionSocial}";
        HorarioLabel.Text = $"Horario: {restaurante.Horario}";
        TiempoLabel.Text = $"Tiempo de atenci�n: {restaurante.TiempoEstimado}";
        MetodosLabel.Text = $"M�todos de pago: {restaurante.MetodosPago}";
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // vuelve a la pantalla anterior
    }
}
