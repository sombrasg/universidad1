using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly string _cadenaConexion;

        public CalificacionesController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<CalificacionViewModel> lista = new List<CalificacionViewModel>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // AQUÍ ESTÁ LA MAGIA DEL PROYECTO: Un INNER JOIN para no mostrar el ID del alumno
                // TRIPLE INNER JOIN: Unimos calificaciones -> alumnos -> grupos -> materias
                string query = @"
                    SELECT 
                        c.id, 
                        CONCAT(a.nombre, ' ', a.apellido_paterno, ' ', IFNULL(a.apellido_materno, '')) AS NombreAlumno, 
                        m.nombre_materia AS NombreMateria, 
                        c.parcial_1, 
                        c.parcial_2, 
                        c.promedio_final, 
                        c.estatus_aprobacion
                    FROM calificaciones c
                    INNER JOIN alumnos a ON c.alumno_id = a.id
                    INNER JOIN grupos g ON c.grupo_id = g.id
                    INNER JOIN materias m ON g.materia_id = m.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new CalificacionViewModel
                            {
                                Id = reader.GetInt32("id"),
                                NombreAlumno = reader.GetString("NombreAlumno"),
                                NombreMateria = reader.GetString("NombreMateria"),
                                Parcial1 = reader.IsDBNull(reader.GetOrdinal("parcial_1")) ? null : reader.GetDecimal("parcial_1"),
                                Parcial2 = reader.IsDBNull(reader.GetOrdinal("parcial_2")) ? null : reader.GetDecimal("parcial_2"),
                                PromedioFinal = reader.IsDBNull(reader.GetOrdinal("promedio_final")) ? null : reader.GetDecimal("promedio_final"),
                                EstatusAprobacion = reader.IsDBNull(reader.GetOrdinal("estatus_aprobacion")) ? null : reader.GetString("estatus_aprobacion")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }
    }
}