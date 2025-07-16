using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/restaurantes")]
    public class RestaurantesController : ControllerBase
    {
        [HttpGet("filtrar")]
        public async Task<IActionResult> FiltrarRestaurantes(
            [FromQuery] string? presupuesto,
            [FromQuery] string? preferencia,
            [FromQuery] string? situacion,
            [FromQuery] string? tiempo,
            [FromQuery] string? metodo)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var db = client.GetDatabase(conn.GetDB());
            var collection = db.GetCollection<BsonDocument>("restaurantes");

            var filtros = new List<FilterDefinition<BsonDocument>>();

            if (!string.IsNullOrEmpty(preferencia))
                filtros.Add(
                    Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>(
                        "categoriasMenu",
                        Builders<BsonDocument>.Filter.Eq("nombreCategoria", preferencia)
                    )
                );

            if (!string.IsNullOrEmpty(situacion))
                filtros.Add(Builders<BsonDocument>.Filter.AnyEq("situacionesSociales", situacion));

            if (!string.IsNullOrEmpty(tiempo) && int.TryParse(tiempo, out var maxMinutos))
                filtros.Add(Builders<BsonDocument>.Filter.Lte("tiempoAtencionMinutos", maxMinutos));

            if (!string.IsNullOrEmpty(metodo))
                filtros.Add(Builders<BsonDocument>.Filter.AnyEq("metodosPago", metodo));

            var filtroFinal = filtros.Count > 0
                ? Builders<BsonDocument>.Filter.And(filtros)
                : Builders<BsonDocument>.Filter.Empty;

            // Paso 1: obtener documentos con los filtros simples
            var bsonList = await collection.Find(filtroFinal).ToListAsync();

            // Paso 2: aplicar filtro por presupuesto manualmente
            if (!string.IsNullOrEmpty(presupuesto) && decimal.TryParse(presupuesto, out var maxPresupuesto))
            {
                bsonList = bsonList.Where(doc =>
                    doc["categoriasMenu"].AsBsonArray
                        .SelectMany(cat => cat["platos"].AsBsonArray)
                        .Any(plato => plato["precio"].ToDecimal() <= maxPresupuesto)
                ).ToList();
            }

            // Paso 3: convertir BSON a JSON plano para la respuesta
            var lista = new List<object>();
            foreach (var doc in bsonList)
            {
                var json = doc.ToJson(new JsonWriterSettings
                {
                    OutputMode = JsonOutputMode.RelaxedExtendedJson
                });

                var obj = System.Text.Json.JsonSerializer.Deserialize<object>(json);
                lista.Add(obj);
            }

            return Ok(lista);
        }

        [HttpGet("categorias")]
        public async Task<IActionResult> ObtenerCategorias()
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var db = client.GetDatabase(conn.GetDB());
            var collection = db.GetCollection<BsonDocument>("restaurantes");

            var documentos = await collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();

            var categorias = new HashSet<string>();

            foreach (var doc in documentos)
            {
                if (doc.Contains("categoriasMenu"))
                {
                    foreach (var cat in doc["categoriasMenu"].AsBsonArray)
                    {
                        var nombre = cat["nombreCategoria"];
                        if (nombre != null && nombre.IsString)
                        {
                            categorias.Add(nombre.AsString);
                        }
                    }
                }
            }

            return Ok(categorias.OrderBy(c => c));
        }

        [HttpGet("filtrarPlatos")]
        public async Task<IActionResult> FiltrarPlatos(
     [FromQuery] string? presupuesto,
     [FromQuery] string? preferencia,
     [FromQuery] string? situacion,
     [FromQuery] string? tiempo,
     [FromQuery] string? metodo)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var db = client.GetDatabase(conn.GetDB());
            var collection = db.GetCollection<BsonDocument>("restaurantes");

            var filtros = new List<FilterDefinition<BsonDocument>>();

            if (!string.IsNullOrEmpty(preferencia))
            {
                filtros.Add(
                    Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>(
                        "categoriasMenu",
                        Builders<BsonDocument>.Filter.Eq("nombreCategoria", preferencia)
                    )
                );
            }

            if (!string.IsNullOrEmpty(situacion))
                filtros.Add(Builders<BsonDocument>.Filter.AnyEq("situacionesSociales", situacion));

            if (!string.IsNullOrEmpty(tiempo) && int.TryParse(tiempo, out var maxMinutos))
                filtros.Add(Builders<BsonDocument>.Filter.Lte("tiempoAtencionMinutos", maxMinutos));

            if (!string.IsNullOrEmpty(metodo))
                filtros.Add(Builders<BsonDocument>.Filter.AnyEq("metodosPago", metodo));

            var filtroFinal = filtros.Count > 0
                ? Builders<BsonDocument>.Filter.And(filtros)
                : Builders<BsonDocument>.Filter.Empty;

            var restaurantes = await collection.Find(filtroFinal).ToListAsync();

            var platosFiltrados = new List<object>();

            // ✅ Parsear presupuesto con InvariantCulture para que reconozca el punto como separador decimal
            decimal? maxPresupuesto = null;
            if (!string.IsNullOrEmpty(presupuesto) &&
                decimal.TryParse(presupuesto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var pres))
            {
                maxPresupuesto = pres;
            }

            foreach (var rest in restaurantes)
            {
                string nombreRest = rest.Contains("nombre") && !rest["nombre"].IsBsonNull
                    ? rest["nombre"].AsString
                    : "Sin nombre";

                string ubicacion = rest.Contains("ubicacion") && !rest["ubicacion"].IsBsonNull
                    ? rest["ubicacion"].AsString
                    : "Ubicación desconocida";

                double calificacion = rest.Contains("calificacion") && !rest["calificacion"].IsBsonNull
                    ? rest["calificacion"].ToDouble()
                    : 0.0;

                if (!rest.Contains("categoriasMenu")) continue;

                foreach (var categoria in rest["categoriasMenu"].AsBsonArray)
                {
                    var categoriaDoc = categoria.AsBsonDocument;

                    if (!string.IsNullOrEmpty(preferencia) &&
                        (!categoriaDoc.Contains("nombreCategoria") ||
                         categoriaDoc["nombreCategoria"].IsBsonNull ||
                         categoriaDoc["nombreCategoria"].AsString != preferencia))
                        continue;

                    if (!categoriaDoc.Contains("platos")) continue;

                    foreach (var plato in categoriaDoc["platos"].AsBsonArray)
                    {
                        var platoDoc = plato.AsBsonDocument;

                        if (!platoDoc.Contains("precio") || platoDoc["precio"].IsBsonNull)
                            continue;

                        decimal precio = platoDoc["precio"].ToDecimal();

                        // ✅ Solo incluir si no se pidió presupuesto, o si el precio es menor o igual
                        if (!maxPresupuesto.HasValue || precio <= maxPresupuesto.Value)
                        {
                            platosFiltrados.Add(new
                            {
                                nombre = platoDoc.Contains("nombre") && !platoDoc["nombre"].IsBsonNull
                                    ? platoDoc["nombre"].AsString
                                    : "Plato sin nombre",

                                descripcion = platoDoc.Contains("descripcion") && !platoDoc["descripcion"].IsBsonNull
                                    ? platoDoc["descripcion"].AsString
                                    : "",

                                precio,
                                imagenUrl = platoDoc.Contains("imagenUrl") && !platoDoc["imagenUrl"].IsBsonNull
                                    ? platoDoc["imagenUrl"].AsString
                                    : null,

                                restaurante = nombreRest,
                                ubicacion,
                                calificacion
                            });
                        }
                    }
                }
            }

            return Ok(platosFiltrados);
        }

        [HttpGet("por-nombre/{nombre}")]
        public async Task<IActionResult> ObtenerRestaurantePorNombre(string nombre)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var db = client.GetDatabase(conn.GetDB());

            var collection = db.GetCollection<BsonDocument>("restaurantes");

            var filter = Builders<BsonDocument>.Filter.Eq("nombre", nombre);
            var restaurante = await collection.Find(filter).FirstOrDefaultAsync();

            if (restaurante == null)
                return NotFound($"No se encontró el restaurante con nombre: {nombre}");

            // Convertir el documento BSON a JSON plano
            var json = restaurante.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson });
            var obj = System.Text.Json.JsonSerializer.Deserialize<object>(json);

            return Ok(obj);
        }


    }

}
