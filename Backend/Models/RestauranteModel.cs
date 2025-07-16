using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models
{
    [BsonIgnoreExtraElements]
    public class RestauranteModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("ubicacion")]
        public string Ubicacion { get; set; } = string.Empty;

        [BsonElement("horario")]
        public string Horario { get; set; } = string.Empty;

        [BsonElement("calificacion")]
        public float Calificacion { get; set; }

        [BsonElement("latitud")]
        public double Latitud { get; set; }

        [BsonElement("longitud")]
        public double Longitud { get; set; }

        [BsonElement("precioPromedio")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal PrecioPromedio { get; set; }

        [BsonElement("preferenciasAlimenticias")]
        public List<string> PreferenciasAlimenticias { get; set; } = new();

        [BsonElement("situacionesSociales")]
        public List<string> SituacionesSociales { get; set; } = new();

        [BsonElement("horarioApertura")]
        public string HorarioApertura { get; set; } = "07:00";

        [BsonElement("horarioCierre")]
        public string HorarioCierre { get; set; } = "22:00";

        [BsonElement("tiempoAtencionMinutos")]
        public int TiempoAtencionMinutos { get; set; }

        [BsonElement("metodosPago")]
        public List<string> MetodosPago { get; set; } = new();

        [BsonElement("categoriasMenu")]
        public List<CategoriaMenuModel> CategoriasMenu { get; set; } = new();
    }

    public class CategoriaMenuModel
    {
        [BsonElement("nombreCategoria")]
        public string NombreCategoria { get; set; } = string.Empty;

        [BsonElement("platos")]
        public List<PlatoModel> Platos { get; set; } = new();
    }

    public class PlatoModel
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("precio")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Precio { get; set; }
    }
}
