using Backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Text.Json;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController:ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<UsersModel>>> GetUsers()
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());
            var usersCollection = database.GetCollection<UsersModel>("users");
            List<UsersModel> users = await usersCollection.Find(u => true).ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsersModel>> GetUserById(string id)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());
            var usersCollection = database.GetCollection<UsersModel>("users");
            UsersModel user = await usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<UsersModel>> CreateUser([FromBody] UsersModel user)
        {
            if (user == null || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Invalid user data.");
            }

            ConnDB conn = new ConnDB(); 
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());
            
            var usersCollection = database.GetCollection<UsersModel>("users");

            usersCollection.InsertOne(user);

            return user;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UsersModel>> UpdateUser(string id, [FromBody] UsersModel user)
        {
            user.Id = id; // Ensure the ID matches the route parameter
            
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());
            var usersCollection = database.GetCollection<UsersModel>("users");
            usersCollection.ReplaceOne(u => u.Id == id, user);

            return user;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());
            var usersCollection = database.GetCollection<UsersModel>("users");
            var result = await usersCollection.DeleteOneAsync(u => u.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound("User not found.");
            }
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsersModel>> Login([FromBody] LoginRequest login)
        {
            if (login == null || string.IsNullOrEmpty(login.Banner) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Invalid login data.");
            }

            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());
            var usersCollection = database.GetCollection<UsersModel>("users");

            var foundUser = await usersCollection
                .Find(u => u.Banner == login.Banner && u.Password == login.Password)
                .FirstOrDefaultAsync();

            if (foundUser == null)
            {
                return Unauthorized("Invalid banner or password.");
            }

            return Ok(foundUser.Id);
        }

        [HttpGet("por-correo/{email}")]
        public async Task<ActionResult<UsersModel>> GetUserByEmail(string email)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());

            var collection = database.GetCollection<UsersModel>("users");

            var user = await collection.Find(u => u.Email == email).FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("enviar-codigo")]
        public async Task<IActionResult> EnviarCodigo([FromForm] string correo, [FromForm] string codigo)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(codigo))
                return BadRequest("Correo o código inválido.");

            try
            {
                var servicioCorreo = new EmailService();
                await servicioCorreo.EnviarCodigoAsync(correo, codigo);
                return Ok("✅ Código enviado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al enviar el correo.");
            }
        }

        [HttpPut("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] JsonElement data)
        {
            string email = data.GetProperty("email").GetString();
            string nuevaPassword = data.GetProperty("nuevaPassword").GetString();

            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());

            var usersCollection = database.GetCollection<UsersModel>("users");

            var update = Builders<UsersModel>.Update.Set(u => u.Password, nuevaPassword);
            var result = await usersCollection.UpdateOneAsync(u => u.Email == email, update);

            if (result.MatchedCount == 0)
                return NotFound("Usuario no encontrado");

            return Ok("Contraseña actualizada");
        }


        [HttpPost("favoritos/plato/{userId}")]
        public async Task<IActionResult> AddPlatoFavorito(string userId, [FromBody] string platoId)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());

            var usersCollection = database.GetCollection<UsersModel>("users");

            var filter = Builders<UsersModel>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UsersModel>.Update.AddToSet(u => u.Favoritos, platoId);
            var result = await usersCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return NotFound("Usuario no encontrado.");

            return Ok("Plato añadido a favoritos.");
        }

        [HttpGet("favoritos/plato/{userId}")]
        public async Task<ActionResult<List<object>>> GetPlatosFavoritos(string userId)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());

            var usersCollection = database.GetCollection<UsersModel>("users");
            var restaurantesCollection = database.GetCollection<BsonDocument>("restaurantes");

            var user = await usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return NotFound("Usuario no encontrado.");

            if (user.Favoritos == null || user.Favoritos.Count == 0)
                return Ok(new List<object>());

            var resultadoFinal = new List<object>();

            foreach (var favorito in user.Favoritos)
            {
                var partes = favorito.Split("::::");
                if (partes.Length != 2) continue;

                var nombreRest = partes[0];
                var nombrePlato = partes[1];

                var filtroRest = Builders<BsonDocument>.Filter.Eq("nombre", nombreRest);
                var restaurante = await restaurantesCollection.Find(filtroRest).FirstOrDefaultAsync();

                if (restaurante == null || !restaurante.Contains("categoriasMenu")) continue;

                foreach (var cat in restaurante["categoriasMenu"].AsBsonArray)
                {
                    var catDoc = cat.AsBsonDocument;

                    if (catDoc.Contains("platos"))
                    {
                        foreach (var plato in catDoc["platos"].AsBsonArray)
                        {
                            var platoDoc = plato.AsBsonDocument;
                            if (platoDoc["nombre"] == nombrePlato)
                            {
                                resultadoFinal.Add(new
                                {
                                    nombre = platoDoc["nombre"].AsString,
                                    descripcion = platoDoc.GetValue("descripcion", "").AsString,
                                    precio = platoDoc["precio"].ToDecimal(),
                                    imagenUrl = platoDoc.GetValue("imagenUrl", "").AsString,
                                    restaurante = nombreRest,
                                    ubicacion = restaurante.GetValue("ubicacion", "").AsString,
                                    calificacion = restaurante.GetValue("calificacion", 0).ToDouble()
                                });
                                break;
                            }
                        }
                    }
                }
            }

            return Ok(resultadoFinal);
        }


        [HttpDelete("favoritos/plato/{userId}")]
        public async Task<IActionResult> RemovePlatoFavorito(string userId, [FromBody] string platoId)
        {
            ConnDB conn = new ConnDB();
            var client = new MongoClient(conn.GetConnectionString());
            var database = client.GetDatabase(conn.GetDB());

            var usersCollection = database.GetCollection<UsersModel>("users");

            var filter = Builders<UsersModel>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UsersModel>.Update.Pull(u => u.Favoritos, platoId);
            var result = await usersCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                return NotFound("Usuario no encontrado.");

            return Ok("Plato eliminado de favoritos.");
        }
    }
}
