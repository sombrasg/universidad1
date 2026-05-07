using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class AulasController : Controller
    {
        private readonly string _cadenaConexion;

        public AulasController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<Aula> lista = new List<Aula>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT id, codigo_aula, edificio, capacidad_alumnos, tipo_aula FROM Aulas";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Aula
                            {
                                Id = reader.GetInt32("id"),
                                CodigoAula = reader.GetString("codigo_aula"),
                                Edificio = reader.GetString("edificio"),
                                Capacidad = reader.GetInt32("capacidad_alumnos"), // ¡Aquí está la corrección del error!
                                TipoAula = reader.IsDBNull(reader.GetOrdinal("tipo_aula")) ? "Estándar" : reader.GetString("tipo_aula")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Aula aula)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();

                string query = @"INSERT INTO Aulas (codigo_aula, edificio, capacidad_alumnos, tipo_aula) 
                                 VALUES (@codigo, @edificio, @capacidad, @tipo)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@codigo", aula.CodigoAula);
                    cmd.Parameters.AddWithValue("@edificio", aula.Edificio);
                    cmd.Parameters.AddWithValue("@capacidad", aula.Capacidad);

                    cmd.Parameters.AddWithValue("@tipo", string.IsNullOrEmpty(aula.TipoAula) ? (object)DBNull.Value : aula.TipoAula);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }
    }
}