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
    }
}