using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class CarrerasController : Controller
    {
        private readonly string _cadenaConexion;

        public CarrerasController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        // GET: Muestra la lista
        public IActionResult Index()
        {
            List<Carrera> lista = new List<Carrera>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT id, clave_carrera, nombre_carrera, duracion_semestres, estatus_activo FROM carreras";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Carrera
                            {
                                Id = reader.GetInt32("id"),
                                ClaveCarrera = reader.GetString("clave_carrera"),
                                NombreCarrera = reader.GetString("nombre_carrera"),
                                DuracionSemestres = reader.GetInt32("duracion_semestres"),
                                EstatusActivo = reader.GetBoolean("estatus_activo")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }

        // --- INICIA PROCEDIMIENTO PARA GUARDAR NUEVA CARRERA ---

        // 1. GET: Carreras/Create (Muestra el formulario vacío)
        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Carreras/Create (Guarda en MySQL)
        [HttpPost]
        public IActionResult Create(Carrera carrera)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();

                // Usamos los nombres exactos que sacamos de tu SELECT
                string query = @"INSERT INTO carreras (clave_carrera, nombre_carrera, duracion_semestres, estatus_activo) 
                                 VALUES (@clave, @nombre, @duracion, @estatus)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@clave", carrera.ClaveCarrera);
                    cmd.Parameters.AddWithValue("@nombre", carrera.NombreCarrera);
                    cmd.Parameters.AddWithValue("@duracion", carrera.DuracionSemestres);
                    cmd.Parameters.AddWithValue("@estatus", carrera.EstatusActivo);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}