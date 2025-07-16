using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Backend.Models;

namespace Backend.Controllers
{
    public class AdminController : Controller
    {
        private readonly IMongoCollection<RestauranteModel> _restaurantes;

        public AdminController()
        {
            var conn = new ConnDB(); // Asegúrate de tener esta clase configurada
            var client = new MongoClient(conn.GetConnectionString());
            var db = client.GetDatabase(conn.GetDB());
            _restaurantes = db.GetCollection<RestauranteModel>("restaurantes");
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var lista = await _restaurantes.Find(_ => true).ToListAsync();
            return View(lista);
        }

        // GET: /Admin/Crear
        public IActionResult Crear()
        {
            var restaurante = new RestauranteModel
            {
                CategoriasMenu = new List<CategoriaMenuModel>
                {
                    new CategoriaMenuModel
                    {
                        NombreCategoria = "",
                        Platos = new List<PlatoModel>
                        {
                            new PlatoModel()
                        }
                    }
                }
            };

            return View(restaurante);
        }

        // POST: /Admin/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(RestauranteModel restaurante)
        {
            // Validación básica
            if (!ModelState.IsValid)
            {
                return View(restaurante);
            }

            restaurante.PreferenciasAlimenticias ??= new();
            restaurante.SituacionesSociales ??= new();
            restaurante.MetodosPago ??= new();
            restaurante.CategoriasMenu ??= new();

            foreach (var cat in restaurante.CategoriasMenu)
            {
                cat.Platos ??= new();
            }

            await _restaurantes.InsertOneAsync(restaurante);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Editar/{id}
        public async Task<IActionResult> Editar(string id)
        {
            var restaurante = await _restaurantes.Find(r => r.Id == id).FirstOrDefaultAsync();

            if (restaurante == null)
                return NotFound();

            // ✅ Asegurar que listas no vengan null
            restaurante.PreferenciasAlimenticias ??= new();
            restaurante.MetodosPago ??= new();
            restaurante.SituacionesSociales ??= new();
            restaurante.CategoriasMenu ??= new();

            foreach (var cat in restaurante.CategoriasMenu)
            {
                cat.Platos ??= new List<PlatoModel>();
            }

            return View(restaurante);
        }

        // POST: /Admin/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(RestauranteModel restaurante)
        {
            if (!ModelState.IsValid)
                return View(restaurante);

            restaurante.PreferenciasAlimenticias ??= new();
            restaurante.SituacionesSociales ??= new();
            restaurante.MetodosPago ??= new();
            restaurante.CategoriasMenu ??= new();

            foreach (var cat in restaurante.CategoriasMenu)
            {
                cat.Platos ??= new();
            }

            var result = await _restaurantes.ReplaceOneAsync(r => r.Id == restaurante.Id, restaurante);
            if (result.MatchedCount == 0)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Eliminar/{id}
        public async Task<IActionResult> Eliminar(string id)
        {
            var result = await _restaurantes.DeleteOneAsync(r => r.Id == id);
            if (result.DeletedCount == 0)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
