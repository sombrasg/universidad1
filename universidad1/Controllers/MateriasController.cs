using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;// despegable
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class MateriasController : Controller
    {
        private readonly string _cadenaConexion;

        public MateriasController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        // GET: Muestra la tabla principal
        public IActionResult Index()
        {
            List<Materia> lista = new List<Materia>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"
                    SELECT 
                        m.id, m.clave, m.nombre_materia, m.creditos, 
                        m.horas_teoricas, m.horas_practicas, m.semestre_sugerido, 
                        c.nombre_carrera
                    FROM materias m
                    INNER JOIN carreras c ON m.carrera_id = c.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Materia
                            {
                                Id = reader.GetInt32("id"),
                                Clave = reader.GetString("clave"),
                                NombreMateria = reader.GetString("nombre_materia"),
                                Creditos = reader.GetInt32("creditos"),
                                HorasTeoricas = reader.GetInt32("horas_teoricas"),
                                HorasPracticas = reader.GetInt32("horas_practicas"),
                                SemestreSugerido = reader.GetInt32("semestre_sugerido"),
                                NombreCarrera = reader.IsDBNull(reader.GetOrdinal("nombre_carrera")) ? "Sin Asignar" : reader.GetString("nombre_carrera")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }

        // 1. GET: Muestra el formulario vacío
        public IActionResult Create()
        {
            List<SelectListItem> listaCarreras = new List<SelectListItem>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // Consultamos las carreras para llenar el Select
                string query = "SELECT id, nombre_carrera FROM carreras";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaCarreras.Add(new SelectListItem
                            {
                                Value = reader.GetInt32("id").ToString(),
                                Text = reader.GetString("nombre_carrera")
                            });
                        }
                    }
                }
            }

            // Pasamos la lista a la vista usando ViewBag
            ViewBag.Carreras = listaCarreras;
            return View();
        }

        // 2. POST: Guarda la información en MySQL
        [HttpPost]
        public IActionResult Create(Materia materia)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"INSERT INTO materias 
                                (clave, nombre_materia, creditos, horas_teoricas, horas_practicas, semestre_sugerido, carrera_id) 
                                VALUES 
                                (@clave, @nombre, @creditos, @hteoricas, @hpracticas, @semestre, @carrera_id)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@clave", materia.Clave);
                    cmd.Parameters.AddWithValue("@nombre", materia.NombreMateria);
                    cmd.Parameters.AddWithValue("@creditos", materia.Creditos);
                    cmd.Parameters.AddWithValue("@hteoricas", materia.HorasTeoricas);
                    cmd.Parameters.AddWithValue("@hpracticas", materia.HorasPracticas);
                    cmd.Parameters.AddWithValue("@semestre", materia.SemestreSugerido);
                    cmd.Parameters.AddWithValue("@carrera_id", materia.CarreraId);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}