using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuecasYa;

public class Restaurante
{
    public string Nombre { get; set; }
    public string Imagen { get; set; }  // Ruta local o URL

    public string Presupuesto { get; set; }  // Ej: "$5 - $15"

    public string Recomendaciones { get; set; }  // Platos destacados

    public string Preferencias { get; set; }  // Ej: "Vegano, Sin gluten"

    public string SituacionSocial { get; set; }  // Ej: "Grupo, Cita, Cumpleaños"

    public string Horario { get; set; }  // Ej: "7:00 – 22:00"

    public string TiempoEstimado { get; set; }  // Ej: "15 – 20 min"

    public string MetodosPago { get; set; }  // Ej: "Efectivo, Tarjeta, Transferencia"
}
