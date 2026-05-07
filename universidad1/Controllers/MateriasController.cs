using Microsoft.AspNetCore.Mvc;
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
    }
}